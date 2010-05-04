using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Representation of a tile in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-26</date>
    public class Tile : IModel
    {
        // Data
        private TerrainType type;
        private HashSet<Player> visibleTo;
        private Dictionary<Player, HashSet<Unit>> units;
        private Building building;
        public Vector2 position;

        // Events
        public event Publish<IEnumerable<Player>> visionChanged;
        public event Publish<IEnumerable<Unit>> unitsChanged;
        public event Publish<Building> buildingChanged;

        // Methods

        #region Constructors

        /// <summary>
        /// Creates a Tile of Membrane (default) type.
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        public Tile(int x, int y)
        {
            this.type = new TerrainType();
            this.visibleTo = new HashSet<Player>();
            this.units = new Dictionary<Player, HashSet<Unit>>();
            this.position = new Vector2(x, y);
            this.building = null;
        }

        /// <summary>
        /// Creates a Tile of the type 'type'.
        /// </summary>
        /// <param name="type">Enum of the terrain type.</param>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        public Tile(int x, int y, Globals.TerrainTypes type)
        {
            this.type = new TerrainType(type);
            this.visibleTo = new HashSet<Player>();
            this.units = new Dictionary<Player, HashSet<Unit>>();
            this.position = new Vector2(x, y);
            this.building = null;
        }

        #endregion

        #region Getters/Setters

        /// <summary>
        /// Change TerrainType of this tile.
        /// </summary>
        /// <param name="type">Enum of the terrain type.</param>
        public void ChangeTerrainType(Globals.TerrainTypes type)
        {
            if (this.type.GetEnum() != type)
            {
                this.type = new TerrainType(type);
            }
        }
        /// <summary>
        /// Returns the TerrainType of this tile.
        /// </summary>
        /// <returns>TerrainType for this tile.</returns>
        public TerrainType GetTerrainType()
        {
            return this.type;
        }

        /// <summary>
        /// Adds a list of units to this Tile.
        /// </summary>
        /// <param name="p">Owner of the units</param>
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(Player p, List<Unit> units)
        {
            if (!this.units.ContainsKey(p))
            {
                this.units.Add(p, new HashSet<Unit>());
            }
            foreach (Unit u in units)
            {
                this.units[p].Add(u);
            }

            if (unitsChanged != null)
            {
                unitsChanged(this, new Event<IEnumerable<Unit>>(units, EventType.ADD));
            }
        }
        /// <summary>
        /// Add a unit to this Tile.
        /// </summary>
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(Player p, Unit u)
        {
            if (!this.units.ContainsKey(p))
            {
                this.units.Add(p, new HashSet<Unit>());
            }
            this.units[p].Add(u);

            if (unitsChanged != null)
            {
                //I'm sorry for this ugly hax - John
                List<Unit> temp = new List<Unit>();
                temp.Add(u);
                unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.ADD));
            }
        }

        public void RemoveUnit(Player p, Unit u)
        {
            this.units[p].Remove(u);

            if (unitsChanged != null)
            {
                //I'm sorry for this ugly hax - John
                List<Unit> temp = new List<Unit>();
                temp.Add(u);
                unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.REMOVE));
            }
        }
        public void RemoveUnit(Player p, List<Unit> units)
        {
            foreach (Unit u in units)
            {
                this.units[p].Remove(u);
            }

            if (unitsChanged != null)
            {
                unitsChanged(this, new Event<IEnumerable<Unit>>(units, EventType.REMOVE));
            }
        }

        [Obsolete("Horribly horribly broken!")]
        public HashSet<Unit> GetUnits()
        {
            throw new Exception("NO, YOU CAN'T USE THIS CONSTRUCTOR!");
        }

        /// <summary>
        /// Get the list of units on this tile.
        /// </summary>
        /// <param name="p">Owner of the units to be returned.</param>
        /// <returns>HashSet of units in this tile.</returns>
        public HashSet<Unit> GetUnits(Player p)
        {
            if(this.units.ContainsKey(p))
            {
                return this.units[p];
            }
            else
            {
                return new HashSet<Unit>();
            }
        }

        /// <summary>
        /// Attempt to set a building in this tile.
        /// </summary>
        /// <param name="building">Building to place here.</param>
        /// <returns>True iff building was placed, False if this Tile already is occupied.</returns>
        public bool SetBuilding(Building building)
        {
            if (this.building != null || building == null)
            {
                // Already occupied tile.
                return false;
            }
            else
            {
                // Building placed.
                this.building = building;
                
                if (buildingChanged != null)
                {
                    buildingChanged(this, new Event<Building>(this.building, EventType.ADD));
                }

                return true;
            }
        }
        /// <summary>
        /// Get the building placed in this tile.
        /// </summary>
        /// <returns>If this tile has a building it will be returned, otherwise returns null.</returns>
        public Building GetBuilding()
        {
            return this.building;
        }

        #endregion

        /// <summary>
        /// Make a player able to see this tile.
        /// </summary>
        /// <param name="p">Player to add.</param>
        public void MakeVisibleTo(Player p)
        {
            this.visibleTo.Add(p);

            if (visionChanged != null)
            {
                visionChanged(this, new Event<IEnumerable<Player>>(visibleTo,EventType.ADD));
            }
        }
        /// <summary>
        /// Make a player unable to see this tile.
        /// </summary>
        /// <param name="p">Player to remove.</param>
        public void MakeInvisibleTo(Player p)
        {
            this.visibleTo.Remove(p);

            if (visionChanged != null)
            {
                visionChanged(this, new Event<IEnumerable<Player>>(visibleTo,EventType.REMOVE));
            }
        }

        /// <summary>
        /// Checks if this tile is visible to player 'p'.
        /// </summary>
        /// <param name="p">Player to check against.</param>
        public bool IsVisible(Player p)
        {
            if (visibleTo.Contains(p))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes building in this tile.
        /// </summary>
        public void RemoveBuilding()
        {
            this.building = null;

            if (buildingChanged != null)
            {
                buildingChanged(this, new Event<Building>(this.building, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Checks to see if this Tile is the same as another one.
        /// (Overrides default Equals-check.)
        /// </summary>
        /// <param name="obj">Other tile object</param>
        /// <returns>True if of the same terrain type.</returns>
        public override bool Equals(System.Object obj)
        {
            Tile t = (Tile) obj;
            return this.GetTerrainType().GetEnum().Equals(t.GetTerrainType().GetEnum());
        }

        /// <summary>
        /// Overrides the == operator.
        /// </summary>
        public static bool operator==(Tile obj1, Tile obj2)
        {
            return obj1.Equals(obj2);
        }
        /// <summary>
        /// Overrides the != operator.
        /// </summary>
        public static bool operator !=(Tile obj1, Tile obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>
        /// Override that doesn't actually override things.
        /// Just because I had to write one.
        /// </summary>
        /// <returns>Unique hash code for this object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
