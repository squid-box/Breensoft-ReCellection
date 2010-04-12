using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Recellection.Code.Models
{
    [TestFixture]
    class TileTest
    {
        Tile t1, t2;
        [SetUp]
        public void init()
        {
            t1 = new Tile();
            t2 = new Tile(Globals.TerrainTypes.Slow);
        }

        [Test]
        public void createTest()
        {
            Assert.IsNotNull(t1);
            Assert.IsNotNull(t2);
        }
    }
}
