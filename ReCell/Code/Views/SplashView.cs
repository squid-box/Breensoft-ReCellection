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

        int x = 150;
        int y = 150;
        int width = 512;
        int height = 150;

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
			front = new DrawData(tex, new Rectangle(x, y, x + width, y + height), 0);

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
			if (front.Opacity < 255)
			{
				front.Opacity += 5;
			}
			
            return drawData;
        }
    }
}
