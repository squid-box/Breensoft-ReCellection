using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;

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
        private bool finished;
        private WCState state;
        MenuIcon[,] menuMatrix;
        List<MenuIcon> scrollZone;

        // Create 
        public WorldController(Player p)
        {
            state = WCState.TILES;
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


        private void createGUIRegionGridAndScrollZone()
        {
            int numOfRows = (int)(Recellection.viewPort.Height / Globals.TILE_SIZE) - 2;
            int numOfCols = (int)(Recellection.viewPort.Width / Globals.TILE_SIZE) - 2;

            menuMatrix = new MenuIcon[numOfCols, numOfRows];

            scrollZone = new List<MenuIcon>();

            //This will create a matrix with menuIcons, ignoring the ones
            //closest to the edge.
            for (int x = 1; x < numOfCols-1; x++)
            {
                for (int y = 1; y < numOfRows-1; y++)
                {
                    menuMatrix[x, y] = new MenuIcon("" + x + " " + y, null,Color.NavajoWhite);

                    //Should not need a targetRectangle.
                    /*menuMatrix[x, y].targetRectangle = new Microsoft.Xna.Framework.Rectangle(
                        x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                    */
                    menuMatrix[x, y].region = new GUIRegion(Recellection.windowHandle,
                        new System.Windows.Rect(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
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
            scrollZone.Add(new MenuIcon("top_left",null,Color.NavajoWhite));
            
            scrollZone[0].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));

            //Second is a laying rectangle spanning the screen width minus two tile widths.
            scrollZone.Add(new MenuIcon("top", null, Color.NavajoWhite));
            
            scrollZone[1].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Globals.TILE_SIZE, 0, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));

            //Third is like the first but placed to the far right.
            scrollZone.Add(new MenuIcon("top_right", null, Color.NavajoWhite));
            
            scrollZone[2].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, 0, Globals.TILE_SIZE, Globals.TILE_SIZE));

            //Fourth is a standing rectangle at the left side of the screen, its height is screen height minus two tile heights.
            scrollZone.Add(new MenuIcon("left", null, Color.NavajoWhite));
            
            scrollZone[3].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE*2));

            //Fift is the same as the right but placed at the right side of the screen.
            scrollZone.Add(new MenuIcon("right", null, Color.NavajoWhite));
            
            scrollZone[4].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE * 2));

            //Like the first but at the bottom
            scrollZone.Add(new MenuIcon("bottom_left", null, Color.NavajoWhite));
            
            scrollZone[5].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));

            //Like the second but at the bottom
            scrollZone.Add(new MenuIcon("bottom", null, Color.NavajoWhite));
            
            scrollZone[6].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE, windowWidth - Globals.TILE_SIZE * 2, Globals.TILE_SIZE));

            //Like the third but at the bottom
            scrollZone.Add(new MenuIcon("bottom_right", null, Color.NavajoWhite));
            
            scrollZone[7].region = new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(windowWidth - Globals.TILE_SIZE, windowHeight - Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE));
            #endregion
        }
    }
}
