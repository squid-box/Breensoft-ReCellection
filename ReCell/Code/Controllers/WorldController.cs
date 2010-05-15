using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
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
        public enum State { NONE, BUILDING, TILE, MENU, ZOOMED, SCROLL };
        
        public struct Selection
        {
			public State state;
			public Point point;
        }
        
        private const long SCROLL_ZONE_DWELL_TIME = 0;//250000;
        private char[] REG_EXP = { '_' };
        public bool finished { get; set; }
        private Logger myLogger;
        
        private State state;
        private Selection selection;

		private Building selectedBuilding;
		private Tile selectedTile;
		
		private Player playerInControll;

        private World theWorld;

        private MenuIcon[,] menuMatrix;
        private List<MenuIcon> scrollZone;

        // Create 
        public WorldController(Player p, World theWorld)
        {
            state = State.NONE;
            //Debugging
            finished = false;
            myLogger = LoggerFactory.GetLogger();
            myLogger.SetThreshold(LogLevel.TRACE);

            this.playerInControll = p;
            this.theWorld = theWorld;

            createGUIRegionGridAndScrollZone();
            
        }

        public void Run()
        {
			finished = false;
            while (!finished)
            {
				
                // Generate the appropriate menu for this state.
                // Get the active GUI Region and invoke the associated method.
				Selection sel = retrieveSelection();

				
                // They are used if the state needs true coordinates, scroll only uses deltas.
                Point absoluteCoordinate = new Point(sel.point.X + theWorld.LookingAt.X, 
													 sel.point.Y + theWorld.LookingAt.Y);
				
				World.Map map = theWorld.GetMap();

                switch (sel.state)
                {
                    case State.TILE:
                        // A tile has been selected, store it.
						// Save the selected tile, for later!
                        selectedTile = map.GetTile(absoluteCoordinate);

						// Debug finish
						if (sel.point.X == 1 && sel.point.Y == 1)
						{
							finished = true;
						}
                        break;
					case State.BUILDING:
						// A fromBuilding has been selected!

						// TODO: Let BuildingController do shit! (use retrieveSelection)
						// TODO:  - Activate menu (what u wanna do /w fromBuilding?)
						// TODO:  - DO SHIT!

						selectedBuilding = map.GetTile(absoluteCoordinate).GetBuilding();

						sel = retrieveSelection();
						absoluteCoordinate = new Point(sel.point.X + theWorld.LookingAt.X,
													 sel.point.Y + theWorld.LookingAt.Y);
						
						if (sel.state != State.TILE)
						{
							//Sounds.Instance.LoadSound("Denied").Play();
							continue;
						}
						
						selectedTile = map.GetTile(absoluteCoordinate);



						//do stuff here TODO co
                        MenuIcon baseCell = new MenuIcon(Language.Instance.GetString("BaseCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding), Color.Black);
                        MenuIcon resourceCell = new MenuIcon(Language.Instance.GetString("ResourceCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding), Color.Black);
                        MenuIcon defensiveCell = new MenuIcon(Language.Instance.GetString("DefensiveCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding), Color.Black);
                        MenuIcon aggressiveCell = new MenuIcon(Language.Instance.GetString("AggressiveCell"), Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding), Color.Black);
                        List<MenuIcon> menuIcons =new List<MenuIcon>();
                        menuIcons.Add(baseCell);
                        menuIcons.Add(resourceCell);
                        menuIcons.Add(defensiveCell);
                        menuIcons.Add(aggressiveCell);
                        Menu BuildingMenu = new Menu(Globals.MenuLayout.FourMatrix, menuIcons, Language.Instance.GetString("ChooseBuilding"), Color.Black);
                        MenuController.LoadMenu(BuildingMenu);
                        Recellection.CurrentState = MenuView.Instance;
                        Globals.BuildingTypes Building;

                        MenuIcon choosenMenu = MenuController.GetInput();
                        if(choosenMenu.Equals(baseCell)){
                            Building = Globals.BuildingTypes.Base;
                        }else if(choosenMenu.Equals(resourceCell)){
                            Building = Globals.BuildingTypes.Resource;
                        }else if(choosenMenu.Equals(defensiveCell)){
                            Building = Globals.BuildingTypes.Barrier;
                        }else{
                            Building = Globals.BuildingTypes.Aggressive;
                        }


						// If we have selected a tile, and we can place a building at the selected tile...					

						if (selectedBuilding != null
						 && selectedTile.GetBuilding() == null
						 && selectedBuilding.owner == playerInControll)
						{
							if (! BuildingController.AddBuilding(Building, selectedBuilding,
									selectedTile.position, theWorld, playerInControll))
							{
								Sounds.Instance.LoadSound("Denied").Play();
							}

							selectedBuilding = null;
							
							// We're done here
							finished = true;
						}
                        break;
                    case State.SCROLL:
						theWorld.LookingAt = absoluteCoordinate; 
                        break;
                }
			}
			//Sounds.Instance.LoadSound("acid").Play();
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
			
            Selection s = new Selection();
            if(activatedMenuIcon.labelColor.Equals(Color.NavajoWhite))
            {
				if (theWorld.GetMap().GetTile(new Point(x + theWorld.LookingAt.X, y + theWorld.LookingAt.Y))
						.GetBuilding() != null)
				{
					s.state = State.BUILDING;
					s.point = new Point(x, y);
				}
				else
				{
					s.state = State.TILE;
					s.point = new Point(x, y);
				}
            }
            else if (activatedMenuIcon.labelColor.Equals(Color.Chocolate))
            {
                s.state = State.SCROLL;
                s.point = new Point(x, y);
            }
            else
            {
                throw new ArgumentException("Your argument is invalid, my beard is a windmill.");
            }
            return s;
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
                    menuMatrix[x, y] = new MenuIcon("" + (x+1) + "_" + (y+1), null,Color.NavajoWhite);

                    //Should not need a targetRectangle.
                    /*menuMatrix[x, y].targetRectangle = new Microsoft.Xna.Framework.Rectangle(
                        x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                    */
                    //x + 1 and y + 1 should make them not be placed at the edge.
                    menuMatrix[x, y].region = new GUIRegion(Recellection.windowHandle,
                        new System.Windows.Rect((x+1) * Globals.TILE_SIZE, (y+1) * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
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
            scrollZone.Add(new MenuIcon("-1_-1",null,Color.Chocolate));
            
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
            
            scrollZone[3].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE*2));
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
        }
    }
}
