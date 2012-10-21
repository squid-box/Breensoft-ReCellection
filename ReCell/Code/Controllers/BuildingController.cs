namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Models;
    using global::Recellection.Code.Utility.Logger;
    using global::Recellection.Code.Views;

    /// <summary>
    /// Controls the buildings.
    /// </summary>
    public class BuildingController
    {
        #region Constants

        public const int MaxBuildingRange = 3;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LoggerFactory.GetLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add a fromBuilding to the source buildings owners graph, the source fromBuilding will be used to find the correct graph.
        /// </summary>
        /// <param name="buildingType">
        /// The type of fromBuilding to build.
        /// </param>
        /// <param name="sourceBuilding">
        /// The fromBuilding used to build this fromBuilding.
        /// </param>
        /// <param name="targetCoordinate">
        /// The tile coordinates where the fromBuilding will be built.
        /// </param>
        /// <param name="world">
        /// The world to build the fromBuilding in.
        /// </param>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <returns>
        /// True if building was added, false if not.
        /// </returns>
        public static bool AddBuilding(Globals.BuildingTypes buildingType, Building sourceBuilding, Vector2 targetCoordinate, World world, Player owner)
        {
            if (sourceBuilding != null && buildingType != Globals.BuildingTypes.Base && (Math.Abs(((int)sourceBuilding.position.X) - (int)targetCoordinate.X) > MaxBuildingRange || (Math.Abs(((int)sourceBuilding.position.Y) - (int)targetCoordinate.Y) > MaxBuildingRange)))
            {
                Logger.Debug("Building position out of range");
                throw new BuildingOutOfRangeException();
            }

            uint price = owner.UnitAcc.CalculateBuildingCostInflation(buildingType);
            if (sourceBuilding != null && (uint)sourceBuilding.CountUnits() < price)
            {
                Logger.Debug("Building too expensive");
                return false;
            }

            Logger.Info("Building a building at position " + targetCoordinate + " of " + buildingType + ".");

            lock (owner.GetGraphs())
            {
                LinkedList<Tile> controlZone = CreateControlZone(targetCoordinate, world);

                // The Base building is handled in another way due to it's nature.
                if (buildingType == Globals.BuildingTypes.Base)
                {
                    Logger.Trace("Adding a Base Building and also constructing a new graph");
                    var baseBuilding = new BaseBuilding(
                        "Base Buidling", (int)targetCoordinate.X, (int)targetCoordinate.Y, owner, controlZone);

                    world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).SetBuilding(baseBuilding);

                    owner.AddGraph(GraphController.Instance.AddBaseBuilding(baseBuilding, sourceBuilding));
                }
                else
                {
                    // The other buildings constructs in similiar ways but they are constructed
                    // as the specified type.
                    Building newBuilding = null;
                    switch (buildingType)
                    {
                        case Globals.BuildingTypes.Aggressive:
                            Logger.Trace("Building a new Aggressive building");
                            newBuilding = new AggressiveBuilding(
                                "Aggresive Building", 
                                (int)targetCoordinate.X, 
                                (int)targetCoordinate.Y, 
                                owner, 
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, 
                                controlZone);
                            break;
                        case Globals.BuildingTypes.Barrier:
                            Logger.Trace("Building a new Barrier building");
                            newBuilding = new BarrierBuilding(
                                "Barrier Building", 
                                (int)targetCoordinate.X, 
                                (int)targetCoordinate.Y, 
                                owner, 
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, 
                                controlZone);
                            break;
                        case Globals.BuildingTypes.Resource:
                            Logger.Trace("Building a new Resource building");
                            newBuilding = new ResourceBuilding(
                                "Resource Building", 
                                (int)targetCoordinate.X, 
                                (int)targetCoordinate.Y, 
                                owner, 
                                GraphController.Instance.GetGraph(sourceBuilding).baseBuilding, 
                                controlZone);
                            break;
                    }

                    world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).SetBuilding(newBuilding);
                    if (newBuilding == null)
                    {
                        return false;
                    }
                    else
                    {
                        newBuilding.Parent = sourceBuilding;
                        GraphController.Instance.AddBuilding(sourceBuilding, newBuilding);
                    }
                }

                if (sourceBuilding != null && world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).GetBuilding() != null)
                {
                    Logger.Info("The building has " + sourceBuilding.CountUnits() + " and the building costs " + price);
                    owner.UnitAcc.DestroyUnits(sourceBuilding.units, (int)price);
                    Logger.Info("The source building only got " + sourceBuilding.CountUnits() + " units left.");
                }
                else if (world.map.GetTile((int)targetCoordinate.X, (int)targetCoordinate.Y).GetBuilding() == null)
                {
                    throw new Exception("A building was not placed on the tile even though it should have been.");
                }

                SoundsController.playSound("buildingPlacement");
            }

            return true;
        }

        /// <summary>
        /// Let all Aggressive Buildings for the player Acquire Target(s)
        /// </summary>
        /// <param name="player">The Player</param>
        public static void AggressiveBuildingAct(Player player)
        {
            Logger.Trace("Searching for aggressive buildings");
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
        /// Builds a building.
        /// </summary>
        /// <param name="player">
        /// Player building the building.
        /// </param>
        /// <param name="constructTile">
        /// The tile to construct the building in.
        /// </param>
        /// <param name="sourceBuilding">
        /// The source building.
        /// </param>
        /// <param name="theWorld">
        /// The World the building exists in.
        /// </param>
        public static void ConstructBuilding(Player player, Tile constructTile, Building sourceBuilding, World theWorld)
        {
            Logger.Trace("Constructing a building for a player");

            // TODO Somehow present a menu to the player, and then 
            // use the information to ADD (not the document) the fromBuilding.
            var baseCell =
                new MenuIcon(
                    Language.Instance.GetString("BaseCell") + " ("
                    + player.UnitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Base) + ")", 
                    Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding), 
                    Color.Black);

            var resourceCell =
                new MenuIcon(
                    Language.Instance.GetString("ResourceCell") + " ("
                    + player.UnitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Resource) + ")", 
                    Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding), 
                    Color.Black);

            var defensiveCell =
                new MenuIcon(
                    Language.Instance.GetString("DefensiveCell") + " ("
                    + player.UnitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Barrier) + ")", 
                    Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding), 
                    Color.Black);

            var aggressiveCell =
                new MenuIcon(
                    Language.Instance.GetString("AggressiveCell") + " ("
                    + player.UnitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Aggressive) + ")", 
                    Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding), 
                    Color.Black);
            var cancel = new MenuIcon(
                Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            var menuIcons = new List<MenuIcon>();
            menuIcons.Add(baseCell);
            menuIcons.Add(resourceCell);
            menuIcons.Add(defensiveCell);
            menuIcons.Add(aggressiveCell);
            menuIcons.Add(cancel);
            var constructBuildingMenu = new Menu(
                Globals.MenuLayout.NineMatrix, menuIcons, Language.Instance.GetString("ChooseBuilding"), Color.Black);
            MenuController.LoadMenu(constructBuildingMenu);
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
                if (!AddBuilding(building, sourceBuilding, constructTile.position, theWorld, player))
                {
                    // SoundsController.playSound("Denied");
                }
            }
            catch (BuildingOutOfRangeException bore)
            {
                throw bore;
            }
        }

        /// <summary>
        /// Create the list of tiles that the fromBuilding is surrounded with and the tile it is placed on.
        /// </summary>
        /// <param name="middleTile">The coordinates for the tile the fromBuilding is built on.</param>
        /// <param name="world">The world the fromBuilding is being built in.</param>
        /// <returns>Tiles around the tile.</returns>
        public static LinkedList<Tile> CreateControlZone(Vector2 middleTile, World world)
        {
            var retur = new LinkedList<Tile>();

            // Iterate over the tiles that shall be added to the list
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // The tile the fromBuilding is standing on shall be first in the linked list.
                    if (dx == 0 && dy == 0)
                    {
                        retur.AddFirst(world.GetMap().GetTile(dx + (int)middleTile.X, dy + (int)middleTile.Y));
                    }
                    else
                    {
                        // The other tiles shall be appended to the list
                        try
                        {
                            retur.AddLast(world.GetMap().GetTile(dx + (int)middleTile.X, dy + (int)middleTile.Y));
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Logger.Error(e.Message);

                            // The fromBuilding is being built close to an edge
                            // the exception is not handled.
                        }
                    }
                }
            }

            return retur;
        }

        /// <summary>
        /// Returns two points with the interval of which the building can be built in.
        /// The points is returned inclusively, both points are valid coordinates. 
        /// </summary>
        /// <param name="sourceBuildingPosition">Position of the source building.</param>
        /// <param name="world">World to check in.</param>
        /// <returns>First in the list is the upperLeft point and the last is the lowerRight point.</returns>
        public static List<Point> GetValidBuildingInterval(Vector2 sourceBuildingPosition, World world)
        {
            var retur = new List<Point>(2);
            var upperLeft =
                new Point(
                    (int)MathHelper.Clamp(sourceBuildingPosition.X - MaxBuildingRange, 1, world.GetMap().width - 2),
                    (int)MathHelper.Clamp(sourceBuildingPosition.Y - MaxBuildingRange, 1, world.GetMap().height - 2));
            var lowerRight =
                new Point(
                    (int)MathHelper.Clamp(sourceBuildingPosition.X + MaxBuildingRange, 1, world.GetMap().width - 2),
                    (int)MathHelper.Clamp(sourceBuildingPosition.Y + MaxBuildingRange, 1, world.GetMap().height - 2));

            retur.Add(upperLeft);
            retur.Add(lowerRight);

            return retur;
        }

        /// <summary>
        /// Damages the specified building.
        /// </summary>
        /// <param name="toHurt">
        /// The building to hurt.
        /// </param>
        public static void HurtBuilding(Building toHurt)
        {
            toHurt.Damage(1);

            if (!toHurt.IsAlive())
            {
                RemoveBuilding(toHurt);
            }
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

                    b.Kill();
                    SoundsController.playSound("buildingDeath", b.position);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Search all the controlZone tiles for enemy units,
        /// then set them as a Target for the AggressiveBuilding.
        /// </summary>
        /// <param name="b">Target building.</param>
        private static void AttackTargets(AggressiveBuilding b)
        {
            Logger.Trace("Attacking targets around a aggressive building at x: " + b.position.X + " y: " + b.position.Y);
            foreach (Unit u in b.currentTargets)
            {
                new KamikazeUnit(b.owner, b.position, u);
            }

            Logger.Trace("Killing " + b.currentTargets.Count + " units.");

            // UnitController.MarkUnitsAsDead(b.currentTargets, b.currentTargets.Count);
            b.currentTargets.Clear();
        }

        #endregion

        /// <summary>
        /// Exception thrown when source building is out of range.
        /// </summary>
        public class BuildingOutOfRangeException : Exception
        {
            #region Static Fields

            /// <summary>
            /// Message included in error.
            /// </summary>
            private const string Msg = "Your source building is out of range";

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="BuildingOutOfRangeException"/> class.
            /// </summary>
            public BuildingOutOfRangeException() : base(Msg)
            {
            }

            #endregion
        }
    }
}
