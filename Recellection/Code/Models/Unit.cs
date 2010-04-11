using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private int tileX;  // Current tile
        private int tileY;  // Current tile
        private int targetX;    // Target coordinate
        private int targetY;    // Target coordinate
        private int x;  
        private int y;
        private int angle;
        private bool isDispersed;
        private bool isDead;

        //private Sprite sprite;


        // METHODS
        
        // Constructors
        public Unit()
        {
            this.tileX = this.tileY = this.targetX = this.targetY = this.x = this.y = this.angle = 0;
            this.isDispersed = this.isDead = false;
        }
        public Unit(int posX, int posY)
        {

            this.isDispersed = this.isDead = false;
        }
        public Unit(int posX, int posY, int angle)
        {

            this.isDispersed = this.isDead = false;
        }
        //public Unit()
        //{

        //    this.isDispersed = this.isDead = false;
        //}
        //public Unit()
        //{

        //    this.isDispersed = this.isDead = false;
        //}

        // Properites
        public bool IsDead()
        {
            return isDead;
        }
        public Player GetPlayer()
        {
            return null;
        }
        public int GetTargetX()
        {
            return 0;
        }
        public int GetTargetY()
        {
            return 0;
        }
        public void SetTargetX(int tx)
        {

        }
        public void SetTargetY(int ty)
        {

        }
        public int GetXTile()
        {
            return 0;
        }
        public int GetYTile()
        {
            return 0;
        }
        public void SetDispersed(bool set)
        {

        }
        public bool IsDispersed()
        {
            return isDispersed;
        }
        
        // Graphical representation
        public Texture GetSprite()
        {
            return null;
        }
        public int GetXOffset()
        {
            return 0;
        }
        public int GetYOffset()
        {
            return 0;
        }
        public int GetAngle()
        {
            return 0;
        }
        
        // Modifiers
        public void Kill()
        {
            this.isDead = true;
        }
        public void Update(int systemTime)
        {
            if (!this.isDead)
            {
                this.Move();
            }
        }

        private void Move()
        {
            //TODO: Move unit towards target.
            // Cool trigonometrical functions 'n shit.
        }
    }
}
