using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Recellection.Code.Main.Utility
{
	/**
	 * Provides a re-usable logging interface for the whole application.
	 * 
	 * @author Martin Nycander
	 */
	public class Logger
	{	
		private string name;
		private LogLevel threshold;
		private TextWriter target;
		
		/**
		 * Internal constructor, use getLogger to get an instance.
		 */
		internal Logger(string name, LogLevel threshold, TextWriter target)
		{
			this.name = name;
			this.threshold = threshold;
			this.target = target;
		}
				
		/**
		 * @return the name of this logger.
		 */
		public string GetName()
		{
			return name;
		}
		
		/**
		 * @param threshold the logging threshold for this logger.
		 */
		public void SetThreshold(LogLevel threshold)
		{
			this.threshold = threshold;
		}
		
		/**
		 * @return the current threshold for this logger.
		 */
		public LogLevel GetThreshold()
		{
			return threshold;
		}
		
		/**
		 * @param newTarget the new output for this logger.
		 */
		public void SetTarget(TextWriter newTarget)
		{
			this.target = newTarget;
		}
		
		/**
		 * Logs a message to the target if it's above the threshold.
		 * 
		 * @param message the message to log
		 * @param level the level of importance
		 */
		private void Log(string message, LogLevel level)
		{
			if (level < this.threshold)
				return;
			
			if (level < LoggerFactory.globalThreshold)
				return;
			
			string time = DateTime.Now.ToString("HH:mm:ss");
			
			target.WriteLine(time+" "+name+"["+level+"]: "+message);
		}
		
		#region Logging methods

		/**
		 * Submits a trace log message to the logger.
		 * A trace message is a very detailed log messages, potentially of a high frequency and volume.
		 * 
		 * @param message the message to log.
		 */
		public void Trace(string message)
		{
			Log(message, LogLevel.TRACE);
		}

		/**
		 * Submits a debug log message to the logger.
		 * A debug message is a less detailed and/or less frequent debugging messages
		 * 
		 * @param message the message to log.
		 */
		public void Debug(string message)
		{
			Log(message, LogLevel.DEBUG);
		}

		/**
		 * Submits an info log message to the logger.
		 * An info message is an informal message.
		 * 
		 * @param message the message to log.
		 */
		public void Info(string message)
		{
			Log(message, LogLevel.INFO);
		}

		/**
		 * Submits a warning log message to the logger.
		 * A warning message is for warnings which doesn't appear to the user of the application.
		 * 
		 * @param message the message to log.
		 */
		public void Warn(string message)
		{
			Log(message, LogLevel.WARN);
		}

		/**
		 * Submits an error log message to the logger.
		 *
		 * @param message the message to log.
		 */
		public void Error(string message)
		{
			Log(message, LogLevel.ERROR);
		}

		/**
		 * Submits a fatal log message to the logger.
		 * After a fatal error the application usually terminates.
		 * 
		 * @param message the message to log.
		 */
		public void Fatal(string message)
		{
			Log(message, LogLevel.FATAL);
		}
		#endregion
	}
}
