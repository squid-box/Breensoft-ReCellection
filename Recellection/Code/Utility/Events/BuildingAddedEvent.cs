using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    class BuildingAddedEvent : Event<Building>
    {
        Building newBuilding;

        public Building NewBuilding
        {
            get { return newBuilding; }
            set { newBuilding = value; }
        }

        public BuildingAddedEvent(Building building, EventType type):base(building, type)
        {
            this.newBuilding = building;
        }
    }
}
