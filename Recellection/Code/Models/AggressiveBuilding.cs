using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Graphics;


namespace Recellection.Code.Models
{
    /// <summary>
    /// An Aggressive Building improves upon an ordinary building
    /// by having a targeted unit that will be subject to it's aggressive means.
    /// 
    /// Author: Viktor Eklund
    /// Date: 2010-04-??
    /// 
    /// Signature: Joel Ahlgren (2010-04-20)
    /// Signature: 
    /// </summary>
    public class AggressiveBuilding : Building
    {
        private Unit currentTarget = null;

        //Subscribe to me if you want to know about it when I change my target.
		public event Publish<AggressiveBuilding> targetChanged;

        /// <summary>
        /// Constructs a new AgressiveBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="maxHealth"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public AggressiveBuilding(String name, int posX, int posY, int maxHealth,Player owner,BaseBuilding baseBuilding)
               :base(name, posX, posY, maxHealth, owner, Globals.BuildingTypes.Aggressive, baseBuilding)
        {


        }

        /// <summary>
        /// Getter for the currently targeted unit
        /// </summary>
        /// <returns>
        /// The target of this aggressive building, can be null
        /// </returns>
        public Unit GetTarget()
        {
            return currentTarget;
        }

        /// <summary>
        /// sets a new targeted unit, will overwrite any already targeted unit
        /// null can be passed to just clear the current target
        /// </summary>
        public void SetTarget(Unit newTarget)
        {
            currentTarget = newTarget;
            targetChanged(this,new Event<AggressiveBuilding>(this,EventType.ALTER));
        }

        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding);
        }
    }
}
