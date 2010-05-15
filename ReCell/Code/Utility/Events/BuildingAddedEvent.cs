using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    public class BuildingAddedEvent : Event<Building>
    {
        Building newBuilding;

        public Building NewBuilding
        {
            get { return newBuilding; }
            set { newBuilding = value; }
        }

        /// <summary>
        /// A fromBuilding added event
        /// </summary>
        /// <param name="fromBuilding"></param>
        /// <param name="type"></param>
        public BuildingAddedEvent(Building building, EventType type):base(building, type)
        {
            this.newBuilding = building;
        }
    }
}
