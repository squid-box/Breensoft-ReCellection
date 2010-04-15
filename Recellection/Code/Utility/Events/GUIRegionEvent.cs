using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    public class GUIRegionEvent : Event<GUIRegion>
    {
        public GUIRegionEvent(GUIRegion guiregion, EventType eventType) : base(guiregion,eventType)
        {
        }
    }
}
