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
        Vector2 position;


        public Vector2 Position
        {
            get { return position; }
        }
        Texture2D tex;

        public Texture2D Texture
        {
            get { return tex; }
        }

        float rot;

        public float Rotation
        {
            get { return rot; }
        }

        private int curFrame;

        public int CurrentFrame
        {
            get { return curFrame; }
        }

        private int spriteHeight;

        public int SpriteHeight
        {
            get { return spriteHeight; }
        }

        private int spriteWidth;

        public int SpriteWidth
        {
            get { return spriteWidth; }
        }


        /// <summary>
        /// A Wrapper class that describes a drawable object for the Graphics Renderer.
        /// </summary>
        /// <param name="pposition2D">The position of the entity to draw.</param>
        /// <param name="texture">The texture sprite handle to draw.</param>
        /// <param name="rotation">The entity's rotation.</param>
        /// <param name="currentFrame">Which frame to animate. If the frame has no animation cycle, send 0.</param>
        /// <param name="spriteSize">The sprites size in pixels. All sprites have to be n*n pixels in size.</param>
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