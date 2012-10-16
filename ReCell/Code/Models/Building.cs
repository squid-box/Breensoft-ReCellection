namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// This is the abstract class Building, every
    /// type of fromBuilding will inherit this class.
    /// 
    /// Author: John Forsberg
    /// </summary>
    public abstract class Building : Entity, IModel
    {
        #region Constants

        protected const int AGGRESSIVE_BUILDING_COST = 5;

        protected const int AGGRESSIVE_BUILDING_HEALTH = 40;

        protected const int BARRIER_BUILDING_COST = 5;

        protected const int BARRIER_BUILDING_HEALTH = 30;

        protected const int BASE_BUILDING_COST = 20;

        protected const int BASE_BUILDING_HEALTH = 100;

        protected const int RESOURCE_BUILDING_COST = 15;

        protected const int RESOURCE_BUILDING_HEALTH = 20;

        #endregion

        // Simple valuesa
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        #endregion

        #region Fields

        private Building parent;

        #endregion

        // Events
        #region Constructors and Destructors

        /// <summary>
        /// Creates an unusable fromBuilding with everything set at default values.
        /// </summary>
        public Building():this("noName", -1, -1, 1, null, 
            Globals.BuildingTypes.NoType, null, new LinkedList<Tile>())
        {
            logger.Trace("Constructing new Building with default values");   
        }

        /// <summary>
        /// Creates a fromBuilding with specified parameters, the unit list will
        /// be initiated but empty and the current health will be set at maxHealth.
        /// Regarding the controlZone the first tile should be the 
        /// tile the fromBuilding is standing on.
        /// </summary>
        /// <param name="name">The name for the fromBuilding TODO Decide if this is
        /// needded</param>
        /// <param name="posX">The x tile coordinate</param>
        /// <param name="posY">The y tile coordinate</param>
        /// <param name="maxHealth">The max health of this fromBuilding</param>
        /// <param name="owner">The player that owns the fromBuilding</param>
        /// <param name="type">The </param>
        /// <param name="baseBuilding">The Base Building this fromBuilding belongs
        /// <param name="controlZone">The nine tiles around the fromBuilding
        /// and the tile the fromBuilding is on.</param>
        /// to</param>
        public Building(string name, int posX, int posY, int maxHealth, 
            Player owner, Globals.BuildingTypes type, BaseBuilding baseBuilding, 
            LinkedList<Tile> controlZone) : base(new Vector2(posX + 0.5f, posY + 0.5f), owner)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException("maxHealth", 
                    "The max of health may not be zero or less");

            }

            logger.Trace("Constructing new Building with choosen values");
            this.name = name;
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;

			this.units = new List<Unit>();
			this.incomingUnits = new List<Unit>();
            this.type = type;
            this.IsAggressive = true;

            this.baseBuilding = baseBuilding;

            if (baseBuilding != null)
            {
                this.Accept(baseBuilding);
            }

			this.controlZone = controlZone;
			
			foreach (Tile t in controlZone)
			{
				t.unitsChanged += this.UpdateAggressiveness;
			}
        }

        #endregion

        #region Public Events

        public event Publish<Building> healthChanged;
        public event Publish<Building> unitsChanged;

        #endregion

        #region Public Properties

        public bool IsAggressive { get; set; }

        public Building Parent
        {
            get
            {
                while (this.parent != null && ! this.parent.IsAlive())
                {
                    this.parent = this.parent.Parent;
                }

                return this.parent;
            }

            set
            {
                this.parent = value;
            }
        }

        public BaseBuilding baseBuilding { get; protected set; }

        public LinkedList<Tile> controlZone { get; protected set; }

        public int currentHealth { get; protected set; }

        public List<Unit> incomingUnits { get; internal set; }

        public int maxHealth { get; protected set; }

        public string name { get; protected set; }

        public Globals.BuildingTypes type { get; protected set; }

        public List<Unit> units { get; protected set; }

        #endregion

        #region Public Methods and Operators

        /// <returns>Returns the buy price for a fromBuilding, it is set
        /// at its health divided by 10. Upkeep should be added elsewhere.</returns>
        public static uint GetBuyPrice(Globals.BuildingTypes type)
        {
            switch (type)
            {
                case Globals.BuildingTypes.Base:
                    return BASE_BUILDING_COST;
                case Globals.BuildingTypes.Aggressive:
                    return AGGRESSIVE_BUILDING_COST;
                case Globals.BuildingTypes.Barrier:
                    return BARRIER_BUILDING_COST;
                case Globals.BuildingTypes.Resource:
                    return RESOURCE_BUILDING_COST;

            }

            return 0;
        }

        /// <summary>
        /// Part of visitor pattern
       /// </summary>
       /// <param name="visitor">The Base Building this fromBuilding belongs to
       /// </param>

        public void Accept(BaseBuilding visitor)
        {
            visitor.Visit(this);
        }

        // public abstract Texture2D GetSprite();

        /// <summary>
        /// Add one unit to the unit list if the fromBuilding is alive
        /// </summary>
        /// <param name="unit">The Unit to add to the list</param>
        /// <exception cref="ArgumentNullException">The Unit to add was null
        /// </exception>
        /// <exception cref="BuildingNotAliveException">
        /// The fromBuilding is dead</exception>
        public void AddUnit(Unit unit)
        {
            if(unit == null)
            {
                throw new ArgumentNullException("unit", 
                    "The given parameter unit was null");
            }

            if (this.IsAlive())
            {
                this.units.Add(unit);
                if (this.unitsChanged != null)
                {
                    // I'm sorry for this ugly hax - John
                    var temp = new List<Unit>();
                    temp.Add(unit);
                    this.unitsChanged(this, new BuildingEvent(this, temp, 
                        EventType.ADD));
                }
            }
            else
            {
                throw new BuildingNotAliveException();
            }
        }

        /// <summary>
        /// Add a collection of units to the unit List
        /// </summary>
        /// <param name="units">The collection of units to add</param>
        public void AddUnits(IEnumerable<Unit> units)
        {
            
            if (this.IsAlive())
            {
                this.units.AddRange(units);

                if (this.unitsChanged != null)
                {
                    this.unitsChanged(this, new BuildingEvent(this, units, 
                        EventType.ADD));
                }
            }
		}

        /// <summary>
        /// Returns the number of units the fromBuilding has appointed to itself including units who are on their way.
        /// </summary>
        /// <returns>A positive integer representing the number of units
        /// in the list.</returns>
        public virtual int CountTotalUnits()
        {
            return this.units.Count + this.incomingUnits.Count;
        }

        /// <summary>
        /// Returns the number of units the fromBuilding has appointed to itself.
        /// </summary>
        /// <returns>A positive integer representing the number of units
        /// in the list.</returns>
        public virtual int CountUnits()
        {
            return this.units.Count;
        }

        // Modifiers

        /// <summary>
        /// Reduces health for a fromBuilding by the amount specified in the
        /// parameter. It can change the current health to a negative value.
        /// </summary>
        /// <param name="dmgHealth">The amount of damage to cause to the
        /// fromBuilding</param>
        public void Damage(int dmgHealth)
        {
            // TODO Verify if there should be logic here to detirmine if it dies
            if (this.IsAlive())
            {
                this.currentHealth -= dmgHealth;
                if (this.healthChanged != null)
                {
                    this.healthChanged(this, new Event<Building>(this, EventType.REMOVE));
                }
            }
        }

        /// <summary>
        /// Returns a number between 0 and 100, it is an integer
        /// representing how many % of the buildings health is left.
        /// </summary>
        /// <returns>A number  that is [0,100].</returns>
        public int GetHealthPercentage()
        {
            return (this.currentHealth * 100) / this.maxHealth;
        }

        /// <returns>Returns an IEnumerable which can iterate over the list 
        /// of units</returns>
        public List<Unit> GetUnits()
        {
            lock (this.units)
            {
                return this.units;
            }
        }

        /// <summary>
        /// Checks if the health of the Building is more then zero
        /// </summary>
        /// <returns>If the current health is more then zero
        /// it returns true otherwise false</returns>
        public bool IsAlive()
        {
            return this.currentHealth > 0;
        }

        /// <summary>
        /// Makes sure this building is _really_ dead.
        /// </summary>
        public void Kill()
        {
            this.currentHealth = -1;
            if (this.healthChanged != null)
            {
                this.healthChanged(this, new Event<Building>(this, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Removes one unit from the Unit list
        /// </summary>
        /// <param name="unit">The Unit to remove</param>
        public void RemoveUnit(Unit unit)
        {
            this.units.Remove(unit);
            if (this.unitsChanged != null)
            {
                // I'm sorry for this ugly hax - John
                var temp = new List<Unit>();
                temp.Add(unit);
                this.unitsChanged(this, new BuildingEvent(this, temp, 
                    EventType.REMOVE));
            }
        }

        /// <summary>
        /// Removes a collection of units from the unit List,
        /// </summary>
        /// <param name="units">The collection of units to remove</param>
        public void RemoveUnits(IEnumerable<Unit> units)
        {
            foreach (Unit u in units)
            {
                this.units.Remove(u);
            }

            if (this.unitsChanged != null)
            {
                this.unitsChanged(this, new BuildingEvent(this, units, 
                    EventType.REMOVE));
            }
        }

        /// <summary>
        /// Increases health for a fromBuilding by the amount specified in the
        /// parameter. It can not heal it above max health.
        /// </summary>
        /// <param name="health">The ammount to repair the fromBuilding</param>
        public void Repair(int health)
        {
            if (this.IsAlive())
            {
                if (this.currentHealth + health > this.maxHealth)
                {
                    this.currentHealth = this.maxHealth;
                }
                else
                {
                    this.currentHealth += health;
                }

                if (this.healthChanged != null)
                {
                    this.healthChanged(this, new Event<Building>(this, EventType.ADD));
                }
            }
        }

        public void UpdateAggressiveness(object publisher, Event<IEnumerable<Unit>> ev)
        {
            if (ev.type == EventType.ADD)
            {
                foreach(Unit u in ev.subject)
                {
                    if (u.BaseEntity == this)
                    {
                        u.IsAggressive = this.IsAggressive;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Exception for when a fromBuilding is not alive.
        /// This is serious enough to have its own exception.
        /// </summary>
        public class BuildingNotAliveException : Exception
        {
            #region Static Fields

            private static string msg = "A unit can not be added to a "+
                "building which is not alive.";

            #endregion

            #region Constructors and Destructors

            public BuildingNotAliveException()
                : base(msg)
            {
            }

            #endregion
        }
    }
}
