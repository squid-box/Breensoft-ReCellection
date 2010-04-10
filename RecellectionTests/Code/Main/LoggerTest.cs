using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Recellection.Code.Main
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
			Assert.AreEqual("Recellection.Code.Main.LoggerTest", l.GetName());
		}
	}
}
