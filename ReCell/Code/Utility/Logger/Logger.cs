using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Recellection.Code.Utility.Logger
{
	/// <summary>
	/// Provides a re-usable logging interface for the whole application.
	/// 
	/// Author: Martin Nycander
    /// Signature: John Forsberg (2010-05-07)
	/// </summary>
	public class Logger
	{	
		private string name;
		private LogLevel threshold;
		private TextWriter target;
		
		/// <summary>
		/// Internal constructor, use GetLogger to get an instance.
		/// </summary>
		/// <param name="name">The name of the logger.</param>
		/// <param name="threshold">The threshold for the logger.</param>
		/// <param name="baseEntity">The baseEntity to write to.</param>
		internal Logger(string name, LogLevel threshold, TextWriter target)
		{
			this.name = name;
			this.threshold = threshold;
			this.target = target;
		}
		
		/// <returns>The name of the logger.</returns>
		public string GetName()
		{
			return name;
		}
		
		/// <param name="threshold">The new logging threshold for the logger.</param>
		public void SetThreshold(LogLevel threshold)
		{
			this.threshold = threshold;
		}
		
		/// <returns>The current threshold for this logger.</returns>
		public LogLevel GetThreshold()
		{
			return threshold;
		}
		
		/// <param name="newTarget">The new output baseEntity for this logger.</param>
		public void SetTarget(TextWriter newTarget)
		{
			this.target = newTarget;
		}

		/// <summary>
		/// Logs a message to the baseEntity if it's above the threshold.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">The level of importance.</param>
		private void Log(string message, LogLevel level)
		{
#if DEBUG
			if (level < this.threshold)
				return;
			
			if (level < LoggerFactory.globalThreshold)
				return;
			
			string time = DateTime.Now.ToString("HH:mm:ss");
			
			target.WriteLine(time+" "+name+"["+level+"]: "+message);
#endif
		}
		
		#region Logging methods

		/// <summary>
		/// Submits a trace log message to the logger.
		/// A trace message is a very detailed log messages, potentially of a high frequency and volume.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Trace(string message)
		{
			Log(message, LogLevel.TRACE);
		}

		/// <summary>
		/// Submits a debug log message to the logger.
		/// A debug message is a less detailed and/or less frequent debugging messages
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Debug(string message)
		{
			Log(message, LogLevel.DEBUG);
		}

		/// <summary>
		/// Submits an info log message to the logger.
		/// An info message is an informal message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Info(string message)
		{
			Log(message, LogLevel.INFO);
		}

		/// <summary>
		/// Submits a warning log message to the logger.
		/// A warning message is for warnings which doesn't appear to the user of the application.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Warn(string message)
		{
			Log(message, LogLevel.WARN);
		}


		/// <summary>
		/// Submits an error log message to the logger.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Error(string message)
		{
			Log(message, LogLevel.ERROR);
		}

		/// <summary>
		/// Submits a fatal log message to the logger.
		/// After a fatal error the application usually terminates.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Fatal(string message)
		{
			Log(message, LogLevel.FATAL);
		}
		#endregion
	}
}
