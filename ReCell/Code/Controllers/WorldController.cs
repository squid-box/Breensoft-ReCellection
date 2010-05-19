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
			sel.state = State.NONE;
			finished = false;
            while (!finished)
            {
				previousSelection = sel;
				
                // Generate the appropriate menu for this state.
                // Get the active GUI Region and invoke the associated method.
				sel = retrieveSelection();
				
                // They are used if the state needs true coordinates, scroll only uses deltas.

				World.Map map = theWorld.GetMap();

				// If this is the first time we select a tile...
				if(selectedTile != null)
					selectedTile.active = false;
				selectedTile = map.GetTile(sel.absPoint);
				selectedTile.active = true;

				if (sel.point.X == 1 && sel.point.Y == 1)
				{
					finished = true;
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
            }
        }

		public Selection retrieveSelection()
		{
			myLogger.Debug("Waiting for input...");
			MenuIcon activatedMenuIcon = MenuController.GetInput();
						 
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
            else if (activatedMenuIcon.labelColor.Equals(Color.Chocolate))
            {
				theWorld.LookingAt = new Point(theWorld.LookingAt.X + x, theWorld.LookingAt.Y + y);
				return retrieveSelection();
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
			MenuIcon upgradeUnits = new MenuIcon(Language.Instance.GetString("UpgradeUnits") + " (" + playerInControll.unitAcc.getUpgradeCost() + ")");
            MenuIcon moveUnits = new MenuIcon(Language.Instance.GetString("MoveUnits"));
            MenuIcon repairCell = new MenuIcon(Language.Instance.GetString("RepairCell") + " (" + toHeal + ")");
            MenuIcon Cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            
            List<MenuIcon> menuIcons = new List<MenuIcon>();
            menuIcons.Add(setWeight);
            menuIcons.Add(buildCell);
            menuIcons.Add(removeCell);
            menuIcons.Add(upgradeUnits);
            menuIcons.Add(moveUnits);
            menuIcons.Add(repairCell);
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
                Selection destsel = retrieveSelection();
                if (destsel.state != State.TILE)
				{
					Sounds.Instance.LoadSound("Denied");
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
                        //TODO CO DO STUFF HERE.
                    }
				}
				else
				{
					Sounds.Instance.LoadSound("Denied");
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
                    Sounds.Instance.LoadSound("Denied");
                }
            }
            else if (choosenMenu.Equals(moveUnits))
            {
               
                
                tobii.SetFeedbackColor(Color.Red);
                
                Selection destsel = retrieveSelection();
                Tile selectedTile = map.GetTile(destsel.absPoint);
                UnitController.MoveUnits(building.GetUnits().Count, seltile, selectedTile);
                
                tobii.SetFeedbackColor(Color.White);

            }
            else if (choosenMenu.Equals(repairCell))
            {
                playerInControll.unitAcc.DestroyUnits(building.units, toHeal);
                building.Repair(toHeal);
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
			MenuIcon cancel = new MenuIcon(Language.Instance.GetString("Cancel"), null, Color.Black);
			
			List<MenuIcon> menuIcons = new List<MenuIcon>();
			menuIcons.Add(moveUnits);
			menuIcons.Add(cancel);

			Menu buildingMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("TileMenu"), Color.Black);
			MenuController.LoadMenu(buildingMenu);
			Recellection.CurrentState = MenuView.Instance;
			MenuIcon choosenMenu = MenuController.GetInput();
			Recellection.CurrentState = WorldView.Instance;
			MenuController.UnloadMenu();
			
			if (choosenMenu == moveUnits)
			{
				Selection currSel = retrieveSelection();
				if (currSel.state != State.TILE)
				{
					return;
				}
				
				Tile from = theWorld.GetMap().GetTile(previousSelection.absPoint);
				Tile to = theWorld.GetMap().GetTile(currSel.absPoint);

				UnitController.MoveUnits(from.GetUnits().Count, from, to);
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

            MenuController.LoadMenu(new Menu(allMenuIcons));
            MenuController.DisableMenuInput();
        }
    }
}
