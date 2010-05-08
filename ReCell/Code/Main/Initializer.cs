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
						
			MenuIcon yes = new MenuIcon("Yes", null);
			MenuIcon no = new MenuIcon("No", null);
			
			List<MenuIcon> options = new List<MenuIcon>();
			options.Add(yes);
			options.Add(no);
			
			Menu mainMenu = new Menu(Globals.MenuLayout.Prompt, options, "Do you wanna play a game?");

			backgroundSound.Play();
			MenuView view = MenuView.Instance;
			
			// Just to make sure everything is in there...
			new MenuController(TobiiController.GetInstance(this.windowHandle), mainMenu);

			Recellection.CurrentState = view;
			
            logger.Info("Waiting for Tobii input...");
			MenuIcon response = MenuController.GetInput();
            
            logger.Info("Got input!");
            backgroundSound.Pause();
            if (response.getLabel() == yes.getLabel())
            {
				Cue prego = Sounds.Instance.LoadSound("prego");
				prego.Play();
				while(prego.IsPlaying)
				{
					Thread.Sleep(10);
				}
				
				// START THE GAME ALREADY!
				World w = WorldGenerator.GenerateWorld(1);
				Recellection.CurrentState = new WorldView(w, new Player(PlayerColour.BLUE, "Tester"));
				// Call blocking state? :S
				while(true)
				{
					Thread.Sleep(1000);
					Console.Beep(340, 10);
				}
            }
            else
			{
				Console.Beep(440, 1000);
				Thread.Sleep(100);
				Console.Beep(540, 1000);
				Thread.Sleep(100);
				Console.Beep(640, 1000);
				Environment.Exit(0);
            }

           // Environment.Exit(0);
			
			// TODO: Tell the graphic renderer what is the current view
			// TODO: Spawn main menu, tell it to run.
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
