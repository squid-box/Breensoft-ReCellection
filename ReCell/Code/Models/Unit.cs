namespace Recellection.Code.Models
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Controllers;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// The representation of a Unit in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    public class Unit : Entity, IModel
    {
        #region Constants

        private const float NO_TARGET = -1;

        #endregion

        #region Static Fields

        private readonly static Texture2D UNIT_TEXTURE = Recellection.textureMap.GetTexture(Globals.TextureTypes.Unit);

        private static int id; // Used for randomness

        private static Logger logger = LoggerFactory.GetLogger();

        private static World world;

        #endregion

        // Status of unit
        #region Fields

        public float powerLevel;

        public float speedLevel;

        // Did I mention that I hate floats? // Martin
        protected float movement_speed = 0.01f;

        protected Vector2 targetPosition = new Vector2(NO_TARGET, NO_TARGET);

        private readonly Random rand;

        private float disperseDistance = 1.5f;

        private Entity missionEntity;     // Mission entity

        #endregion

        // METHODS
        #region Constructors and Destructors

        /// <summary>
        /// Creates a "default unit".
        /// </summary>
        public Unit(Player owner) : this(owner, new Vector2(0, 0))
        {
        }
        

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="posX">Unit x-coordinate.</param>
        /// <param name="posY">Unit y-coordinate.</param>
        public Unit(Player owner, float posX, float posY) : this(owner, new Vector2(posX, posY))
        {
        }

		/// <summary>
		/// Creates a unit.
		/// </summary>
		/// <param name="position">Position of unit.</param>
		/// <param name="owner">Owner of this unit.</param>
		public Unit(Player owner, Vector2 position) : this(owner, position, null)
		{
		}

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="position">Position of unit.</param>
        /// <param name="owner">Owner of this unit.</param>
        public Unit(Player owner, Vector2 position, Entity baseEntity) : base(position, owner)
        {
			this.BaseEntity = baseEntity;
			this.returnToBase = baseEntity != null;
			
			this.DisperseDistance = 1.5f;
            this.position = position;
            this.angle = 0;
            this.isDead = false;
            this.owner = owner;
            this.rand = new Random(id++);
            
            this.IsAggressive = true;
            
            world.GetMap().GetTile((int)position.X, (int)position.Y).AddUnit(this);
            world.AddUnit(this);
        }

        #endregion

        #region Public Properties

        public Entity BaseEntity { get; set; }

        public float Buff { get; set; }

        public float DisperseDistance
        { 
            get
            {
                return this.IsAggressive ? this.disperseDistance : 0.5f;
            }

            set
            {
                this.disperseDistance = value;
            }
        }

        public bool IsAggressive { get; set; }

        public Entity MissionEntity
        {
            get
            {
                return this.missionEntity;
            }

            set
            {
                this.callRainCheckOnTarget();
                this.missionEntity = value;
                if (this.missionEntity is Building && this.missionEntity.owner == this.owner)
                {
                    ((Building)this.missionEntity).incomingUnits.Add(this);
                }
            }
        }

        public float PowerLevel
        {
            get
            {
                return this.powerLevel + this.owner.PowerLevel + this.Buff;
            }

            set
            {
                this.powerLevel = value;
            }
        }

        public float SpeedLevel
        {
            get
            {
                return this.owner.SpeedLevel/(10*3) + this.speedLevel;
            }

            set
            {
                this.speedLevel = value;
            }
        }

        public Entity TargetEntity { get; set; }

        public bool isDead { get; set; }

        public bool returnToBase { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void SetWorld(World w)
        {
            Unit.world = w;
        }


        // Graphical representation

        /// <summary>
        /// Returns texture for a unit.
        /// </summary>
        /// <returns>Texture of this unit.</returns>
        public override Texture2D GetSprite()
        {
            return UNIT_TEXTURE;
        }

        public bool IsAtBase()
        {
            if (this.BaseEntity is Building && ! ((Building)this.BaseEntity).units.Contains(this))
            {
                return false;
            }

            return Vector2.Distance(this.BaseEntity.position, this.position) <= this.DisperseDistance;
        }

        public void Kill()
        {
			if (this.PowerLevel >= this.rand.NextDouble())
			{
				// We survive! WOO!
				return;
			}

			// We die! NOO!
			this.isDead = true;
			SoundsController.playSound("Celldeath", this.position);
            UnitController.MarkUnitAsDead(this);
        }
        
        public void RemoveFromWorld()
		{
			world.GetMap().GetTile((int)this.position.X, (int)this.position.Y).RemoveUnit(this.owner, this);
			world.RemoveUnit(this);
			this.callRainCheckOnTarget();
			if (this.BaseEntity != null && this.BaseEntity is Building)
			{
				((Building)this.BaseEntity).RemoveUnit(this);
			}
        }

        /// <summary>
        /// Cool override of XNA Update function.
        /// </summary>
        /// <param name="systemTime">Time variable passed from XNA main loop.</param>
        public void Update(int systemTime)
        {
            if (! this.isDead)
            {
				this.targetPosition = this.CalculateTargetPosition();
				
				this.Move(systemTime);
				this.stopMovingIfGoalIsReached();
            }
		}

        #endregion

        #region Methods

        virtual protected bool stopMovingIfGoalIsReached()
		{
			float distance = float.MaxValue;
			Vector2 here = this.position;
			Vector2 there = this.targetPosition;
			
			Vector2.Distance(ref here, ref there, out distance);
			
			if (distance == 0)
			{
				if (this.TargetEntity != null)
				{
					this.ActOnEntity(this.TargetEntity);
					this.TargetEntity = null;
				}
				else if (this.MissionEntity != null)
				{
					this.ActOnEntity(this.MissionEntity);
					this.MissionEntity = null;
				}

				// We neither have a target or a mission, return to base!
				if (this.TargetEntity == null && this.MissionEntity == null)
				{
					if (this.BaseEntity == null)
					{
						// If no home exists, call current tile home and turn passive.
						this.BaseEntity = world.GetMap().GetTile(this.GetPosition());
						this.IsAggressive = false; // free kills!
					}
					else if (this.BaseEntity is Building && ! ((Building)this.BaseEntity).IsAlive())
					{
						// If home just died, call the tile base of that home our home.
                        if (((Building)this.BaseEntity).Parent != null)
                        {
                            this.BaseEntity = ((Building)this.BaseEntity).Parent;
                        }
                        else
                        {
                            this.BaseEntity = world.GetMap().GetTile(this.BaseEntity.GetPosition());
                            this.IsAggressive = false; // free kills!
                        }
					}
					else
					{
						this.DisperseDistance = 1.5f;
					}

					this.returnToBase = true;
				}

				return true;
			}
			else
			{
				return false;
			}
		}
        

        /// <summary>
        /// Should run when we have reached a target.
		/// This method decides what to do with the target.
        /// </summary>
        private void ActOnEntity(Entity ent)
        {
			if (ent is Tile)
			{
				return;
			}

			if (ent.owner == this.owner)
			{
				
				if (ent is Building)
				{
					var targetBuilding = (Building)ent;
					
					if (targetBuilding.IsAlive())
					{
					    this.BaseEntity = targetBuilding;

					    targetBuilding.AddUnit(this);
					    targetBuilding.incomingUnits.Remove(this);

					    // We will now recieve new positions within a radius of our secondary baseEntity.
					}
				}
				
			}
			else
			{
			    // If this is an enemy! KILL IT! OMG				
				
				if (ent is Unit && !((Unit)ent).isDead)
				{
					var targetUnit = (Unit)ent;
					if (! targetUnit.isDead)
					{
						this.Kill();
						((Unit)ent).Kill();
					}
				}
				else if (ent is Building)
				{
					var targetBuilding = (Building)ent;
					
					if (targetBuilding.IsAlive())
					{
						this.Kill();
						BuildingController.HurtBuilding((Building)ent);
					}
				}
				
			}
        }

        private Vector2 CalculateTargetPosition()
        {
            if (this.TargetEntity != null)
            {
                return this.TargetEntity.position;
            }
            else if (this.MissionEntity != null)
            {
                // If we target a tile, we want to be in the middle of it.
                return this.MissionEntity.position;
            }
            else if (this.BaseEntity != null && this.returnToBase)
            {
                this.returnToBase = false;
				
                if (Vector2.Distance(this.BaseEntity.position, this.position) > this.DisperseDistance)
                {
                    // We are to far away from base! Return to base origo!
                    this.MissionEntity = this.BaseEntity;
                    return this.CalculateTargetPosition();
                }
                else
                {
                    // Pick random point around base within DisperseDistance
                    double angle = this.rand.NextDouble() * 2 * Math.PI;
                    double distance = this.rand.NextDouble() * this.DisperseDistance;

                    return this.BaseEntity.position + (new Vector2((float)(Math.Cos(angle) * distance), 
                                                          (float)(Math.Sin(angle) * distance)));
                }
            }
            else
            {
                return this.targetPosition;
            }
        }
		

        /// <summary>
        /// Internal move logic. Uses targetPosition.
        /// </summary>
        private void Move(float deltaTime)
        {
            var beforeX = (int)this.position.X;
            var beforeY = (int)this.position.Y;

            Vector2 direction = Vector2.Subtract(this.targetPosition, this.position);
            direction.Normalize();

            // Move unit towards baseEntity.
            if (this.targetPosition.X != NO_TARGET)
            {
                float distance = this.targetPosition.X - this.position.X;

                if (Math.Abs(distance) < (this.movement_speed + this.SpeedLevel))
                {
                    this.position = new Vector2(this.targetPosition.X, this.position.Y);
                }
                else
                {
                    float newX = this.position.X + (this.movement_speed + this.SpeedLevel) * deltaTime * direction.X * direction.Length();
                    this.position = new Vector2(newX, this.position.Y);
                }
            }

            if (this.targetPosition.Y != NO_TARGET)
            {
                float distance = this.targetPosition.Y - this.position.Y;

                if (Math.Abs(distance) < (this.movement_speed + this.SpeedLevel))
                {
                    this.position = new Vector2(this.position.X, this.targetPosition.Y);
                }
                else
                {
                    float newY = this.position.Y + (this.movement_speed + this.SpeedLevel) * deltaTime * direction.Y * direction.Length();
                    this.position = new Vector2(this.position.X, newY);
                }
            }
			
            var afterX = (int)this.position.X;
            var afterY = (int)this.position.Y;

            // Tile management!
            if (afterX != beforeX || afterY != beforeY)
            {
                Unit.world.map.GetTile(beforeX, beforeY).RemoveUnit(this);
                Unit.world.map.GetTile(afterX, afterY).AddUnit(this);
                Unit.world.map.GetTile(afterX, afterY).MakeVisibleTo(this.owner);

                // Let's update the fog of war!
                /*for (int i = -3; i <= 3; i++)
				{
					for (int j = -3; j <= 3; j++)
					{
						try
						{
							Unit.world.map.GetTile(afterX + j, afterY + i).MakeVisibleTo(this.owner);
						}
						catch (IndexOutOfRangeException e)
						{
						}
					}
				}*/
            }
        }

        /// <summary>
        /// Should be called when the course has been changed and the unit will not be able to reach a friendly building.
        /// </summary>
        private void callRainCheckOnTarget()
        {
            if (this.missionEntity is Building && this.missionEntity.owner == this.owner)
            {
                ((Building)this.missionEntity).incomingUnits.Remove(this);
            }
        }

        #endregion
    }
}
