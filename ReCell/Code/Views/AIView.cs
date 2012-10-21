/*
 * The AI View keeps track of things relevant for the AI Player.
 * 
 * Author: Lukas Mattsson
 */

namespace Recellection.Code
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Models;
    using global::Recellection.Code.Utility.Logger;

    public class AIView
    {
        #region Fields

        internal int CRITICAL = 100;
        internal int SAFE = 1;

        internal int THREATENED = 50;

        private readonly Logger log;

        private Player ai;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AIView"/> class. 
        /// </summary>
        /// <param name="p_world">
        /// World this player exists in.
        /// </param>
        public AIView(World p_world)
        {
            this.log = Utility.Logger.LoggerFactory.GetLogger();

            // LoggerFactory.globalThreshold = LogLevel.FATAL;
            this.World = p_world;
            this.MapHeight = this.World.GetMap().map.GetLength(1);
            this.MapWidth = this.World.GetMap().map.GetLength(0);

            this.MyBuildings = new List<Building>();

            this.Opponents = this.World.players; // Remove the AI player when it has called RegisterPlayer

            this.FriendlyPoints = new List<Vector2>();
            this.ResourcePoints = new List<Vector2>();
            this.EnemyPoints = new List<Vector2>();
        }

        #endregion

        #region Properties

        internal List<Vector2> EnemyPoints { get; private set; }

        internal List<Vector2> FriendlyPoints { get; private set; }

        internal int MapHeight { get; private set; }
        
        internal int MapWidth { get; private set; }

        internal List<Building> MyBuildings { get; private set; }
        
        internal List<Player> Opponents { get; private set; }

        internal List<Vector2> ResourcePoints { get; private set; }

        internal World World { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Causes the AIView to add the building at the given location to the list of buildings.
        /// 
        /// </summary>
        /// <param name="point">
        /// 
        /// </param>
        internal void BuildingAddedAt(Vector2 point)
        {
            Building b = Util.GetBuildingAt(point, this.World);
            if (b != null)
            {
                this.log.Info("Adding building " + b.name + " to the myBuildings list.");
                this.MyBuildings.Add(b);
                this.FriendlyPoints.Add(b.GetPosition());
            }
        }

        /// <summary>
        /// Checks whether or not the given tile contains an enemy building.
        /// </summary>
        /// <param name="t">Tile to check.</param>
        /// <returns>Value indicating whether or not the given tile contains an enemy building.</returns>
        internal bool ContainsEnemyBuilding(Tile t)
        {
            if (t.GetBuilding() == null)
            {
                return false;
            }

            if (t.GetBuilding().GetOwner() != this.ai)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether or not the given coordinates contains a friendly building.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>Value indicating whether or not the given coordinates contains a friendly building.</returns>
        internal bool ContainsFriendlyBuilding(Vector2 point)
        {
            Tile temp = Util.GetTileAt(point, this.World);
            if (temp != null && temp.GetBuilding() != null && temp.GetBuilding().GetOwner() == this.ai)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether or not there is a Resource Point at the given coordinates.
        /// Overloaded function, also works with a tile.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        internal bool ContainsResourcePoint(Vector2 current, World world)
        {
            Tile tempTile = Util.GetTileAt(current, world);
            return this.ContainsResourcePoint(tempTile);
        }

        /// <summary>
        /// Checks whether or not there is a Resource Point on the given Tile.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        internal bool ContainsResourcePoint(Tile current)
        {
            if (current.GetTerrainType().GetEnum() == Globals.TerrainTypes.Mucus)
            {
                return true;
            }

            return false;
        }

        // ############## Getter functions ##############//


        /// <summary>
        /// Returns the coordinates of all the friendly buildings
        /// </summary>
        /// <returns></returns>
        internal List<Vector2> GetFriendlyBuildings()
        {
            var coordinates = new List<Vector2>();
            for (int i = 0; i < this.MyBuildings.Count; i++)
            {
                coordinates.Add(this.MyBuildings[i].position);
            }

            return coordinates;
        }

        /// <summary>
        /// Returns a list of all the resource locations.
        /// </summary>
        /// <returns></returns>
        internal List<Vector2> GetResourceLocations()
        {
            var result = new List<Vector2>();
            for (int i = 0; i < this.MyBuildings.Count; i++)
            {
                if (this.MyBuildings[i].type == Globals.BuildingTypes.Resource)
                {
                    result.Add(this.MyBuildings[i].GetPosition());
                }
            }
 
            return result;
        }

        /// <summary>
        /// Returns the player who is harvesting at the given coordinates or null if noone is.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Player Harvesting(Vector2 point)
        {
            Building tempBuilding = Util.GetBuildingAt(point, this.World);

            if (tempBuilding == null)
            {
                return null;
            }

            if (tempBuilding.type == Globals.BuildingTypes.Resource)
            {
                return tempBuilding.GetOwner();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The AI takes a look at all the tiles it can see (everything as of2010-05-16) and
        /// looks for enemy buildings and resource points.
        /// </summary>
        internal void LookAtScreen()
        {
            // First, clear all lists
            this.EnemyPoints.Clear();
            this.FriendlyPoints.Clear();
            this.MyBuildings.Clear();
            this.ResourcePoints.Clear();

            for (int i = 0; i < this.MapWidth; i++)
            {
                for (int j = 0; j < this.MapHeight; j++)
                {
                    Tile temp = this.World.GetMap().GetTile(i, j);
                    if (this.ContainsResourcePoint(temp))
                    {
                        this.ResourcePoints.Add(temp.GetPosition());
                    }

                    if (this.ContainsEnemyBuilding(temp))
                    {
                        this.EnemyPoints.Add(temp.GetPosition());
                    }

                    if (this.ContainsFriendlyBuilding(temp.GetPosition()))
                    {
                        this.MyBuildings.Add(temp.GetBuilding());
                        this.FriendlyPoints.Add(temp.GetPosition());
                    }
                }
            }
        }

        /// <summary>
        /// Internal function. Allows the AI Player to register itself so that this view can keep track
        /// of who it is making calls for.
        /// </summary>
        /// <param name="p">
        /// 
        /// </param>
        internal void RegisterPlayer(Player p)
        {
            this.ai = p;
            this.Opponents.Remove(this.ai);
        }

        /// <summary>
        /// Returns true if the coordinates provided are within the maps boundaries
        /// </summary>
        /// <param name="coords">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        internal bool Valid(Vector2 coords)
        {
            if (coords.X < this.MapWidth && coords.Y < this.MapHeight)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
