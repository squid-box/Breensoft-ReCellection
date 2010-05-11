using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Views
{
	/// <summary>
	/// Show a splash screen with the logo fading in.
	/// 
	///  Author: Lukas Mattsson
	///  Co-author: Martin Nycander
	/// </summary>
    public sealed class SplashView : IView
    {
		private static Logger logger = LoggerFactory.GetLogger();
		private Texture2D back;
		private Texture2D front;
		
		private byte opacity;
		private float fadeInTime = 1.5f;

        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
		{
			back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			front = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
			opacity = 0;
			
			logger.SetThreshold(LogLevel.ERROR);
			logger.SetTarget(Console.Out);
        }

		/// <summary>
		/// Updates the view by slowly fading in the logo.
		/// </summary>
		/// <param name="passedTime">The XNA gametime object.</param>
		override public void Update(GameTime passedTime)
		{
			if (opacity < 255)
			{
				opacity = (byte)((float)passedTime.TotalGameTime.TotalSeconds * (255f / fadeInTime));
				logger.Trace("Passed time: " + passedTime.TotalGameTime.TotalSeconds + ", Opacity = " + opacity);
			}
		}
		
		/// <summary>
		/// Draw the splashview.
		/// </summary>
		/// <param name="spriteBatch">The spritebatch to draw upon.</param>
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
