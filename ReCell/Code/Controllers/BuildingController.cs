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
        public const int MAX_BUILDING_RANGE = 3;

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

            logger.Trace("Attacking targets around a aggressive building at x: "+b.position.X+" y: "+b.position.Y );
            foreach (Unit u in b.currentTargets)
            {
                new KamikazeUnit(b.owner, b.position, u);
            }
            logger.Trace("Killing " + b.currentTargets.Count + " units.");
            //UnitController.MarkUnitsAsDead(b.currentTargets, b.currentTargets.Count);
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

			MenuIcon baseCell = new MenuIcon(Language.Instance.GetString("BaseCell") + 
					" (" + player.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Base)+")", 
					Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding), Color.Black);

			MenuIcon resourceCell = new MenuIcon(Language.Instance.GetString("ResourceCell") +
                    " (" + player.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Resource) + ")", 
					Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding), Color.Black);

			MenuIcon defensiveCell = new MenuIcon(Language.Instance.GetString("DefensiveCell") +
                    " (" + player.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Barrier) + ")", 
					Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding), Color.Black);

			MenuIcon aggressiveCell = new MenuIcon(Language.Instance.GetString("AggressiveCell") +
                    " (" + player.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Aggressive) + ")", 
					Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding), Color.Black);
            MenuIcon cancel = new MenuIcon(Language.Instance.GetString("Cancel"),
                    Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            List<MenuIcon> menuIcons = new List<MenuIcon>();
            menuIcons.Add(baseCell);
            menuIcons.Add(resourceCell);
            menuIcons.Add(defensiveCell);
            menuIcons.Add(aggressiveCell);
            menuIcons.Add(cancel);
            Menu ConstructBuildingMenu = new Menu(Globals.MenuLayout.NineMatrix, menuIcons, Language.Instance.GetString("ChooseBuilding"), Color.Black);
            MenuController.LoadMenu(ConstructBuildingMenu);
            Recellection.CurrentState = MenuView.Instance;
            Globals.BuildingTypes building;

            MenuIcon choosenMenu = MenuController.GetInput();
            Recellection.CurrentState = WorldView.Instance;
            MenuController.UnloadMenu();
            if (choosenMenu.Equals(baseCell))
            {
                building = Globals.BuildingTypes.Base;
            }
            else if (choosenMenu.Equals(resourceCell))
            {
                building = Globals.BuildingTypes.Resource;
            }
            else if (choosenMenu.Equals(defensiveCell))
            {
                building = Globals.BuildingTypes.Barrier;
            }
            else if (choosenMenu.Equals(aggressiveCell))
            {
                building = Globals.BuildingTypes.Aggressive;
            }
            else
            {
                return;
            }


            // If we have selected a tile, and we can place a building at the selected tile...					
            try
            {
                if (!AddBuilding(building, sourceBuilding,
                        constructTile.position, theWorld, player))
                {
                    SoundsController.playSound("Denied");
                }
            }
            catch (BuildingOutOfRangeException bore)
            {
                throw bore;
            }
        }

        public class BuildingOutOfRangeException : Exception
        {
            private static string msg = "Your source building is out of range";

            public BuildingOutOfRangeException()
                : base(msg)
            {
            }
        }

        /// <summary>
        /// Returns two points with the interval of wich the building can be built in.
        /// The points is returned inclusively, both points are valid coordinates. 
        /// </summary>
        /// <param name="sourceBuildingPosition"></param>
        /// <param name="world"></param>
        /// <returns>First in the list is the upperLeft point and the last is the lowerRight point.</returns>
        public static List<Point> GetValidBuildingInterval(Vector2 sourceBuildingPosition, World world)
        {
            List<Point> retur = new List<Point>(2);
            Point upperLeft = new Point(
                (int)MathHelper.Clamp(sourceBuildingPosition.X - MAX_BUILDING_RANGE, 1, world.GetMap().width-2),
                (int)MathHelper.Clamp(sourceBuildingPosition.Y - MAX_BUILDING_RANGE, 1, world.GetMap().height-2));
            Point lowerRight = new Point(
                (int)MathHelper.Clamp(sourceBuildingPosition.X + MAX_BUILDING_RANGE, 1, world.GetMap().width-2),
                (int)MathHelper.Clamp(sourceBuildingPosition.Y + MAX_BUILDING_RANGE, 1, world.GetMap().height-2));

            retur.Add(upperLeft);
            retur.Add(lowerRight);

            return retur;
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
            if (sourceBuilding != null && buildingType != Globals.BuildingTypes.Base && (Math.Abs(((int)sourceBuilding.position.X) - (int)targetCoordinate.X) > MAX_BUILDING_RANGE || (Math.Abs(((int)sourceBuilding.position.Y) - (int)targetCoordinate.Y) > MAX_BUILDING_RANGE)))
            {
				logger.Debug("Building position out of range");
                throw new BuildingOutOfRangeException();
            }
            uint price = owner.unitAcc.CalculateBuildingCostInflation(buildingType);
            if (sourceBuilding != null && (uint)sourceBuilding.CountUnits() < price)
            {
				logger.Debug("Building too expensive");
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
                                (int)targetCoordinate.X, (int)targetCoordinate.Y, owner,
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                            break;
                        case Globals.BuildingTypes.Barrier:
                            logger.Trace("Building a new Barrier building");
                            newBuilding = new BarrierBuilding("Barrier Building",
                                (int)targetCoordinate.X, (int)targetCoordinate.Y, owner,
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                            break;
                        case Globals.BuildingTypes.Resource:
                            logger.Trace("Building a new Resource building");
                            newBuilding = new ResourceBuilding("Resource Building",
                                (int)targetCoordinate.X, (int)targetCoordinate.Y, owner,
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, controlZone);
                            break;
                    }

					world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).SetBuilding(newBuilding);
					newBuilding.Parent = sourceBuilding;
                    GraphController.Instance.AddBuilding(sourceBuilding, newBuilding);
                }
                if (sourceBuilding != null && world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).GetBuilding() != null)
                {
                    logger.Info("The building has " + sourceBuilding.CountUnits() + " and the building costs " + price);
                    owner.unitAcc.DestroyUnits(sourceBuilding.units, (int)price);
                    logger.Info("The source building only got " + sourceBuilding.CountUnits() + " units left.");
                }
                else if (world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).GetBuilding() == null)
                {
                    throw new Exception("A building was not placed on the tile even though it should have been.");
                }

                SoundsController.playSound("buildingPlacement");
			}

			// Let's update the fog of war!
			/*
			for (int i = -3; i <= 3; i++)
			{
				for (int j = -3; j <= 3; j++)
				{
					try
					{
						world.map.GetTile((int)targetCoordinate.X + j, (int)targetCoordinate.Y + i).MakeVisibleTo(owner);
					}
					catch(IndexOutOfRangeException e)
					{
					}
				}
			}
			 */
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
            lock (b)
            {
                if (b is ResourceBuilding && GraphController.Instance.GetGraph(b).baseBuilding != null)
                {
                    GraphController.Instance.GetGraph(b).baseBuilding.RateOfProduction -= ((ResourceBuilding)b).RateOfProduction;
				}
				
				lock (b.controlZone)
				{
					b.controlZone.First().RemoveBuilding();
					GraphController.Instance.RemoveBuilding(b);
					b.Damage(Math.Max(0, b.currentHealth)); // Kill it!
                    SoundsController.playSound("buildingDeath",b.position);
				}
            }
        }

        public static void HurtBuilding(Building toHurt)
        {
            toHurt.Damage(1);

            if (!toHurt.IsAlive())
            {
                RemoveBuilding(toHurt);
            }
            
        }
    }
}
