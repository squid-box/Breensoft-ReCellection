using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Logger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Views;

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
            //TODO remove when middlepoint position is implemeted.
            Vector2 buildingOffset = new Vector2(0.125f, 0.125f);

            logger.Trace("Attacking targets around a aggressive building at x: "+b.position.X+" y: "+b.position.Y );
            foreach (Unit u in b.currentTargets)
            {
                //Show kill graphix and make sound.

                //Kill units here.....
                KamikazeUnit temp = new KamikazeUnit(b.owner, Vector2.Add(b.position, buildingOffset), u);

            }
            logger.Trace("Killing " + b.currentTargets.Count + " units.");
            //UnitController.KillUnits(b.currentTargets, b.currentTargets.Count);
            b.currentTargets.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void ConstructBuilding(Player player, Tile constructTile, Building sourceBuilding, World theWorld)
        {
            logger.Trace("Constructing a building for a player");
            //TODO Somehow present a menu to the player, and then 
            //use the information to ADD (not the document) the fromBuilding.


            MenuIcon baseCell = new MenuIcon(Language.Instance.GetString("BaseCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding), Color.Black);
            MenuIcon resourceCell = new MenuIcon(Language.Instance.GetString("ResourceCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding), Color.Black);
            MenuIcon defensiveCell = new MenuIcon(Language.Instance.GetString("DefensiveCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding), Color.Black);
            MenuIcon aggressiveCell = new MenuIcon(Language.Instance.GetString("AggressiveCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding), Color.Black);
            List<MenuIcon> menuIcons = new List<MenuIcon>();
            menuIcons.Add(baseCell);
            menuIcons.Add(resourceCell);
            menuIcons.Add(defensiveCell);
            menuIcons.Add(aggressiveCell);
            Menu ConstructBuildingMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("ChooseBuilding"), Color.Black);
            MenuController.LoadMenu(ConstructBuildingMenu);
            Recellection.CurrentState = MenuView.Instance;
            Globals.BuildingTypes Building;

            MenuIcon choosenMenu = MenuController.GetInput();
            Recellection.CurrentState = WorldView.Instance;
            MenuController.UnloadMenu();
            if (choosenMenu.Equals(baseCell))
            {
                Building = Globals.BuildingTypes.Base;
            }
            else if (choosenMenu.Equals(resourceCell))
            {
                Building = Globals.BuildingTypes.Resource;
            }
            else if (choosenMenu.Equals(defensiveCell))
            {
                Building = Globals.BuildingTypes.Barrier;
            }
            else
            {
                Building = Globals.BuildingTypes.Aggressive;
            }


            // If we have selected a tile, and we can place a building at the selected tile...					

            if (! BuildingController.AddBuilding(Building, sourceBuilding,
                    constructTile.position, theWorld, player))
            {
                Sounds.Instance.LoadSound("Denied").Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void RazeBuilding(Player player, Tile constructTile, Building sourceBuilding)
        {
            logger.Trace("Razing a building for a player");

        }

        /// <summary>
        /// Add a fromBuilding to the source buildings owners graph, 
        /// the source fromBuilding will be used to find the correct graph.
        /// </summary>
        /// <param name="buildingType">The type of fromBuilding to build.</param>
        /// <param name="sourceBuilding">The fromBuilding used to build this fromBuilding.</param>
        /// <param name="targetCoordinate">The tile coordinates where the fromBuilding will be built.</param>
        /// <param name="world">The world to build the fromBuilding in.</param>
        public static bool AddBuilding(Globals.BuildingTypes buildingType,
            Building sourceBuilding, Vector2 targetCoordinate, World world, Player owner)
        {
            uint price = CalculateBuildingCostInflation(buildingType,owner);
            if (sourceBuilding != null && (uint)sourceBuilding.CountUnits() < price)
            {
                return false;
            }
            
            logger.Info("Building a building at position "+targetCoordinate+" of "+buildingType+".");
            
            lock (owner.GetGraphs())
            {
                LinkedList<Tile> controlZone = CreateControlZone(targetCoordinate, world);
                //The Base building is handled in another way due to it's nature.
                if (buildingType == Globals.BuildingTypes.Base)
                {
                    logger.Trace("Adding a Base Building and also constructing a new graph");
                    BaseBuilding baseBuilding = new BaseBuilding("Base Buidling",
                    (int)targetCoordinate.X, (int)targetCoordinate.Y, owner, controlZone);

                    world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).SetBuilding(baseBuilding);

                    owner.AddGraph(GraphController.Instance.AddBaseBuilding(baseBuilding, sourceBuilding));
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

                    world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).SetBuilding(newBuilding);
                    GraphController.Instance.AddBuilding(sourceBuilding, newBuilding);

                }
                if (sourceBuilding != null)
                {
                    logger.Info("The building has " + sourceBuilding.CountUnits() + " and the building costs " + price);
                    UnitController.KillUnits(sourceBuilding.units, (int)price);
                    logger.Info("The source building only got " + sourceBuilding.CountUnits() + " units left.");
                }

				//Sounds.Instance.LoadSound("buildingPlacement").Play();
				Sounds.Instance.LoadSound("prego").Play();
            }
            return true;
        }

        /// <summary>
        /// Create the list of tiles that the fromBuilding is surrounded with and the
        /// tile it is placed on.
        /// </summary>
        /// <param name="middleTile">The coordinates for the tile the fromBuilding is built on.</param>
        /// <param name="world">The world the fromBuilding is being built in.</param>
        /// <returns></returns>
        public static LinkedList<Tile> CreateControlZone(Vector2 middleTile, World world)
        {
            
            LinkedList<Tile> retur = new LinkedList<Tile>();

           
            //Iterate over the tiles that shall be added to the list
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    //The tile the fromBuilding is standing on shall be first in the
                    //linked list.
                    if (dx == 0 && dy == 0)
                    {
                        retur.AddFirst(world.GetMap().GetTile(dx + (int)middleTile.X, dy + (int)middleTile.Y));
                    }
                    //The other tiles shall be appended to the list
                    else
                    {
                        try
                        {
                            retur.AddLast(world.GetMap().GetTile(dx + (int)middleTile.X, dy + (int)middleTile.Y));
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            logger.Error(e.Message);
                            //The fromBuilding is being built close to an edge
                            //the exception is not handled.
                        }
                    }
                }
            }
            return retur;
        }

        /// <summary>
        /// Removes a fromBuilding from the graph containing it.
        /// </summary>
        /// <param name="b">The buiding to remove.</param>
        public static void RemoveBuilding(Building b)
        {
            GraphController.Instance.RemoveBuilding(b);
        }

        /// <summary>
        /// The increese in cost is 50% extra for each building of that type built.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="payer">The player this building is built for</param>
        /// <returns>The cost when considering the price inflation</returns>
        public static uint CalculateBuildingCostInflation(Globals.BuildingTypes type, Player payer)
        {
            uint defaultCost = Building.GetBuyPrice(type);
            uint buildingCount = payer.CountBuildingsOfType(type);
            return (uint)(defaultCost + (buildingCount * buildingCount * defaultCost / 2));

        }


        public static void HurtBuilding(Building toHurt, World theWorld)
        {
            /*toHurt.Damage(1);

            if (!toHurt.IsAlive())
            {
                RemoveBuilding(toHurt);
            }
            lock (theWorld)
            {
                //theWorld.GetMap().GetTile((int)toHurt.position.X, (int)toHurt.position.Y).RemoveBuilding();
            }*/
            HurtBuilding(toHurt);
        }
        public static void HurtBuilding(Building toHurt)
        {
            toHurt.Damage(1);

            if (!toHurt.IsAlive())
            {
                RemoveBuilding(toHurt);
                lock (toHurt.controlZone)
                {
                    toHurt.controlZone.First().RemoveBuilding();
                    //theWorld.GetMap().GetTile((int)toHurt.position.X, (int)toHurt.position.Y).RemoveBuilding();
                }
            }
            
        }
    }
}
