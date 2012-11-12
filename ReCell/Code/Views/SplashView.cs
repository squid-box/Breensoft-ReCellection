namespace Recellection.Code.Views
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
	/// Show a splash screen with the logo fading in.
	/// 
	///  Author: Lukas Mattsson
	///  Co-author: Martin Nycander
	/// </summary>
    public sealed class SplashView : IView
    {
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        #endregion

        #region Fields

        private readonly Texture2D back;
		private readonly Texture2D front;

        private float fadeInTime = 1.5f;

        private byte opacity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Instantiates a SplashView with the default Breensoft logo
        /// </summary>
        public SplashView()
		{
			this.back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			this.front = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
			this.opacity = 0;
			
			logger.SetThreshold(LogLevel.ERROR);
			logger.SetTarget(Console.Out);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
		/// Draw the splashview.
		/// </summary>
		/// <param name="spriteBatch">The spritebatch to draw upon.</param>
        override public void Draw(SpriteBatch spriteBatch)
		{
			this.Layer = 1.0f;
			this.DrawTexture(spriteBatch, this.back, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			int x = Recellection.viewPort.Width / 2 - this.front.Width / 2;
			int y = Recellection.viewPort.Height / 2 - this.front.Height / 2;

			this.Layer = 0.0f;
			this.DrawTexture(spriteBatch, this.front, new Rectangle(x, y, this.front.Width, this.front.Height), new Color(255, 255, 255, this.opacity));
        }

        /// <summary>
        /// Updates the view by slowly fading in the logo.
        /// </summary>
        /// <param name="passedTime">The XNA gametime object.</param>
        override public void Update(GameTime passedTime)
        {
            if (this.opacity < 255)
            {
                this.opacity += (byte)((float)passedTime.ElapsedGameTime.TotalSeconds * (255f / this.fadeInTime));

                // opacity = (byte)((float)passedTime.TotalGameTime.TotalSeconds * (255f / fadeInTime));
                // logger.Trace("Passed time: " + passedTime.TotalGameTime.TotalSeconds + ", Opacity = " + opacity);
            }
        }

        #endregion
    }
}
