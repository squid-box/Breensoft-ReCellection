/*
 * Creates and keeps track of all particles in its system.
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    class GrainSystem
    {
        #region Fields

        readonly int NUMOFTEX = 10;

        readonly float alphaBlend;

        readonly Texture2D[] backgrounds;

        readonly Random random;

        readonly float updateTres;

        int currentTex;

        float lumency;

        float updateTime;

        #endregion

        #region Constructors and Destructors

        public GrainSystem(float updateRate, float visibility, float lumency, ContentManager content)
        {
            this.backgrounds = new Texture2D[this.NUMOFTEX];
            this.random = new Random();
            var workTex = new Texture2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2, true, SurfaceFormat.Color);
            var workColors = new Color[(Recellection.viewPort.Width / 2) * (Recellection.viewPort.Height / 2)];

            this.alphaBlend = visibility;
            this.updateTres = updateRate;
            this.updateTime = 0;
            this.currentTex = 0;
            this.lumency = lumency;

            for (int i = 0; i < this.NUMOFTEX; i++)
            {
                workTex = new Texture2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2, true, SurfaceFormat.Color);

                for (int x = 0; x < Recellection.viewPort.Width / 2; x++)
                {
                    for (int y = 0; y < Recellection.viewPort.Height / 2; y++)
                    {
                        var roll = (float)this.random.NextDouble();
                        if (roll > 0.5f)
                            workColors[x + y * (Recellection.viewPort.Width / 2)] = new Color(roll * lumency, roll * lumency, roll * lumency, 1f);
                        else
                            workColors[x + y * (Recellection.viewPort.Width / 2)] = Color.Transparent;
                    }
                }

                workTex.SetData(workColors);
                this.backgrounds[i] = workTex;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void UpdateAndDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            this.updateTime += gameTime.ElapsedGameTime.Milliseconds;
            if (this.updateTime > this.updateTres)
            {
                this.updateTime -= this.updateTres;

                this.currentTex++;
                if (this.currentTex >= this.NUMOFTEX)
                    this.currentTex = 0;
            }

            spriteBatch.Draw(this.backgrounds[this.currentTex], new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height), new Color(1f, 1f, 1f, this.alphaBlend));
        }

        #endregion
    }
}                                                     