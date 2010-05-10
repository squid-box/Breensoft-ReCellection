using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Logger;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    class BuildingController
    {
        private static Logger logger = LoggerFactory.GetLogger();
        /// <summary>
        /// Let all Aggressive Buildings for the player Acquire Target(s)
        /// </summary>
        /// <param name="player">The Player</param>
        public static void AggressiveBuildingAct(Player player)
        {
            logger.Trace("Searching for aggressive buildings");
            foreach (Graph g in player.GetGraphs())
            {
                foreach (Building b in g.GetBuildings())
                {

                    if (b.type == Globals.BuildingTypes.Aggressive)
                    {
                        AttackTargets((AggressiveBuilding)b);
                    }
                }
            }

        }

        /// <summary>
        /// Search all the controlZone tiles for enemy units,
        /// then set them as a Target for the AggressiveBuilding.
        /// </summary>
        /// <param name="b"></param>
        private static void AttackTargets(AggressiveBuilding b)
        {
            logger.Trace("Attacking targets around a aggressive building at x: "+b.coordinates.X+" y: "+b.coordinates.Y );
            foreach (Unit u in b.currentTargets)
            {
                //Show kill graphix and make sound.

                //Kill units here.....

            }
            b.currentTargets.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void ConstructBuilding(Player player)
        {
            logger.Trace("Constructing a building for a player");
            //TODO Somehow present a menu to the player, and then 
            //use the information to ADD (not the document) the building.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void RazeBuilding(Player player)
        {
            logger.Trace("Razing a building for a player");

        }

        /// <summary>
        /// Add a building to the source buildings owners graph, 
        /// the source building will be used to find the correct graph.
        /// </summary>
        /// <param name="buildingType">The type of building to build.</param>
        /// <param name="sourceBuilding">The building used to build this building.</param>
        /// <param name="targetCoordinate">The tile coordinates where the building will be built.</param>
        /// <param name="world">The world to build the building in.</param>
        public static void AddBuilding(Globals.BuildingTypes buildingType,
            Building sourceBuilding, Vector2 targetCoordinate, World world)
        {
            LinkedList<Tile> controlZone = CreateControlZone(targetCoordinate,world);

            //The Base building is handled in another way due to it's nature.
            if (buildingType == Globals.BuildingTypes.Base)
            {
                logger.Trace("Adding a Base Building and also constructing a new graph");
                BaseBuilding baseBuilding = new BaseBuilding("Base Buidling",
                (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,controlZone);

                world.map.GetTile((int)targetCoordinate.Y, (int)targetCoordinate.X).SetBuilding(baseBuilding);

                GraphController.Instance.AddBaseBuilding(baseBuilding, sourceBuilding);
            }
            else
            {
                //The other buildings constructs in similiar ways but they are constructed
                //as the specified type.
                Building newBuilding = null;
                switch (buildingType)
                {
                    case Globals.BuildingTypes.Aggressive:
                        logger.Trace("Building a new Aggressive building");
                        newBuilding = new AggressiveBuilding("Aggresive Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                        break;
                    case Globals.BuildingTypes.Barrier:
                        logger.Trace("Building a new Barrier building");
                        newBuilding = new BarrierBuilding("Barrier Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                        break;
                    case Globals.BuildingTypes.Resource:
                        logger.Trace("Building a new Resource building");
                        newBuilding = new ResourceBuilding("Resource Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                        break;

                }

                world.map.GetTile((int)targetCoordinate.Y, (int)targetCoordinate.X).SetBuilding(newBuilding);
                GraphController.Instance.AddBuilding(sourceBuilding,newBuilding);

            }
        }

        /// <summary>
        /// Create the list of tiles that the building is surrounded with and the
        /// tile it is placed on.
        /// </summary>
        /// <param name="middleTile">The coordinates for the tile the building is built on.</param>
        /// <param name="world">The world the building is being built in.</param>
        /// <returns></returns>
        public static LinkedList<Tile> CreateControlZone(Vector2 middleTile, World world)
        {
            
            LinkedList<Tile> retur = new LinkedList<Tile>();

           
            //Iterate over the tiles that shall be added to the list
            for (int x = (int)middleTile.X-1; x < 1+(int)middleTile.X; x++)
            {
                for (int y = (int)middleTile.Y-1; y < 1+(int)middleTile.Y; y++)
                {
                    //The tile the building is standing on shall be first in the
                    //linked list.
                    if (x == (int)middleTile.X && y == (int)middleTile.Y)
                    {
                        retur.AddFirst(world.GetMap().GetTile(x,y));
                    }
                    //The other tiles shall be appended to the list
                    else
                    {
                        try
                        {
                            retur.AddLast(world.GetMap().GetTile(x, y));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            //The building is being built close to an edge
                            //the exception is not handled.
                        }
                    }
                }
            }
            return retur;
        }

        /// <summary>
        /// Removes a building from the graph containing it.
        /// </summary>
        /// <param name="b">The buiding to remove.</param>
        public static void RemoveBuilding(Building b)
        {
            GraphController.Instance.RemoveBuilding(b);
        }

    }
}
