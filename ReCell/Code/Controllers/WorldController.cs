using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// The purpose of this component is to control the entire world. It is part of the realization of SR1.7.
    /// </summary>
    class WorldController
    {
        /// <summary>
        /// The different states this controller will assume
        /// </summary>
        public enum State { NONE, BUILDING, TILE, MENU };
        
        public struct Selection
        {
			public State state;
            public Point point;
            public Point absPoint;
        }
        
        private const long SCROLL_ZONE_DWELL_TIME = 0;//250000;
        private char[] REG_EXP = { '_' };
        public static bool finished { get; set; }
        private Logger myLogger;
        
		private Selection previousSelection;
		private Tile selectedTile;
		
		private Player playerInControll;

        private World theWorld;
        TobiiController tobii = TobiiController.GetInstance(Recellection.windowHandle);
        private MenuIcon[,] menuMatrix;
        private List<MenuIcon> scrollZone;
		
		private static Logger logger = LoggerFactory.GetLogger();

        //offscreenregions
        MenuIcon topOff;
        MenuIcon botOff;
        MenuIcon leftOff;
		MenuIcon rightOff;
        // Create 
        public WorldController(Player p, World theWorld)
        {
            //Debugging
            finished = false;
            myLogger = LoggerFactory.GetLogger();
            myLogger.SetThreshold(LogLevel.TRACE);

            this.playerInControll = p;
            this.theWorld = theWorld;

            createGUIRegionGridAndScrollZone();
        }
        
        public void Stop()
        {
			MenuController.UnloadMenu();
        }

        public void Run()
		{
			Selection sel = new Selection();
			logger.Info("Logger started");

			sel.state = State.NONE;
			finished = false;
            while (!finished)
            {
				//previousSelection = sel;
				
                // Generate the appropriate menu for this state.
                // Get the active GUI Region and invoke the associated method.
                MenuIcon activatedMenuIcon = MenuController.GetInput();
                if (activatedMenuIcon == leftOff)
                {
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(false);
                    TileMenu(previousSelection);
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(true);
				}
                else if (activatedMenuIcon == rightOff)
                {
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(false);
                    BuildingMenu(previousSelection);
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(true);
                }
                else if (activatedMenuIcon == topOff)
                {
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(false);
                    GameMenu();
					TobiiController.GetInstance(Recellection.windowHandle).SetRegionsEnabled(true);
                }
                else if (activatedMenuIcon == botOff)
                {

                }
                else
                {
                    sel = retrieveSelection(activatedMenuIcon);
                    previousSelection = sel;
                    // They are used if the state needs true coordinates, scroll only uses deltas.

                    World.Map map = theWorld.GetMap();

                    // If this is the first time we select a tile...
                    if (selectedTile != null)
                        selectedTile.active = false;
                    selectedTile = map.GetTile(sel.absPoint);
                    selectedTile.active = true;
                    /*
                    if (sel.point.X == 1 && sel.point.Y == 1)
                    {
                        GameMenu();
                    }
                    if (sel.point.X == 2 && sel.point.Y == 1)
                    {
                        if (previousSelection.state == State.BUILDING)
                        {
                            BuildingMenu(previousSelection);
                        }
                        else if (previousSelection.state == State.TILE)
                        {
                            TileMenu(previousSelection);
                        }
                    }
                     * */
                }
            }
        }

		private void GameMenu()
		{
			MenuIcon endTurn = new MenuIcon(Language.Instance.GetString("EndTurn"));
			MenuIcon endGame = new MenuIcon(Language.Instance.GetString("EndGame"));
			MenuIcon cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
			
			List<MenuIcon> options = new List<MenuIcon>(4);
			options.Add(endTurn);
			options.Add(endGame);
			options.Add(cancel);
			
			Menu menu = new Menu(Globals.MenuLayout.FourMatrix, options, "");
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
					List<MenuIcon> promptOptions = new List<MenuIcon>(2);
					MenuIcon yes = new MenuIcon(Language.Instance.GetString("Yes"), Recellection.textureMap.GetTexture(Globals.TextureTypes.Yes));
					MenuIcon no = new MenuIcon(Language.Instance.GetString("No"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
					promptOptions.Add(yes);
					promptOptions.Add(no);
					MenuController.LoadMenu(new Menu(Globals.MenuLayout.Prompt, promptOptions, Language.Instance.GetString("AreYouSureYouWantToEndTheGame")));
					MenuIcon inp = MenuController.GetInput();
					MenuController.UnloadMenu();
					
					if (inp == yes)
					{
						// This should make the player lose :D
						List<Building> buildingsToRemove = new List<Building>();
						foreach(Graph g in playerInControll.GetGraphs())
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

        public Selection getSelection()
        {
			while (true)
			{
				MenuIcon activatedMenuIcon = MenuController.GetInput();
				if (activatedMenuIcon.label != null)
				{
					return retrieveSelection(activatedMenuIcon);
				}
			}
        }

        public Selection retrieveSelection(MenuIcon activatedMenuIcon)
		{

			myLogger.Debug("Waiting for input...");
				
		    int x = 0;
            int y = 0;
            String[] splitted = activatedMenuIcon.label.Split(REG_EXP);
            try
            {
                myLogger.Trace("Splitted string = " + splitted[0] + "\t" + splitted[1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("Your argument is invalid, my beard is a windmill.");
            }

			x = Int32.Parse(splitted[0]);
			y = Int32.Parse(splitted[1]);

            Point absoluteCordinate = new Point(x + theWorld.LookingAt.X, y + theWorld.LookingAt.Y);
            Selection s = new Selection();
            if(activatedMenuIcon.labelColor.Equals(Color.NavajoWhite))
            {
				if (theWorld.GetMap().GetTile(new Point(x + theWorld.LookingAt.X, y + theWorld.LookingAt.Y))
						.GetBuilding() != null)
				{
                    tobii.SetFeedbackColor(Color.Blue);
					s.state = State.BUILDING;
					s.point = new Point(x, y);
                    s.absPoint = absoluteCordinate;
				}
				else
				{
                    tobii.SetFeedbackColor(Color.White);
					s.state = State.TILE;
					s.point = new Point(x, y);
                    s.absPoint = absoluteCordinate;
				}
            }
			// If we selected a scroll zone?
            else if (activatedMenuIcon.labelColor.Equals(Color.Chocolate))
            {
				theWorld.LookingAt = new Point(theWorld.LookingAt.X + x, theWorld.LookingAt.Y + y);
				return getSelection();
            }
            else
            {
                throw new ArgumentException("Your argument is invalid, my beard is a windmill.");
            }
            return s;
		}
		
		/// <summary>
		/// Loads the Building menu from a selection.
		/// Must have building on tile.
		/// </summary>
		/// <param name="theSelection"></param>
        private void BuildingMenu(Selection theSelection)
        {
            
            World.Map map = theWorld.GetMap();
            Tile seltile = map.GetTile(theSelection.absPoint);
            Building building = seltile.GetBuilding();
            if (building == null || building.owner != playerInControll)
            {
                return;
            }
            int toHeal = Math.Min(building.maxHealth - building.currentHealth, building.units.Count());

            MenuIcon setWeight = new MenuIcon(Language.Instance.GetString("SetWeight"));
            MenuIcon buildCell = new MenuIcon(Language.Instance.GetString("BuildCell"));
            MenuIcon removeCell = new MenuIcon(Language.Instance.GetString("RemoveCell"));
			MenuIcon upgradeUnits = new MenuIcon(Language.Instance.GetString("UpgradeUnits") + " (" + playerInControll.unitAcc.GetUpgradeCost() + ")");
            MenuIcon moveUnits = new MenuIcon(Language.Instance.GetString("MoveUnits"));
            MenuIcon repairCell = new MenuIcon(Language.Instance.GetString("RepairCell") + " (" + toHeal + ")");
            MenuIcon setAggro = new MenuIcon(Language.Instance.GetString("SetAggro"));
            MenuIcon Cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            
            List<MenuIcon> menuIcons = new List<MenuIcon>();
            menuIcons.Add(setWeight);
            menuIcons.Add(buildCell);
            menuIcons.Add(removeCell);
            menuIcons.Add(upgradeUnits);
            menuIcons.Add(moveUnits);
            menuIcons.Add(repairCell);
            menuIcons.Add(setAggro);
            menuIcons.Add(Cancel);

            Menu buildingMenu = new Menu(Globals.MenuLayout.NineMatrix, menuIcons, Language.Instance.GetString("BuildingMenu"), Color.Black);
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
                tobii.SetFeedbackColor(Color.DarkGreen);
                Selection destsel = getSelection();
                if (destsel.state != State.TILE)
				{
					SoundsController.playSound("Denied");
                    tobii.SetFeedbackColor(Color.White);
					return;
				}
                Tile selectedTile = map.GetTile(destsel.absPoint);

                //TODO Add a check to see if the tile is a correct one. The diffrence between the selected tiles coordinates and the source building shall not exceed 3.
				if (selectedTile.GetBuilding() == null)
                {
                    try
                    {
                        BuildingController.ConstructBuilding(playerInControll, selectedTile, building, theWorld);
                        tobii.SetFeedbackColor(Color.White);
                    }
                    catch (BuildingController.BuildingOutOfRangeException bore)
                    {
						logger.Debug("Caught BuildingOutOfRangeExcpetion");
                    }
				}
				else
				{
                    SoundsController.playSound("Denied");
                    tobii.SetFeedbackColor(Color.White);
					return;
				}
            }
            else if (choosenMenu.Equals(removeCell))
            {
                BuildingController.RemoveBuilding(building);
            }
            else if (choosenMenu.Equals(upgradeUnits))
            {
                if (!playerInControll.unitAcc.PayAndUpgrade(building))
                {
                    SoundsController.playSound("Denied");
                }
            }
            else if (choosenMenu.Equals(moveUnits))
            {
                tobii.SetFeedbackColor(Color.Red);
                
                Selection destsel = getSelection();
                Tile selectedTile = map.GetTile(destsel.absPoint);
                UnitController.MoveUnits(playerInControll, seltile, selectedTile, building.GetUnits().Count);
                
                tobii.SetFeedbackColor(Color.White);

            }
            else if (choosenMenu.Equals(repairCell))
            {
                playerInControll.unitAcc.DestroyUnits(building.units, toHeal);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousSelection"></param>
		private void TileMenu(Selection previousSelection)
		{
			MenuIcon moveUnits = new MenuIcon(Language.Instance.GetString("MoveUnits"), null, Color.Black);
			MenuIcon cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No), Color.Black);
			
			List<MenuIcon> menuIcons = new List<MenuIcon>();
			if (theWorld.GetMap().GetTile(previousSelection.absPoint).GetUnits(playerInControll).Count > 0)
			{
				// Only show this options if there are units.
				menuIcons.Add(moveUnits);
			}
			menuIcons.Add(cancel);

			Menu buildingMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("TileMenu"), Color.Black);
			MenuController.LoadMenu(buildingMenu);
			Recellection.CurrentState = MenuView.Instance;
			MenuIcon choosenMenu = MenuController.GetInput();
			Recellection.CurrentState = WorldView.Instance;
			MenuController.UnloadMenu();
			
			if (choosenMenu == moveUnits)
			{
				Selection currSel = getSelection();
				if (! (currSel.state == State.TILE || currSel.state == State.BUILDING))
				{
					return;
				}
				
				Tile from = theWorld.GetMap().GetTile(previousSelection.absPoint);
				Tile to = theWorld.GetMap().GetTile(currSel.absPoint);

				UnitController.MoveUnits(playerInControll, from, to, from.GetUnits().Count);
			}
		}

        private void createGUIRegionGridAndScrollZone()
        {
            int numOfRows = (int)(Recellection.viewPort.Height / Globals.TILE_SIZE) - 2;
            int numOfCols = (int)(Recellection.viewPort.Width / Globals.TILE_SIZE) - 2;

            menuMatrix = new MenuIcon[numOfCols, numOfRows];

            scrollZone = new List<MenuIcon>();

            //This will create a matrix with menuIcons, ignoring the ones
            //closest to the edge.
            for (int x = 0; x < numOfCols; x++)
            {
                for (int y = 0; y < numOfRows; y++)
                {
                    menuMatrix[x, y] = new MenuIcon("" + (x + 1) + "_" + (y + 1), null, Color.NavajoWhite);

                    //Should not need a targetRectangle.
                    /*menuMatrix[x, y].targetRectangle = new Microsoft.Xna.Framework.Rectangle(
                        x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                    */
                    //x + 1 and y + 1 should make them not be placed at the edge.
                    menuMatrix[x, y].region = new GUIRegion(Recellection.windowHandle,
                        new System.Windows.Rect((x + 1) * Globals.TILE_SIZE, (y + 1) * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
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


            #region UglyCode
            int windowWidth = Recellection.viewPort.Width;
            int windowHeight = Recellection.viewPort.Height;
            //Will code the scroll zones in one line.

            //First is a tile sized square top left on the screen.
            scrollZone.Add(new MenuIcon("-1_-1", null, Color.Chocolate));

            scrollZone[0].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));
            scrollZone[0].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[0].region.HideFeedbackIndicator = true;

            //Second is a laying rectangle spanning the screen width minus two tile widths.
            scrollZone.Add(new MenuIcon("0_-1", null, Color.Chocolate));

            scrollZone[1].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Globals.TILE_SIZE, 0, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));
            scrollZone[1].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[1].region.HideFeedbackIndicator = true;

            //Third is like the first but placed to the far right.
            scrollZone.Add(new MenuIcon("1_-1", null, Color.Chocolate));

            scrollZone[2].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));
            scrollZone[2].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[2].region.HideFeedbackIndicator = true;

            //Fourth is a standing rectangle at the left side of the screen, its height is screen height minus two tile heights.
            scrollZone.Add(new MenuIcon("-1_0", null, Color.Chocolate));

            scrollZone[3].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE * 2));
            scrollZone[3].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[3].region.HideFeedbackIndicator = true;

            //Fift is the same as the right but placed at the right side of the screen.
            scrollZone.Add(new MenuIcon("1_0", null, Color.Chocolate));

            scrollZone[4].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE * 2));
            scrollZone[4].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[4].region.HideFeedbackIndicator = true;

            //Like the first but at the bottom
            scrollZone.Add(new MenuIcon("-1_1", null, Color.Chocolate));

            scrollZone[5].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
            scrollZone[5].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[5].region.HideFeedbackIndicator = true;
            //Like the second but at the bottom
            scrollZone.Add(new MenuIcon("0_1", null, Color.Chocolate));

            scrollZone[6].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));
            scrollZone[6].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[6].region.HideFeedbackIndicator = true;

            //Like the third but at the bottom
            scrollZone.Add(new MenuIcon("1_1", null, Color.Chocolate));

            scrollZone[7].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
            scrollZone[7].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[7].region.HideFeedbackIndicator = true;

            #endregion

            List<MenuIcon> allMenuIcons = new List<MenuIcon>();

            foreach (MenuIcon mi in menuMatrix)
            {
                allMenuIcons.Add(mi);
            }
            foreach (MenuIcon mi in scrollZone)
            {
                allMenuIcons.Add(mi);
            }
            //here be offscreen regions!
            leftOff = new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(-700, 0, 700, Globals.VIEWPORT_HEIGHT)));
            rightOff = new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(Globals.VIEWPORT_WIDTH, 0, 700, Globals.VIEWPORT_HEIGHT)));
            topOff = new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(0, Globals.VIEWPORT_HEIGHT, Globals.VIEWPORT_WIDTH, 700)));
            botOff = new MenuIcon(new GUIRegion(IntPtr.Zero, new System.Windows.Rect(0, -700, Globals.VIEWPORT_WIDTH, 700)));
            MenuController.LoadMenu(new Menu(allMenuIcons, leftOff, rightOff, topOff, botOff));
            MenuController.DisableMenuInput();
        }
    }
}
