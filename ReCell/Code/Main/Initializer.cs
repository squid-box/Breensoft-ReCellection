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

            ShowSplashScreen();

			Cue backgroundSound = Sounds.Instance.LoadSound("Menu");
			
			#region Show main menu. TODO: Make a real menu.
			MenuIcon yes = new MenuIcon("Yes", null,Color.Black);
			MenuIcon no = new MenuIcon("No", null,Color.Black);
			
			List<MenuIcon> options = new List<MenuIcon>();
			options.Add(yes);
			options.Add(no);
			
			Menu mainMenu = new Menu(Globals.MenuLayout.Prompt, options, "Do you wanna play a\ngame?",Color.Black);

			backgroundSound.Play();
			MenuView view = MenuView.Instance;
			
			// Just to make sure everything is in there...
			new MenuController(TobiiController.GetInstance(this.windowHandle), mainMenu);

			Recellection.CurrentState = view;
			#endregion
            logger.Info("Waiting for Tobii input...");
			MenuIcon response = MenuController.GetInput();
            MenuController.UnloadMenu();
            
            logger.Info("Got input!");
            backgroundSound.Pause();
            if (response.label == yes.label)
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
            else
			{
				Recellection.playBeethoven();
				Environment.Exit(0);
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
