using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    public class Unit
    {
        /**
         * DATA
         */
        private int tileX;
        private int tileY;
        private int targetX;
        private int targetY;
        private int x;
        private int y;
        private int angle;
        private bool isDispersed;
        private bool isDead;

        
        /**
         * METHODS
         */
        
        // Constructors
        public Unit()
        {
            this.tileX = this.tileY = this.targetX = this.targetY = this.x = this.y = this.angle = 0;
            this.isDispersed = this.isDead = false;
        }

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
            this.Move();
        }

        private void Move()
        {
            //TODO: Move unit towards target.
        }
    }
}
