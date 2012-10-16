namespace Recellection.Code.Views
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class LoadView : IRenderable
    {
        #region Public Methods and Operators

        public List<DrawData> GetDrawData(ContentManager content)
        {
            var r2 = new RenderTarget2D(Recellection.graphics.GraphicsDevice, 500, 500, false, SurfaceFormat.Color, DepthFormat.Depth24);
            var derrp = content.Load<SpriteFont>("Fonts/ScreenFont");

            // DrawString
            
            var ret = new List<DrawData>();

            return ret;
        }

        #endregion
    }
}
