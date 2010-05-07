using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

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
    }
}
