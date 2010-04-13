using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
    class World : IModel
    {
        /// <summary>
        /// Event for buildings created and deleted
        /// </summary>
        public event Publish<Building> BuildingEvent;
        /// <summary>
        /// Event for players created and deleted
        /// </summary>
        public event Publish<Player> PlayerEvent;
        /// <summary>
        /// Event for tiles created and deleted 
        /// </summary>
        public event Publish<Tile> MapEventE;
        /// <summary>
        /// Event for units created and deleted
        /// </summary>
        public event Publish<Unit> UnitEvent;

        /// <summary>
        /// The world's internal list of players
        /// </summary>
        private List<Player> players;
        /// <summary>
        /// The world's internal list of buildings
        /// </summary>
        private List<Building> buildings;
        /// <summary>
        /// The world's internal list of units
        /// </summary>
        private List<Unit> units;
        /// <summary>
        /// The tiles of the world arranged in a row-column matrix
        /// </summary>
        private Tile[ , ] map;

        /// <summary>
        /// The number of columns in the map
        /// </summary>
        public int mapColumns { get; private set; }
        /// <summary>
        /// The number of rows in the map
        /// </summary>
        public int mapRows { get; private set; }

        /// <summary>
        /// Constructor of the game world. Creates containers for all game entities
        /// as well as the map of the world.
        /// </summary>
        /// <param name="rows">Number of rows in the map of the world</param>
        /// <param name="cols">Number of columns in the map of the world</param>
        public World(int rows, int cols)
        {
            players = new List<Player>();
            buildings = new List<Building>();
            units = new List<Unit>();
        }

        /// <summary>
        /// Add a player to the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be added to the world</param>
        public void AddPlayer(Player p) 
        {
            players.Add(p);
            PlayerEvent(this, new Event<Player>(p, EventType.ADD));
        }

        /// <summary>
        /// Remove a player from the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be removed</param>
        public void RemovePlayer(Player p)
        {
            players.Remove(p);
            PlayerEvent(this, new Event<Player>(p, EventType.REMOVE));
        }

        /// <summary>
        /// Add a unit to the game world. Invokes the UnitEvent event.
        /// </summary>
        /// <param name="u">The unit to be added</param>
        public void AddUnit(Unit u)
        {
            units.Add(u);
            UnitEvent(this, new Event<Unit>(u, EventType.ADD));
        }

        /// <summary>
        /// Remove a unit from the game world. Invokes the UnitEvent event.
        /// </summary>
        /// <param name="u">The unit to be removed</param>
        public void RemoveUnit(Unit u)
        {
            units.Remove(u);
            UnitEvent(this, new Event<Unit>(u, EventType.REMOVE));
        }

        /// <summary>
        /// Add a building to the game world. Invokes the BuildingEvent event.
        /// </summary>
        /// <param name="u">The building to be added</param>
        public void AddBuilding(Building b)
        {
            buildings.Add(b);
            BuildingEvent(this, new Event<Building>(b, EventType.ADD));
        }

        /// <summary>
        /// Remove a building from the game world. Invokes the BuildingEvent event.
        /// </summary>
        /// <param name="u">The building to be removed</param>
        public void RemoveBuilding(Building b)
        {
            buildings.Remove(b);
            BuildingEvent(this, new Event<Building>(b, EventType.REMOVE));
        }


        /// <summary>
        /// Set a tile in the world. Invokes the MapEvent event. Will throw 
        /// IndexOutOfRangeException if placement of a tile was attempted outside
        /// the range of the map.
        /// </summary>
        /// <param name="row">The row in which the tile will be set (index 0).</param>
        /// <param name="col">The column in which the tile will be set (index 0).</param>
        /// <param name="t">Tile tile to be set.</param>
        public void SetTile(int row, int col, Tile t)
        {
            if (row < mapRows || col < mapColumns)
            {
                throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
            }
            map[row, col] = t;
            MapEventE(this, new MapEvent(map, row, col, EventType.ALTER));
        }

        public Tile GetTile(int row, int col)
        {
            if (row < mapRows || col < mapColumns)
            {
                throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
            }
            return map[row, col];
        }


        public void setMap(Tile[,] map)
        {
            this.map = map;
        }

    }
}