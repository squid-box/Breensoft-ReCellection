using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Models
{
    /* The representation of a Unit in the game world.
     * 
     * Author: Joel Ahlgren
     * Date: 2010-04-11
     */
    public class Unit
    {
        // DATA
        private Vector2 pos;       // Current tile
        private Vector2 target;     // Target coordinate
        private Vector2 offset;     // The y-offset in pixels of this unit in the world
        private int angle;          // Angle of unit, for drawing
        private bool isDispersed;   // Whether or not this unit should recieve a new target from the dispersion procedure
        private bool isDead;        // Status of unit

        private Player owner;
        //private Sprite sprite;


        // METHODS

        #region Constructors

        /// <summary>
        /// Creates a "default unit".
        /// </summary>
        public Unit()
        {
            this.pos = new Vector2(0, 0);
            this.target = new Vector2();    // No target
            this.offset = new Vector2();    // No offset.
            this.angle = 0;
            this.isDispersed = this.isDead = false;
        }
        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="posX">Tile x-coordinate.</param>
        /// <param name="posY">Tile y-coordinate.</param>
        public Unit(int posX, int posY)
        {
            this.pos = new Vector2(posX, posY);
            this.target = new Vector2();    // No target
            this.offset = new Vector2();    // No offset
            this.angle = 0;
            this.isDispersed = this.isDead = false;
        }
        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="posX">Tile x-coordinate.</param>
        /// <param name="posY">Tile y-coordinate.</param>
        /// <param name="angle">Draw-angle if this unit.</param>
        public Unit(int posX, int posY, int angle)
        {
            this.pos = new Vector2(posX, posY);
            this.target = new Vector2();    // No target
            this.offset = new Vector2();    // No offset
            this.angle = angle;
            this.isDispersed = this.isDead = false;
        }

        #endregion


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
        /// <param name="x">X coordinates of new target.</param>
        /// <param name="y">Y coordinates of new target.</param>
        public void SetTargetX(int x, int y)
        {
            this.target = new Vector2(x, y);
        }
        /// <summary>
        /// Change target.
        /// </summary>
        /// <param name="newTarget">X and Y coordinates of new target.</param>
        public void SetTargetX(Vector2 newTarget)
        {
            this.target = newTarget;
        }
        /// <summary>
        /// Get current position of this Unit.
        /// </summary>
        /// <returns>X and Y coordinates for tile.</returns>
        public Vector2 GetPosition()
        {
            return this.pos;
        }
        /// <summary>
        /// Magically teleport this Unit somewhere.
        /// </summary>
        /// <param name="newPos">X and Y coordinate of destination tile.</param>
        public void SetPosition(Vector2 newPos)
        {
            this.pos = newPos;
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
        /// Not Yet Implemented, waiting for Sprite-class?
        /// </summary>
        /// <returns>null</returns>
        public Texture GetSprite()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetOffset()
        {
            return this.offset;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                this.Move();
            }
        }

        /// <summary>
        /// Internal move logic.
        /// </summary>
        private void Move()
        {
            //TODO: Move unit towards target.
            // Cool trigonometrical functions 'n shit.
        }
    }
}
