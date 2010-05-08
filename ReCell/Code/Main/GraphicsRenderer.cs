using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Content;
using Recellection.Code.Utility.Logger;


namespace Recellection
{
    public class GraphicsRenderer
	{
		private static Logger logger = LoggerFactory.GetLogger();
		public static IRenderable currentState = new TestView();
		
        public GraphicsRenderer()
        {

        }

        public void Draw(ContentManager content, SpriteBatch spriteBatch)
        {
			List<DrawData> drawData = GraphicsRenderer.currentState.GetDrawData(content);
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
            Recellection.graphics.GraphicsDevice.Clear(Recellection.breen);
			
			//logger.Info("Viewport size: "+Recellection.viewPort.Width+"x"+Recellection.viewPort.Height);
			
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (DrawData d in drawData)
            {
                spriteBatch.Draw(d.Texture, 
					new Rectangle((int)d.Position.X, (int)d.Position.Y, (int)d.Position.X+d.SpriteWidth, (int)d.Position.Y+d.SpriteHeight),
					null,
					Color.White, 
					d.Rotation, 
					new Vector2(0, 0),
					SpriteEffects.None, 
					0);
            }
            spriteBatch.End();


        }
    }
}
