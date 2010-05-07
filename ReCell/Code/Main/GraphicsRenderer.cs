using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Recellection
{
    public class GraphicsRenderer
    {
        public GraphicsRenderer()
        {
        }

        public void Draw(SpriteBatch spriteBatch, List<DrawData> drawData)
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
            Recellection.graphics.GraphicsDevice.Clear(Recellection.breen);

            //Test code for the menu class
            //Code.Models.Menu test = new Code.Models.Menu(Globals.MenuTypes.MainMenu, false);
            //Texture2D temp = test.GetMenuPic();
            

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (DrawData d in drawData)
            {
                spriteBatch.Draw(d.Texture, d.Position, new Rectangle(d.CurrentFrame * d.SpriteSize, 0, d.SpriteSize, d.SpriteSize), Color.White, d.Rotation, new Vector2(d.SpriteSize / 2, d.SpriteSize / 2), 1.0f, SpriteEffects.None, 0);
            }

            //Test call for the menu
            //spriteBatch.Draw(temp, Vector2.Zero, Color.White);
             
            

            spriteBatch.End();


        }
    }
}
