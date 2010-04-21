using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    /// <summary>
    /// A BarrierBuilding improves upon an 
    /// ordinary building by providing a defensive bonus
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class BarrierBuilding : Building
    {
        private readonly float powerBonus = 1.1f;

        public float PowerBonus
        {
            get { return powerBonus; }
        }
        
        /// <summary>
        /// Constructs a new BarrierBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public BarrierBuilding(String name, int posX, int posY,
            Player owner, BaseBuilding baseBuilding)
            : base(name, posX, posY, BARRIER_BUILDING_HEALTH, owner, Globals.BuildingTypes.Barrier, baseBuilding)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding);
        }
    }
}
