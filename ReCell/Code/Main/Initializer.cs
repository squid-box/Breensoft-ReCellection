﻿using System;
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
			
			#region Build main menu
			MenuIcon newgame = new MenuIcon(Language.Instance.GetString("NewGame"), null, Color.Black);
			MenuIcon options = new MenuIcon(Language.Instance.GetString("Options"), null, Color.Black);
			MenuIcon help = new MenuIcon(Language.Instance.GetString("Help"), null, Color.Black);
			MenuIcon quit = new MenuIcon(Language.Instance.GetString("Quit"), null, Color.Black);

			List<MenuIcon> menuOptions = new List<MenuIcon>();
			menuOptions.Add(newgame);
			menuOptions.Add(options);
			menuOptions.Add(help);
			menuOptions.Add(quit);

            Menu mainMenu = new Menu(Globals.MenuLayout.FourMatrix, menuOptions, "", Color.Black);

			MenuView view = MenuView.Instance;

			// Just to make sure everything is in there...
			new MenuController(TobiiController.GetInstance(this.windowHandle), mainMenu);
			#endregion

            ShowSplashScreen();
            
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
					
					GameInitializer gameInit = new GameInitializer();
					backgroundSound.Pause();
					WorldView.Initiate(gameInit.theWorld);
					Recellection.CurrentState = WorldView.Instance;// new WorldView(gameInit.theWorld);
					VictorTurner vt = new VictorTurner(gameInit);
					vt.Run();
				}
				else if (response == quit)
				{
					List<MenuIcon> promptOptions = new List<MenuIcon>(2);
					MenuIcon yes = new MenuIcon(Language.Instance.GetString("Yes"), Recellection.textureMap.GetTexture(Globals.TextureTypes.Yes));
					MenuIcon no = new MenuIcon(Language.Instance.GetString("No"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
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
                    List<MenuIcon> opt = new List<MenuIcon>(1);
                    MenuIcon cancel = new MenuIcon("");
                    cancel.region = new GUIRegion(Recellection.windowHandle,
                        new System.Windows.Rect(0, Globals.VIEWPORT_HEIGHT - 100, Globals.VIEWPORT_WIDTH, 100));
                    opt.Add(cancel);
                    Menu menu = new Menu(opt);
                    MenuController.LoadMenu(menu);

                    Recellection.CurrentState = new HelpView();

                    MenuController.GetInput();

                    MenuController.UnloadMenu();
                }
                else
                {
                    Recellection.playBeethoven();
                }
			}
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
