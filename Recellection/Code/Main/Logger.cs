using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Recellection.Code.Main
{
	public class Logger
	{
		private enum LogLevel { TRACE = 1, DEBUG = 2, INFO = 3, WARN = 4, ERROR = 5, FATAL = 6 };
		
		private static LinkedList<Logger> loggers = new LinkedList<Logger>();
		private static LogLevel globalThreshold = LogLevel.TRACE;

		public static Logger getLogger(string name)
		{
			Logger l = new Logger(name);

			loggers.AddLast(l);

			return l;
		}
		
		public static Logger getLogger()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);
			string className = stackFrame.GetMethod().ReflectedType.FullName;
			return getLogger(className);
		}
		
		private string name;
		private LogLevel threshold;

		private Logger(string name)
		{
			this.name = name;
			this.threshold = LogLevel.TRACE;
		}
		
		public string GetName()
		{
			return name;
		}
		
		private void Log(string message, LogLevel level)
		{
			if (level < this.threshold)
				return;
			
			if (level < Logger.globalThreshold)
				return;
			
			System.Console.WriteLine(message);
		}
		
		public void Trace(string message)
		{
			Log(message, LogLevel.TRACE);
		}
		
		public void Debug(string message)
		{
			Log(message, LogLevel.DEBUG);
		}
		
		public void Info(string message)
		{
			Log(message, LogLevel.INFO);
		}
		
		public void Warn(string message)
		{
			Log(message, LogLevel.WARN);
		}
		
		public void Error(string message)
		{
			Log(message, LogLevel.ERROR);
		}
		
		public void Fatal(string message)
		{
			Log(message, LogLevel.FATAL);
		}
	}
}
