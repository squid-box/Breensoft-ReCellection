using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Controllers;

namespace Recellection.Code.Models
{
    /// <summary>
    /// The representation of a Unit in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    public class Unit : Entity, IModel
    {
        private const long TID_MARCO_SPELAR_STARCRAFT_2 = 15125161231512L;
		private static Logger logger = LoggerFactory.GetLogger();
		private static int id = 0; // Used for randomness
		
		protected Vector2 targetPosition = new Vector2(NO_TARGET, NO_TARGET);
		
        // DATA
		private Entity targetEntity = null;   // Target entity
        public Entity TargetEntity
        {
			get
			{
				return targetEntity;
			}
			set
			{
				targetEntity = value;
			}
        }
		private Entity missionEntity = null;     // Mission entity
		public Entity MissionEntity
		{
			get
			{
				return missionEntity;
			}
			set
			{
				callRainCheckOnTarget();
				missionEntity = value;
				if (missionEntity is Building && missionEntity.owner == this.owner)
				{
					((Building)missionEntity).incomingUnits.Add(this);
				}
			}
		}
		// Target to fall back to if the primary baseEntity disappears. Also acts as center of dispersion
		private Entity baseEntity = null;
        public Entity BaseEntity
        {
			get
			{
				return baseEntity;
			}
			set
			{
				baseEntity = value;
			}
		}
		
		private float disperseDistance = 1.5f;
        public float DisperseDistance
        { 
			get
			{
				return (IsAggressive ? disperseDistance : 0.5f);
			}
			set
			{
				disperseDistance = value;
			}
        }
        
		public bool returnToBase { get; set; }         // Whether or not this unit should recieve a new baseEntity from the dispersion procedure

		public bool isDead { get; set; }              // Status of unit
		public float powerLevel;
		public float PowerLevel
		{
			get
			{
				return powerLevel + owner.PowerLevel + Buff;
			}
			set
			{
				powerLevel = value;
			}
		}

		public float speedLevel;
		public float SpeedLevel
		{
			get
			{
				return speedLevel;
			}
			set
			{
				speedLevel = value;
			}
		}
		
		public float Buff { get; set; }
		
		public bool IsAggressive { get; set; }
		
        private readonly static Texture2D UNIT_TEXTURE = Recellection.textureMap.GetTexture(Globals.TextureTypes.Unit);
		
        private static World world;

		/// <summary>
		/// Should be called when the course has been changed and the unit will not be able to reach a friendly building.
		/// </summary>
		private void callRainCheckOnTarget()
		{
			if (missionEntity is Building && missionEntity.owner == this.owner)
			{
				((Building)missionEntity).incomingUnits.Remove(this);
			}
		}
		
		private Random rand;

		// Did I mention that I hate floats? // Martin
        protected float movement_speed = 0.01f;
        private const float NO_TARGET = -1;
        
        // METHODS

        #region Constructors

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
			this.returnToBase = (baseEntity != null);
			
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
        
        public void Kill()
        {
			if (this.PowerLevel >= rand.NextDouble())
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
			world.GetMap().GetTile((int)position.X, (int)position.Y).RemoveUnit(owner, this);
			world.RemoveUnit(this);
			callRainCheckOnTarget();
			if (BaseEntity != null && BaseEntity is Building)
			{
				((Building)BaseEntity).RemoveUnit(this);
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
				targetPosition = CalculateTargetPosition();
				this.Move(systemTime);
				stopMovingIfGoalIsReached();
            }
		}

		private Vector2 CalculateTargetPosition()
		{
			if (TargetEntity != null)
			{
				return TargetEntity.position;
			}
			else if (MissionEntity != null)
			{
				return MissionEntity.position;
			}
			else if (BaseEntity != null && returnToBase)
			{
				returnToBase = false;
				
				if (Vector2.Distance(BaseEntity.position, this.position) > DisperseDistance)
				{
					// We are to far away from base! Return to base origo!
					MissionEntity = BaseEntity;
					return CalculateTargetPosition();
				}
				else
				{
					// Pick random point around base within DisperseDistance
					double angle = rand.NextDouble() * 2 * Math.PI;
					double distance = rand.NextDouble() * (double)DisperseDistance;

					return BaseEntity.position + (new Vector2((float)(Math.Cos(angle) * distance), 
															  (float)(Math.Sin(angle) * distance)));
				}
			}
			else
			{
				return targetPosition;
			}
		}

		/// <summary>
		/// Internal move logic. Uses targetPosition.
		/// </summary>
		private void Move(float deltaTime)
		{
			int beforeX = (int)this.position.X;
			int beforeY = (int)this.position.Y;

            Vector2 direction = Vector2.Subtract(this.targetPosition, this.position);
            direction.Normalize();

			// Move unit towards baseEntity.
			if (this.targetPosition.X != NO_TARGET)
			{
				float distance = this.targetPosition.X - this.position.X;

				if (Math.Abs(distance) < (movement_speed + speedLevel))
				{
					position = new Vector2(targetPosition.X, position.Y);
				}
                else
                {
                    float newX = position.X + (movement_speed + speedLevel) * deltaTime * direction.X * direction.Length();
                    position = new Vector2(newX, position.Y);
                }
			}
			if (this.targetPosition.Y != NO_TARGET)
			{
				float distance = this.targetPosition.Y - this.position.Y;

				if (Math.Abs(distance) < (movement_speed + (speedLevel * 0.1)))
                {
					position = new Vector2(position.X, targetPosition.Y);
                }
                else
                {
					float newY = position.Y + (movement_speed + speedLevel) * deltaTime * direction.Y * direction.Length();
                    position = new Vector2(position.X, newY);
                }
			}
			
			int afterX = (int)this.position.X;
			int afterY = (int)this.position.Y;

			// Tile management!
			if (afterX != beforeX || afterY != beforeY)
			{
				Unit.world.map.GetTile(beforeX, beforeY).RemoveUnit(this);
				Unit.world.map.GetTile(afterX, afterY).AddUnit(this);
			}
		}

		virtual protected bool stopMovingIfGoalIsReached()
		{
			float distance = float.MaxValue;
			Vector2 here = position;
			Vector2 there = targetPosition;
			
			Vector2.Distance(ref here, ref there, out distance);
			
			if (distance == 0)
			{
				if (TargetEntity != null)
				{
					ActOnEntity(TargetEntity);
					TargetEntity = null;
				}
				else if (MissionEntity != null)
				{
					ActOnEntity(MissionEntity);
					MissionEntity = null;
				}

				// We neither have a target or a mission, return to base!
				if (TargetEntity == null && MissionEntity == null)
				{
					if (BaseEntity == null)
					{
						// If no home exists, call current tile home.
						DisperseDistance = 0.5f;
						BaseEntity = world.GetMap().GetTile(this.GetPosition());
					}
					else
					{
						DisperseDistance = 1.5f;
					}

					returnToBase = true;
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
				#region Friendly actions!
				if (ent is Building)
				{
					Building targetBuilding = (Building)ent;
					
					if (targetBuilding.IsAlive())
					{
						BaseEntity = targetBuilding;
						
						targetBuilding.AddUnit(this);
						targetBuilding.incomingUnits.Remove(this);
						// We will now recieve new positions within a radius of our secondary baseEntity.
					}
				}
				#endregion
			}
			else // If this is an enemy! KILL IT! OMG				
			{
				#region Try to kill enemies!
				if (ent is Unit && !((Unit)ent).isDead)
				{
					Unit targetUnit = (Unit)ent;
					if (! targetUnit.isDead)
					{
						this.Kill();
						((Unit)ent).Kill();
					}
				}
				else if (ent is Building)
				{
					Building targetBuilding = (Building)ent;
					
					if (targetBuilding.IsAlive())
					{
						this.Kill();
						BuildingController.HurtBuilding((Building)ent);
					}
				}
				#endregion
			}
        }
        
        public bool IsAtBase()
		{
			if (BaseEntity is Building && ! ((Building)BaseEntity).units.Contains(this))
			{
				return false;
			}
			return Vector2.Distance(BaseEntity.position, this.position) <= DisperseDistance;
        }
    }
}
