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
		private static Texture2D pixel = Recellection.textureMap.GetTexture(Globals.TextureTypes.Pixel);
		
		private float currentLayer = 0;
		public float Layer
		{ 
			get { return currentLayer; }
			set { currentLayer = value; }
		}

		
		public abstract void Update(GameTime passedTime);
		public abstract void Draw(SpriteBatch spriteBatch);
		
		public void DrawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea)
		{
			spriteBatch.Draw(t, targetArea, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, Layer);
		}

        public void DrawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea,Color color)
        {
            spriteBatch.Draw(t, targetArea, null, color, 0, new Vector2(0, 0), SpriteEffects.None, Layer);
		}
		
		public void DrawCenteredString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
		{
			position.Y -= Recellection.screenFont.MeasureString(text).Y/2;
			// Split up multiliners
			char[] splits = {'\n'};
			foreach(string line in text.Split(splits))
			{	
				// Measure text
				Vector2 origin = new Vector2(Recellection.screenFont.MeasureString(line).X/2, 0);
				
				spriteBatch.DrawString(Recellection.screenFont, line, position, color, 0, origin, 1.0f, SpriteEffects.None, Layer);

				position.Y += Recellection.screenFont.MeasureString((line == "" ? " " : line)).Y;
			}
		}
		
		public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
		{
			spriteBatch.DrawString(Recellection.screenFont, text, position, color, 0, new Vector2(), 1.0f, SpriteEffects.None, Layer);
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
