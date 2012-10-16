namespace Recellection.Code.Utility.Logger
{
    using System;
    using System.IO;

    /// <summary>
	/// Provides a re-usable logging interface for the whole application.
	/// 
	/// Author: Martin Nycander
    /// Signature: John Forsberg (2010-05-07)
	/// </summary>
	public class Logger
	{
        #region Fields

        private readonly string name;

        private TextWriter target;

        private LogLevel threshold;

        #endregion

        #region Constructors and Destructors

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
			this.Active = false;
		}

        #endregion

        #region Public Properties

        public bool Active {get; set;}

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Submits a debug log message to the logger.
        /// A debug message is a less detailed and/or less frequent debugging messages
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message)
        {
            this.Log(message, LogLevel.DEBUG);
        }

        /// <summary>
        /// Submits an error log message to the logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message)
        {
            this.Log(message, LogLevel.ERROR);
        }

        /// <summary>
        /// Submits a fatal log message to the logger.
        /// After a fatal error the application usually terminates.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Fatal(string message)
        {
            this.Log(message, LogLevel.FATAL);
        }

        /// <returns>The name of the logger.</returns>
		public string GetName()
		{
			return this.name;
		}

        /// <returns>The current threshold for this logger.</returns>
		public LogLevel GetThreshold()
		{
			return this.threshold;
		}

        /// <summary>
        /// Submits an info log message to the logger.
        /// An info message is an informal message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message)
        {
            this.Log(message, LogLevel.INFO);
        }

        /// <param name="newTarget">The new output baseEntity for this logger.</param>
		public void SetTarget(TextWriter newTarget)
		{
			this.target = newTarget;
		}

        /// <param name="threshold">The new logging threshold for the logger.</param>
        public void SetThreshold(LogLevel threshold)
        {
            this.threshold = threshold;
        }

        /// <summary>
		/// Submits a trace log message to the logger.
		/// A trace message is a very detailed log messages, potentially of a high frequency and volume.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Trace(string message)
		{
			this.Log(message, LogLevel.TRACE);
		}

        /// <summary>
		/// Submits a warning log message to the logger.
		/// A warning message is for warnings which doesn't appear to the user of the application.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Warn(string message)
		{
			this.Log(message, LogLevel.WARN);
		}

        #endregion

        #region Methods

        /// <summary>
        /// Logs a message to the baseEntity if it's above the threshold.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of importance.</param>
        private void Log(string message, LogLevel level)
        {
#if DEBUG
            if(!this.Active)
                return;

            if (level < this.threshold)
                return;
			
            if (level < LoggerFactory.globalThreshold)
                return;
			
            string time = DateTime.Now.ToString("HH:mm:ss");
			
            this.target.WriteLine(time+" "+this.name+"["+level+"]: "+message);
#endif
        }

        #endregion
	}
}
