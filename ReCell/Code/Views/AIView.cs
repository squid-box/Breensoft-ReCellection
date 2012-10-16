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

    class AIView
    {

        // ############## Variables ##############//
        #region Fields

        internal int CRITICAL = 100;
        internal int SAFE = 1;

        internal int THREATENED = 50;

        private readonly Logger log;

        private Player ai;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_world"></param>
        public AIView(World p_world)
        {
            this.log = Utility.Logger.LoggerFactory.GetLogger();

            // LoggerFactory.globalThreshold = LogLevel.FATAL;
            this.world = p_world;
            this.mapHeight = this.world.GetMap().map.GetLength(1);
            this.mapWidth = this.world.GetMap().map.GetLength(0);

            this.myBuildings = new List<Building>();

            this.opponents = this.world.players; // Remove the AI player when it has called RegisterPlayer

            this.friendlyPoints = new List<Vector2>();
            this.resourcePoints = new List<Vector2>();
            this.enemyPoints = new List<Vector2>();
        }

        #endregion

        #region Properties

        internal List<Vector2> enemyPoints { get; private set; }

        internal List<Vector2> friendlyPoints { get; private set; }

        internal int mapHeight { get; private set; }
        internal int mapWidth { get; private set; }

        internal List<Building> myBuildings { get; private set; }
        internal List<Player> opponents { get; private set; }

        internal List<Vector2> resourcePoints { get; private set; }

        internal World world { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Causes the AIView to add the building at the given location to the list of buildings.
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal void BuildingAddedAt(Vector2 point)
        {
            Building b = Util.GetBuildingAt(point, this.world);
            if (b != null)
            {
                this.log.Info("Adding building " + b.name + " to the myBuildings list.");
                this.myBuildings.Add(b);
                this.friendlyPoints.Add(b.GetPosition());
            }
        }

        /// <summary>
        /// Checks whether or not the given tile contains an enemy building.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        internal bool ContainsFriendlyBuilding(Vector2 point)
        {
            Tile temp = Util.GetTileAt(point, this.world);
            if (temp != null && temp.GetBuilding() != null && temp.GetBuilding().GetOwner() == this.ai)
            {
                return true;
            }

            return false;
        }

        // ############## Construction ##############//

        // ############## Utility functions ##############//



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
            for (int i = 0; i < this.myBuildings.Count; i++)
            {
                coordinates.Add(this.myBuildings[i].position);
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
            for (int i = 0; i < this.myBuildings.Count; i++)
            {
                if (this.myBuildings[i].type == Globals.BuildingTypes.Resource)
                {
                    result.Add(this.myBuildings[i].GetPosition());
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
            Building tempBuilding = Util.GetBuildingAt(point, this.world);

            if (tempBuilding == null)
                return null;

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
            this.enemyPoints.Clear();
            this.friendlyPoints.Clear();
            this.myBuildings.Clear();
            this.resourcePoints.Clear();

            for (int i = 0; i < this.mapWidth; i++)
            {
                for (int j = 0; j < this.mapHeight; j++)
                {
                    Tile temp = this.world.GetMap().GetTile(i, j);
                    if (this.ContainsResourcePoint(temp))
                    {
                        this.resourcePoints.Add(temp.GetPosition());
                    }

                    if (this.ContainsEnemyBuilding(temp))
                    {
                        this.enemyPoints.Add(temp.GetPosition());
                    }

                    if (this.ContainsFriendlyBuilding(temp.GetPosition()))
                    {
                        this.myBuildings.Add(temp.GetBuilding());
                        this.friendlyPoints.Add(temp.GetPosition());
                    }
                }
            }
        }

        /// <summary>
        /// Internal function. Allows the AI Player to register itself so that this view can keep track
        /// of who it is making calls for.
        /// </summary>
        /// <param name="p"></param>
        internal void RegisterPlayer(Player p)
        {
            this.ai = p;
            this.opponents.Remove(this.ai);
        }

        /// <summary>
        /// Returns true if the coordinates provided are within the maps boundaries
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal bool Valid(Vector2 coords)
        {
            if (coords.X < this.mapWidth && coords.Y < this.mapHeight)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
