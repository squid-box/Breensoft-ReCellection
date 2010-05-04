using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Logger;

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
			
			// TODO: Tell the graphic renderer what is the current view
			
			// TODO: Spawn main menu, tell it to run.
		}
	}
}
