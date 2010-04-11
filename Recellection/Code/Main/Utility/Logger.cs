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
	 * TODO: This class is a little long. Extract creation to a factory-class?
	 * 
	 * @author Martin Nycander
	 */
	public class Logger
	{
		private static LinkedList<Logger> loggers = new LinkedList<Logger>();
		protected static LogLevel globalThreshold = LogLevel.TRACE;
		protected static TextWriter globalTarget = System.Console.Out;

		/**
		 * Retrieves a logger with the provided name.
		 * Loggers are re-used and identified by name.
		 * 
		 * @return an new instance of a Logger
		 */
		public static Logger getLogger(string name)
		{
			// Try re-using a logger with that name
			// TODO: Use Dictionary for loggers?
			foreach (Logger l in loggers)
			{
				if (l.GetName() == name)
				{
					return l;
				}
			}

			Logger newLogger = new Logger(name);
			loggers.AddLast(newLogger);
			return newLogger;
		}

		/**
		 * Initializes a logger with the current class as name.
		 * It searches the stackframe for this name, use getLogger(string) for better performance.
		 * 
		 * @return an new instance of a Logger
		 */
		public static Logger getLogger()
		{
			// Get the caller of this method
			StackFrame stackFrame = new StackTrace().GetFrame(1);

			// Use the name of that class as this loggers name
			string className = stackFrame.GetMethod().ReflectedType.FullName;

			return getLogger(className);
		}

		/**
		 * Will change target of all current and new loggers.
		 * 
		 * @param newTarget the new target for all loggers.
		 */
		public static void setGlobalTarget(TextWriter newTarget)
		{
			Logger.globalTarget = newTarget;

			foreach (Logger l in loggers)
			{
				l.SetTarget(newTarget);
			}
		}
		
		private string name;
		private LogLevel threshold;
		private TextWriter target;

		/**
		 * Private constructor, use getLogger to get an instance.
		 */
		private Logger(string name)
		{
			this.name = name;
			this.threshold = LogLevel.TRACE;
			this.target = Logger.globalTarget;
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
			
			if (level < Logger.globalThreshold)
				return;
			
			string time = DateTime.Now.ToString("HH:mm:ss");
			
			target.WriteLine(time+" "+name+": "+message);
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
