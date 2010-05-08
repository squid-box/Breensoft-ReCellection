using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Views
{
    /**
     * Most of this code is cannibalized from the MenuView Class and then slightly edited.
     * 
     * 
     * Author: Lukas Mattsson
     */

    public sealed class SplashView : IRenderable
    {

		DrawData back;
		DrawData front;
		
		List<DrawData> drawData;
		
        static readonly object padlock = new object(); 
        //No idea how the padlock works but I'm not one to argue with code that works.


        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
		{
			Texture2D bg = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			Texture2D tex = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);

			back = new DrawData(bg, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			int x = Recellection.viewPort.Width / 2 - tex.Width / 2;
			int y = Recellection.viewPort.Height / 2 - tex.Height / 2;
			front = new DrawData(tex, new Rectangle(x, y, tex.Width, tex.Height), 0);

			drawData = new List<DrawData>();
			drawData.Add(back);
			drawData.Add(front);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<DrawData> GetDrawData(ContentManager content)
        {
			front.Opacity = (byte)Math.Min(front.Opacity + 5, 255);
			
            return drawData;
        }
    }
}
