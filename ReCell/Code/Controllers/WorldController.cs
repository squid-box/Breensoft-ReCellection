using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

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
        private enum WCState { TILES, BUILDING, TILE, MENU, ZOOMED };
        private const long SCROLL_ZONE_DWELL_TIME = 2500000;
        private char[] REG_EXP = { '_' };
        public bool finished { get; set; }
        private Logger myLogger;
        private WCState state;


        private MenuIcon[,] menuMatrix;
        private List<MenuIcon> scrollZone;

        // Create 
        public WorldController(Player p)
        {
            state = WCState.TILES;
            //Debugging
            finished = true;
            myLogger = LoggerFactory.GetLogger();

            createGUIRegionGridAndScrollZone();
            while (!finished)
            {
                // Generate the appropriate menu for this state.
                // Get the active GUI Region and invoke the associated method.
                switch (state)
                {
                    case WCState.TILES:
                        // A tile has been selected, store it.
                        break;
                    case WCState.BUILDING:
                        // We are in a building menu, do the action mapped to the region on that building
                        break;
                    case WCState.MENU:
                        // We are in the main menu, perform the action mapped to the region on the application
                        break;
                    case WCState.ZOOMED:
                        // We have selected a tile in zoomed-out mode.
                        break;
                }
            }
            
        }

        public Point retriveCoordinateInformation(MenuIcon activatedMenuIcon)
        {
            if(activatedMenuIcon.labelColor.Equals(Color.NavajoWhite))
            {
                String[] splitted = activatedMenuIcon.label.Split(REG_EXP);
                myLogger.Trace("Splitted string = " + splitted[0] + "\t" + splitted[1]);
                
            }
            else if (activatedMenuIcon.labelColor.Equals(Color.Chocolate))
            {
                String[] splitted = activatedMenuIcon.label.Split(REG_EXP);
                myLogger.Trace("Splitted string = " + splitted[0] + "\t" + splitted[1]);
            }
            else
            {
                throw new ArgumentException("Your argument is invalid, my beard is a windmill.");

            }
            return new Point();
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
                    menuMatrix[x, y] = new MenuIcon("" + x + "_" + y, null,Color.NavajoWhite);

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
            scrollZone.Add(new MenuIcon("", null, Color.Chocolate));
            
            scrollZone[3].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE*2));
            scrollZone[3].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[3].region.HideFeedbackIndicator = true;

            //Fift is the same as the right but placed at the right side of the screen.
            scrollZone.Add(new MenuIcon("right", null, Color.Chocolate));
            
            scrollZone[4].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE * 2));
            scrollZone[4].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[4].region.HideFeedbackIndicator = true;

            //Like the first but at the bottom
            scrollZone.Add(new MenuIcon("bottom_left", null, Color.Chocolate));
            
            scrollZone[5].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
            scrollZone[5].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[5].region.HideFeedbackIndicator = true;
            //Like the second but at the bottom
            scrollZone.Add(new MenuIcon("bottom", null, Color.Chocolate));
            
            scrollZone[6].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));
            scrollZone[6].region.DwellTime = new TimeSpan(SCROLL_ZONE_DWELL_TIME);
            scrollZone[6].region.HideFeedbackIndicator = true;

            //Like the third but at the bottom
            scrollZone.Add(new MenuIcon("bottom_right", null, Color.Chocolate));
            
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
