using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    /// <summary>
    /// A resource building improves upon an ordinary building by also having a certain
    /// rate of unit production
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class ResourceBuilding : Building
    {
        private int rateOfProduction;

        public int RateOfProduction
        {
            get { return rateOfProduction; }
            set { rateOfProduction = value; }
        }

        /// <summary>
        /// Constructs a new ResourceBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="maxHealth"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public ResourceBuilding(String name, int posX, int posY, int maxHealth,
            Player owner, BaseBuilding baseBuilding)
            : base(name, posX, posY, maxHealth, owner, Globals.BuildingTypes.Resource, baseBuilding)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding);
        }
    }
}
