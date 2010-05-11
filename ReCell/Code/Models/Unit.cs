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
    public class Unit : IModel
    {
        // DATA
        private Vector2 position;   // Current tile
        private Vector2 target;     // Target coordinate
        private int angle;          // Angle of unit, for drawing
        private bool isDispersed;   // Whether or not this unit should recieve a new target from the dispersion procedure
        private bool isDead;        // Status of unit
        private float powerLevel;
        private Player owner;
        private static World world;


        private const float MOVEMENT_SPEED = 0.01f;
        private const float NO_TARGET = -1;
        private const float TARGET_THRESHOLD = 0.05f;

        // METHODS

        #region Constructors

        /// <summary>
        /// Creates a "default unit".
        /// </summary>
        public Unit(Player owner)
        {
            this.position = new Vector2(0, 0);
            this.target = new Vector2(NO_TARGET,NO_TARGET);
            this.angle = 0;
            this.isDispersed = this.isDead = false;
            this.owner = owner;
        }
        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="posX">Unit x-coordinate.</param>
        /// <param name="posY">Unit y-coordinate.</param>
        public Unit(Player owner, float posX, float posY)
        {
            this.position = new Vector2(posX, posY);
            this.target = new Vector2(NO_TARGET, NO_TARGET);
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
        {
            this.position = position;
            this.target = new Vector2(NO_TARGET, NO_TARGET);
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

        /// <summary>
        /// Returns status of this Unit.
        /// </summary>
        /// <returns>True if dead, False if alive.</returns>
        public bool IsDead()
        {
            return isDead;
        }
        /// <summary>
        /// Returns the player which owns this Unit.
        /// </summary>
        /// <returns>Player that owns this Unit.</returns>
        public Player GetOwner()
        {
            return this.owner;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>X and Y coordinates of target tile.</returns>
        public Vector2 GetTarget()
        {
            return this.target;
        }
        /// <summary>
        /// Change target.
        /// </summary>
        /// <param name="newTarget">X and Y coordinates of new target.</param>
        public void SetTarget(Vector2 newTarget)
        {
            this.target = newTarget;
        }
        /// <summary>
        /// Get current position of this Unit.
        /// </summary>
        /// <returns>X and Y coordinates for tile.</returns>
        public Vector2 GetPosition()
        {
            return this.position;
        }
        /// <summary>
        /// Magically teleport this Unit somewhere.
        /// </summary>
        /// <param name="newPos">X and Y coordinate of destination tile.</param>
        public void SetPosition(Vector2 newPos)
        {
            this.position = newPos;
        }
        /// <summary>
        /// Set whether or not this unit should recieve a new 
        /// target from the dispersion procedure
        /// </summary>
        /// <param name="set">Self-explanatory boolean value.</param>
        public void SetDispersed(bool set)
        {
            this.isDispersed = set;
        }
        /// <summary>
        /// Get whether or not this unit should recieve a new 
        /// target from the dispersion procedure.
        /// </summary>
        /// <returns>Self-explanatory boolean.</returns>
        public bool IsDispersed()
        {
            return isDispersed;
        }


        // Graphical representation

        /// <summary>
        /// Returns texture for a unit.
        /// </summary>
        /// <returns>Texture of this unit.</returns>
        public Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.Unit);
        }
        /// <summary>
        /// Get current angle of this unit.
        /// </summary>
        public int GetAngle()
        {
            return this.angle;
        }

        // Modifiers
        /// <summary>
        /// Kill this unit.
        /// </summary>
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

            Unit.world.map.GetTile(y, x).RemoveUnit(this);

            // Move unit towards target.
            if (this.target.X != NO_TARGET)
            {
                if (this.target.X > this.position.X)
                {
                    this.position.X += MOVEMENT_SPEED * deltaTime;
                }
                else if (this.target.X < this.position.X)
                {
                    this.position.X += MOVEMENT_SPEED * deltaTime;
                }
                else
                {
                    this.target.X = NO_TARGET;
                }
            }
            if (this.target.Y != NO_TARGET)
            {
                if (this.target.Y > this.position.Y)
                {
                    this.position.Y += MOVEMENT_SPEED * deltaTime;
                }
                else if (this.target.Y < this.position.Y)
                {
                    this.position.Y += MOVEMENT_SPEED * deltaTime;
                }
            }
            // Reasonably close to target.
            if ((Math.Abs(this.position.X - this.target.X) < TARGET_THRESHOLD) && (Math.Abs(this.position.Y - this.target.Y) < TARGET_THRESHOLD))
            {
                if (!isDispersed)
                {
                    isDispersed = true;
                }
                this.target = new Vector2(NO_TARGET, NO_TARGET);
            }

            // Tile management!

            x = (int)this.position.X;
            y = (int)this.position.Y;

            Unit.world.map.GetTile(y, x).AddUnit(this);
        }
    }
}
