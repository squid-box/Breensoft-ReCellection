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

        /// <summary>
        /// Constructs a new ResourceBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="maxHealth"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public ResourceBuilding(String name, int posX, int posY,
            Player owner, BaseBuilding baseBuilding)
            : base(name, posX, posY, RESOURCE_BUILDING_HEALTH, owner, Globals.BuildingTypes.Resource, baseBuilding)
        {

        }

        /// <summary>
        /// Sets the producion rate of this ResourceBuilding in terms of units / turns
        /// </summary>
        /// <param name="rate"></param>
        public void SetProductionRate(int rate)
        {
            rateOfProduction = rate;
        }

        /// <summary>
        /// Gets the production rate of this ResourceBuilding in terms of units / turns
        /// </summary>
        /// <returns>
        /// ProductionRate as an int
        /// </returns>
        public int GetProductionRate()
        {
            return rateOfProduction;
        }

        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding);
        }
    }
}
