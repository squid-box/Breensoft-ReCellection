namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    using global::Recellection.Code.Views;

    /// <summary>
    /// The purpose of this component is to control the entire world. It is part of the realization of SR1.7.
    /// </summary>
    class WorldController
    {
        #region Constants

        private const long SCROLL_ZONE_DWELL_TIME = 0;// 250000;

        #endregion

        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        #endregion

        #region Fields

        private readonly char[] REG_EXP = { '_' };

        private readonly Logger myLogger;

        private readonly Player playerInControll;

        private readonly World theWorld;

        readonly TobiiController tobii = TobiiController.GetInstance(Recellection.windowHandle);

        MenuIcon botOff;
        MenuIcon leftOff;

        private MenuIcon[,] menuMatrix;

        private Selection previousSelection;

        MenuIcon rightOff;

        private List<MenuIcon> scrollZone;

        private Tile selectedTile;

        MenuIcon topOff;

        #endregion

        // Create 
        #region Constructors and Destructors

        public WorldController(Player p, World theWorld)
        {
            // Debugging
            finished = false;
            this.myLogger = LoggerFactory.GetLogger();
            this.myLogger.SetThreshold(LogLevel.TRACE);

            this.playerInControll = p;
            this.theWorld = theWorld;

            this.createGUIRegionGridAndScrollZone();
        }

        #endregion

        #region Enums

        /// <summary>
        /// The different states this controller will assume
        /// </summary>
        public enum State { NONE, BUILDING, TILE, OFFSCREEN };

        #endregion

        #region Public Properties

        public static bool finished { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Run()
		{
			var sel = new Selection();
			logger.Info("Logger started");

			sel.state = State.NONE;
			finished = false;
            while (!finished)
            {
                this.previousSelection = sel;

                // Generate the appropriate menu for this state.
                // Get the active GUI Region and invoke the associated method.
                sel = this.retrieveSelection();

                World.Map map = this.theWorld.GetMap();

                switch (sel.state)
                {
                    case State.BUILDING:
                    case State.TILE:
                        this.SelectTile(map.GetTile(sel.absPoint));
                        break;
                    case State.OFFSCREEN:
                        if (sel.point.X == -1)
                        {
                            this.ContextMenu();
                        }

                        if (sel.point.X == 1)
                        {
                            this.GameMenu();
                        }

                        break;
                }
            }
		}

        public void Stop()
        {
            MenuController.UnloadMenu();
        }

        public Selection retrieveSelection()
		{
			this.myLogger.Debug("Waiting for input...");

			MenuIcon activatedMenuIcon = MenuController.GetInput();

			var s = new Selection();
			
		    int x = 0;
            int y = 0;
			if (activatedMenuIcon.label != null)
			{
				string[] splitted = activatedMenuIcon.label.Split(this.REG_EXP);
				try
				{
					this.myLogger.Trace("Splitted string = " + splitted[0] + "\t" + splitted[1]);
				}
				catch (IndexOutOfRangeException)
				{
					throw new ArgumentException("Your argument is invalid, my beard is a windmill.");
				}

				x = int.Parse(splitted[0]);
				y = int.Parse(splitted[1]);
			}
			else
			{
				if (activatedMenuIcon == this.leftOff)
				{
					x = -1;
				}
				else if (activatedMenuIcon == this.rightOff)
				{
					x = 1;
				}
				else if (activatedMenuIcon == this.topOff)
				{
					y = -1;
				}
				else if (activatedMenuIcon == this.botOff)
				{
					y = 1;
				}
				
				s.state = State.OFFSCREEN;
				s.point = new Point(x, y);
				return s;
			}
			

            var absoluteCordinate = new Point(x + this.theWorld.LookingAt.X, y + this.theWorld.LookingAt.Y);
            if (activatedMenuIcon.labelColor.Equals(Color.NavajoWhite))
            {
                if (
                    this.theWorld.GetMap().GetTile(
                        new Point(x + this.theWorld.LookingAt.X, y + this.theWorld.LookingAt.Y)).GetBuilding() != null)
                {
                    this.tobii.SetFeedbackColor(Color.Blue);
                    s.state = State.BUILDING;
                    s.point = new Point(x, y);
                    s.absPoint = absoluteCordinate;
                }
                else
                {
                    this.tobii.SetFeedbackColor(Color.White);
                    s.state = State.TILE;
                    s.point = new Point(x, y);
                    s.absPoint = absoluteCordinate;
                }
            }
			
                // If we selected a scroll zone?
            else if (activatedMenuIcon.labelColor.Equals(Color.Chocolate))
            {
                this.theWorld.LookingAt = new Point(this.theWorld.LookingAt.X + x, this.theWorld.LookingAt.Y + y);
                return this.retrieveSelection();
            }
            else
            {
                throw new ArgumentException("Your argument is invalid, my beard is a windmill.");
            }

            return s;
		}

        #endregion

        #region Methods

        /// <summary>
		/// Loads the Building menu from a selection.
		/// Must have building on tile.
		/// </summary>
		/// <param name="theSelection"></param>
        private void BuildingMenu()
        {
            
            World.Map map = this.theWorld.GetMap();
            Building building = this.selectedTile.GetBuilding();
            if (building == null || building.owner != this.playerInControll)
            {
                return;
            }

            int toHeal = Math.Min(building.maxHealth - building.currentHealth, building.units.Count());

            var setWeight = new MenuIcon(Language.Instance.GetString("SetWeight"));
            var buildCell = new MenuIcon(Language.Instance.GetString("BuildCell"));
            var removeCell = new MenuIcon(Language.Instance.GetString("RemoveCell"));
			var upgradeUnits = new MenuIcon(Language.Instance.GetString("UpgradeUnits") + " (" + this.playerInControll.unitAcc.GetUpgradeCost() + ")");
            var moveUnits = new MenuIcon(Language.Instance.GetString("MoveUnits"));
            var repairCell = new MenuIcon(Language.Instance.GetString("RepairCell") + " (" + toHeal + ")");
            var setAggro = new MenuIcon(Language.Instance.GetString("SetAggro"));
            var Cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            
            var menuIcons = new List<MenuIcon>();
            menuIcons.Add(setWeight);
            menuIcons.Add(buildCell);
            menuIcons.Add(removeCell);
            menuIcons.Add(upgradeUnits);
            menuIcons.Add(moveUnits);
            menuIcons.Add(repairCell);
            menuIcons.Add(setAggro);
            menuIcons.Add(Cancel);

            var buildingMenu = new Menu(Globals.MenuLayout.NineMatrix, menuIcons, Language.Instance.GetString("BuildingMenu"), Color.Black);
            MenuController.LoadMenu(buildingMenu);
            Recellection.CurrentState = MenuView.Instance;
            MenuIcon choosenMenu = MenuController.GetInput();
            Recellection.CurrentState = WorldView.Instance;
            MenuController.UnloadMenu();

            if (choosenMenu.Equals(setWeight))
            {
                GraphController.Instance.SetWeight(building);
            }
            else if (choosenMenu.Equals(buildCell))
            {
                this.tobii.SetFeedbackColor(Color.DarkGreen);
                Selection destsel;
                do
                {
					this.SetConstructionLines(BuildingController.GetValidBuildingInterval(this.selectedTile.position, this.theWorld));
					destsel = this.retrieveSelection();
					this.RemoveconstructionTileLines(BuildingController.GetValidBuildingInterval(this.selectedTile.position, this.theWorld));
				}
				while (destsel.state != State.TILE);
				
				this.tobii.SetFeedbackColor(Color.White);
				
				this.SelectTile(map.GetTile(destsel.absPoint));

                // TODO Add a check to see if the tile is a correct one. The diffrence between the selected tiles coordinates and the source building shall not exceed 3.
				if (this.selectedTile.GetBuilding() == null)
                {
                    try
                    {
                        BuildingController.ConstructBuilding(this.playerInControll, this.selectedTile, building, this.theWorld);
                        this.tobii.SetFeedbackColor(Color.White);
                    }
                    catch (BuildingController.BuildingOutOfRangeException)
                    {
						logger.Debug("Caught BuildingOutOfRangeExcpetion");
                    }
				}
				else
				{
                    // SoundsController.playSound("Denied");
                    this.tobii.SetFeedbackColor(Color.White);
					return;
				}
            }
            else if (choosenMenu.Equals(removeCell))
            {
                BuildingController.RemoveBuilding(building);
            }
            else if (choosenMenu.Equals(upgradeUnits))
            {
				this.upgradeMenu();
            }
            else if (choosenMenu.Equals(moveUnits))
            {
                this.tobii.SetFeedbackColor(Color.Red);

				Selection destsel = this.retrieveSelection();
				if (destsel.state == State.BUILDING || destsel.state == State.TILE)
				{
					Tile selTile = map.GetTile(destsel.absPoint);
					UnitController.MoveUnits(this.playerInControll, this.selectedTile, selTile, building.GetUnits().Count);
				}

				this.tobii.SetFeedbackColor(Color.White);
            }
            else if (choosenMenu.Equals(repairCell))
            {
                this.playerInControll.unitAcc.DestroyUnits(building.units, toHeal);
                building.Repair(toHeal);
            }
            else if (choosenMenu.Equals(setAggro))
            {
				building.IsAggressive = !building.IsAggressive;
				building.UpdateAggressiveness(null, new Event<IEnumerable<Unit>>(building.GetUnits(), EventType.ADD));
            }
            else if (choosenMenu.Equals(Cancel))
            {
                return;
            }
            else
            {
                return;
            }
        }

        private void ContextMenu()
        {
            if (this.selectedTile == null)
            {
                return;
            }
			
            if (this.selectedTile.GetBuilding() != null)
            {
                this.BuildingMenu();
            }
            else if (this.selectedTile.GetUnits(this.playerInControll).Count > 0)
            {
                this.TileMenu();
            }
        }

        private void GameMenu()
        {
            var endTurn = new MenuIcon(Language.Instance.GetString("EndTurn"));
            var endGame = new MenuIcon(Language.Instance.GetString("EndGame"));
            var cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
			
            var options = new List<MenuIcon>(4);
            options.Add(endTurn);
            options.Add(endGame);
            options.Add(cancel);
			
            var menu = new Menu(Globals.MenuLayout.FourMatrix, options, string.Empty);
            MenuController.LoadMenu(menu);
			
            bool done = false;
            while(! done)
            {
                Recellection.CurrentState = MenuView.Instance;
                MenuIcon input = MenuController.GetInput();
                if (input == endTurn)
                {
                    finished = true;
                    break;
                }
                else if (input == endGame)
                {
                    var promptOptions = new List<MenuIcon>(2);
                    var yes = new MenuIcon(Language.Instance.GetString("Yes"), Recellection.textureMap.GetTexture(Globals.TextureTypes.Yes));
                    var no = new MenuIcon(Language.Instance.GetString("No"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
                    promptOptions.Add(yes);
                    promptOptions.Add(no);
                    MenuController.LoadMenu(new Menu(Globals.MenuLayout.Prompt, promptOptions, Language.Instance.GetString("AreYouSureYouWantToEndTheGame")));
                    MenuIcon inp = MenuController.GetInput();
                    MenuController.UnloadMenu();
					
                    if (inp == yes)
                    {
                        // This should make the player lose :D
                        var buildingsToRemove = new List<Building>();
                        foreach(Graph g in this.playerInControll.GetGraphs())
                        {
                            foreach(Building b in g.GetBuildings())
                            {
                                buildingsToRemove.Add(b);
                            }
                        }

                        foreach(Building b in buildingsToRemove)
                        {
                            BuildingController.RemoveBuilding(b);
                        }

                        finished = true;
                        break;
                    }
                }
                else if (input == cancel)
                {
                    break;
                }
            }

            Recellection.CurrentState = WorldView.Instance;
            MenuController.UnloadMenu();
        }

        private void RemoveconstructionTileLines(List<Point> tileCoords)
        {
            for (int i = 0; i < 2; i++)
            {
                this.theWorld.map.GetTile(tileCoords[i].X, tileCoords[i].Y).ClearDrawLine();
            }

            this.theWorld.DrawConstructionLines = null;
        }

        private void SelectTile(Tile t)
        {
            // If this is the first time we select a tile...
            if (this.selectedTile != null)
                this.selectedTile.active = false;

            this.selectedTile = t;
            this.selectedTile.active = true;
        }

        private void SetConstructionLines(List<Point> tileCoords)
        {
            float onTileOffset = (Globals.TILE_SIZE - 1) / ((float)(Globals.TILE_SIZE));
            float constructionAreaOffsetX = tileCoords[1].X - tileCoords[0].X;
            float constructionAreaOffsetY = tileCoords[1].Y - tileCoords[0].Y;

            for (int i = 0; i < 2; i++)
            {
                var tempVectors = new List<Vector2>(2);

                tempVectors.Add(new Vector2(tileCoords[i].X + onTileOffset * i, tileCoords[i].Y + onTileOffset * i));
                tempVectors.Add(new Vector2(tileCoords[i].X + onTileOffset * i, tileCoords[i].Y + onTileOffset * ((i + 1) & 1) + constructionAreaOffsetY * (1 - i * 2)));

                tempVectors.Add(new Vector2(tileCoords[i].X + onTileOffset * i, tileCoords[i].Y + onTileOffset * i));
                tempVectors.Add(new Vector2(tileCoords[i].X + onTileOffset * ((i + 1) & 1) + constructionAreaOffsetX * (1 - i*2), tileCoords[i].Y + onTileOffset * i));

                Tile temp = this.theWorld.map.GetTile(tileCoords[i].X, tileCoords[i].Y);
                temp.SetDrawLine(tempVectors);
            }

            this.theWorld.DrawConstructionLines = tileCoords;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousSelection"></param>
		private void TileMenu()
		{
			var moveUnits = new MenuIcon(Language.Instance.GetString("MoveUnits"), null, Color.Black);
			var cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No), Color.Black);
			
			var menuIcons = new List<MenuIcon>();
			if (this.theWorld.GetMap().GetTile(this.previousSelection.absPoint).GetUnits(this.playerInControll).Count > 0)
			{
				// Only show this options if there are units.
				menuIcons.Add(moveUnits);
			}

			menuIcons.Add(cancel);

			var buildingMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("TileMenu"), Color.Black);
			MenuController.LoadMenu(buildingMenu);
			Recellection.CurrentState = MenuView.Instance;
			MenuIcon choosenMenu = MenuController.GetInput();
			Recellection.CurrentState = WorldView.Instance;
			MenuController.UnloadMenu();
			
			if (choosenMenu == moveUnits)
			{
				Selection currSel = this.retrieveSelection();
				if (! (currSel.state == State.TILE || currSel.state == State.BUILDING))
				{
					return;
				}
				
				
				Tile from = this.theWorld.GetMap().GetTile(this.previousSelection.absPoint);
				this.SelectTile(this.theWorld.GetMap().GetTile(currSel.absPoint));

				UnitController.MoveUnits(this.playerInControll, from, this.selectedTile, from.GetUnits().Count);
			}
		}

        private void createGUIRegionGridAndScrollZone()
        {
            int numOfRows = (Recellection.viewPort.Height / Globals.TILE_SIZE) - 2;
            int numOfCols = (Recellection.viewPort.Width / Globals.TILE_SIZE) - 2;

            this.menuMatrix = new MenuIcon[numOfCols, numOfRows];

            this.scrollZone = new List<MenuIcon>();

            // This will create a matrix with menuIcons, ignoring the ones
            // closest to the edge.
            for (int x = 0; x < numOfCols; x++)
            {
                for (int y = 0; y < numOfRows; y++)
                {
                    this.menuMatrix[x, y] = new MenuIcon(
                        string.Empty + (x + 1) + "_" + (y + 1), null, Color.NavajoWhite);

                    // Should not need a targetRectangle.
                    /*menuMatrix[x, y].targetRectangle = new Microsoft.Xna.Framework.Rectangle(
                        x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                    */
                    // x + 1 and y + 1 should make them not be placed at the edge.
                    this.menuMatrix[x, y].region = new GUIRegion(
                        Recellection.windowHandle, 
                        new System.Windows.Rect(
                            (x + 1) * Globals.TILE_SIZE, 
                            (y + 1) * Globals.TILE_SIZE, 
                            Globals.TILE_SIZE, 
                            Globals.TILE_SIZE));
                }
            }

            /*
             * The following code creates the scroll zones with this pattern:
             * 1 2 2 2 2 3
             * 4         5
             * 4         5
             * 4         5
             * 6 7 7 7 7 8
             * 
             * A number indicates wich index it has in the list, the label describes its position
             */
            int windowWidth = Recellection.viewPort.Width;
            int windowHeight = Recellection.viewPort.Height;

            // Will code the scroll zones in one line.

            // First is a tile sized square top left on the screen.
            this.scrollZone.Add(new MenuIcon("-1_-1", null, Color.Chocolate));

            this.scrollZone[0].region = new GUIRegion(
                Recellection.windowHandle, new System.Windows.Rect(0, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));
            this.scrollZone[0].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[0].region.HideFeedbackIndicator = true;

            // Second is a laying rectangle spanning the screen width minus two tile widths.
            this.scrollZone.Add(new MenuIcon("0_-1", null, Color.Chocolate));

            this.scrollZone[1].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(Globals.TILE_SIZE, 0, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));
            this.scrollZone[1].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[1].region.HideFeedbackIndicator = true;

            // Third is like the first but placed to the far right.
            this.scrollZone.Add(new MenuIcon("1_-1", null, Color.Chocolate));

            this.scrollZone[2].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));
            this.scrollZone[2].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[2].region.HideFeedbackIndicator = true;

            // Fourth is a standing rectangle at the left side of the screen, its height is screen height minus two tile heights.
            this.scrollZone.Add(new MenuIcon("-1_0", null, Color.Chocolate));

            this.scrollZone[3].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(0, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE * 2));
            this.scrollZone[3].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[3].region.HideFeedbackIndicator = true;

            // Fift is the same as the right but placed at the right side of the screen.
            this.scrollZone.Add(new MenuIcon("1_0", null, Color.Chocolate));

            this.scrollZone[4].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(
                    windowWidth - Globals.TILE_SIZE, 
                    Globals.TILE_SIZE, 
                    Globals.TILE_SIZE, 
                    windowHeight - Globals.TILE_SIZE * 2));
            this.scrollZone[4].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[4].region.HideFeedbackIndicator = true;

            // Like the first but at the bottom
            this.scrollZone.Add(new MenuIcon("-1_1", null, Color.Chocolate));

            this.scrollZone[5].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(0, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
            this.scrollZone[5].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[5].region.HideFeedbackIndicator = true;

            // Like the second but at the bottom
            this.scrollZone.Add(new MenuIcon("0_1", null, Color.Chocolate));

            this.scrollZone[6].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(
                    Globals.TILE_SIZE, 
                    windowHeight - Globals.TILE_SIZE, 
                    windowWidth - Globals.TILE_SIZE * 2, 
                    Globals.TILE_SIZE));
            this.scrollZone[6].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[6].region.HideFeedbackIndicator = true;

            // Like the third but at the bottom
            this.scrollZone.Add(new MenuIcon("1_1", null, Color.Chocolate));

            this.scrollZone[7].region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(
                    windowWidth - Globals.TILE_SIZE, 
                    windowHeight - Globals.TILE_SIZE, 
                    Globals.TILE_SIZE, 
                    Globals.TILE_SIZE));
            this.scrollZone[7].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            this.scrollZone[7].region.HideFeedbackIndicator = true;

            var allMenuIcons = new List<MenuIcon>();

            foreach (MenuIcon mi in this.menuMatrix)
            {
                allMenuIcons.Add(mi);
            }

            foreach (MenuIcon mi in this.scrollZone)
            {
                allMenuIcons.Add(mi);
            }

            // here be offscreen regions!
            this.leftOff =
                new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(-700, 0, 700, Globals.VIEWPORT_HEIGHT)));
            this.rightOff =
                new MenuIcon(
                    new GUIRegion(
                        IntPtr.Zero, new System.Windows.Rect(Globals.VIEWPORT_WIDTH, 0, 700, Globals.VIEWPORT_HEIGHT)));
            this.topOff =
                new MenuIcon(
                    new GUIRegion(
                        IntPtr.Zero, new System.Windows.Rect(0, Globals.VIEWPORT_HEIGHT, Globals.VIEWPORT_WIDTH, 700)));
            this.botOff =
                new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(0, -700, Globals.VIEWPORT_WIDTH, 700)));
            MenuController.LoadMenu(new Menu(allMenuIcons, this.leftOff, this.rightOff, this.topOff, this.botOff));
            MenuController.DisableMenuInput();
        }

        private void upgradeMenu()
        {
            Building building = this.selectedTile.GetBuilding();

            var speed = new MenuIcon(Language.Instance.GetString("UpgradeSpeed"), null, Color.Black);
            var power = new MenuIcon(Language.Instance.GetString("UpgradePower"), null, Color.Black);
            var cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No), Color.Black);

            var menuIcons = new List<MenuIcon>();
            menuIcons.Add(speed);
            menuIcons.Add(power);
            menuIcons.Add(cancel);

            var upgradeMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("UpgradeMenu"), Color.Black);
            MenuController.LoadMenu(upgradeMenu);
            Recellection.CurrentState = MenuView.Instance;
            MenuIcon chosenMenu = MenuController.GetInput();
            Recellection.CurrentState = WorldView.Instance;
            MenuController.UnloadMenu();

            if (chosenMenu == speed)
            {
                if (!this.playerInControll.unitAcc.PayAndUpgradeSpeed(building))
                {
                    // SoundsController.playSound("Denied");
                }
            }
            else if (chosenMenu == power)
            {
                if (!this.playerInControll.unitAcc.PayAndUpgradePower(building))
                {
                    // SoundsController.playSound("Denied");
                }
            }
        }

        #endregion

        public struct Selection
        {
            #region Fields

            public Point absPoint;

            public Point point;

            public State state;

            #endregion
        }
    }
}
