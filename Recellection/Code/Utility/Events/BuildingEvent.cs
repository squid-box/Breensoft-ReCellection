using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    class BuildingEvent : Event<Building>
    {
        List<Unit> units;

        public BuildingEvent(Building building, IEnumerable<Unit> units, 
            EventType type) : base(building, type)
		{
			this.units = units;
		}
    }
}
