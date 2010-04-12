using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Recellection.Code.Main.Utility
{
	/**
	 * Factory class, provides methods for supplying Loggers to the people.
	 * @author Martin Nycander
	 */
	public class LoggerFactory
	{
		private static LinkedList<Logger> loggers = new LinkedList<Logger>();
		private static TextWriter globalTarget = System.Console.Out;

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

			Logger newLogger = new Logger(name, LogLevel.TRACE, globalTarget);
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
			LoggerFactory.globalTarget = newTarget;

			foreach (Logger l in loggers)
			{
				l.SetTarget(newTarget);
			}
		}
		
	}
}
