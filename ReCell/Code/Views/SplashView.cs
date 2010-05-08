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

    public sealed class SplashView : IView
    {
		private Texture2D back;
		private Texture2D front;
		
        static readonly object padlock = new object(); 
        //No idea how the padlock works but I'm not one to argue with code that works.


        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
		{
			back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			front = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
        }
        
        override public void Draw(SpriteBatch spriteBatch)
        {
			//front.Opacity = (byte)Math.Min(front.Opacity + 5, 255);

			drawTexture(spriteBatch, back, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			int x = Recellection.viewPort.Width / 2 - front.Width / 2;
			int y = Recellection.viewPort.Height / 2 - front.Height / 2;
			drawTexture(spriteBatch, front, new Rectangle(x, y, front.Width, front.Height));
        }
    }
}
