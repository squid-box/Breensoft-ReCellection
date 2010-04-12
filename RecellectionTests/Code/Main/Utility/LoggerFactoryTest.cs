using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Recellection.Code.Main.Utility
{
	[TestFixture]
	class LoggerFactoryTest
	{
		private Logger l;
		
		[Test]
		public void GetLoggerName()
		{
			l = LoggerFactory.GetLogger("Test");
			Assert.AreEqual("Test", l.GetName());
		}

		[Test]
		public void GetLoggerAuto()
		{
			l = LoggerFactory.GetLogger();
			Assert.AreEqual("Recellection.Code.Main.Utility.LoggerFactoryTest", l.GetName());
		}
		[Test]
		public void LoggersAreReused()
		{
			l.SetThreshold(LogLevel.FATAL);

			Logger l2 = LoggerFactory.GetLogger(l.GetName());

			Assert.AreEqual(LogLevel.FATAL, l2.GetThreshold());
		}

		[Test]
		public void SetTargetGlobally()
		{
			l = LoggerFactory.GetLogger("Test");
			StringWriter target = new StringWriter();
			StringBuilder output = target.GetStringBuilder();

			l.SetThreshold(LogLevel.TRACE);
			l.SetTarget(target);
			
			// Setup second logger
			Logger l2 = LoggerFactory.GetLogger("Test2");
			StringWriter target2 = new StringWriter();
			StringBuilder output2 = target2.GetStringBuilder();
			l2.SetTarget(target2);

			// Log some stuff to both loggers
			l.Trace("abc");
			l2.Trace("def");

			// Change target for all
			LoggerFactory.setGlobalTarget(target);

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
