using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Views
{
	public abstract class IView
	{	
		private float currentLayer = 0;
		public float Layer
		{ 
			get { return currentLayer; }
			set { currentLayer = value; }
		}
		
		public abstract void Update(GameTime passedTime);
		public abstract void Draw(SpriteBatch spriteBatch);
		
		public void drawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea)
		{
			spriteBatch.Draw(t, targetArea, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, Layer);
		}

        public void drawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea,Color color)
        {
            spriteBatch.Draw(t, targetArea, null, color, 0, new Vector2(0, 0), SpriteEffects.None, Layer);
        }
	}
}
