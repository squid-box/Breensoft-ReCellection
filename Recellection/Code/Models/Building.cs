using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Utility;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
    public abstract class Building : IModel
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
        private Globals.BuildingTypes type;
        private BaseBuilding baseBuilding;

        //Events
        public event Publish<Building> healthChanged;
        public event Publish<Building> unitsChanged;

        /// <summary>
        /// Creates an unusable building with everything set at defualt values.
        /// </summary>
        public Building()
        {
            this.name = "noName";
            this.posX = -1;
            this.posY = -1;
            this.currentHealth = -1;
            this.maxHealth = -1;
            this.owner = null;
            this.units = new List<Unit>();
            this.type = Globals.BuildingTypes.NoType;
            this.baseBuilding = null;
        }

        /// <summary>
        /// Creates a building with specified parameters, the unit list will
        /// be initated but empty and the current health will be set at maxHealth.
        /// </summary>
        /// <param name="name">The name for the building TODO Decide if this is needded</param>
        /// <param name="posX">The x tile koordinate</param>
        /// <param name="posY">The y tile koordinate</param>
        /// <param name="maxHealth">The max health of this building</param>
        /// <param name="owner">The player that owns the building</param>
        /// <param name="type">The </param>
        /// <param name="baseBuilding"></param>
        public Building(String name, int posX, int posY, int maxHealth,
            Player owner, Globals.BuildingTypes type, BaseBuilding baseBuilding)
        {

            this.name = name;
            this.posX = posX;
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;

            this.owner = owner;
            this.units = new List<Unit>();
            this.type = type;

            this.baseBuilding = baseBuilding;


        }
        /**
         * Methods 'n things.
         */

        // Part of visitor pattern
        public void Accept(BaseBuilding visitor)
        {
            visitor.Visit(this);
        }
        /**
         *Returns the owner of the building
         */
        public Player GetPlayer()
        {
            return this.owner;
        }

        public bool isAlive()
        {
            return GetHealth() > 0;
        }

        /**
         * Returns a list of units if the building is alive
         * else it returns null
         */
        public List<Unit> GetUnits()
        {
            if (isAlive())
            {
                return this.units;
            }
            else
            {
                return null;
            }
        }

        /**
         * Add one unig to the unit list if the building is alive
         */
        public void AddUnit(Unit unit)
        {
            if (isAlive())
            {
                units.Add(unit);

                unitsChanged(this, new Event<Building>(this, EventType.ALTER));
            }
            else
            {
                //TODO Add a notify to notify that it failed.
            }
        }

        public void RemoveUnit(Unit unit)
        {
            this.units.Remove(unit);

            unitsChanged(this, new Event<Building>(this, EventType.ALTER));
        }

        public void AddUnits(Unit[] units)
        {
            if (!isAlive())
            {
                return;
            }
            else
            {
                foreach(Unit u in units){
                    this.units.Add(u);
                }

                unitsChanged(this, new Event<Building>(this, EventType.ALTER));
            }
        }

        public void RemoveUnits(Unit[] units)
        {
            foreach (Unit u in units)
            {
                this.units.Remove(u);
            }

            unitsChanged(this, new Event<Building>(this, EventType.ALTER));
        }

        // Properties
        public string GetName()
        {
            return this.name;
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

        /**
         * Reduces health for a building by the ammount specified in the 
         * parameter
         */
        public void damage(int dmgHealth)
        {
            //TODO Verify if there should be logic here to detirmine if it dies
            if (isAlive())
            {
                this.currentHealth -= dmgHealth;

                healthChanged(this, new Event<Building>(this, EventType.ALTER));
            }
            else
            {
                return;
            }
        }

        /**
         * Increases health for a building by the ammount specified in the 
         * parameter
         */
        public void repair(int health)
        {
            if (isAlive())
            {
                this.currentHealth += health;

                healthChanged(this, new Event<Building>(this, EventType.ALTER));
            }
            else
            {
                return;
            }
        }
    }
}
