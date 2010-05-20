using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Recellection.Code.Models;
using Recellection.Code.Controllers;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// Test code for UnitAccountant.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-05</date>
    [TestFixture]
    public class UnitAccountantTest
    {
        private Player p;
        private Graph g;
        private BaseBuilding bb;
        private UnitAccountant ua;
        
        [SetUp]
        public void init()
        {
            this.p = new Player();
			this.bb = new BaseBuilding("test", 0, 0, new Player(), new LinkedList<Tile>());
            this.g = new Graph(bb);
            this.ua = new UnitAccountant(p);
            this.p.AddGraph(g);
        }

        [Test]
        public void ProductionTest()
        {
            bb.RateOfProduction = 5;

            Assert.AreEqual(1, g.CountBuildings());
            Assert.AreEqual(0, bb.units.Count);

            ua.ProduceUnits();

            Assert.AreEqual(5, bb.units.Count);
        }
    }
}
