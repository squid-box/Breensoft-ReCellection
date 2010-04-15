using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
	[TestFixture]
	public class GraphControllerTest
	{
		private class TestBuilding : Building
		{
		}
		
		GraphController gc;
		Graph g1;
		Graph g2;
		
		BaseBuilding b1;
		Building b2;
		Building b3;
		
		BaseBuilding ba;
		Building bb;
		Building bc;
		
		[SetUp]
		public void Init()
		{
			b1 = new BaseBuilding();
			b2 = new TestBuilding();
			b3 = new TestBuilding();

			ba = new BaseBuilding();
			bb = new TestBuilding();
			bc = new TestBuilding();
			
			gc = new GraphController();
		}
		
		[Test]
		public void GetGraph()
		{
			AddBaseBuildings();
			
			Assert.AreEqual(g1, gc.GetGraph(b1));
			Assert.AreEqual(g2, gc.GetGraph(ba));

			Assert.Throws<GraphLessBuildingException>(delegate { gc.GetGraph(b2); });
		}
		
		[Test]
		public void AddBuilding()
		{
			AddBaseBuildings();
			
			gc.AddBuilding(b1, b2);
			Assert.AreEqual(gc.GetGraph(b1), gc.GetGraph(b2));
			
			gc.AddBuilding(b1, b3);
			Assert.AreEqual(gc.GetGraph(b1), gc.GetGraph(b3));
		}
		
		[Test]
		public void AddBaseBuildings()
		{
			g1 = gc.AddBaseBuilding(b1);
			g2 = gc.AddBaseBuilding(ba);

			Assert.AreEqual(g1, gc.GetGraph(b1));
			Assert.AreEqual(g2, gc.GetGraph(ba));
		}
		
		[Test]
		public void SetWeight()
		{
			// TODO
		}
		
		[Test]
		public void CalculateWeights()
		{
			// TODO
		}
		
		[Test]
		public void SumUnitsInGraph()
		{
			// TODO
		}
		
		[Test]
		public void MoveUnits()
		{
			// TODO
		}
	}
}
