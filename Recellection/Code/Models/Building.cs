using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Utility;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    public abstract class Building : IModel
    {
        /**
         * Variables 'n stuff.
         */
        // Simple values
        protected string name;
        protected int posX;
        protected int posY;
        protected int currentHealth;
        protected int maxHealth;

        // References
        protected Player owner;
        protected List<Unit> units;
        protected Globals.BuildingTypes type;
        protected BaseBuilding baseBuilding;

        //Events
		public event Publish<Building, Event<Building>> healthChanged;
		public event Publish<Building, Event<Building>> unitsChanged;

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
        /// <param name="name">The name for the building TODO Decide if this is
        /// needded</param>
        /// <param name="posX">The x tile koordinate</param>
        /// <param name="posY">The y tile koordinate</param>
        /// <param name="maxHealth">The max health of this building</param>
        /// <param name="owner">The player that owns the building</param>
        /// <param name="type">The </param>
        /// <param name="baseBuilding">The Base Building this building belongs
        /// to</param>
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

       /// <summary>
        /// Part of visitor pattern
       /// </summary>
       /// <param name="visitor">The Base Building this building belongs to
       /// </param>

        public void Accept(BaseBuilding visitor)
        {
            visitor.Visit(this);
        }
        /// <summary>
        /// Returns the owner of the building
        /// </summary>
        /// <returns>The Player that owns the building</returns>
        public Player GetPlayer()
        {
            return this.owner;
        }

        /// <summary>
        /// Checks if the health of the Building is more then zero
        /// </summary>
        /// <returns>If the current health is more then zero
        /// it returns true othervice false</returns>
        public bool isAlive()
        {
            return GetHealth() > 0;
        }

        /// <summary>
        /// Returns a list of units if the building is alive else it returns
        /// null
        /// </summary>
        /// <returns>A List of units that belongs to this building</returns>
        private List<Unit> GetUnits()
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

        /// <summary>
        /// Returns the number of units the building has appointed to itself.
        /// </summary>
        /// <returns>A possitive integer representing the number of units
        /// in the list.</returns>
        public virtual int CountUnits()
		{
            return units.Count;
        }

        /// <summary>
        /// Add one unig to the unit list if the building is alive
        /// </summary>
        /// <param name="unit">The Unit to add to the list</param>
        /// <exception cref="ArgumentNullException">The Unit to add was null
        /// </exception>
        public void AddUnit(Unit unit)
        {
            if(unit == null)
            {
                throw new ArgumentNullException("unit",
                    "The given parameter unit was null");
            }

            if (isAlive())
            {
                units.Add(unit);

                unitsChanged(this, new BuildingEvent(this, this.units,
                    EventType.ADD));
            }
            else
            {
                //TODO Add a notification to notify that it failed.
            }
        }

        /// <summary>
        /// Removes one unit fromt the Unit list
        /// </summary>
        /// <param name="unit">The Unit to remove</param>
        public void RemoveUnit(Unit unit)
        {
            this.units.Remove(unit);

            unitsChanged(this, new BuildingEvent(this, this.units,
                    EventType.REMOVE));
        }

        /// <summary>
        /// Add an array of units to the unit List
        /// </summary>
        /// <param name="units">The array of units to add</param>
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

                unitsChanged(this, new BuildingEvent(this, this.units,
                    EventType.ADD));
            }
        }

        /// <summary>
        /// Removes an array of units from the unit List,
        /// </summary>
        /// <param name="units">The array of units to remove</param>
        public void RemoveUnits(Unit[] units)
        {
            foreach (Unit u in units)
            {
                this.units.Remove(u);
            }

            unitsChanged(this, new BuildingEvent(this, this.units,
                    EventType.REMOVE));
        }

        //TODO Decide if they are needed, i will leave them uncommented until
        //it is decided.

        public string GetName()
        {
            return this.name;
        }

        public Texture2D GetSprite()
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

        /// <summary>
        /// Reduces health for a building by the ammount specified in the
        /// parameter. It can change the current health to a negative value.
        /// </summary>
        /// <param name="dmgHealth">The ammount of damage to cause to the
        /// building</param>
        public void Damage(int dmgHealth)
        {
            //TODO Verify if there should be logic here to detirmine if it dies
            if (isAlive())
            {
                this.currentHealth -= dmgHealth;

                healthChanged(this, new Event<Building>(this, EventType.REMOVE));
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Increases health for a building by the ammount specified in the
        /// parameter. It can not heal it above max health.
        /// </summary>
        /// <param name="health">The ammount to repair the building</param>
        public void Repair(int health)
        {
            if (isAlive())
            {
                if (this.currentHealth + health > this.maxHealth)
                {
                    this.currentHealth = this.maxHealth;
                }
                else
                {
                    this.currentHealth += health;
                }

                healthChanged(this, new Event<Building>(this, EventType.ADD));
            }
            else
            {
                return;
            }
        }
    }
}
