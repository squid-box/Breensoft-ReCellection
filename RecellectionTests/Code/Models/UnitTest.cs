using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    [TestFixture]
    class UnitTest
    {
        Unit u1, u2;
        Player p;

        [SetUp]
        public void init()
        {
            p = new Player();
            u1 = new Unit(p);
            u2 = new Unit(p);
        }

        [Test]
        public void KillTest()
        {
            Assert.IsFalse(u1.isDead);
            u1.Kill();
            Assert.IsTrue(u1.isDead);
        }

        [Test]
        public void MovementTest()
        {
            Assert.AreEqual(0, u2.position.X);
            Assert.AreEqual(0, u2.position.Y);

            Assert.AreEqual(-1, u2.targetPosition.X);
			Assert.AreEqual(-1, u2.targetPosition.Y);

			u2.targetPosition = new Vector2(5, 5);

            for (int i = 0; i < 2000; i++)
            {
                u2.Update(1);
            }

			Assert.AreNotEqual(0, u2.targetPosition.X);
			Assert.AreNotEqual(0, u2.targetPosition.Y);

			Assert.AreEqual(-1, u2.targetPosition.X);
            Assert.AreEqual(-1, u2.targetPosition.Y);
        }
    }
}
