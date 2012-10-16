namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Audio;

    using global::Recellection.Code.Main;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    using global::Recellection.Code.Views;

    /// <summary>
    /// 
    /// 
    /// 
    /// Author: John Forsberg
    /// </summary>
    class VictorTurner
    {
        #region Fields

        private readonly Cue backgroundSound = Sounds.Instance.LoadSound("inGameMusic");

        private readonly GraphController graphControl;

        private readonly WorldController humanControl;

        private readonly Logger logger = LoggerFactory.GetLogger();

        private readonly List<Player> players;

        private readonly World world;

        bool finished;

        private GameInitializer gameInitializer;

        #endregion

        #region Constructors and Destructors

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
            this.humanControl = new WorldController(this.players[0], this.world);
            this.graphControl = GraphController.Instance;
        }

        #endregion

        #region Public Methods and Operators

        public void Run()
		{
			this.backgroundSound.Play();
			
            while (!this.finished)
            {
                
				this.logger.Debug("Victor turner is turning the page!");
                foreach (Player player in this.players)
                {
					if (player is AIPlayer)
					{
						this.logger.Debug(player.Color + " is a AIPlayer!");
						((AIPlayer)player).MakeMove();
					}
					else if (player is Player)
					{
					    this.logger.Debug(player.Color + " is human!");

					    // This only makes the grid of GUIRegions and scroll zones, remove later.
					    this.humanControl.Run();
					}
					else
					{
						this.logger.Fatal("Could not identify "+player.Color+" player!");
					}

                    if (this.CheckIfLostOrWon(this.players))
                    {
                        this.finished = true;
                        this.EndGame(this.players[0]);
                        break;
                    }
                }

                if (this.finished)
                {
                    break;
                }

				this.logger.Info("Weighting graphs!");
                foreach (Player player in this.players)
                {
                    player.UnitAcc.ProduceUnits();
                }

				this.graphControl.CalculateWeights();

                // This is where we start "animating" all movement
                // FIXME: This ain't okay, hombrey
                // Let the units move!
                this.logger.Info("Moving units!");
				
				for(int u = 0; u < 5; u++)
				{
					foreach (Player p in this.players)
					{
						BuildingController.AggressiveBuildingAct(p);
					}
					
					for(int i = 0; i < 100; i++)
					{
						UnitController.Update(this.world.units, 1, this.world.GetMap());
						System.Threading.Thread.Sleep(10);
					}
				}

                
            }

        }

        #endregion

        #region Methods

        private bool CheckIfLostOrWon(List<Player> players)
        {
            var toBeRemoved = new List<Player>();
            foreach(Player p in players)
            {
                if (this.HasLost(p))
                {
                    toBeRemoved.Add(p);
                }
            }
			
            foreach(Player p in toBeRemoved)
            {
                this.world.players.Remove(p);
            }
			
            if (this.HasWon())
            {
                return true;
            }

            return false;
        }

        private void EndGame(Player winner)
        {
            if (this.backgroundSound.IsPlaying)
            {
                this.backgroundSound.Pause();
            }

            this.humanControl.Stop();

            // Build menu
            var options = new List<MenuIcon>(1);
            var cancel = new MenuIcon(string.Empty);
            cancel.region = new GUIRegion(
                Recellection.windowHandle, 
                new System.Windows.Rect(0, Globals.VIEWPORT_HEIGHT - 100, Globals.VIEWPORT_WIDTH, 100));
            options.Add(cancel);
            var menu = new Menu(options);
            MenuController.LoadMenu(menu);

            Recellection.CurrentState = new EndGameView(! (winner is AIPlayer));

            MenuController.GetInput();

            MenuController.UnloadMenu();
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
        private bool HasLost(Player player)
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
        private bool HasWon()
        {
            return this.world.players.Count == 1;
        }

        private void updateFogOfWar(Player player)
        {
            lock (this.world)
            {
                foreach (Tile t in this.world.GetMap().map)
                {
                    if (t.GetBuilding() == null)
                        continue;
						
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            if (! this.world.isWithinMap(x + (int)t.position.X, y + (int)t.position.Y))
                            {
                                continue;
                            }

                            // Get the tile and check if it is visible already else set it to visible.
                            if (
                                this.world.GetMap().GetTile(x + (int)t.position.X, y + (int)t.position.Y).IsVisible(
                                    t.GetBuilding().owner))
                            {
                                continue;
                            }

                            // TODO: Do stuff.
                        }
                    }
                }
            }
        }

        #endregion
    }
}
