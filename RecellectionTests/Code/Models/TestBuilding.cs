using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
	public class TestBuilding : Building
	{
		public int numberOfUnits = 5;

		public override int CountUnits()
		{
			return numberOfUnits;
		}
		
		public override Microsoft.Xna.Framework.Graphics.Texture2D GetSprite()
		{
			throw new NotImplementedException();
		}
	}
}
