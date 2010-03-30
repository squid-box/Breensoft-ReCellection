using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Models
{
    abstract class Building
    {
        /**
         * Variables 'n stuff.
         */
        // Simple values
        private string name;
        private int posX;
        private int posY;

        // References
        private Player owner;
        private List<Unit> units;

        /**
         * Methods 'n things.
         */

        // Part of visitor pattern
        public void Accept(BaseBuilding visitor);
        
        public Player GetPlayer()
        {
            return this.owner;
        }

        public List<Unit> GetUnits()
        {
            return this.units;
        }
        public void AddUnits(Unit[] units)
        {
            
        }

        // Properties
        public string GetName()
        {
            return this.name;
        }
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
