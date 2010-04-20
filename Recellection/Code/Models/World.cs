using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Part of the model describing the game world. Contains a list of the players and the matrix
    /// of tiles that make up the game map.
    /// </summary>
    public class World : IModel
    {
        /// <summary>
        /// Event for tiles created and deleted 
        /// </summary>
        public event Publish<Tile> TileEvent;

        public event Publish<Tile[,]> MapEvent;

        public event Publish<Player> PlayerEvent;
        
        /// <summary>
        /// The tiles of the world arranged in a row-column matrix
        /// </summary>
        public Tile[,] map { get; private set; }

        public List<Player> players { get; private set; }

        /// <summary>
        /// The number of columns in the map
        /// </summary>
        public int mapColumns { get; private set; }
        /// <summary>
        /// The number of rows in the map
        /// </summary>
        public int mapRows { get; private set; }

        /// <summary>
        /// Constructor of the game world. Creates an empty list of players
        /// as well as an empty matrix of the given dimensions.
        /// </summary>
        /// <param name="rows">Number of rows in the map of the world</param>
        /// <param name="cols">Number of columns in the map of the world</param>
        public World(int rows, int cols, Tile[,] map)
        {
            players = new List<Player>();
            SetMap(map, rows, cols);
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
            TileEvent(this, new Event<Tile>(t, EventType.REMOVE));
        }

        /// <summary>
        /// Retrieve the tile at row row and column col in the map. Invokes an EMapEvent.
        /// </summary>
        /// <param name="row">The row of the tile to be retrieved</param>
        /// <param name="col">The column of the tile to be retrieved</param>
        /// <returns></returns>
        public Tile GetTile(int row, int col)
        {
            if (row < mapRows || col < mapColumns)
            {
                throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
            }
            return map[row, col];
        }

        /// <summary>
        /// Sets the game map to a specific Tile-matrix. Dimensions are required.
        /// Invokes two EMapEvents.
        /// </summary>
        /// <param name="map">The Tilematrix to be the new map</param>
        /// <param name="rows">The number of rows in the new map</param>
        /// <param name="cols">The number of columns in the new map</param>
        public void SetMap(Tile[,] map, int rows, int cols)
        {
            ClearMap();
            this.map = map;
            this.mapColumns = rows;
            this.mapRows = cols;
            MapEvent(this, new Event<Tile[,]>(map, EventType.ADD));
        }

        /// <summary>
        /// Empty the game map. Invokes an EMapEvent.
        /// </summary>
        public void ClearMap()
        {
            this.map = null;
            this.mapColumns = 0;
            this.mapRows = 0;
            MapEvent(this, new Event<Tile[,]>(map, EventType.REMOVE));
        }

        /// <summary>
        /// Returns the matrix of Tiles that is the game map
        /// </summary>
        /// <returns></returns>
        public Tile[,] GetMap()
        {
            return map;
        }

    }
}