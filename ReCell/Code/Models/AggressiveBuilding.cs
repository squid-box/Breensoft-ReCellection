namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Events;

    /// <summary>
    /// An Aggressive Building improves upon an ordinary fromBuilding
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
        #region Constants

        private const int MAXIMUM_NUMBER_OF_TARGETS = 5;

        #endregion

        // Subscribe to me if you want to know about it when I change my baseEntity.
        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new AgressiveBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="maxHealth"></param>
        /// <param name="owner"></param>
        /// <param name="baseBuilding"></param>
        public AggressiveBuilding(string name, int posX, int posY, Player owner, BaseBuilding baseBuilding, 
            LinkedList<Tile> controlZone)
            : base(name, posX, posY, AGGRESSIVE_BUILDING_HEALTH, owner, Globals.BuildingTypes.Aggressive, baseBuilding, controlZone)
        {
            this.currentTargets = new List<Unit>();

            /*for(int i = 0; i < controlZone.Count; i++)
            {
                controlZone.ElementAt(i).unitsChanged += AggressiveBuilding_unitsChanged;
            }*/
            foreach (Tile t in controlZone)
            {
                t.unitsChanged += this.AggressiveBuilding_unitsChanged;
            }

        }

        #endregion

        #region Public Events

        public event Publish<AggressiveBuilding> targetChanged;

        #endregion

        #region Public Properties

        public List<Unit> currentTargets { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding);
        }

        /// <summary>
        /// Getter for the currently targeted unit
        /// </summary>
        /// <returns>
        /// The baseEntity of this aggressive fromBuilding, can be null
        /// </returns>
        public List<Unit> GetTargets()
        {
            return this.currentTargets;
        }

        /// <summary>
        /// sets a new targeted unit, will overwrite any already targeted unit
        /// null can be passed to just clear the current baseEntity
        /// </summary>
        public void SetTargets(List<Unit> newTargets)
        {
            this.currentTargets = newTargets;
            if (this.targetChanged != null)
            {
                this.targetChanged(this, new Event<AggressiveBuilding>(this, EventType.ALTER));
            }
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// This function is called when a unit enters or exits the control zone tiles for the
        /// Aggressive Building. It might be ineffiecient due to the fact when a unit exit a tile
        /// and then enter another tile in the control zone it will first be removed and then added.
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="ev"></param>
        void AggressiveBuilding_unitsChanged(object publisher, Event<IEnumerable<Unit>> ev)
        {
            // TODO implement a way to add this to the que to be shoot.
            if (ev.type == EventType.ADD)
            {
                foreach (Unit u in ev.subject)
                {
                    if (this.currentTargets.Count < MAXIMUM_NUMBER_OF_TARGETS && u.GetOwner() != this.owner)
                    {
                        if (!this.currentTargets.Contains(u))
                        {
                            this.currentTargets.Add(u);
                        }
                    }
                }
            }
            else if (ev.type == EventType.REMOVE)
            {
                /*foreach (Unit u in ev.subject)
                {
                    if (currentTargets.Count > 0 && u.GetOwner() != this.owner)
                    {
                        if (!currentTargets.Contains(u))
                        {
                            currentTargets.Remove(u);
                        }
                    }
                }*/
            }
        }

        #endregion
    }
}
