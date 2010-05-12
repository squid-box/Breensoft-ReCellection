using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Models;
using Recellection.Code.Controllers;
using Recellection.Code.Main;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Author: John Forsberg
    /// </summary>
    class VictorTurner
    {
        private List<Player> players;

        private World world;

        private GameInitializer gameInitializer;
		private Logger logger = LoggerFactory.GetLogger();
		
        Boolean finished = false;
        /// <summary>
        /// The constructor used to initiate the Victor Turner
        /// </summary>
        /// <param name="players">The players in the game</param>
        /// <param name="world">The world the game takes place in</param>
        public VictorTurner(GameInitializer gameInitializer)
        {
            this.gameInitializer = gameInitializer;
            this.players = gameInitializer.theWorld.players;
            this.world = gameInitializer.theWorld;
        }

        public void Run()
        {

            while (!finished)
            {	
				logger.Debug("Victor turner is turning the page!");
                foreach (Player player in players)
                {
                    gameInitializer.suitGuys[player].ProduceUnits(); 

                    if(HasLost(player))
                    {
                        world.RemovePlayer(player);
                    }
                    if(HasWon())
                    {

                        finished = true;
                        break;  
                    }
                    
                    
					if (player is AIPlayer)
					{
						logger.Debug(player.colour + " is a AIPlayer!");
						//((AIPlayer)player).MakeMove();
					}
					else if (player is Player)
					{
						logger.Debug(player.colour+" is human!");
						//This only makes the grid of GUIRegions and scroll zones, remove later.
						new WorldController(player);
					}
					else
					{
						logger.Fatal("Could not identify "+player.colour+" player!");
					}
					
                }
                //UnitController.Update(world.GetMap().);

            }

        }

        /// <summary>
        /// Counts how many graphs a player has, if it is zero
        /// the player has lost. 
        /// 
        /// Condition: when a graph is empty it is
        /// deleted or a empty graph is not counted.
        /// </summary>
        /// <param name="player">The player which might have lost</param>
        /// <returns>True if the player has no graphs false other vice</returns>
        private Boolean HasLost(Player player)
        {
            if (player.GetGraphs().Count == 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method returns true if there is only one player left playing.
        /// 
        /// </summary>
        /// <returns>Returns true if the length of currently active players 
        /// in the world is zero false other vice.</returns>
        private Boolean HasWon()
        {
            if (world.players.Count == 1)
            {
                return true;
            }

            return false;
        }
    }
}
