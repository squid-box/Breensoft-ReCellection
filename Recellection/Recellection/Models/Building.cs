using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.XNA.Vector2D;

namespace Recellection.Models
{
    abstract class Building
    {
        /**
         * Variables 'n stuff.
         */
        private Player player;
        private string name;
        private int posX;
        private int posY;

        /**
         * Methods 'n things.
         */

        // Part of visitor pattern
        public void Accept(BaseBuilding visitor);
        public Player GetPlayer();
        public Unit[] GetUnits();
        public void AddUnits(Unit[] units);

        // Properties
        public string GetName();
        public Globals.BuildingType GetType();
        public Globals.Texture GetSprite();

        public int GetX()
        {
            return this.posX;
        }
        public int GetY()
        {
            return this.posY;
        }
        
        public BaseBuilding GetBase();
        public int GetHealth();
        public int GetHealthMax();
        public int GetHealthPercentage();

        // Modifiers
        public void damage(int dmghealth);
        public void repair(int health);
    }
}
