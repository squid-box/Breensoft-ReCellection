using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
	public class TestBuilding : Building
	{
		public int numberOfUnits = 5;

		public override int CountUnits()
		{
			return numberOfUnits;
		}
		
		public override Texture2D GetSprite()
		{
			throw new NotImplementedException();
		}
	}
}
