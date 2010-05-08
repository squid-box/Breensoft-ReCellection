using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Recellection.Code.Views
{
    public class TestView : IRenderable
    {
        int x = 250;
        int y = 250;
        int size = 512;
        int angle = 0;
        public List<DrawData> GetDrawData(ContentManager content)
        {
            Texture2D tex = content.Load<Texture2D>("Graphics/dracula");
            DrawData d = new DrawData(tex, new Rectangle(x, y, size, size), angle, 0);

            if(Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                x += 5;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                x -= 5;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                size += 16;
                if(size > 512)
                {
                    size = 64;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                angle += 1;
                if(angle >= 360)
                {
                    angle = angle - 360;
                }
            }
            
            List<DrawData> ret = new List<DrawData>();
            ret.Add(d);

            return ret;
        }
    }
}
