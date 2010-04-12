using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Recellection.Code.Main.Utility
{
	public static class LoggerSetup
	{
		/**
		 * Initializes the testing environment for this specific application.
		 * Individual settings can be adjusted here.
		 */
		public static void Initialize()
		{
			LoggerFactory.setGlobalTarget(GetLogFileTarget("recellection.log"));
			LoggerFactory.setGlobalThreshold(LogLevel.TRACE);
			
			Logger l = LoggerFactory.GetLogger();
			l.Trace("Initialized Logger.");
		}
		
		/**
		 * Creates and opens a new file for writing and returns a TextWriter for the file.
		 * @return a textWriter to be used for a logger.
		 */
		public static TextWriter GetLogFileTarget(string filename)
		{
			FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(file);
			sw.AutoFlush = true;
			return sw;
		}
	}
}
