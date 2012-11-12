namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Events;

    /// <summary>
    /// Representation of a tile in the game world.
    /// 
    /// Signed: Marco Ahumada Juntunen 2010-05-06
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-07</date>
    /// 
    /// Signature: Name (date)
    /// Signature: Name (date)
    public class Tile : Entity, IModel
    {
        // Data
        #region Fields

        private readonly List<Unit> allUnits;
        private readonly List<Vector2> lineDrawPoints;

        private readonly Dictionary<Player, HashSet<Unit>> units;

        private readonly HashSet<Player> visibleTo;

        private Building building;

        private TerrainType type;

        #endregion

        // Methods
        #region Constructors and Destructors

        /// <summary>
        /// Creates a Tile of Membrane (default) type.
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        public Tile(int x, int y) : base (new Vector2((float)(x+0.5), (float)(y+0.5)), null)
        {
            this.type = new TerrainType();
            this.visibleTo = new HashSet<Player>();
            this.units = new Dictionary<Player, HashSet<Unit>>();
            this.allUnits = new List<Unit>();
            this.building = null;
            this.lineDrawPoints = new List<Vector2>();
        }

        /// <summary>
        /// Creates a Tile of the type 'type'.
        /// </summary>
        /// <param name="type">Enum of the terrain type.</param>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        public Tile(int x, int y, Globals.TerrainTypes type) : base(new Vector2(x, y), null)
        {
            this.type = new TerrainType(type);
            this.visibleTo = new HashSet<Player>();
            this.units = new Dictionary<Player, HashSet<Unit>>();
            this.allUnits = new List<Unit>();
            this.position = new Vector2(x, y);
            this.building = null;
            this.lineDrawPoints = new List<Vector2>();
        }

        #endregion

        #region Public Events

        public event Publish<Building> buildingChanged;

        public event Publish<IEnumerable<Unit>> unitsChanged;

        public event Publish<IEnumerable<Player>> visionChanged;

        #endregion

        #region Public Properties

        public Vector2 CenterPosition
        {
            get
            {
                return this.position + new Vector2(0.5f, 0.5f);
            }
        }

        public bool active {get; set;}

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a list of units to this Tile.
        /// </summary>
        /// <param name="p">Owner of the units</param>
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(Player p, List<Unit> units)
        {
            lock (units)
            {
                lock (this.allUnits)
                {
                    if (!this.units.ContainsKey(p))
                    {
                        this.units.Add(p, new HashSet<Unit>());
                    }

                    foreach (Unit u in units)
                    {
                        this.units[p].Add(u);
                        this.allUnits.Add(u);
                    }

                    if (this.unitsChanged != null)
                    {
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(units, EventType.ADD));
                    }
                }
            }
        }

        /// <summary>
        /// Add a unit to this Tile.
        /// </summary>
        /// <param name="units">Units to be added to this Tile.</param>
        public void AddUnit(Player p, Unit u)
        {
            lock (this.units)
            {
                lock (this.allUnits)
                {
                    if (!this.units.ContainsKey(p))
                    {
                        this.units.Add(p, new HashSet<Unit>());
                    }

                    this.units[p].Add(u);
                    this.allUnits.Add(u);

                    if (this.unitsChanged != null)
                    {
                        // I'm sorry for this ugly hax - John
                        var temp = new List<Unit>();
                        temp.Add(u);
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.ADD));
                    }
                }
            }
        }

        public void AddUnit(Unit u)
        {
            lock (this.units)
            {
                lock (this.allUnits)
                {
                    HashSet<Unit> nits;
                    if (!this.units.TryGetValue(u.GetOwner(), out nits))
                    {
                        nits = new HashSet<Unit>();
                    }

                    nits.Add(u);

                    this.units[u.GetOwner()] = nits;
                    this.allUnits.Add(u);

                    if (this.unitsChanged != null)
                    {
                        // I'm sorry for this ugly hax - John
                        var temp = new List<Unit>();
                        temp.Add(u);
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.ADD));
                    }
                }
            }
        }

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

        public void ClearDrawLine()
        {
            this.lineDrawPoints.Clear();
        }

        /// <summary>
        /// Get the fromBuilding placed in this tile.
        /// </summary>
        /// <returns>If this tile has a fromBuilding it will be returned, otherwise returns null.</returns>
        public Building GetBuilding()
        {
            return this.building;
        }

        public List<Vector2> GetDrawPoints()
        {
            return this.lineDrawPoints;
        }

        /// <summary>
        /// Checks to see if this Tile is the same as another one.
        /// (Overrides default Equals-check.)
        /// </summary>
        /// <param name="obj">Other tile object</param>
        /// <returns>True if of the same terrain type.</returns>
        // public override bool Equals(System.Object obj)
        // {
        // Tile t = (Tile) obj;
        // return this.GetTerrainType().GetEnum().Equals(t.GetTerrainType().GetEnum());
        // }

        ///// <summary>
        ///// Overrides the == operator.
        ///// </summary>
        // public static bool operator==(Tile obj1, Tile obj2)
        // {
        // return obj1.Equals(obj2);
        // }
        ///// <summary>
        ///// Overrides the != operator.
        ///// </summary>
        // public static bool operator !=(Tile obj1, Tile obj2)
        // {
        // return !obj1.Equals(obj2);
        // }

        /// <summary>
        /// Override that doesn't actually override things.
        /// Just because I had to write one.
        /// </summary>
        /// <returns>Unique hash code for this object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Rectangle GetRectangle()
        {
            var x = (int) this.position.X;
            var y = (int) this.position.Y;
            return new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
        }

        // Graphical representation

        /// <summary>
        /// Returns texture for a unit.
        /// </summary>
        /// <returns>Texture of this unit.</returns>
        public override Texture2D GetSprite()
        {
            return this.type.GetTexture();

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
        /// Returns all units on this Tile, regardless of owner.
        /// </summary>
        public List<Unit> GetUnits()
        {
            lock (this.allUnits)
            {
                return this.allUnits;

            }
        }

        /// <summary>
        /// Get the list of units on this tile.
        /// </summary>
        /// <param name="p">Owner of the units to be returned.</param>
        /// <returns>HashSet of units in this tile.</returns>
        public HashSet<Unit> GetUnits(Player p)
        {
            lock (this.units)
            {
                if (this.units.ContainsKey(p))
                {
                    return this.units[p];
                }
                else
                {
                    return new HashSet<Unit>();
                }
            }
        }

        /// <summary>
        /// Checks if this tile is visible to player 'p'.
        /// </summary>
        /// <param name="p">Player to check against.</param>
        public bool IsVisible(Player p)
        {
            if (this.visibleTo.Contains(p))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Make a player unable to see this tile.
        /// </summary>
        /// <param name="p">Player to remove.</param>
        public void MakeInvisibleTo(Player p)
        {
            this.visibleTo.Remove(p);

            if (this.visionChanged != null)
            {
                this.visionChanged(this, new Event<IEnumerable<Player>>(this.visibleTo, EventType.REMOVE));
            }
        }

        /// <summary>
        /// Make a player able to see this tile.
        /// </summary>
        /// <param name="p">Player to add.</param>
        public void MakeVisibleTo(Player p)
        {
            this.visibleTo.Add(p);

            if (this.visionChanged != null)
            {
                this.visionChanged(this, new Event<IEnumerable<Player>>(this.visibleTo, EventType.ADD));
            }
        }

        /// <summary>
        /// Removes fromBuilding in this tile.
        /// </summary>
        public void RemoveBuilding()
        {
            this.building = null;

            if (this.buildingChanged != null)
            {
                this.buildingChanged(this, new Event<Building>(this.building, EventType.REMOVE));
            }
        }

        public void RemoveUnit(Player p, Unit u)
        {
            lock (this.units)
            {
                lock (this.allUnits)
                {
                    this.units[p].Remove(u);
                    this.allUnits.Remove(u);

                    if (this.unitsChanged != null)
                    {
                        // I'm sorry for this ugly hax - John
                        var temp = new List<Unit>();
                        temp.Add(u);
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.REMOVE));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="units"></param>
        public void RemoveUnit(Player p, List<Unit> units)
        {
            lock (units)
            {
                lock (this.allUnits)
                {
                    foreach (Unit u in units)
                    {
                        this.units[p].Remove(u);
                        this.allUnits.Remove(u);
                    }

                    if (this.unitsChanged != null)
                    {
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(units, EventType.REMOVE));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        public void RemoveUnit(Unit u)
        {
            lock (this.units)
            {
                lock (this.allUnits)
                {
                    this.units[u.GetOwner()].Remove(u);
                    this.allUnits.Remove(u);

                    if (this.unitsChanged != null)
                    {
                        // I'm sorry for this ugly hax - John
                        var temp = new List<Unit>();
                        temp.Add(u);
                        this.unitsChanged(this, new Event<IEnumerable<Unit>>(temp, EventType.REMOVE));
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to set a fromBuilding in this tile.
        /// </summary>
        /// <param name="fromBuilding">Building to place here.</param>
        /// <returns>True iff fromBuilding was placed, False if this Tile already is occupied.</returns>
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
                
                if (this.buildingChanged != null)
                {
                    this.buildingChanged(this, new Event<Building>(this.building, EventType.ADD));
                }

                return true;
            }
        }

        public void SetDrawLine(List<Vector2> points)
        {
            if (points.Count != 4)
            {
                throw new ArgumentOutOfRangeException("To many points in list, it only accepts four Vector2 points.");
            }

            this.lineDrawPoints.Clear();
            this.lineDrawPoints.AddRange(points);
        }

        #endregion
    }
}
