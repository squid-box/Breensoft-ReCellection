using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private Globals.TextureTypes type;


        // Methods

        #region Constructors

        /// <summary>
        /// Creates a TerrainType of the type Membrane.
        /// </summary>
        public TerrainType()
        {
            // Assume default type.
            this.type = Globals.TextureTypes.Membrane;
            this.dmgMod = 0;
            this.spdMod = 10;
            this.rscMod = 10;
        }
        /// <summary>
        /// Creates a TerrainType of the type specified in t.
        /// </summary>
        /// <param name="t">A type of Globals.TerrainTypes.</param>
        public TerrainType(Globals.TextureTypes t)
        {
            this.type = t;
            switch (t)
            {
                case (Globals.TextureTypes.Membrane):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 10;
                        this.rscMod = 10;
                        break;
                    }
                case (Globals.TextureTypes.Mucus):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 8;
                        this.rscMod = 12;
                        break;
                    }
                case (Globals.TextureTypes.Slow):
                    {
                        this.dmgMod = 0;
                        this.spdMod = 5;
                        this.rscMod = 10;
                        break;
                    }
                case (Globals.TextureTypes.Infected):
                    {
                        this.dmgMod = 5;
                        this.spdMod = 10;
                        this.rscMod = 5;
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// Gets the type of this TerrainType.
        /// </summary>
        /// <returns>Enum of the TerrainType-type</returns>
        public Globals.TextureTypes GetEnum()
        {
            return this.type;
        }
        /// <summary>
        /// Change the type of this TerrainType.
        /// </summary>
        /// <param name="newType">Enum of the new type of type.</param>
        public void setType(Globals.TextureTypes newType)
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
