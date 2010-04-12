using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Recellection.Code.Utility
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
			
			l = LoggerFactory.GetLogger("Test");
			l.SetThreshold(LogLevel.TRACE);
			l.SetTarget(target);
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
		
	}
}
