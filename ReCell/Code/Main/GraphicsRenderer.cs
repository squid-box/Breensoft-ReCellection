using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Content;



namespace Recellection
{
    public class GraphicsRenderer
    {
		public static IRenderable currentState = new TestView();
		
        public GraphicsRenderer()
        {

        }

        public void Draw(ContentManager content, SpriteBatch spriteBatch)
        {
			List<DrawData> drawData = GraphicsRenderer.currentState.GetDrawData(content);
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
            Recellection.graphics.GraphicsDevice.Clear(Recellection.breen);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (DrawData d in drawData)
            {
                spriteBatch.Draw(d.Texture, d.Position, new Rectangle(d.CurrentFrame * d.SpriteHeight, 0, d.SpriteWidth, ((d.SpriteWidth+d.SpriteHeight)/2)), Color.White, d.Rotation, new Vector2(d.SpriteHeight / 2, d.SpriteWidth / 2), 1.0f, SpriteEffects.None, 0);
            }

            spriteBatch.End();


        }
    }
}
