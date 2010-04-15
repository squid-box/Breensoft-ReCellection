using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    public class BaseBuilding : Building
    {
        LinkedList<Building> childBuildings;

        BaseBuilding(String name, int posX, int posY, int maxHealth,
            Player owner, Globals.BuildingTypes type)
            : base(name, posX, posY, maxHealth, owner, type, null)
        {

        }

        public void Visit(AggressiveBuilding building)
        {
            
        }

        public void Visit(ResourceBuilding building)
        {
        }

        public void Visit(BarrierBuilding building)
        {

        }

        public void Visit(BaseBuilding building)
        {
            throw new DivideByZeroException();
        }

    }
}
