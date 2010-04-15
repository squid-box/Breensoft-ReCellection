using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    public class BarrierBuilding : Building
    {
        private readonly float powerBonus = 1.1f;

        public readonly float PowerBonus
        {
            get { return powerBonus; }
        }
        
        BarrierBuilding(String name, int posX, int posY, int maxHealth,
            Player owner, BaseBuilding baseBuilding)
            : base(name, posX, posY, maxHealth, owner, Globals.BuildingTypes.Barrier, baseBuilding)
        {
            //unitsChanged += new global::Recellection.Code.Utility.Events.Publish<Building>(BarrierBuilding_unitsChanged);
        }

        //void BarrierBuilding_unitsChanged(object publisher, global::Recellection.Code.Utility.Events.Event<Building> ev)
        //{
        //    if (ev.type == global::Recellection.Code.Utility.Events.EventType.ADD)
        //    {

        //    }
        //    else if (ev.type == global::Recellection.Code.Utility.Events.EventType.REMOVE)
        //    {

        //    }
        //}


        //Not my job =(
        //public void applyBonus()
        //{
        //    foreach (Unit unit in units)
        //    {
        //        unit.SetPowerLevel(powerBonus);
        //    }
        //}
    }
}
