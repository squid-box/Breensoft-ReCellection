using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Recellection.Code.Models
{
    [TestFixture]
    class UnitTest
    {
        Unit u1, u2;

        [SetUp]
        public void init()
        {
            u1 = new Unit();
        }

        [Test]
        public void createTest()
        {
            Assert.IsFalse(u1.IsDead());
            u1.Kill();
            Assert.IsTrue(u1.IsDead());
        }
    }
}
