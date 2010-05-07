using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Recellection.Code.Utility.Logger
{
    /// <summary>
    /// 
    /// 
    /// Signature: John Forsberg (2010-05-07)
    /// </summary>
	public static class LoggerSetup
	{
		public static TextWriter target = GetLogFileTarget("recellection.log");
		public static LogLevel threshold = LogLevel.TRACE;
		
		/// <summary>
		/// Initializes the testing environment for this specific application.
		/// Individual settings can be adjusted here.
		/// </summary>
		public static void Initialize()
		{
			LoggerFactory.SetGlobalTarget(target);
			LoggerFactory.SetGlobalThreshold(threshold);
			
			Logger l = LoggerFactory.GetLogger();
			l.Trace("Initialized Logger.");
		}
		
		/// <summary>
		/// Creates and opens a new file for writing and returns a TextWriter for the file.
		/// </summary>
		/// <param name="filename">The path to the logfile.</param>
		/// <returns>A textWriter to be used for a logger.</returns>
		public static TextWriter GetLogFileTarget(string filename)
		{
			FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(file);
			sw.AutoFlush = true;
			return sw;
		}
	}
}
