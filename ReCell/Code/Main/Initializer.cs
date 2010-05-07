using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Controllers;
using Recellection.Code.Models;

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
			
			MenuIcon yes = new MenuIcon("Yes", null);
			MenuIcon no = new MenuIcon("No", null);
			
			List<MenuIcon> options = new List<MenuIcon>();
			options.Add(yes);
			options.Add(no);
			
			Menu mainMenu = new Menu(Globals.MenuLayout.Prompt, options);
			
			//MenuIcon input = MenuController.GetInput();

            // Just to make sure everything is in there...
            new MenuController(TobiiController.GetInstance(this.windowHandle), mainMenu);

            logger.Info("Waiting for Tobii input...");
            MenuIcon response = MenuController.GetInput();

            logger.Info("Got input!");
            if (response.getLabel() == yes.getLabel())
            {
                Console.Beep(37, 1000);
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    Console.Beep(4711, 100);
                }
            }

            Environment.Exit(0);
			
			// TODO: Tell the graphic renderer what is the current view
			// TODO: Spawn main menu, tell it to run.
		}
	}
}
