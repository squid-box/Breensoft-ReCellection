using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Recellection.Code.Utility.Events;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Part of the model describing the game world. Contains a list of the players and the matrix
    /// of tiles that make up the game map.
    /// </summary>
    public class World : IModel
    {
        public Logger myLogger;

        # region Inner Class Map
        /// <summary>
        /// This class is a wrapper for the Tile matrix used by the world.
        /// It has functions for setting and getting tiles on certain locations.
        /// </summary>
        public class Map : IModel
        {
            public event Publish<Tile> TileEvent;

            public Tile[, ] map { get; private set; }

            public int Rows { get; private set; }

            public int Cols { get; private set; }

            /// <summary>
            /// Constructs a new Map model from a matrix of tiles.
            /// </summary>
            /// <param name="map">The matrix of tiles that will form the map</param>
            public Map(Tile[,] map)
            {
                
                this.map = map;
                this.Rows = map.GetLength(0);
                this.Cols = map.GetLength(1);
            }


            /// <summary>
            /// Retrieve the tile at row row and column col in the map. Invokes an EMapEvent.
            /// </summary>
            /// <param name="row">The row of the tile to be retrieved</param>
            /// <param name="col">The column of the tile to be retrieved</param>
            /// <returns></returns>
            public Tile GetTile(int row, int col)
            {
                if (row < Rows || col < Cols)
                {
                    throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
                }
                return map[row, col];
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
                if (row < Rows || col < Cols)
                {
                    throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
                }
                map[row, col] = t;
                if (TileEvent != null)
                {
                    TileEvent(this, new Event<Tile>(t, EventType.REMOVE));
                }
            }
        }

        # endregion


        #region Events
        
        /// <summary>
        /// Event that is invoked when the map is changed
        /// </summary>
        public event Publish<Map> MapEvent;

        /// <summary>
        /// Event that is invoked when the set of players in the world changes
        /// </summary>
        public event Publish<Player> PlayerEvent; 
        #endregion
        
        /// <summary>
        /// The tiles of the world arranged in a row-column matrix
        /// </summary>
        public Map map { get; private set; }

        public int seed { get; private set; }

        public List<Player> players { get; private set; }

        /// <summary>
        /// Constructor of the game world. Creates an empty list of players
        /// as well as an empty matrix of the given dimensions.
        /// </summary>
        /// <param name="rows">Number of rows in the map of the world</param>
        /// <param name="cols">Number of columns in the map of the world</param>
        public World(Tile[,] map, int seed)
        {
            myLogger = LoggerFactory.GetLogger();
            players = new List<Player>();
            this.seed = seed;
            SetMap(map);
        }

        /// <summary>
        /// Add a player to the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be added to the world</param>
        public void AddPlayer(Player p) 
        {
            players.Add(p);
            if (PlayerEvent != null)
            {
                PlayerEvent(this, new Event<Player>(p, EventType.ADD));
            }
        }

        /// <summary>
        /// Remove a player from the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be removed</param>
        public void RemovePlayer(Player p)
        {
            players.Remove(p);
            if (PlayerEvent != null)
            {
                PlayerEvent(this, new Event<Player>(p, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Sets the game map to a specific Tile-matrix. Dimensions are required.
        /// Invokes two EMapEvents.
        /// </summary>
        /// <param name="map">The Tilematrix to be the new map</param>
        /// <param name="rows">The number of rows in the new map</param>
        /// <param name="cols">The number of columns in the new map</param>
        public void SetMap(Tile[,] map)
        {
            ClearMap();
            this.map = new Map(map);
            if (MapEvent!=null)
            {
                MapEvent(this, new Event<Map>(this.map, EventType.ADD)); 
            }
        }

        /// <summary>
        /// Empty the game map. Invokes an EMapEvent.
        /// </summary>
        public void ClearMap()
        {
            this.map = null;
            Map temp = this.map;
            if (MapEvent != null)
            {
                MapEvent(this, new Event<Map>(temp, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Returns the matrix of Tiles that is the game map
        /// </summary>
        /// <returns></returns>
        public World.Map GetMap()
        {
            return map;
        }
    }
}