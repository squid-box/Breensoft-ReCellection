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
        int width = 400;
        int height = 334;
        String splashFile = "Graphics/logo";
        String background = "Graphics/white";

        static readonly object padlock = new object(); 
        //No idea how the padlock works but I'm not one to argue with code that works.


        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
        {
        }
        /// <summary>
        /// Instantiates a SplashView using a given logo
        /// </summary>
        /// <param name="fileName"></param>
        public SplashView(String fileName)
        {
            splashFile = fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<DrawData> GetDrawData(ContentManager content)
        {
            Texture2D bg = content.Load<Texture2D>(background);
            Texture2D tex = content.Load<Texture2D>(splashFile);
            DrawData c = new DrawData(bg, new Rectangle(0, 0, 800, 600));
            DrawData d = new DrawData(tex, new Rectangle(x, y, x + width, y + height));


            List<DrawData> ret = new List<DrawData>();
            ret.Add(c);
            ret.Add(d);

            return ret;
        }
    }
}
