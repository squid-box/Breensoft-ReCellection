using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Recellection.Code.Main.Utility
{
	[TestFixture]
	class LoggerTest
	{
		private Logger l;
		private StringWriter target;
		private StringBuilder output;
				
		[SetUp]
		public void init()
		{
			target = new StringWriter();
			output = target.GetStringBuilder();
			
			l = Logger.getLogger("Test");
			l.SetThreshold(LogLevel.TRACE);
			l.SetTarget(target);
		}
		
		[Test]
		public void GetLoggerName()
		{
			l = Logger.getLogger("Test");
			Assert.AreEqual("Test", l.GetName());
		}
		
		[Test]
		public void GetLoggerAuto()
		{
			l = Logger.getLogger();
			Assert.AreEqual("Recellection.Code.Main.Utility.LoggerTest", l.GetName());
		}
		
		[Test]
		public void LoggersAreReused()
		{
			l.SetThreshold(LogLevel.FATAL);
			
			Logger l2 = Logger.getLogger(l.GetName());
			
			Assert.AreEqual(LogLevel.FATAL, l2.GetThreshold());
		}
		
		[Test]
		public void TestLoggingWithTrace()
		{
			l.Trace("abc");
			Assert.True(output.ToString().Contains("abc"), "Test string does not occur in log.");
			l.Trace("def");
			Assert.True(output.ToString().Contains("abc"), "Test string does not occur in log.");
			Assert.True(output.ToString().Contains("def"), "Test string does not occur in log.");
		}
		
		[Test]
		public void SetThreshold()
		{
			l.SetThreshold(LogLevel.DEBUG);
			
			l.Trace("abc");
			Assert.False(output.ToString().Contains("abc"));
			
			l.Debug("abc");
			Assert.True(output.ToString().Contains("abc"));

			l.SetThreshold(LogLevel.TRACE);

			l.Trace("def");
			Assert.True(output.ToString().Contains("abc"));
			Assert.True(output.ToString().Contains("def"));
		}
		
		[Test]
		public void SetTargetLocally()
		{
			l.Trace("abc");
		
			StringWriter target2 = new StringWriter();
			StringBuilder output2 = target2.GetStringBuilder();
			
			l.SetTarget(target2);

			l.Trace("def");

			Assert.True(output.ToString().Contains("abc"));
			Assert.False(output.ToString().Contains("def"));

			Assert.False(output2.ToString().Contains("abc"));
			Assert.True(output2.ToString().Contains("def"));
		}
		
		[Test]
		public void SetTargetGlobally()
		{
			// Setup second logger
			Logger l2 = Logger.getLogger("Test2");
			StringWriter target2 = new StringWriter();
			StringBuilder output2 = target2.GetStringBuilder();
			l2.SetTarget(target2);

			// Log some stuff to both loggers
			l.Trace("abc");
			l2.Trace("def");
			
			// Change target for all
			Logger.setGlobalTarget(target);

			l.Trace("123");
			l2.Trace("456");

			// The two loggers should then have gotten the following output
			Assert.True(output.ToString().Contains("abc"));
			Assert.False(output.ToString().Contains("def"));
			Assert.True(output.ToString().Contains("123"));
			Assert.True(output.ToString().Contains("456"));

			Assert.False(output2.ToString().Contains("abc"));
			Assert.True(output2.ToString().Contains("def"));
			Assert.False(output2.ToString().Contains("123"));
			Assert.False(output2.ToString().Contains("456"));
		}
		
	}
}
