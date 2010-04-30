using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Recellection
{
    class GraphicsRenderer
    {
        public GraphicsRenderer()
        {
        }

        public void Draw(SpriteBatch spriteBatch, List<DrawData> drawData)
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (DrawData d in drawData)
            {
                spriteBatch.Draw(d.Texture, d.Position, new Rectangle(d.CurrentFrame * d.SpriteSize, 0, d.SpriteSize, d.SpriteSize), Color.White, d.Rotation, new Vector2(d.SpriteSize / 2, d.SpriteSize / 2), 1.0f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }
    }
}
