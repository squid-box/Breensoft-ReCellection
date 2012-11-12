namespace Recellection.Code.Utility.Events
{
    using global::Recellection.Code.Models;

    public class GUIRegionEvent : Event<GUIRegion>
    {
        #region Constructors and Destructors

        public GUIRegionEvent(GUIRegion guiregion, EventType eventType) : base(guiregion, eventType)
        {
        }

        #endregion
    }
}
