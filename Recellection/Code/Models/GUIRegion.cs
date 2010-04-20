using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;

using Recellection.Code.Utility.Events;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Models
{
    /// <summary>
    /// The GUIRegion class is a wrapper around a WindowBoundInteractionRegion
    /// to provide for our publisher-observer pattern.
    /// </summary>
    public class GUIRegion : WindowBoundInteractionRegion, IModel
    {
        public static Logger logger = LoggerFactory.GetLogger();
        public event Publish<GUIRegion> regionActivated;
        
        /// <summary>
        /// General Constructor for GUIRegion.
        /// </summary>
        /// <param name="id">An identifier for the region</param>
        public GUIRegion(WindowBoundInteractionRegionIdentifier id)
            : base(id)
        {
            logger.Trace("Creating a new GUIRegion.");
            Activate += new EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs>(OnActivate);
            Publish(this, EventType.ADD);
        }

        /// <summary>
        /// General Constructor for GUIRegion.
        /// </summary>
        /// <param name="nativeHwnd">The window handle for the game window.</param>
        /// <param name="innerBounds">A Rect object to define where and how big the region will be.</param>
        public GUIRegion(IntPtr nativeHwnd, System.Windows.Rect innerBounds)
            : base(nativeHwnd, innerBounds)
        {
            logger.Trace("Creating a new GUIRegion.");
            Publish(this, EventType.ADD);
            Activate += new EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs>(OnActivate);
        }
        
        /// <summary>
        /// Internal method fired by the Tobii eventhandler.
        /// </summar>
        private void OnActivate(object sender, Tobii.TecSDK.Client.Interaction.ActivateEventArgs e)
        {
            Publish(this, EventType.ALTER);
        }
        
        /// <summary>
        /// Publishes events.
        /// </summary>
        /// <param name="guiregion">The GUIRegion instance that triggered the event.</param>
        /// <param name="t">The type of event triggered.</param>
        public void Publish(GUIRegion guiregion, EventType t)
        {
            if (regionActivated != null)
            {
               regionActivated(this, new GUIRegionEvent(this, t));
            }
        }

    }
}
