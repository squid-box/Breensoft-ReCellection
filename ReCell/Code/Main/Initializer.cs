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

		private GraphicsRenderer graphicRendering;
        private IntPtr windowHandle;
		
		public Initializer(GraphicsRenderer gfx, IntPtr windowHandle)
		{
			logger.Debug("Initializer was instantiated.");
			this.graphicRendering = gfx;
            this.windowHandle = windowHandle;
		}
		
		public void Run()
		{
			logger.Debug("Initializer is running.");
			
			// TODO: Sound logo!

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

			GraphicsRenderer.currentState = view;
			
            logger.Info("Waiting for Tobii input...");
            MenuIcon response = MenuController.GetInput();
            

            logger.Info("Got input!");
            backgroundSound.Stop(AudioStopOptions.Immediate);
            if (response.getLabel() == yes.getLabel())
            {
				Cue prego = Sounds.Instance.LoadSound("prego");
				prego.Play();
				while(prego.IsPlaying)
				{
					Thread.Sleep(10);
				}
                GraphicsRenderer.currentState = new TestView();
            }
            else
            {
				Console.Beep(440, 1000);
				Console.Beep(37, 500);
				Console.Beep(440, 1000);
				Console.Beep(37, 500);
				Console.Beep(440, 1000);
            }

           // Environment.Exit(0);
			
			// TODO: Tell the graphic renderer what is the current view
			// TODO: Spawn main menu, tell it to run.
		}
	}
}
