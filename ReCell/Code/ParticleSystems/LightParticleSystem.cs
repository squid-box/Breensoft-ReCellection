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
        readonly float maxLumency = 0.15f;

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

                lights.ElementAt(i).curPresence += lights.ElementAt(i).presenceChangeSpeed;

                if (lights.ElementAt(i).curPresence > lights.ElementAt(i).maxPresence || lights.ElementAt(i).curPresence < lights.ElementAt(i).minPresence)
                    lights.ElementAt(i).presenceChangeSpeed *= -1;
                
                lights.ElementAt(i).TTL -= gameTime.ElapsedGameTime.Milliseconds;

                if (lights.ElementAt(i).TTL < 0)
                {
                    lights.Remove(lights.ElementAt(i));
                    i--;
                }
            }

            float roll = (float)random.NextDouble();
            if(roll < lightSpawnRate)
                lights.Add(new Light(new Vector2(random.Next(1000) - 100, random.Next(800) - 100), random.Next(2000) + 4000, (float)random.NextDouble(), (float)random.NextDouble(), (float)(random.NextDouble() * 6) + 6, (float)random.NextDouble() * 2, (float)(random.NextDouble() * 0.009f) + 0.001f, (float)random.NextDouble() * MathHelper.TwoPi, (float)random.NextDouble()));

            foreach (Light l in lights)
            {
                spriteBatch.Draw(lightTex, l.position, null, new Color(1, 1, 1, maxLumency * CalcIntensity(l.TTL / l.maxTTL)), l.rotation, new Vector2(spriteSz / 2, spriteSz /2), l.curPresence, SpriteEffects.None, 0);
                //spriteBatch.Draw(lightTex, l.position, null, new Color(1, 1, 1, Math.Abs(Math.Abs((l.TTL / l.maxTTL) - (1 - maxLumency)) - maxLumency)), l.rotation, new Vector2(spriteSz / 2, spriteSz /2), (l.TTL * l.agingSwell) / l.maxTTL, SpriteEffects.None, 0); DECAPRICATED
            }
        }

        private float CalcIntensity(float x)
        {
            return -(float)Math.Pow((2*x) - 1, 2) + 1;
        }
    }
}

                     
