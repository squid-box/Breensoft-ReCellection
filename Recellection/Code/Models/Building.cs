using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Utility;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Models
{
    /// <summary>
    /// This is the abstract class Building, every
    /// type of building will inherit this class.
    /// 
    /// Author: John Forsberg
    /// </summary>
    public abstract class Building : IModel
    {
        // Simple values
        public string name { get; protected set; }
        public int posX { get; protected set; }
        public int posY { get; protected set; }
        public int currentHealth { get; protected set; }
        public int maxHealth { get; protected set; }

        // References
        public Player owner { get; protected set; }
        public List<Unit> units { get; protected set; }
        public Globals.BuildingTypes type { get; protected set; }
        public BaseBuilding baseBuilding { get; protected set; }

        private static Logger logger = LoggerFactory.GetLogger();

        //Events
		public event Publish<Building> healthChanged;
		public event Publish<Building> unitsChanged;

        /// <summary>
        /// Creates an unusable building with everything set at default values.
        /// </summary>
        public Building()
        {
            logger.Trace("Constructing new Building with default values");
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
        /// be initiated but empty and the current health will be set at maxHealth.
        /// </summary>
        /// <param name="name">The name for the building TODO Decide if this is
        /// needded</param>
        /// <param name="posX">The x tile coordinate</param>
        /// <param name="posY">The y tile coordinate</param>
        /// <param name="maxHealth">The max health of this building</param>
        /// <param name="owner">The player that owns the building</param>
        /// <param name="type">The </param>
        /// <param name="baseBuilding">The Base Building this building belongs
        /// to</param>
        public Building(String name, int posX, int posY, int maxHealth,
            Player owner, Globals.BuildingTypes type, BaseBuilding baseBuilding)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException("maxHealth", 
                    "The max of health may not be zero or less");

            }

            logger.Trace("Constructing new Building with choosed values");
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
        /// Checks if the health of the Building is more then zero
        /// </summary>
        /// <returns>If the current health is more then zero
        /// it returns true other vice false</returns>
        public bool IsAlive()
        {
            return currentHealth > 0;
        }

        /// <returns>Returns an IEnumerable which can iterate over the list 
        /// of units</returns>
        public IEnumerable<Unit> GetUnits()
        {
            foreach (Unit u in this.units)
            {
                yield return u;
            }
        }

        /// <summary>
        /// Returns the number of units the building has appointed to itself.
        /// </summary>
        /// <returns>A positive integer representing the number of units
        /// in the list.</returns>
        public virtual int CountUnits()
		{
            return units.Count;
        }

        public abstract Texture2D GetSprite();

        /// <summary>
        /// Add one unit to the unit list if the building is alive
        /// </summary>
        /// <param name="unit">The Unit to add to the list</param>
        /// <exception cref="ArgumentNullException">The Unit to add was null
        /// </exception>
        /// <exception cref="BuildingNotAliveException">
        /// The building is dead</exception>
        public void AddUnit(Unit unit)
        {
            if(unit == null)
            {
                throw new ArgumentNullException("unit",
                    "The given parameter unit was null");
            }

            if (IsAlive())
            {
                units.Add(unit);
                if (unitsChanged != null)
                {
                    unitsChanged(this, new BuildingEvent(this, this.units,
                        EventType.ADD));
                }
            }
            else
            {
                throw new BuildingNotAliveException();
            }
        }

        /// <summary>
        /// Removes one unit from the Unit list
        /// </summary>
        /// <param name="unit">The Unit to remove</param>
        public void RemoveUnit(Unit unit)
        {
            this.units.Remove(unit);
            if (unitsChanged != null)
            {
                unitsChanged(this, new BuildingEvent(this, this.units,
                        EventType.REMOVE));
            }
        }

        /// <summary>
        /// Add an array of units to the unit List
        /// </summary>
        /// <param name="units">The array of units to add</param>
        public void AddUnits(Unit[] units)
        {
            if (IsAlive())
            {
                foreach (Unit u in units)
                {
                    this.units.Add(u);
                }
                if (unitsChanged != null)
                {
                    unitsChanged(this, new BuildingEvent(this, this.units,
                        EventType.ADD));
                }
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
            if (unitsChanged != null)
            {
                unitsChanged(this, new BuildingEvent(this, this.units,
                        EventType.REMOVE));
            }
        }

        /// <summary>
        /// Returns a number between 0 and 100, it is an integer
        /// representing how many % of the buildings health is left.
        /// </summary>
        /// <returns>A number  that is [0,100].</returns>
        public int GetHealthPercentage()
        {
            return ((this.currentHealth * 100) / this.maxHealth);
        }


        // Modifiers

        /// <summary>
        /// Reduces health for a building by the amount specified in the
        /// parameter. It can change the current health to a negative value.
        /// </summary>
        /// <param name="dmgHealth">The amount of damage to cause to the
        /// building</param>
        public void Damage(int dmgHealth)
        {
            //TODO Verify if there should be logic here to detirmine if it dies
            if (IsAlive())
            {
                this.currentHealth -= dmgHealth;
                if (healthChanged != null)
                {
                    healthChanged(this, new Event<Building>(this, EventType.REMOVE));
                }
            }
        }

        /// <summary>
        /// Increases health for a building by the amount specified in the
        /// parameter. It can not heal it above max health.
        /// </summary>
        /// <param name="health">The ammount to repair the building</param>
        public void Repair(int health)
        {
            if (IsAlive())
            {
                if (this.currentHealth + health > this.maxHealth)
                {
                    this.currentHealth = this.maxHealth;
                }
                else
                {
                    this.currentHealth += health;
                }
                if (healthChanged != null)
                {
                    healthChanged(this, new Event<Building>(this, EventType.ADD));
                }
            }
        }

        /// <summary>
        /// Exception for when a building is not alive.
        /// This is serious enough to have its own exception.
        /// </summary>
        public class BuildingNotAliveException : Exception
        {
            private static string msg = "A unit can not be added to a "+
                "building which is not alive.";

            public BuildingNotAliveException()
                : base(msg)
            {
            }
        }
    }
}
