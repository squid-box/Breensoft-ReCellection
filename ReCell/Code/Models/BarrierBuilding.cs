namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Events;

    /// <summary>
    /// A BarrierBuilding improves upon an 
    /// ordinary fromBuilding by providing a defensive bonus
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class BarrierBuilding : Building
    {
        #region Fields

        private readonly float powerBonus = 0.1f;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new BarrierBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public BarrierBuilding(string name, int posX, int posY, 
            Player owner, BaseBuilding baseBuilding, LinkedList<Tile> controlZone)
            : base(name, posX, posY, BARRIER_BUILDING_HEALTH, owner, Globals.BuildingTypes.Barrier, baseBuilding, 
            controlZone)
        {

            foreach(Tile t in controlZone)
            {
                t.unitsChanged += this.BarrierBuilding_unitsChanged;
                foreach(Unit u in t.GetUnits(owner))
                {
					u.Buff = this.powerBonus;
                }
            }

        }

        #endregion

        #region Public Properties

        public float PowerBonus
        {
            get { return this.powerBonus; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding);
        }

        #endregion

        #region Methods

        void BarrierBuilding_unitsChanged(object publisher, Event<IEnumerable<Unit>> ev)
        {
            if (ev.type == EventType.ADD)
            {
                foreach (Unit u in ev.subject)
                {
                    if (u.GetOwner() == this.owner)
                    {
                        u.Buff = this.powerBonus;
                    }
                }
            }
            else if (ev.type == EventType.REMOVE)
            {
                foreach (Unit u in ev.subject)
                {
                    if (u.GetOwner() == this.owner)
                    {
                        u.Buff = 0f;
                    }
                }
            }
        }

        #endregion
    }
}
