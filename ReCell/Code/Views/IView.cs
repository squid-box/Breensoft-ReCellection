using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

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

		private Texture2D pixel = Recellection.textureMap.GetTexture(Globals.TextureTypes.Pixel);
		
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

		// Draws a line with specified thickness 
		public void DrawLine(SpriteBatch batch, Vector2 start, Vector2 end, Color color, int thickness)
		{
			// Calculate distance between both points 
			int distance = (int)Vector2.Distance(start, end);
			
			
			// Calculate angle between both points 
			float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

			LoggerFactory.GetLogger("Recellection.Code.Views.WorldView").Info("Drawing line between " + start + " and " + end + ".");

			Vector2 modifier = new Vector2(0, thickness / 2);

			
			// Draw line 
			batch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, distance, thickness), 
					null, color, angle, new Vector2(0, 1), SpriteEffects.None, Layer);
		} 
	}
}
