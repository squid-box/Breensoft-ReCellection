/*
 * Creates and keeps track of all particles in its system.
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class LightParticleSystem
    {
        #region Fields

        readonly float lightSpawnRate;

        readonly Texture2D lightTex;

        readonly List<Light> lights;

        readonly float maxLumency = 0.15f;

        readonly Random random;

        readonly int spriteSz = 64;

        #endregion

        #region Constructors and Destructors

        public LightParticleSystem(float spawnRate, Texture2D tex)
        {
            this.lights = new List<Light>();

            this.lightSpawnRate = spawnRate;
            this.lightTex = tex;

            this.random = new Random();
        }

        #endregion

        #region Public Methods and Operators

        public void UpdateAndDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.lights.Count; i++)
            {
                this.lights.ElementAt(i).yVelocity += (float)(this.random.NextDouble() * 2) - 1;
                this.lights.ElementAt(i).xVelocity += (float)(this.random.NextDouble() * 2) - 1;

                this.lights.ElementAt(i).position += new Vector2(
                    this.lights.ElementAt(i).xVelocity, this.lights.ElementAt(i).yVelocity);

                // lights.ElementAt(i).rotation += lights.ElementAt(i).rotSpeed;
                this.lights.ElementAt(i).curPresence += this.lights.ElementAt(i).presenceChangeSpeed;

                if (this.lights.ElementAt(i).curPresence > this.lights.ElementAt(i).maxPresence
                    || this.lights.ElementAt(i).curPresence < this.lights.ElementAt(i).minPresence)
                {
                    this.lights.ElementAt(i).presenceChangeSpeed *= -1;
                }

                this.lights.ElementAt(i).TTL -= gameTime.ElapsedGameTime.Milliseconds;

                if (this.lights.ElementAt(i).TTL < 0)
                {
                    this.lights.Remove(this.lights.ElementAt(i));
                    i--;
                }
            }

            var roll = (float)this.random.NextDouble();
            if(roll < this.lightSpawnRate)
                this.lights.Add(new Light(new Vector2(this.random.Next(1000) - 100, this.random.Next(800) - 100), this.random.Next(2000) + 4000, (float)this.random.NextDouble(), (float)this.random.NextDouble(), (float)(this.random.NextDouble() * 6) + 6, (float)this.random.NextDouble() * 2, (float)(this.random.NextDouble() * 0.009f) + 0.001f, (float)this.random.NextDouble() * MathHelper.TwoPi, (float)this.random.NextDouble()));

            foreach (Light l in this.lights)
            {
                spriteBatch.Draw(
                    this.lightTex, 
                    l.position, 
                    null, 
                    new Color(1, 1, 1, this.maxLumency * this.CalcIntensity(l.TTL / l.maxTTL)), 
                    l.rotation, 
                    new Vector2(this.spriteSz / 2, this.spriteSz / 2), 
                    l.curPresence, 
                    SpriteEffects.None, 
                    0);

                // spriteBatch.Draw(lightTex, l.position, null, new Color(1, 1, 1, Math.Abs(Math.Abs((l.TTL / l.maxTTL) - (1 - maxLumency)) - maxLumency)), l.rotation, new Vector2(spriteSz / 2, spriteSz /2), (l.TTL * l.agingSwell) / l.maxTTL, SpriteEffects.None, 0); DECAPRICATED
            }
        }

        #endregion

        #region Methods

        private float CalcIntensity(float x)
        {
            return -(float)Math.Pow((2*x) - 1, 2) + 1;
        }

        #endregion
    }
}

                     
