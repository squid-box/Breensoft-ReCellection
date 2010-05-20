using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Recellection.Code.Utility.Console;

namespace Recellection.Code.Utility.Logger
{
	/// <summary>
	/// Factory class, provides methods for supplying Loggers to the people.
	/// Author: Martin Nycander
    /// Signature: John Forsberg (2010-05-07)
	/// </summary>
	public class LoggerFactory
	{
		//private static LinkedList<Logger> loggers = new LinkedList<Logger>();
		private static Dictionary<String, Logger> loggers = new Dictionary<string,Logger>();
		
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
			Logger l;
			if(loggers.TryGetValue(name, out l))
			{
				return l;
			}

			l = new Logger(name, LogLevel.TRACE, globalTarget);
			loggers.Add(name, l);
			return l;
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

			foreach (KeyValuePair<String, Logger> l in loggers)
			{
				l.Value.SetTarget(newTarget);
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

		public static bool HasLogger(string name)
		{
			//System.Console.WriteLine()
			return loggers.ContainsKey(name);
		}

		public static string ListLoggers()
		{
			string s = "";
			foreach (string z in loggers.Keys)
			{
				Logger l = loggers[z];

				s += z + " ";

				if (l.Active)
					s += "[Active]\n";
				else
					s += "[Disabled]\n";
			}
			return s;
		}


		internal static void SetAll(bool active)
		{
			foreach (Logger l in loggers.Values)
			{
				l.Active = active;
			}
		}

		internal static void ToggleAll()
		{
			foreach (Logger l in loggers.Values)
			{
				l.Active = !l.Active;
			}
		}
	}
}
