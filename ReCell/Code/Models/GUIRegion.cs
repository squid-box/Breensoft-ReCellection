namespace Recellection.Code.Models
{
    using System;
    using System.Windows;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    using Tobii.TecSDK.Client.Interaction;
    using Tobii.TecSDK.Client.Interaction.RegionImplementations;

    /// <summary>
    /// The GUIRegion class is a wrapper around a WindowBoundInteractionRegion
    /// to provide for our publisher-observer pattern.
    /// </summary>
    public class GUIRegion : WindowBoundInteractionRegion, IModel
    {
        #region Static Fields

        public static Logger logger = LoggerFactory.GetLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// General Constructor for GUIRegion.
        /// </summary>
        /// <param name="id">An identifier for the region</param>
        public GUIRegion(WindowBoundInteractionRegionIdentifier id)
            : base(id)
        {
            logger.Trace("Creating a new GUIRegion.");
            this.Activate += this.OnActivate;
            this.Publish(this, EventType.ADD);
        }

        /// <summary>
        /// General Constructor for GUIRegion.
        /// </summary>
        /// <param name="nativeHwnd">The window handle for the game window.</param>
        /// <param name="innerBounds">A Rect object to define where and how big the region will be.</param>
        public GUIRegion(IntPtr nativeHwnd, Rect innerBounds)
            : base(nativeHwnd, innerBounds)
        {
            logger.Trace("Creating a new GUIRegion.");
            this.Publish(this, EventType.ADD);
            this.Activate += this.OnActivate;
        }

        #endregion

        #region Public Events

        public event Publish<GUIRegion> regionActivated;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Publishes events.
        /// </summary>
        /// <param name="guiregion">The GUIRegion instance that triggered the event.</param>
        /// <param name="t">The type of event triggered.</param>
        public void Publish(GUIRegion guiregion, EventType t)
        {
            if (this.regionActivated != null)
            {
               this.regionActivated(this, new GUIRegionEvent(this, t));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Internal method fired by the Tobii eventhandler.
        /// </summar>
        private void OnActivate(object sender, ActivateEventArgs e)
        {
            this.Publish(this, EventType.ALTER);
        }

        #endregion
    }
}
