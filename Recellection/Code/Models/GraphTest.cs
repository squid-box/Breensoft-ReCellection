using NUnit.Framework;


namespace Recellection.Code.Models
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
		}
		
		[Test]
		public void Add()
		{
			
		}
	}
}