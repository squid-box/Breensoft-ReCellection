using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
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
        private int currentHealth;
        private int maxHealth;

        // References
        private Player owner;
        private List<Unit> units;
        private BuildingType type;
        private BaseBuilding baseBuilding;

        /**
         * Methods 'n things.
         */

        // Part of visitor pattern
        public void Accept(BaseBuilding visitor)
        {
            visitor.Visit(this);
        }
        
        public Player GetPlayer()
        {
            return this.owner;
        }

        public List<Unit> GetUnits()
        {
            return this.units;
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
        }
        public void AddUnits(Unit[] units)
        {
            //TODO Find someone who knows how to do this?
        }

        // Properties
        public string GetName()
        {
            return this.name;
        }

        public BuildingType GetType()
        {
            return this.type;
        }

        public Texture GetSprite()
        {
            //TODO When the sprite map is done add code here
            return null;
        }

        public int GetX()
        {
            return this.posX;
        }
        public int GetY()
        {
            return this.posY;
        }

        public BaseBuilding GetBase()
        {
            return this.baseBuilding;
        }
        public int GetHealth()
        {
            return this.currentHealth;
        }

        public int GetHealthMax()
        {
            return this.maxHealth;
        }
        public int GetHealthPercentage()
        {
            //TODO Check if it really should be an int that is returned.
            return ((this.currentHealth * 100) / this.maxHealth);
        }


        // Modifiers
        public void damage(int dmgHealth)
        {
            //TODO Verify if there should be logic here to detirmine if it dies
            this.currentHealth -= dmgHealth;
        }
        public void repair(int health)
        {
            this.currentHealth += health;
        }
    }
}
