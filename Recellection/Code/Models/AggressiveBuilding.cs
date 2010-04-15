using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Events;


namespace Recellection.Code.Models
{
    public class AggressiveBuilding : Building
    {
        private Unit currentTarget = null;
        public event Publish<AggressiveBuilding> targetChanged;

        public AggressiveBuilding(String name, int posX, int posY, int maxHealth,Player owner, Globals.BuildingTypes type, BaseBuilding baseBuilding)
               :base(name, posX, posY, maxHealth, owner, type, baseBuilding)
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
    }
}
