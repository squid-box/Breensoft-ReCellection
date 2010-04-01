using NUnit.Framework;
using Recellection.Code.Models;

namespace UnitTests
{
	[TestFixture]
	public class GraphTest
	{
		class TestBuilding : Building
		{
		}
		
		private Graph g;
		
		private Building b1;
		private Building b2;
		private Building b3;
		
		[SetUp]
		public void Init()
		{
			g = new Graph();
			
			b1 = new TestBuilding();
			b2 = new TestBuilding();
			b3 = new TestBuilding();
		}
		
		[Test]
		public void Add()
		{
			g.Add(b1);

			Assert.AreEqual(1, g.CountBuildings());
			
			g.Add(b2);
			
			Assert.AreEqual(2, g.CountBuildings());
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
		public void Weight()
		{
			g.SetWeight(b1, 10);
			Assert.AreEqual(10, g.GetWeight(b1));
			
			g.Remove(b1);
			Assert.AreEqual(0, g.GetWeight(b1));
			
		}
	}
}