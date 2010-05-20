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
    class GrainSystem
    {
        readonly int NUMOFTEX = 10;

        Texture2D[] backgrounds;
        int currentTex;
        float updateTime, updateTres;
        float alphaBlend;
        float lumency;

        Random random;

        public GrainSystem(float updateRate, float visibility, float lumency, ContentManager content)
        {
            backgrounds = new Texture2D[NUMOFTEX];
            random = new Random();
            Texture2D workTex = new Texture2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] workColors = new Color[(Recellection.viewPort.Width / 2) * (Recellection.viewPort.Height / 2)];

            alphaBlend = visibility;
            updateTres = updateRate;
            updateTime = 0;
            currentTex = 0;
            this.lumency = lumency;

            for (int i = 0; i < NUMOFTEX; i++)
            {
                workTex = new Texture2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2, 1, TextureUsage.None, SurfaceFormat.Color);

                for (int x = 0; x < Recellection.viewPort.Width / 2; x++)
                {
                    for (int y = 0; y < Recellection.viewPort.Height / 2; y++)
                    {
                        float roll = (float)random.NextDouble();
                        if (roll > 0.5f)
                            workColors[x + y * (Recellection.viewPort.Width / 2)] = new Color(roll * lumency, roll * lumency, roll * lumency, 1f);
                        else
                            workColors[x + y * (Recellection.viewPort.Width / 2)] = Color.TransparentBlack;
                    }
                }

                workTex.SetData(workColors);
                backgrounds[i] = workTex;
            }
        }

        public void UpdateAndDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            updateTime += gameTime.ElapsedGameTime.Milliseconds;
            if (updateTime > updateTres)
            {
                updateTime -= updateTres;

                currentTex++;
                if (currentTex >= NUMOFTEX)
                    currentTex = 0;
            }

            spriteBatch.Draw(backgrounds[currentTex], new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height), new Color(1f, 1f, 1f, alphaBlend));
        }
    }
}                                                     