using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Recellection.Code.Views
{
    public class TestView : IRenderable
    {
        public List<DrawData> GetDrawData(ContentManager content)
        {
            Texture2D tex = content.Load<Texture2D>("Graphics/Terrains/art");
            DrawData d = new DrawData(new Vector2(250, 250), tex, 0, 0, 128);
            
            List<DrawData> ret = new List<DrawData>();
            ret.Add(d);

            return ret;
        }
    }
}
