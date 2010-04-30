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
		GraphController gc;
		Graph g1;
		Graph g2;
		
		BaseBuilding b1;
		TestBuilding b2;
		TestBuilding b3;
		
		BaseBuilding ba;
		TestBuilding bb;
		TestBuilding bc;
		
		[SetUp]
		public void Init()
		{
			b1 = new BaseBuilding("test", 0, 0, new Player());
			b2 = new TestBuilding();
			b3 = new TestBuilding();

			ba = new BaseBuilding("test", 0, 0, new Player());
			bb = new TestBuilding();
			bc = new TestBuilding();
			
			gc = GraphController.Instance;
		}
		
		[Test]
		public void Singleton()
		{
			Assert.AreSame(GraphController.Instance, gc);
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
			AddBaseBuildings();
			
			gc.AddBuilding(b1, b2);
			gc.SetWeight(b1, 200);
			Assert.AreEqual(200, g1.GetWeight(b1));
		}
		
		[Test]
		public void CalculateWeights()
		{
			AddBaseBuildings();
			b2.numberOfUnits = 10; gc.AddBuilding(b1, b2);
			b3.numberOfUnits = 10; gc.AddBuilding(b1, b3);
			bb.numberOfUnits = 10; gc.AddBuilding(b1, bb);
			bc.numberOfUnits = 10; gc.AddBuilding(b1, bc);
			
			gc.CalculateWeights();

			// TODO: Finish this test when Unit controller is present
			bool UnitControllerIsPresent = false;
			Assert.IsTrue(UnitControllerIsPresent, "Unit Controller must be present.");
		}
		
		[Test]
		public void SumUnitsInGraph()
		{
			AddBaseBuildings();
			b2.numberOfUnits = 5;
			b3.numberOfUnits = 5;
			gc.AddBuilding(b1, b2);
			gc.AddBuilding(b1, b3);
			
			Graph g = gc.GetGraph(b1);
			
			Assert.AreEqual(5+5, GraphController.SumUnitsInGraph(g));
			
			b2.numberOfUnits = 10;
			
			Assert.AreEqual(5+10, GraphController.SumUnitsInGraph(g));
		}

		[Test]
		public void MoveUnits()
		{
			
		}
	}
}
