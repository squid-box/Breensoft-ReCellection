using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Models
{
    class Tile
    {
        /**
         * Data
         */
        private TerrainType type;
        private List<Player> visibleTo;

        // Unspecified
        private List<Unit> units;
        private Building building;

        /**
         * Methods
         */
        
        // Constructors
        public Tile()
        {
            this.type = new TerrainType();
            this.visibleTo = new List<Player>();
            this.units = new List<Unit>();
            this.building = null;
        }
        public Tile(TerrainType type)
        {
            this.type = type;
            this.visibleTo = new List<Player>();
            this.units = new List<Unit>();
            this.building = null;
        }

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
    }
}
