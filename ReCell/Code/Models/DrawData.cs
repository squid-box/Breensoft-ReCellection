using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection
{
    public class DrawData
    {
        Texture2D tex;
        public Texture2D Texture { get { return tex; } }

		private Rectangle targetRectangle;
		public Rectangle TargetRectangle { get { return targetRectangle; } }

		private float rot = 0.0f;
		public float Rotation { get { return rot; } }

		private int curFrame = 0;
		public int CurrentFrame { get { return curFrame; } }
		
		private byte alpha = 255;
		public byte Opacity { get { return alpha; } set { alpha = value; } }
		
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
			rot = rotation;
			curFrame = currentFrame;
		}

		/// <summary>
		/// A Wrapper class that describes a drawable object for the Graphics Renderer.
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="targetRectangle">Where to draw the texture.</param>
		[System.Obsolete("You won't be using this no more!")]
		public DrawData(Texture2D texture, Rectangle rect, byte opacity)
		{
			tex = texture;
			targetRectangle = rect;
			alpha = opacity;
		}

		/// <summary>
		/// A Wrapper class that describes a drawable object for the Graphics Renderer.
		/// </summary>
		/// <param name="texture">The texture to draw.</param>
		/// <param name="targetRectangle">Where to draw the texture.</param>
		[System.Obsolete("You won't be using this no more!")]
		public DrawData(Texture2D texture, Rectangle rect)
		{
			tex = texture;
			targetRectangle = rect;
		}


		[System.Obsolete("Use targetRectangle instead")]
		public Vector2 Position { get { return position; } }
		Vector2 position;

		[System.Obsolete("Use targetRectangle instead")]
		public int SpriteHeight { get { return spriteHeight; } }
		private int spriteHeight;

		[System.Obsolete("Use targetRectangle instead")]
		public int SpriteWidth { get { return spriteWidth; } }
		private int spriteWidth;


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
            position = position2D;
            tex = texture;
            rot = rotation;
            curFrame = currentFrame;
            this.spriteHeight = spriteHeight;
            this.spriteWidth = spriteWidth;
        }

    }
}