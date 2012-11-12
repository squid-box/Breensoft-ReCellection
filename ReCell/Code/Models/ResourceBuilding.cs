namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A resource fromBuilding improves upon an ordinary fromBuilding by also having a certain
    /// rate of unit production
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class ResourceBuilding : Building
    {
        #region Constants

        private const int DEFAULT_PRODUCTION = 1;

        #endregion

        #region Fields

        private int rateOfProduction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new ResourceBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="maxHealth"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public ResourceBuilding(string name, int posX, int posY, 
            Player owner, BaseBuilding baseBuilding, LinkedList<Tile> controlZone)
            : base(name, posX, posY, RESOURCE_BUILDING_HEALTH, owner, Globals.BuildingTypes.Resource, baseBuilding, controlZone)
        {
            this.rateOfProduction = controlZone.First().GetTerrainType().getResourceModifier() + DEFAULT_PRODUCTION;
            if (baseBuilding != null)
            {
                baseBuilding.RateOfProduction += this.rateOfProduction;
            }
        }

        #endregion

        #region Public Properties

        public int RateOfProduction
        {
            get { return this.rateOfProduction; }
            set { this.rateOfProduction = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding);
        }

        #endregion
    }
}
