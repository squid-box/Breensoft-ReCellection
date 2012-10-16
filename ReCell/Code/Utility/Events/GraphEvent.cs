namespace Recellection.Code.Utility.Events
{
    using global::Recellection.Code.Models;

    public class GraphEvent : Event<Building>
	{
        #region Constructors and Destructors

        public GraphEvent(Building building, int weight, EventType type) : base(building, type)
		{
			this.weight = weight;
		}

        #endregion

        #region Public Properties

        public int weight { get; private set; }

        #endregion
	}
}
