using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    public class BarrierBuilding : Building
    {
        private static float powerBonus = 1.1f;
        BarrierBuilding(String name, int posX, int posY, int maxHealth,
            Player owner, Globals.BuildingTypes type, BaseBuilding baseBuilding)
            : base(name, posX, posY, maxHealth, owner, type, baseBuilding)
        {

        }

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
