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
		
		public Initializer(GraphicsRenderer gfx)
		{
			logger.Debug("Initializer was instantiated.");
			graphicRendering = gfx;
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
			
			MenuIcon input = MenuController.GetInput();
			
			// TODO: Tell the graphic renderer what is the current view
			// TODO: Spawn main menu, tell it to run.
		}
	}
}
