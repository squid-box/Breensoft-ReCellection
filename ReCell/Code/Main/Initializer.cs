namespace Recellection.Code.Main
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;

    using global::Recellection.Code.Controllers;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    using global::Recellection.Code.Views;

    public class Initializer
	{
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        #endregion

        #region Fields

        private readonly IntPtr windowHandle;

        #endregion

        #region Constructors and Destructors

        public Initializer(IntPtr windowHandle)
		{
			logger.Debug("Initializer was instantiated.");
            this.windowHandle = windowHandle;
		}

        #endregion

        #region Public Methods and Operators

        public void Run()
		{
			logger.Debug("Initializer is running.");
			
			
			var newgame = new MenuIcon(Language.Instance.GetString("NewGame"), null, Color.Black);
			var options = new MenuIcon(Language.Instance.GetString("Options"), null, Color.Black);
			var help = new MenuIcon(Language.Instance.GetString("Help"), null, Color.Black);
			var quit = new MenuIcon(Language.Instance.GetString("Quit"), null, Color.Black);

			var menuOptions = new List<MenuIcon>();
			menuOptions.Add(newgame);
			menuOptions.Add(options);
			menuOptions.Add(help);
			menuOptions.Add(quit);

            var mainMenu = new Menu(Globals.MenuLayout.FourMatrix, menuOptions, string.Empty, Color.Black);

            MenuView view = MenuView.Instance;

            // Just to make sure everything is in there...
			new MenuController(TobiiController.GetInstance(), mainMenu);
			

            this.ShowSplashScreen();
            
			Cue backgroundSound = Sounds.Instance.LoadSound("Menu");
			backgroundSound.Play();
			
            while(true)
			{
				backgroundSound.Resume();
				Recellection.CurrentState = view;
				logger.Info("Waiting for Tobii input...");
				MenuIcon response = MenuController.GetInput();
	            
				logger.Info("Got input!");

				if (response == newgame)
				{
				    // START THE GAME ALREADY!
				    var gameInit = new GameInitializer();
				    backgroundSound.Pause();
				    WorldView.Initiate(gameInit.theWorld);
				    Recellection.CurrentState = WorldView.Instance; // new WorldView(gameInit.theWorld);
				    var vt = new VictorTurner(gameInit);
				    vt.Run();
				}
				else if (response == quit)
				{
					var promptOptions = new List<MenuIcon>(2);
					var yes = new MenuIcon(Language.Instance.GetString("Yes"), Recellection.textureMap.GetTexture(Globals.TextureTypes.Yes));
					var no = new MenuIcon(Language.Instance.GetString("No"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
					promptOptions.Add(yes);
					promptOptions.Add(no);
					MenuController.LoadMenu(new Menu(Globals.MenuLayout.Prompt, promptOptions, Language.Instance.GetString("AreYouSureYouWantToQuit")));
					Recellection.CurrentState = MenuView.Instance;
					
					if (MenuController.GetInput() == yes)
					{
						Environment.Exit(0);
					}
					
					MenuController.UnloadMenu();
				}
				else if (response == options)
				{
					Configurator.Instance.ChangeOptions();
				}
                else if (response == help)
                {
                    var opt = new List<MenuIcon>(1);
                    var cancel = new MenuIcon(string.Empty);
                    cancel.region = new GUIRegion(Recellection.windowHandle, 
                        new System.Windows.Rect(0, Globals.VIEWPORT_HEIGHT - 100, Globals.VIEWPORT_WIDTH, 100));
                    opt.Add(cancel);
                    var menu = new Menu(opt);
                    MenuController.LoadMenu(menu);

                    Recellection.CurrentState = new HelpView();

                    MenuController.GetInput();

                    MenuController.UnloadMenu();
                }
                else
                {
                    Recellection.PlayBeethoven();
                }
			}
		}

        #endregion

        #region Methods

        /// <summary>
        /// Displays the breensoft logo for a given amount of time
        /// and plays the logoIntro sound.
        /// </summary>
        /// <param name="time"></param>
        private void ShowSplashScreen()
        {
            var splash = new SplashView();
            
            Recellection.CurrentState = splash;
            
			Cue intro = Sounds.Instance.LoadSound("logoIntro");
			intro.Play();
			while (intro.IsPlaying)
			{
				Thread.Sleep(10);
			}
        }

        #endregion
	}
}
