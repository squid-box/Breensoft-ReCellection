using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Graphics;

namespace ReCell.Code.Views
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
        int angle = 0;

        static SplashView instance = null;
        static readonly object padlock = new object(); 
        //No idea how the padlock works but I'm not one to argue with code that works.

        public static SplashView Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SplashView();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<DrawData> GetDrawData(ContentManager content)
        {
            Texture2D tex = content.Load<Texture2D>("Graphics/dracula");
            DrawData d = new DrawData(new Vector2(x, y), tex, angle, 0, height, width);


            List<DrawData> ret = new List<DrawData>();
            ret.Add(d);

            return ret;
        }
    }
}
