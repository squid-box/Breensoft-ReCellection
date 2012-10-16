using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace Recellection.Code.Views
{
    public class LoadView : IRenderable
    {
        public List<DrawData> GetDrawData(ContentManager content)
        {
            RenderTarget2D r2 = new RenderTarget2D(Recellection.graphics.GraphicsDevice, 500, 500, false, SurfaceFormat.Color, DepthFormat.Depth24);
            SpriteFont derrp = content.Load<SpriteFont>("Fonts/ScreenFont");

            //DrawString
            
            List<DrawData> ret = new List<DrawData>();

            return ret;
        }
    }
}
