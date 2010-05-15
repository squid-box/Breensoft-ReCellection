using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Models
{
    /// <summary>
    /// The representation of a Unit in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    public class Unit : Entity, IModel
    {
		private static Logger logger = LoggerFactory.GetLogger();
		private static int id = 0; // Used for random
        // DATA
        public Vector2 targetPosition { get; set; }   // Target coordinate
        public Entity targetEntity { get; set; }      // Target entity
        public Entity disperseAround { get; set; }		// Target to fall back to if the primary target disappears. Also acts as center of dispersion
        public bool isDispersed { get; set; }         // Whether or not this unit should recieve a new target from the dispersion procedure
		public bool hasArrived { get { return (targetPosition.X == NO_TARGET && targetPosition.Y == NO_TARGET); } }
        public bool isDead { get; set; }              // Status of unit
        public float powerLevel { get; set; }
        private static World world;

		private Random rand;

		// Did I mention that I hate floats? // Martin
        private const float MOVEMENT_SPEED = 0.01f;
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
        public Unit(Player owner, Vector2 position, Entity target) : base(position, owner)
        {
			this.disperseAround = target;
            this.position = position;
            this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
            this.angle = 0;
            this.isDispersed = this.isDead = false;
            this.owner = owner;
            this.rand = new Random(id++);
            world.GetMap().GetTile((int)position.X, (int)position.Y).AddUnit(this);
        }

        #endregion


        public static void SetWorld(World w)
        {
            Unit.world = w;
        }

        // Properites

		


        // Graphical representation

        /// <summary>
        /// Returns texture for a unit.
        /// </summary>
        /// <returns>Texture of this unit.</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.Unit);
        }
        
        public void Kill()
        {
            this.isDead = true;
            world.GetMap().GetTile((int)position.X, (int)position.Y).RemoveUnit(owner, this);
            if (disperseAround != null && disperseAround is Building)
            {
				((Building)disperseAround).RemoveUnit(this);
            }
        }

        /// <summary>
        /// Cool override of XNA Update function.
        /// </summary>
        /// <param name="systemTime">Time variable passed from XNA main loop.</param>
        public void Update(int systemTime)
        {
            if (!this.isDead)
            {
				targetPosition = calculateTargetPosition();
				this.Move(systemTime);
				stopMovingIfGoalIsReached();
            }
		}

		private Vector2 calculateTargetPosition()
		{
			if (targetEntity != null)
			{
				return targetEntity.position;
			}
			else if (this.isDispersed && disperseAround != null)
			{
				// We will wander around our disperseAround
				isDispersed = false;
                //The Floor is to makes sure that the entity does not have an offset for its position (like buildings who have 0.25)
                //Then add 0.5 to end up in the middle of the tile and last the random should random a number between -1.3 to 1.3
				return new Vector2(((float)Math.Floor(disperseAround.position.X))+ 0.5f + ((float)rand.NextDouble() * 2.6f - 1.3f ),
                                        ((float)Math.Floor(disperseAround.position.Y))+ 0.5f + ((float)rand.NextDouble() * 2.6f - 1.3f));
			}
			else
			{
				return targetPosition;
			}
		}

		/// <summary>
		/// Internal move logic.
		/// </summary>
		private void Move(float deltaTime)
		{
			int x = (int)this.position.X;
			int y = (int)this.position.Y;
			Unit.world.map.GetTile(x, y).RemoveUnit(this);

			// Move unit towards target.
			if (this.targetPosition.X != NO_TARGET)
			{
				float distance = this.targetPosition.X - this.position.X;

				if (Math.Abs(distance) < MOVEMENT_SPEED)
				{
					position = new Vector2(targetPosition.X, position.Y);
				}
				else if (this.targetPosition.X > this.position.X)
				{
					float newX = position.X + MOVEMENT_SPEED * deltaTime;
					position = new Vector2(newX, position.Y);
				}
				else if (this.targetPosition.X < this.position.X)
				{
					float newX = position.X - MOVEMENT_SPEED * deltaTime;
					position = new Vector2(newX, position.Y);
				}
			}
			if (this.targetPosition.Y != NO_TARGET)
			{
				float distance = this.targetPosition.Y - this.position.Y;

				if (Math.Abs(distance) < MOVEMENT_SPEED)
				{
					position = new Vector2(position.X, targetPosition.Y);
				}
				else if (distance > 0)
				{
					float newY = position.Y + MOVEMENT_SPEED * deltaTime;
					position = new Vector2(position.X, newY);
				}
				else if (distance < 0)
				{
					float newY = position.Y - MOVEMENT_SPEED * deltaTime;
					position = new Vector2(position.X, newY);
				}
			}

			// Tile management!

			x = (int)this.position.X;
			y = (int)this.position.Y;
			Unit.world.map.GetTile(x, y).AddUnit(this);
		}

		private bool stopMovingIfGoalIsReached()
		{
			// If we are reasonably close to target.
			float dx = this.position.X - this.targetPosition.X;
			float dy = this.position.Y - this.targetPosition.Y;
			double distance = Math.Sqrt(dx*dx + dy*dy);
			
			if (distance == 0)
			{
				if (targetEntity != null)
				{
					// If it's a home-building, we disperse around it :)
					if (targetEntity is Building && targetEntity.owner == this.owner)
					{
						// We will now recieve new positions within a radius of our secondary target.
						this.disperseAround = targetEntity;
						isDispersed = true;
					}
					this.targetEntity = null;
				}
				
				if (disperseAround != null)
				{
					isDispersed = true;
				}
				
				// Set no target, we are here.
				this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
        /// <summary>
        /// Modify or set a new powerlevel for this unit.
        /// </summary>
        /// <param name="newPowerLevel">Default should be one, set a new value as 1.1 for a 10% powerbonus.</param>
        public void SetPowerLevel(float newPowerLevel)
        {
            powerLevel = newPowerLevel;
        }
        
        /// <returns>
        /// Powerlevel of this unit.
        /// </returns>
        public float GetPowerLevel()
        {
            return powerLevel;
        }
    }
}
