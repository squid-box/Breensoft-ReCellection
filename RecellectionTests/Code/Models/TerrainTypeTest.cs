using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Recellection;

namespace RecellectionTests
{
    [TestFixture]
    class TerrainTypeTest
    {
        [SetUp]
        public void init()
        {
            TerrainType tp1 = new TerrainType();
            TerrainType tp1 = new TerrainType(Globals.TerrainTypes.Infected);
        }

        [Test]
        public void getTerrainType()
        {
            Assert.AreEqual(Globals.TerrainTypes.Membrane, tp1.getType());
            Assert.AreNotEqual(Globals.TerrainTypes.Infected, tp1.getType());
            tp1.setType(Globals.TerrainTypes.Slow);
            Assert.AreEqual(Globals.TerrainTypes.Slow, tp1.getType());
        }
    }
}
