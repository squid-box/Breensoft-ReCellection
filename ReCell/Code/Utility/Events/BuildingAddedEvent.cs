namespace Recellection.Code.Utility.Events
{
    using global::Recellection.Code.Models;

    public class BuildingAddedEvent : Event<Building>
    {
        #region Constructors and Destructors

        /// <summary>
        /// A fromBuilding added event
        /// </summary>
        /// <param name="fromBuilding"></param>
        /// <param name="type"></param>
        public BuildingAddedEvent(Building building, EventType type):base(building, type)
        {
            this.NewBuilding = building;
        }

        #endregion

        #region Public Properties

        public Building NewBuilding { get; set; }

        #endregion
    }
}
