using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Controllers;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Main
{
	public class Initializer
	{
		private static Logger logger = LoggerFactory.GetLogger();

        private IntPtr windowHandle;
		
		public Initializer(IntPtr windowHandle)
		{
			logger.Debug("Initializer was instantiated.");
            this.windowHandle = windowHandle;
		}
		
		public void Run()
		{
			logger.Debug("Initializer is running.");

			#region Show main menu. TODO: Make a real menu.
			MenuIcon newgame = new MenuIcon("New game", null, Color.Black);
			MenuIcon options = new MenuIcon("Options", null, Color.Black);
			MenuIcon help = new MenuIcon("Help", null, Color.Black);
			MenuIcon quit = new MenuIcon("Quit", null, Color.Black);

			List<MenuIcon> menuOptions = new List<MenuIcon>();
			menuOptions.Add(newgame);
			menuOptions.Add(options);
			menuOptions.Add(help);
			menuOptions.Add(quit);

            Menu mainMenu = new Menu(Globals.MenuLayout.FourMatrix, menuOptions, "Would you like to\nplay a game?", Color.Black);

			MenuView view = MenuView.Instance;

			// Just to make sure everything is in there...
			new MenuController(TobiiController.GetInstance(this.windowHandle), mainMenu);
			#endregion

            ShowSplashScreen();

			Cue backgroundSound = Sounds.Instance.LoadSound("Menu");
			backgroundSound.Play();
			Recellection.CurrentState = view;
			
            Input:
            logger.Info("Waiting for Tobii input...");
			MenuIcon response = MenuController.GetInput();
            MenuController.UnloadMenu();
            
            logger.Info("Got input!");
            backgroundSound.Pause();

			if (response == newgame)
            {
				// Det börjar bli jobbigt...
				//Cue prego = Sounds.Instance.LoadSound("prego");
				// prego.Play();
				//while(prego.IsPlaying)
				//{
				//	Thread.Sleep(10);
				//}
				
				// START THE GAME ALREADY!
				GameInitializer gameInit = new GameInitializer();
				Recellection.CurrentState = new WorldView(gameInit.theWorld);
				VictorTurner vt = new VictorTurner(gameInit);
				vt.Run();
                
                // Heartbeat
                while (true)
                {
                    Thread.Sleep(100);
                    Console.Beep(66, 150);
                }
            }
            else if (response == quit)
            {
                Recellection.playBeethoven();
                Environment.Exit(0);
            }
            else if (response == options)
            {
                Configurator.Instance.ChangeOptions();
            }
            else
            {
                Recellection.playBeethoven();
                goto Input; // FUCK YES GOTO!
            }

           // Environment.Exit(0);
		}


        /// <summary>
        /// Displays the breensoft logo for a given amount of time
        /// and plays the logoIntro sound.
        /// </summary>
        /// <param name="time"></param>
        private void ShowSplashScreen()
        {
            SplashView splash = new SplashView();
            
            Recellection.CurrentState = splash;
            
			Cue intro = Sounds.Instance.LoadSound("logoIntro");
			intro.Play();
			while (intro.IsPlaying)
			{
				Thread.Sleep(10);
			}
        }

	}
}
