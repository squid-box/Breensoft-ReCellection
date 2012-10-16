namespace Recellection
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class DrawData
    {
        #region Fields

        private readonly int curFrame;

        readonly Vector2 position;

        private readonly float rot;

        private readonly int spriteHeight;

        private readonly int spriteWidth;

        private readonly Rectangle targetRectangle;

        readonly Texture2D tex;

        private byte alpha = 255;

        #endregion

        #region Constructors and Destructors

        /// <summary>
		/// A Wrapper class that describes a drawable object for the Graphics Renderer.
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="targetRectangle">Where to draw the texture.</param>
		/// <param name="rotation">How much the texture should be rotated.</param>
		/// <param name="currentFrame">The current frame? :S</param>
		[System.Obsolete("You won't be using this no more!")]
		public DrawData(Texture2D texture, Rectangle targetRectangle, float rotation, int currentFrame) : this(texture, targetRectangle)
		{
			this.rot = rotation;
			this.curFrame = currentFrame;
		}

		/// <summary>
		/// A Wrapper class that describes a drawable object for the Graphics Renderer.
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="targetRectangle">Where to draw the texture.</param>
		[System.Obsolete("You won't be using this no more!")]
		public DrawData(Texture2D texture, Rectangle rect, byte opacity)
		{
			this.tex = texture;
			this.targetRectangle = rect;
			this.alpha = opacity;
		}

		/// <summary>
		/// A Wrapper class that describes a drawable object for the Graphics Renderer.
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="targetRectangle">Where to draw the texture.</param>
		[System.Obsolete("You won't be using this no more!")]
		public DrawData(Texture2D texture, Rectangle rect)
		{
			this.tex = texture;
			this.targetRectangle = rect;
		}

        /// <summary>
        /// A Wrapper class that describes a drawable object for the Graphics Renderer.
        /// </summary>
        /// <param name="pposition2D">The position of the entity to draw.</param>
        /// <param name="texture">The texture sprite handle to draw.</param>
        /// <param name="rotation">The entity's rotation.</param>
        /// <param name="currentFrame">Which frame to animate. If the frame has no animation cycle, send 0.</param>
        /// <param name="spriteSize">The sprites size in pixels. All sprites have to be n*n pixels in size.</param>
        [System.Obsolete("Use targetRectangle instead")]
        public DrawData(Vector2 position2D, Texture2D texture, float rotation, int currentFrame, int spriteHeight, int spriteWidth)
        {
            this.position = position2D;
            this.tex = texture;
            this.rot = rotation;
            this.curFrame = currentFrame;
            this.spriteHeight = spriteHeight;
            this.spriteWidth = spriteWidth;
        }

        #endregion

        #region Public Properties

        public int CurrentFrame { get { return this.curFrame; } }

        public byte Opacity { get { return this.alpha; } set { this.alpha = value; } }

        [System.Obsolete("Use targetRectangle instead")]
		public Vector2 Position { get { return this.position; } }

        public float Rotation { get { return this.rot; } }

        [System.Obsolete("Use targetRectangle instead")]
		public int SpriteHeight { get { return this.spriteHeight; } }

        [System.Obsolete("Use targetRectangle instead")]
		public int SpriteWidth { get { return this.spriteWidth; } }

        public Rectangle TargetRectangle { get { return this.targetRectangle; } }

        public Texture2D Texture { get { return this.tex; } }

        #endregion
    }
}