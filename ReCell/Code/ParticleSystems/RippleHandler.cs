using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
 * Creates and keeps track of all particles in its system.
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    class RippleHandler
    {
        List<Ripple> ripples;
        Random random;
        readonly int maxRipples;

        public RippleHandler()
        {
            ripples = new List<Ripple>();
            random = new Random();

            maxRipples = 1;
        }

        public void Update(GameTime gameTime, ref Effect rippleShader, Viewport viewPort, MouseState ms, MouseState pms)
        {
            for (int i = 0; i < ripples.Count; i++)
            {
                ripples.ElementAt(i).waveDivider += ripples.ElementAt(i).movementRate;
                ripples.ElementAt(i).waveDivider = MathHelper.Clamp(ripples.ElementAt(i).waveDivider, ripples.ElementAt(i).waveInitDivider, 1.5f);

                ripples.ElementAt(i).distortion -= ripples.ElementAt(i).disappearRate;
                ripples.ElementAt(i).distortion = MathHelper.Clamp(ripples.ElementAt(i).distortion, 0, ripples.ElementAt(i).maxDistortion);

                if (ripples.ElementAt(i).distortion < 0)
                {
                    ripples.Remove(ripples.ElementAt(i));
                    i--;
                }
            }

            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
                ripples.Add(new Ripple(new Vector2((float)ms.X / (float)viewPort.Width, (float)ms.Y / (float) viewPort.Height), 1f, 0.3f, 0.01f, 0.01f));

            if (ripples.Count != 0)
            {
                rippleShader.Parameters["doRipples"].SetValue(true);
            }
        }
    }
}
