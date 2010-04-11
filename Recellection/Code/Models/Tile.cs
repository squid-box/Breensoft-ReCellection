using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Models
{
    /* The representation of a Tile in the game world.
     * 
     * Author: Joel Ahlgren
     * Date: 2010-04-11
     */
    public class Tile
    {
        // Data
        private TerrainType type;
        private HashSet<Player> visibleTo;
        private List<Unit> units;
        private Building building;


        // Methods

        #region Constructors

        public Tile()
        {
            this.type = new TerrainType();
            this.visibleTo = new HashSet<Player>();
            this.units = new List<Unit>();
            this.building = null;
        }
        public Tile(TerrainType t)
        {
            this.type = t;
            this.visibleTo = new HashSet<Player>();
            this.units = new List<Unit>();
            this.building = null;
        }
        public Tile(Globals.TerrainTypes type)
        {
            this.type = new TerrainType(type);
            this.visibleTo = new HashSet<Player>();
            this.units = new List<Unit>();
            this.building = null;
        }

        #endregion

        #region Getters/Setters

        public void SetTerrainType(TerrainType type)
        {
            this.type = type;
        }
        public TerrainType GetTerrainType()
        {
            return this.type;
        }

        public void SetUnits(List<Unit> units)
        {
            foreach (Unit u in units)
            {
                this.units.Add(u);
            }
        }
        public List<Unit> GetUnits()
        {
            return this.units;
        }

        public bool SetBuilding(Building building)
        {
            if (this.building != null)
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
        public Building GetBuilding()
        {
            return this.building;
        }
        
        #endregion

        public void addPlayerAbleToSee(Player p)
        {
            this.visibleTo.Add(p);
        }
        public void removePlayerAbleToSee(Player p)
        {
            this.visibleTo.Remove(p);
        }
    }
}
