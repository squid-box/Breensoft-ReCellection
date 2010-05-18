using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
    /// <summary>
    /// A BarrierBuilding improves upon an 
    /// ordinary fromBuilding by providing a defensive bonus
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class BarrierBuilding : Building
    {
        private readonly float powerBonus = 0.3f;

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
        /// Constructs a new BarrierBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public BarrierBuilding(String name, int posX, int posY,
            Player owner, BaseBuilding baseBuilding, LinkedList<Tile> controlZone)
            : base(name, posX, posY, BARRIER_BUILDING_HEALTH, owner, Globals.BuildingTypes.Barrier, baseBuilding,
            controlZone)
        {

            foreach(Tile t in controlZone)
            {
                t.unitsChanged += BarrierBuilding_unitsChanged;
            }

        }

        void BarrierBuilding_unitsChanged(object publisher, Event<IEnumerable<Unit>> ev)
        {
            if (ev.type == EventType.ADD)
            {
                foreach (Unit u in ev.subject)
                {
                    if (u.GetOwner() == this.owner)
                    {
                        u.PowerLevel += powerBonus;
                    }
                }
            }
            else if (ev.type == EventType.REMOVE)
            {
                foreach (Unit u in ev.subject)
                {
                    if (u.GetOwner() == this.owner)
                    {
                        u.PowerLevel -= powerBonus;
                    }
                }
            }
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
