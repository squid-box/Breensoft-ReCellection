using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Recellection.Code.Models
{
    [TestFixture]
    class TerrainTypeTest
    {
        TerrainType tp1, tp2;

        [SetUp]
        public void init()
        {
            tp1 = new TerrainType();
            tp2 = new TerrainType(Globals.TerrainTypes.Infected);
        }

        [Test]
        public void getTerrainType()
        {
            Assert.AreEqual(Globals.TerrainTypes.Membrane, tp1.GetTerrainType());
            Assert.AreNotEqual(Globals.TerrainTypes.Infected, tp1.GetTerrainType());
            tp1.setType(Globals.TerrainTypes.Slow);
            Assert.AreEqual(Globals.TerrainTypes.Slow, tp1.GetTerrainType());
        }

        [Test]
        public void getModifiers()
        {
            Assert.AreEqual(0, tp1.getDamageModifier());
            Assert.AreEqual(10, tp1.getSpeedModifier());
            Assert.AreEqual(10, tp1.getResourceModifier());

            Assert.AreEqual(5, tp2.getDamageModifier());
            Assert.AreEqual(10, tp2.getSpeedModifier());
            Assert.AreEqual(5, tp2.getResourceModifier());
        }
    }
}
