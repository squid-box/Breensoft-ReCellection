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
        public boolean IsDead();
        public Player GetPlayer();
        public int GetTargetX();
        public int GetTargetY();
        public void SetTargetX(int tx);
        public void SetTargetY(int ty);
        public int GetXTile();
        public int GetYTile();
        public void SetDispersed(bool set);
        public bool IsDispersed();
        
        // Graphical representation
        public Texture GetSprite();
        public int GetXOffset();
        public int GetYOffset();
        public int GetAngle();
        
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
