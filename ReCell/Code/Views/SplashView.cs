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
		
		private byte opacity;
		private float fadeTime;
		
        static readonly object padlock = new object(); 
        //No idea how the padlock works but I'm not one to argue with code that works.


        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
		{
			back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			front = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
			opacity = 0;
			fadeTime = 0.0f;
        }

		override public void Update(GameTime passedTime)
		{
			if (opacity < 255)
			{
				fadeTime += passedTime.ElapsedRealTime.Milliseconds;
				opacity = (byte)(255.0f * (fadeTime / 1000.0f));
			}
		}
		
        override public void Draw(SpriteBatch spriteBatch)
        {
			drawTexture(spriteBatch, back, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			int x = Recellection.viewPort.Width / 2 - front.Width / 2;
			int y = Recellection.viewPort.Height / 2 - front.Height / 2;
			
			spriteBatch.Draw(front, new Rectangle(x, y, front.Width, front.Height), null, 
				new Color(255, 255, 255, opacity), 0, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
