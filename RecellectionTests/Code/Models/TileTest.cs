using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Test code for Tile-class.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    [TestFixture]
    class TileTest
    {  
        Tile t1, t2;
        Player p;
        Unit u1, u2, u3;
        BaseBuilding b1;
        Building b2;

        [SetUp]
        public void init()
        {
            t1 = new Tile(0,0);
            t2 = new Tile(1,1,Globals.TerrainTypes.Slow);
            p = new Player();
            u1 = new Unit(p, Vector2.Zero);
            u2 = new Unit(p, Vector2.Zero);
            u3 = new Unit(p, Vector2.Zero);
            b1 = new BaseBuilding("TestBase", 0, 0, p);
            b2 = new BarrierBuilding("TestBuilding1", 1, 1, p, b1);
            
        }

        [Test]
        public void CheckType()
        {
            Assert.AreEqual(Globals.TerrainTypes.Membrane, t1.GetTerrainType().GetEnum());
            Assert.AreEqual(Globals.TerrainTypes.Slow, t2.GetTerrainType().GetEnum());

            t1.ChangeTerrainType(Globals.TerrainTypes.Mucus);

            Assert.AreEqual(Globals.TerrainTypes.Mucus, t1.GetTerrainType().GetEnum());
        }

        [Test]
        public void VisibleTo()
        {
            Assert.IsFalse(t1.IsVisible(p));
            t1.MakeVisibleTo(p);
            Assert.IsTrue(t1.IsVisible(p));
            Assert.IsFalse(t2.IsVisible(p));

            t1.MakeInvisibleTo(p);
            Assert.IsFalse(t1.IsVisible(p));
        }

        [Test]
        public void EqualsTest()
        {
            Assert.IsFalse(t1.Equals(t2));
            t1.ChangeTerrainType(t2.GetTerrainType().GetEnum());

            Assert.IsTrue(t1.Equals(t2));
        }

        [Test]
        public void UnitTest()
        {
            Assert.AreEqual(0, t1.GetUnits(p).Count);
            t1.AddUnit(p, u1);
            List<Unit> l1 = new List<Unit>();
            l1.Add(u2);
            l1.Add(u3);
            t1.AddUnit(p, l1);

            Assert.AreEqual(3, t1.GetUnits(p).Count);

            t1.AddUnit(p, u2);
            Assert.AreEqual(3, t1.GetUnits(p).Count);

            t1.RemoveUnit(u1.GetOwner(), u1);
            Assert.AreEqual(2, t1.GetUnits(p).Count);

            t1.RemoveUnit(u1.GetOwner(), u1);
            Assert.AreEqual(2, t1.GetUnits(p).Count);

            t1.RemoveUnit(l1[0].GetOwner(), l1);
            Assert.AreEqual(0, t1.GetUnits(p).Count);
        }

        [Test]
        public void OverrideOperator()
        {
            t1.ChangeTerrainType(t2.GetTerrainType().GetEnum());
            Assert.IsTrue(t1.Equals(t2));
            Assert.IsTrue(t1 == t2);
        }

        [Test]
        public void BuildingTest()
        {
            Assert.IsNull(t1.GetBuilding());
            Assert.IsTrue(t1.SetBuilding(b1));
            Assert.IsFalse(t1.SetBuilding(b2));

            t1.RemoveBuilding();

            Assert.IsNull(t1.GetBuilding());
        }
    }
}
