using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Representation of a tile in the game world.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-12</date>
    public class Tile : IModel
    {
        // Data
        private TerrainType type;
        private HashSet<Player> visibleTo;
        private HashSet<Unit> units;
        private Building building;


        // Methods

        #region Constructors

        /// <summary>
        /// Creates a Tile of Membrane (default) type.
        /// </summary>
        public Tile()
        {
            this.type = new TerrainType();
            this.visibleTo = new HashSet<Player>();
            this.units = new HashSet<Unit>();
            this.building = null;
        }
        /// <summary>
        /// Creates a Tile of the type 'type'.
        /// </summary>
        /// <param name="type">Enum of the terrain type.</param>
        public Tile(Globals.TerrainTypes type)
        {
            this.type = new TerrainType(type);
            this.visibleTo = new HashSet<Player>();
            this.units = new HashSet<Unit>();
            this.building = null;
        }

        [Obsolete]
        public Tile(TerrainType t)
        {
            this.type = new TerrainType();
            this.visibleTo = new HashSet<Player>();
            this.units = new HashSet<Unit>();
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
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(List<Unit> units)
        {
            foreach (Unit u in units)
            {
                this.units.Add(u);
            }
        }
        /// <summary>
        /// Add a unit to this Tile.
        /// </summary>
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(Unit u)
        {
            this.units.Add(u);
        }
        /// <summary>
        /// Get the list of units on this tile.
        /// </summary>
        /// <returns>HashSet of units in this tile.</returns>

        public void RemoveUnit(Unit u)
        {
            this.units.Remove(u);
        }
        public void RemoveUnit(List<Unit> units)
        {
            foreach (Unit u in units)
            {
                this.units.Remove(u);
            }
        }
        public HashSet<Unit> GetUnits()
        {
            return this.units;
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
        }
        /// <summary>
        /// Make a player unable to see this tile.
        /// </summary>
        /// <param name="p">Player to remove.</param>
        public void MakeInvisibleTo(Player p)
        {
            this.visibleTo.Remove(p);
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
    }
}
