using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Recellection.Code.Models
{
	class TestBuilding : Building
	{
	}
	
	[TestFixture]
	public class GraphTest
	{
		private Graph g;
		
		private Building b1 = new TestBuilding();
		private Building b2 = new TestBuilding();
		private Building b3 = new TestBuilding();
		
		[SetUp]
		public void Init()
		{
			g = new Graph();
		}
	
		[Test]
		public void AddAndCount()
		{
			g.Add(b1);
			Assert.AreEqual(1, g.CountBuildings());
			g.Add(b2);
			Assert.AreEqual(2, g.CountBuildings());
			g.Add(b3);
			Assert.AreEqual(3, g.CountBuildings());
			
			// Adding same building twice should be ignored
			g.Add(b1);
			Assert.AreEqual(3, g.CountBuildings());
		}
		
		[Test]
		public void Remove()
		{
			g.Add(b1);
			Assert.AreEqual(1, g.CountBuildings());

			g.Remove(b1);
			Assert.AreEqual(0, g.CountBuildings());

			g.Remove(b1);
			Assert.AreEqual(0, g.CountBuildings());
		}
		
		[Test]
		public void SetWeight()
		{
			g.Add(b1);
			g.SetWeight(b1, 100);
			g.SetWeight(b2, 200);
			
			Assert.AreEqual(2, g.CountBuildings());
		}
		
		[Test]
		public void GetWeight()
		{
			g.Add(b1);
			g.SetWeight(b1, 100);
			Assert.AreEqual(100, g.GetWeight(b1));
			g.SetWeight(b1, 200);
			Assert.AreEqual(200, g.GetWeight(b1));
		}
		
		[Test]
		public void GetWeightOfNonExistingBuilding()
		{
			Assert.Throws<ArgumentException>(delegate{ g.GetWeight(b1); });
			
			g.Add(b1);
			g.Remove(b1);
			
			Assert.Throws<ArgumentException>(delegate { g.GetWeight(b1); });
		}
	}
}
