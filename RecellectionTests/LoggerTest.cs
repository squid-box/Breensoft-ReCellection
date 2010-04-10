using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Recellection.Code.Main;

namespace RecellectionTests
{
	[TestFixture]
	class LoggerTest
	{
		private Logger l;
		
		[SetUp]
		public void init()
		{
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
			Assert.AreEqual("RecellectionTests.LoggerTest", l.GetName());
		}
	}
}
