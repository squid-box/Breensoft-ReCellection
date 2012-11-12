namespace Recellection.Code.Utility.Events
{
    using System.Collections.Generic;

    using global::Recellection.Code.Models;

    class BuildingEvent : Event<Building>
    {
        #region Fields

        IEnumerable<Unit> units;

        #endregion

        #region Constructors and Destructors

        public BuildingEvent(Building building, IEnumerable<Unit> units, 
            EventType type) : base(building, type)
		{
			this.units = units;
		}

        #endregion
    }
}
