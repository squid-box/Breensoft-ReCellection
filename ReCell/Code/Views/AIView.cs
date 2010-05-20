using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Recellection.Code.Models;
using Recellection.Code.Utility.Logger;

/*
 * The AI View keeps track of things relevant for the AI Player.
 * 
 * Author: Lukas Mattsson
 */

namespace Recellection.Code
{
    class AIView
    {

        //############## Variables ##############//

        private Player ai;
        private Logger log;

        //Threat levels
        internal int THREATENED = 50;
        internal int CRITICAL = 100;
        internal int SAFE = 1;


        internal List<Vector2> resourcePoints { get; private set; }
        internal List<Vector2> friendlyPoints { get; private set; }

        internal World world { get; private set; }
        internal List<Building> myBuildings { get; private set; }
        internal List<Player> opponents { get; private set; }

        internal List<Vector2> enemyPoints { get; private set; }
        internal int mapHeight { get; private set; }
        internal int mapWidth { get; private set; }



        //############## Construction ##############//

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_world"></param>
        public AIView(World p_world)
        {
            log = Utility.Logger.LoggerFactory.GetLogger();
            //LoggerFactory.globalThreshold = LogLevel.FATAL;

            world = p_world;
            mapHeight = world.GetMap().map.GetLength(1);
            mapWidth = world.GetMap().map.GetLength(0);

            myBuildings = new List<Building>();

            opponents = world.players; //Remove the AI player when it has called RegisterPlayer

            friendlyPoints = new List<Vector2>();
            resourcePoints = new List<Vector2>();
            enemyPoints = new List<Vector2>();
        }


        /// <summary>
        /// Internal function. Allows the AI Player to register itself so that this view can keep track
        /// of who it is making calls for.
        /// </summary>
        /// <param name="p"></param>
        internal void RegisterPlayer(Player p)
        {
            ai = p;
            opponents.Remove(ai);
        }


        //############## Logic functions ##############//

        /// <summary>
        /// The AI takes a look at all the tiles it can see (everything as of2010-05-16) and
        /// looks for enemy buildings and resource points.
        /// </summary>
        internal void LookAtScreen()
        {
            //First, clear all lists
            enemyPoints.Clear();
            friendlyPoints.Clear();
            myBuildings.Clear();
            resourcePoints.Clear();

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    Tile temp = world.GetMap().GetTile(i,j);
                    if (ContainsResourcePoint(temp))
                    {
                        resourcePoints.Add(temp.GetPosition());
                    }
                    if (ContainsEnemyBuilding(temp))
                    {
                        enemyPoints.Add(temp.GetPosition());
                    }
                    if (ContainsFriendlyBuilding(temp.GetPosition()))
                    {
                        myBuildings.Add(temp.GetBuilding());
                        friendlyPoints.Add(temp.GetPosition());
                    }
                }
            }
        }


        //############## Utility functions ##############//



        /// <summary>
        /// Checks whether or not there is a Resource Point at the given coordinates.
        /// Overloaded function, also works with a tile.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        internal bool ContainsResourcePoint(Vector2 current)
        {
            Tile tempTile = GetTileAt(current);
            return ContainsResourcePoint(tempTile);
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
       
        /// <summary>
        /// Checks whether or not the given coordinates contains a friendly building.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal bool ContainsFriendlyBuilding(Vector2 point)
        {
            Tile temp = GetTileAt(point);
            if (temp != null && temp.GetBuilding() != null && temp.GetBuilding().GetOwner() == ai)
            {
                return true;
            }
            return false;
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
            if (t.GetBuilding().GetOwner() != ai)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the coordinates provided are within the maps boundaries
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal bool Valid(Vector2 coords)
        {
            if (coords.X < mapWidth && coords.Y < mapHeight)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the player who is harvesting at the given coordinates or null if noone is.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Player Harvesting(Vector2 point)
        {
            Building tempBuilding = GetBuildingAt(point);

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



        //############## Getter functions ##############//



        /// <summary>
        /// Returns the Tile located in the given coordinates provided that it is visible.
        /// If it is not visible, null is returned.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal Tile GetTileAt(Vector2 coords)
        {
            //log.Fatal("Accessing Tile at "+coords.X+","+coords.Y);
            Tile tempTile = world.GetMap().GetTile((int)coords.X, (int)coords.Y);

            ///* Uncomment when fog of war is properly implemented
            //if (tempTile.IsVisible(ai))
            //{
            //    return tempTile;
            //}

            return tempTile;
        }

        /// <summary>
        /// Returns the fromBuilding at the given coordinates provided that it is visible.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Building GetBuildingAt(Vector2 point)
        {
            return GetTileAt(point).GetBuilding();
        }

        /// <summary>
        /// Returns the coordinates of all the friendly buildings
        /// </summary>
        /// <returns></returns>
        internal List<Vector2> GetFriendlyBuildings()
        {
            List<Vector2> coordinates = new List<Vector2>();
            for (int i = 0; i < myBuildings.Count; i++)
            {
                coordinates.Add(myBuildings[i].position);
            }
            return coordinates;
        }

        /// <summary>
        /// Returns a list of all the resource locations.
        /// </summary>
        /// <returns></returns>
        internal List<Vector2> GetResourceLocations()
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < myBuildings.Count; i++)
            {
                if (myBuildings[i].type == Globals.BuildingTypes.Resource)
                {
                    result.Add(myBuildings[i].GetPosition());
                }
            } 
            return result;
        }


        /// <summary>
        /// Causes the AIView to add the building at the given location to the list of buildings.
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal void BuildingAddedAt(Vector2 point)
        {
            Building b = GetBuildingAt(point);
            if (b != null)
            {
                log.Info("Adding building " + b.name + " to the myBuildings list.");
                myBuildings.Add(b);
            }
        }
    }
}
