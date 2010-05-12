using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    /// <summary>
    /// The representation of a Unit in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    public class Unit : Entity, IModel
    {
        // DATA
        public Vector2 targetPosition { get; set; }   // Target coordinate
        public Entity targetEntity { get; set; }      // Target entity
        public Entity defaultTarget { get; set; }		// Target to fall back to if the primary target disappears. Also acts as center of dispersion
        public bool isDispersed { get; set; }         // Whether or not this unit should recieve a new target from the dispersion procedure
		public bool hasArrived { get; set; }
        public bool isDead { get; set; }              // Status of unit
        public float powerLevel { get; set; }
        private static World world;


        private const float MOVEMENT_SPEED = 0.01f;
        private const float NO_TARGET = -1;
        private const float TARGET_THRESHOLD = 0.05f;

        // METHODS

        #region Constructors

        /// <summary>
        /// Creates a "default unit".
        /// </summary>
        public Unit(Player owner) : base(new Vector2(NO_TARGET,NO_TARGET), owner)
        {
            this.position = new Vector2(0, 0);
            this.targetPosition = new Vector2(NO_TARGET,NO_TARGET);
            this.angle = 0;
            this.isDispersed = this.isDead = false;
            this.owner = owner;
        }
        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="posX">Unit x-coordinate.</param>
        /// <param name="posY">Unit y-coordinate.</param>
        public Unit(Player owner, float posX, float posY) : base(new Vector2(posX, posY), owner)
        {
            this.position = new Vector2(posX, posY);
            this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
            this.angle = 0;
            this.isDispersed = this.isDead = false;
            this.owner = owner;
        }

		/// <summary>
		/// Creates a unit.
		/// </summary>
		/// <param name="position">Position of unit.</param>
		/// <param name="owner">Owner of this unit.</param>
		public Unit(Player owner, Vector2 position)
			: base(position, owner)
		{
			this.defaultTarget = null;
			this.position = position;
			this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
			this.angle = 0;
			this.isDispersed = this.isDead = false;
			this.owner = owner;
		}

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="position">Position of unit.</param>
        /// <param name="owner">Owner of this unit.</param>
        public Unit(Player owner, Vector2 position, Entity target) : base(position, owner)
        {
			this.defaultTarget = target;
            this.position = position;
            this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
            this.angle = 0;
            this.isDispersed = this.isDead = false;
            this.owner = owner;
        }

        #endregion


        public static void SetWorld(World w)
        {
            Unit.world = w;
        }

        // Properites

		

        private void updateTarget()
        {
			if (targetEntity != null)
			{
				targetPosition = targetEntity.position;
			}
			else if (defaultTarget != null && !this.isDispersed)
			{
				targetEntity = defaultTarget;
				updateTarget();
			}
			targetPosition = new Vector2(NO_TARGET, NO_TARGET);
        }

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
        }

        /// <summary>
        /// Cool override of XNA Update function.
        /// </summary>
        /// <param name="systemTime">Time variable passed from XNA main loop.</param>
        public void Update(int systemTime)
        {
            if (!this.isDead)
            {
				updateTarget();
                this.Move(systemTime);
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
                if (this.targetPosition.X > this.position.X)
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
                if (this.targetPosition.Y > this.position.Y)
                {
                    float newY = position.Y + MOVEMENT_SPEED * deltaTime;
                    position = new Vector2(position.X, newY);
                }
                else if (this.targetPosition.Y < this.position.Y)
                {
                    float newY = position.Y - MOVEMENT_SPEED * deltaTime;
                    position = new Vector2(position.X, newY);
                }
            }
            // If we are reasonably close to target.
			if ((Math.Abs(this.position.X - this.targetPosition.X) < TARGET_THRESHOLD) && (Math.Abs(this.position.Y - this.targetPosition.Y) < TARGET_THRESHOLD))
			{
				hasArrived = true;
				// If we have no primary target we will be dispersed.
				if (targetEntity == null)
				{
					// We will now recieve new positions within a radius of our secondary target.
					isDispersed = true;
				}
				this.targetPosition = new Vector2(NO_TARGET, NO_TARGET);
			}
			else
			{
				isDispersed = false;
			}

            // Tile management!

            x = (int)this.position.X;
            y = (int)this.position.Y;

            Unit.world.map.GetTile(x, y).AddUnit(this);
        }
    }
}
