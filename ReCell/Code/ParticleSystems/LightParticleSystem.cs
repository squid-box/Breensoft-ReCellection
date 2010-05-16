using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
 * Creates and keeps track of all particles in its system.
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    class LightParticleSystem
    {
        List<Light> lights;
        Random random;
        float lightSpawnRate;
        Texture2D lightTex;
        readonly int spriteSz = 64;
        readonly float maxLumency = 0.3f;

        public LightParticleSystem(float spawnRate, Texture2D tex)
        {
            lights = new List<Light>();

            lightSpawnRate = spawnRate;
            lightTex = tex;

            random = new Random();
        }

        public void UpdateAndDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights.ElementAt(i).yVelocity += (float)(random.NextDouble() * 2) - 1;
                lights.ElementAt(i).xVelocity += (float)(random.NextDouble() * 2) - 1;

                lights.ElementAt(i).position += new Vector2(lights.ElementAt(i).xVelocity, lights.ElementAt(i).yVelocity);
                //lights.ElementAt(i).rotation += lights.ElementAt(i).rotSpeed;
                
                lights.ElementAt(i).TTL -= gameTime.ElapsedGameTime.Milliseconds;

                if (lights.ElementAt(i).TTL < 0)
                {
                    lights.Remove(lights.ElementAt(i));
                    i--;
                }
            }

            float roll = (float)random.NextDouble();
            if(roll < lightSpawnRate)
                lights.Add(new Light(new Vector2(random.Next(1000) - 100, random.Next(800) - 100), random.Next(2000) + 4000, (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble() * 12, (float)random.NextDouble() * MathHelper.TwoPi, (float)random.NextDouble()));

            foreach (Light l in lights)
            {
                spriteBatch.Draw(lightTex, l.position, null, new Color(1, 1, 1, Math.Abs(Math.Abs((l.TTL / l.maxTTL) - (1 - maxLumency)) - maxLumency)), l.rotation, new Vector2(spriteSz / 2, spriteSz /2), (l.TTL * l.agingSwell) / l.maxTTL, SpriteEffects.None, 0);
            }
        }
    }
}                                                     