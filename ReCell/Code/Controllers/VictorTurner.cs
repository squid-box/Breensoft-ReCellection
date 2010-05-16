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

        private WorldController humanControl;
        private GraphController graphControl;
		
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
            this.humanControl = new WorldController(players[0],world);
            this.graphControl = GraphController.Instance;
        }

        public void Run()
        {

            while (!finished)
            {	
				logger.Debug("Victor turner is turning the page!");
                foreach (Player player in players)
                {
                    updateFogOfWar(player);

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
						logger.Debug(player.color + " is a AIPlayer!");
						//((AIPlayer)player).MakeMove();
					}
					else if (player is Player)
					{
						logger.Debug(player.color+" is human!");
						//This only makes the grid of GUIRegions and scroll zones, remove later.
                        humanControl.Run();
					}
					else
					{
						logger.Fatal("Could not identify "+player.color+" player!");
					}
                }

				logger.Info("Weighting graphs!");
				graphControl.CalculateWeights();

                foreach (Player player in players)
                {
                    gameInitializer.suitGuys[player].ProduceUnits();
                }

                // This is where we start "animating" all movement
                // FIXME: This ain't okay, hombrey
                // Let the units move!
                logger.Info("Moving units!");
                
                for(int i = 0; i < 100; i++)
                {
					Code.Models.World.Map theWholeFuckingWorld = world.GetMap();
					for (int x = 0; x < theWholeFuckingWorld.width; x++)
					{
						for (int y = 0; y < theWholeFuckingWorld.height; y++)
						{
							UnitController.Update(theWholeFuckingWorld.GetTile(x, y).GetUnits(), 1);
						}
					}
					System.Threading.Thread.Sleep(10);
				}

                foreach( Player p in players)
                {
                    BuildingController.AggressiveBuildingAct(p);
                }
            }

        }

        private void updateFogOfWar(Player player)
        {
            lock (world)
            {
                foreach (Tile t in world.GetMap().map)
                {
                    if (t.GetBuilding() != null)
                    {
                        for (int x = -2; x <= 2; x++)
                        {
                            for (int y = -2; y <= 2; y++)
                            {
                                if (world.isWithinMap(x + (int)t.position.X, y + (int)t.position.Y))
                                {
                                    //Get the tile and check if it is visible already else set it to visible.
                                    if (!world.GetMap().GetTile(x + (int)t.position.X, y + (int)t.position.Y).IsVisible(t.GetBuilding().owner))
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
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
