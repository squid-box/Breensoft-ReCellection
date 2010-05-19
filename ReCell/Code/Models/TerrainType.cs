using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Contains information about traits of different Terrain Types.
    /// 
    /// Signed: Marco Ahumada Juntunen (2010-05-06
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-07</date>
    /// 
    /// Signature: Name (date)
    /// Signature: Name (date)
    public class TerrainType : IModel
    {
        // Data
        private int dmgMod; // Damage modifier
        private int spdMod; // Speed modifier
        private int rscMod; // Resource modifier
        private Globals.TerrainTypes type;

        private readonly static Texture2D MEMBRANE_TEXTURE =    Recellection.textureMap.GetTexture(Globals.TextureTypes.Membrane);
        private readonly static Texture2D MUCUS_TEXTURE =       Recellection.textureMap.GetTexture(Globals.TextureTypes.Mucus);
        private readonly static Texture2D SLOW_TEXTURE =        Recellection.textureMap.GetTexture(Globals.TextureTypes.Slow);
        private readonly static Texture2D INFECTED_TEXTURE =    Recellection.textureMap.GetTexture(Globals.TextureTypes.Infected);
        private readonly static Texture2D WATER_TEXTURE = Recellection.textureMap.GetTexture(Globals.TextureTypes.Water);
        private readonly static Texture2D TILE_A = Recellection.textureMap.GetTexture(Globals.TextureTypes.tile2a);
        private readonly static Texture2D TILE_B = Recellection.textureMap.GetTexture(Globals.TextureTypes.tile2b);
        private readonly static Texture2D TILE_C = Recellection.textureMap.GetTexture(Globals.TextureTypes.tile2c);
        private static Random rnd = new Random();
        // Methods

        #region Constructors

        /// <summary>
        /// Creates a TerrainType of the type Membrane.
        /// </summary>
        public TerrainType()
        {
            // Assume default type.
            this.type = Globals.TerrainTypes.Membrane;
            this.dmgMod = 0;
            this.spdMod = 10;
            this.rscMod = 10;
        }
        /// <summary>
        /// Creates a TerrainType of the type specified in t.
        /// </summary>
        /// <param name="t">A type of Globals.TerrainTypes.</param>
        public TerrainType(Globals.TerrainTypes t)
        {
            this.type = t;
            switch (t)
            {
                case (Globals.TerrainTypes.Membrane):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 10;
                        this.rscMod = 0;
                        break;
                    }
                case (Globals.TerrainTypes.Mucus):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 8;
                        this.rscMod = 4;
                        break;
                    }
                case (Globals.TerrainTypes.Slow):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 5;
                        this.rscMod = 0;
                        break;
                    }
                case (Globals.TerrainTypes.Infected):
                    {
                        this.dmgMod = 5;
                        this.spdMod = 10;
                        this.rscMod = 0;
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// Gets the type of this TerrainType.
        /// </summary>
        /// <returns>Enum of the TerrainType-type</returns>
        public Globals.TerrainTypes GetEnum()
        {
            return this.type;
        }

        public Texture2D GetTexture()
        {
           switch (type)
            {
                case Globals.TerrainTypes.Infected:
            /*int random = rnd.Next(1, 4);
            if (random == 3)
               {
                  return TILE_A;
               }
            else if (random == 2)
               {
                  return TILE_B;
               }
             else
               {
                  return TILE_C;
               }*/
                    return INFECTED_TEXTURE;
                case Globals.TerrainTypes.Water:
                    return WATER_TEXTURE;
                case Globals.TerrainTypes.Mucus:
                    return MUCUS_TEXTURE;
                case Globals.TerrainTypes.Slow:
                    return SLOW_TEXTURE;
                default:
                    return MEMBRANE_TEXTURE; 
            }
        }
        /// <summary>
        /// Change the type of this TerrainType.
        /// </summary>
        /// <param name="newType">Enum of the new type of type.</param>
        public void setType(Globals.TerrainTypes newType)
        {
            this.type = newType;
        }

        /// <summary>
        /// Returns the damage modifier of this TerrainType.
        /// </summary>
        /// <returns>Integer-modifier</returns>
        public int getDamageModifier()
        {
            return this.dmgMod;
        }
        /// <summary>
        /// Returns the speed modifier of this TerrainType.
        /// </summary>
        /// <returns>Integer-modifier</returns>
        public int getSpeedModifier()
        {
            return this.spdMod;
        }
        /// <summary>
        /// Returns the resource modifier of this TerrainType.
        /// </summary>
        /// <returns>Integer-modifier</returns>
        public int getResourceModifier()
        {
            return this.rscMod;
        }
    }
}
