using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Content;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Models;
using Recellection.Code.Controllers;


namespace Recellection
{
    public class GraphicsRenderer
	{
		private static Logger logger = LoggerFactory.GetLogger();
        public static IRenderable currentState = null;
		
        public GraphicsRenderer()
        {
			logger.SetThreshold(LogLevel.INFO);
        }

        public void Draw(ContentManager content, SpriteBatch spriteBatch)
		{
			Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
			Recellection.graphics.GraphicsDevice.Clear(Color.Black);
			
			if( currentState == null)
			{
				logger.Warn("No state to render!");
				return;
			}
			
			List<DrawData> drawData = GraphicsRenderer.currentState.GetDrawData(content);
            
			
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (DrawData d in drawData)
            {
				logger.Trace("Drawing " + d + " at rectangle (" + d.TargetRectangle + ")");
				
				Color c = Color.White;
				
				if (d.Opacity < 1.0f)
				{
					c = new Color(255, 255, 255, d.Opacity*255);
				}
				
                spriteBatch.Draw(d.Texture, 
					d.TargetRectangle,
					null,
					c, 
					d.Rotation, 
					new Vector2(0, 0),
					SpriteEffects.None, 
					0);
            }
            spriteBatch.End();


        }
    }
}
