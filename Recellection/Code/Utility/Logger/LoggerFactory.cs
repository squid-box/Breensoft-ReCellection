using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Recellection.Code.Utility.Logger
{
	/// <summary>
	/// Factory class, provides methods for supplying Loggers to the people.
	/// Author: Martin Nycander
	/// </summary>
	public class LoggerFactory
	{
		private static LinkedList<Logger> loggers = new LinkedList<Logger>();
		private static TextWriter globalTarget = System.Console.Out;
		internal static LogLevel globalThreshold = LogLevel.TRACE;

		/// <summary>
		/// Retrieves a logger with the provided name.
		/// Loggers are re-used and identified by name.
		/// </summary>
		/// <param name="name">The name of the logger.</param>
		/// <returns>A new instance of a Logger.</returns>
		public static Logger GetLogger(string name)
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

			Logger newLogger = new Logger(name, LogLevel.TRACE, globalTarget);
			loggers.AddLast(newLogger);
			return newLogger;
		}

		/// <summary>
		/// Initializes a logger with the current class as name.
		/// It searches the stackframe for this name, use GetLogger(string) for better performance.
		/// </summary>
		/// <returns>A new instance of a Logger.</returns>
		public static Logger GetLogger()
		{
			// Get the caller of this method
			StackFrame stackFrame = new StackTrace().GetFrame(1);

			// Use the name of that class as this loggers name
			string className = stackFrame.GetMethod().ReflectedType.FullName;

			return GetLogger(className);
		}

		/// <summary>
		/// Will change target of all current and new loggers.
		/// </summary>
		/// <param name="newTarget">The new target for all loggers.</param>
		public static void SetGlobalTarget(TextWriter newTarget)
		{
			LoggerFactory.globalTarget = newTarget;

			foreach (Logger l in loggers)
			{
				l.SetTarget(newTarget);
			}
		}

		/// <summary>
		/// Sets the global threshold. No logs will have a loglevel below this threshold.
		/// </summary>
		/// <param name="newThreshold">The new threshold for the application.</param>
		public static void SetGlobalThreshold(LogLevel newThreshold)
		{
			LoggerFactory.globalThreshold = newThreshold;
		}
		
	}
}
