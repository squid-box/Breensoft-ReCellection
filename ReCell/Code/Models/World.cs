namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// Part of the model describing the game world. Contains a list of the players and the matrix
    /// of tiles that make up the game map.
    /// </summary>
    public class World : IModel
    {
        #region Static Fields

        private static readonly int maxCols = (int)(Recellection.viewPort.Width / (float)Globals.TILE_SIZE);
        private static readonly int maxRows = (int)(Recellection.viewPort.Height / (float)Globals.TILE_SIZE);

        #endregion

        #region Fields

        public Logger myLogger;

        private Point lookingAt;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor of the game world. Creates an empty list of players
        /// as well as an empty matrix of the given dimensions.
        /// </summary>
        /// <param name="rows">Number of rows in the map of the world</param>
        /// <param name="cols">Number of columns in the map of the world</param>
        public World(Tile[,] map, int seed)
        {
            this.myLogger = LoggerFactory.GetLogger();
            this.players = new List<Player>();
            this.seed = seed;
            this.SetMap(map);
            this.lookingAt = Point.Zero;
            this.units = new HashSet<Unit>();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event that is invoked when the map is changed
        /// </summary>
        public event Publish<Map> MapEvent;

        /// <summary>
        /// Event that is invoked when the set of players in the world changes
        /// </summary>
        public event Publish<Player> PlayerEvent;

        public event Publish<Point> lookingAtEvent;

        #endregion

        #region Public Properties

        public List<Point> DrawConstructionLines { get; set; }

        public Point LookingAt
        {
            get
            {
                return this.lookingAt;
            }

            set
            {
                this.lookingAt = value;
                this.lookingAt.X = (int)MathHelper.Clamp(this.lookingAt.X, 0, this.map.width - maxCols);
                this.lookingAt.Y = (int)MathHelper.Clamp(this.lookingAt.Y, 0, this.map.height - maxRows);
                if (this.lookingAtEvent != null)
                {
                    this.lookingAtEvent(this, new Event<Point>(value, EventType.ALTER));
                }
            }
        }

        /// <summary>
        /// The tiles of the world arranged in a row-column matrix
        /// </summary>
        public Map map { get; private set; }

        public List<Player> players { get; private set; }

        public int seed { get; private set; }

        public HashSet<Unit> units { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add a player to the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be added to the world</param>
        public void AddPlayer(Player p) 
        {
            this.players.Add(p);
            if (this.PlayerEvent != null)
            {
                this.PlayerEvent(this, new Event<Player>(p, EventType.ADD));
            }
        }

        public void AddUnit(Unit u)
        {
            lock (this.units)
            {
                this.units.Add(u);
            }
        }

        /// <summary>
        /// Empty the game map. Invokes an EMapEvent.
        /// </summary>
        public void ClearMap()
        {
            this.map = null;
            Map temp = this.map;
            if (this.MapEvent != null)
            {
                this.MapEvent(this, new Event<Map>(temp, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Returns the matrix of Tiles that is the game map
        /// </summary>
        /// <returns></returns>
        public Map GetMap()
        {
            return this.map;
        }

        /// <summary>
        /// Remove a player from the game world. Invokes the PlayerEvent event.
        /// </summary>
        /// <param name="p">The player to be removed</param>
        public void RemovePlayer(Player p)
        {
            this.players.Remove(p);
            if (this.PlayerEvent != null)
            {
                this.PlayerEvent(this, new Event<Player>(p, EventType.REMOVE));
            }
        }

        public void RemoveUnit(Unit u)
        {
            lock (this.units)
            {
                this.units.Remove(u);
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
            this.ClearMap();
            this.map = new Map(map);
            if (this.MapEvent!=null)
            {
                this.MapEvent(this, new Event<Map>(this.map, EventType.ADD)); 
            }
        }

        public bool isWithinMap(int x, int y)
        {
            return x > 0 && x < this.map.width && y > 0 && y < this.map.height;
        }

        #endregion

        /// <summary>
        /// This class is a wrapper for the Tile matrix used by the world.
        /// It has functions for setting and getting tiles on certain locations.
        /// </summary>
        public class Map : IModel
        {
            #region Constructors and Destructors

            /// <summary>
            /// Constructs a new Map model from a matrix of tiles.
            /// </summary>
            /// <param name="map">The matrix of tiles that will form the map</param>
            public Map(Tile[,] map)
            {
                this.map = map;
                this.width = map.GetLength(0);
                this.height = map.GetLength(1);
            }

            #endregion

            #region Public Events

            public event Publish<Tile> TileEvent;

            #endregion

            #region Public Properties

            public int height { get; private set; }

            public Tile[,] map { get; private set; }

            public int width { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// Retrieve the tile at row row and column col in the map. Invokes an EMapEvent.
            /// </summary>
            /// <param name="row">The row of the tile to be retrieved</param>
            /// <param name="col">The column of the tile to be retrieved</param>
            /// <returns></returns>
            public Tile GetTile(int x, int y)
            {
                if (x < 0 || y < 0 || x > this.width || y > this.height)
                {
                    throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
                }

                return this.map[x, y];
            }
            
            public Tile GetTile(Point p)
            {
                return this.GetTile(p.X, p.Y);
            }

            public Tile GetTile(Vector2 p)
            {
                return this.GetTile((int)p.X, (int)p.Y);
            }

            /// <summary>
            /// Set a tile in the world. Invokes the MapEvent event. Will throw 
            /// IndexOutOfRangeException if placement of a tile was attempted outside
            /// the range of the map.
            /// </summary>
            /// <param name="row">The row in which the tile will be set (index 0).</param>
            /// <param name="col">The column in which the tile will be set (index 0).</param>
            /// <param name="t">Tile tile to be set.</param>
            public void SetTile(int x, int y, Tile t)
            {
                if (x < this.width || y < this.height)
                {
                    throw new IndexOutOfRangeException("Attempted to set a tile outside the range of the map.");
                }

                this.map[x, y] = t;
                if (this.TileEvent != null)
                {
                    this.TileEvent(this, new Event<Tile>(t, EventType.REMOVE));
                }
            }

            #endregion
        }
    }
}