using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

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
            u2 = new Unit();
        }

        [Test]
        public void KillTest()
        {
            Assert.IsFalse(u1.IsDead());
            u1.Kill();
            Assert.IsTrue(u1.IsDead());
        }

        [Test]
        public void MovementTest()
        {
            Assert.AreEqual(0, u2.GetPosition().X);
            Assert.AreEqual(0, u2.GetPosition().Y);

            Assert.AreEqual(-1, u2.GetTarget().X);
            Assert.AreEqual(-1, u2.GetTarget().Y);

            u2.SetTargetX(new Vector2(5,5));

            for (int i = 0; i < 2000; i++)
            {
                u2.Update(1);
            }

            Assert.AreNotEqual(0, u2.GetPosition().X);
            Assert.AreNotEqual(0, u2.GetPosition().Y);

            Assert.AreEqual(-1, u2.GetTarget().X);
            Assert.AreEqual(-1, u2.GetTarget().Y);
        }
    }
}
