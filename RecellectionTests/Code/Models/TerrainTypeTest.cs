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
            tp2 = new TerrainType(Globals.TextureTypes.Infected);
        }

        [Test]
        public void getTerrainType()
        {
            Assert.AreEqual(Globals.TextureTypes.Membrane, tp1.GetEnum());
            Assert.AreNotEqual(Globals.TextureTypes.Infected, tp1.GetEnum());
            tp1.setType(Globals.TextureTypes.Slow);
            Assert.AreEqual(Globals.TextureTypes.Slow, tp1.GetEnum());
        }

        [Test]
        public void GetModifiers()
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
