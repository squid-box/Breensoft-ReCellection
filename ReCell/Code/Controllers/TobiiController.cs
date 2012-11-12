namespace Recellection
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    using Code.Models;
    using Code.Utility.Events;
    using Code.Utility.Logger;

    using Tobii.TecSDK.Client;
    using Tobii.TecSDK.Client.Interaction;

    /// <summary>
    /// The TobiiController serves the purpose of simplifying region creation and handling
    /// for our software.
    /// 
    /// Signature: Marco Ahumada Juntunen (2010-05-06)
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public sealed class TobiiController
    {
        #region Constants

        private const int DefaultTimeSpan = 1;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LoggerFactory.GetLogger();

        private static TobiiController instance;

        #endregion

        #region Fields

        private GUIRegion bot;

        private KeyboardState keyboardState;

        private KeyboardState lastKeyboardState;

        private GUIRegion left;

        private GUIRegion newActivatedRegion;

        private GUIRegion right;

        private GUIRegion top;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="TobiiController"/> class from being created.
        /// Main constructor for the controller, remember to call Init() or the controller will not work.
        /// </summary>
        private TobiiController()
        {       
            Logger.Info("Created the Tobii Controller");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Provides singleton functionality.
        /// </summary>
        /// <returns>The (singleton) instance of the TobiiController.</returns>
        public static TobiiController GetInstance()
        {
            if (instance == null)
            {
                instance = new TobiiController();
            }

            return instance;
        }

        /// <summary>
        /// Blocking function that will, eventually, return an Event
        /// consisting of the GUIRegion that Published the event, and any EventArgs
        /// </summary>
        /// <returns>The region that was activated.</returns>
        public GUIRegion GetActivatedRegion()
        {
            for (;;)
            {
                System.Threading.Thread.Sleep(200); // so I heard you like hogging cpu time                
                this.lastKeyboardState = this.keyboardState;
                this.keyboardState = Recellection.publicKeyBoardState;
                
                #if DEBUG
                if (this.keyboardState.IsKeyDown(Keys.W) && this.lastKeyboardState.IsKeyUp(Keys.W))
                {
                    if (this.top != null)
                    {
                        this.top.Publish(this.top, new EventType());
                    }
                }
                else if (this.keyboardState.IsKeyDown(Keys.S) && this.lastKeyboardState.IsKeyUp(Keys.S))
                {
                    if (this.bot != null)
                    {                    
                        this.bot.Publish(this.bot, new EventType());
                    }
                }
                else if (this.keyboardState.IsKeyDown(Keys.A) && this.lastKeyboardState.IsKeyUp(Keys.A))
                {
                    if (this.left != null)
                    {
                        this.left.Publish(this.left, new EventType());
                    }
                }
                else if (this.keyboardState.IsKeyDown(Keys.D) && this.lastKeyboardState.IsKeyUp(Keys.D))
                {
                    if (this.right != null)
                    {
                        this.right.Publish(this.right, new EventType());
                    }
                }

                #endif
                if (this.newActivatedRegion != null)
                {
                    GUIRegion temp = this.newActivatedRegion;
                    this.newActivatedRegion = null;
                    return temp;
                }
            }
        }

        /// <summary>
        /// Initializes the Tobii Controller
        /// </summary>
        /// <returns>
        /// False if for any reason the initialization did not complete successfully, true otherwise.
        /// </returns>
        public bool Init()
        {
            if (!TecClient.IsInitialized)
            {
                try
                {
                    TecClient.Init("ReCellection");

                    /*
                    var newProfile = new Tobii.TecSDK.Core.Interaction.Contracts.UserProfileProperties("RecellectionProfile");
                    UserProfile.Add("RecellectionProfile", "RecellectionProfile");
                    UserProfile.SetCurrent("RecellectionProfile");
                    UserProfile.Current.FeedbackSettings = new Tobii.TecSDK.Core.Interaction.Contracts.FeedbackSettings();
                    UserProfile.Current.Enabled = true;

                    // these lines will, it seems, create a new ClientApplicationProperties object
                    // based on the current client settings and current user profile.
                    // the new object sets OffWindowProcessing
                    // **there is probably an easier way to go about this**
                    // **have to wait and see if this will mess with the UserProfile outside of our software**
                    var props = new Tobii.TecSDK.Core.Interaction.Contracts.ClientApplicationProperties(TecClient.ClientSettings, UserProfile.Current);

                    props.OffWindowProcessing = true;
                    props.Enabled = true;

                    // we copy our newly created ClientApplicationProperties object to our CurrentApplicationProfile
                    // the change is then applies by calling UpdateApplicationProfile
                    TecClient.CurrentApplicationProfile.CopyProperties(props);
                    TecClient.CurrentApplicationProfile.Enabled = true;
                    TecClient.UpdateApplicationProfile();

                    TecClient.ClientSettings.OffWindowProcessing = true;
                    TecClient.ClientSettings.ApplySettings();
                    TecClient.SettingsManager.ApplySettings();

                    this.SetFeedbackColor(Color.White);
                     */
                }
                catch (Exception)
                {
                    Logger.Warn("The Tobii Controller did not initialize correctly");
                    return false;
                }
    
                Logger.Info("Successfully initialized the Tobii Controller");
                return true;
            }
            else
            {
                // the controller was already initialized
                return true;
            }
        }

        /// <summary>
        /// Reads the GUIRegions from the Menu parameter
        /// and makes sure they get tracked.
        /// </summary>
        /// <param name="menu">Menu to load.</param>
        public void LoadMenu(Menu menu)
        {
            // Interaction.Regions.Clear();    // This seems to break, at least on my laptop.          
            foreach (GUIRegion region in menu.GetRegions())
            {
                this.AddRegion(region);
            }

            if (menu.leftOff != null)
            {
                this.AddLeftOffScreen(menu.leftOff.region);
            }

            if (menu.rightOff != null)
            {
                this.AddRightOffScreen(menu.rightOff.region);
            }

            if (menu.botOff != null)
            {
                this.AddTopOffScreen(menu.botOff.region);
            }

            if (menu.topOff != null)
            {
                this.AddBotOffScreen(menu.topOff.region);
            }
        }

        /// <summary>
        /// Change color of the dwell indicator.
        /// </summary>
        /// <param name="color">Eye tracking feedback point color.</param>
        public void SetFeedbackColor(Color color)
        {
            /*
            var col = new System.Windows.Media.Color();
            col.A = color.A;
            col.B = color.B;
            col.G = color.G;
            col.R = color.R;
            TecClient.CurrentApplicationProfile.FeedbackSettings.DwellFeedbackColor = col;
            TecClient.UpdateApplicationProfile();
            */
        }

        /// <summary>
        /// Enable/disable all currently loaded regions
        /// </summary>
        /// <param name="value">
        /// False will disable, true will enable.
        /// </param>
        public void SetRegionsEnabled(bool value)
        {
            return;
            foreach (IInteractionRegion region in Regions.RegisteredRegions)
            {
                region.CanActivate = value;
                region.Enabled = value;
            }        
        }

        /// <summary>
        /// Unloads a Menu, preferably only provide this
        /// function with the menu that is currently loaded
        /// </summary>
        /// <param name="menu">Menu to unload.</param>
        public void UnloadMenu(Menu menu)
        {            
            foreach (GUIRegion region in menu.GetRegions())
            {
                Regions.Remove(region.RegionIdentifier);
            }

            if (menu.leftOff != null)
            {
                Regions.Remove(menu.leftOff.region.RegionIdentifier);
            }

            if (menu.rightOff != null)
            {
                Regions.Remove(menu.rightOff.region.RegionIdentifier);
            }

            if (menu.botOff != null)
            {
                Regions.Remove(menu.botOff.region.RegionIdentifier);
            }

            if (menu.topOff != null)
            {
                Regions.Remove(menu.topOff.region.RegionIdentifier);
            }
        }

        #endregion

        #region Methods

        private void AddBotOffScreen(GUIRegion bot)
        {
            this.bot = bot;
            this.AddRegion(this.bot);
            this.bot.Enabled = true;
            this.bot.FocusEnter += this.FocusEnter;
        }

        // We'll just do them like this for now
        private void AddLeftOffScreen(GUIRegion left)
        {
            this.left = left;            
            this.AddRegion(left);
            this.left.Enabled = true;
            this.left.FocusEnter += this.FocusEnter;
        }

        /// <summary>
        /// Attempts to add a region
        /// Note that a region will be disabled by default
        /// </summary>
        /// <param name="newRegion">Region to add.</param>
        /// <returns>
        /// will return an identifier to your newly created region
        /// should you want to fiddle with it later
        /// </returns>
        private WindowBoundInteractionRegionIdentifier AddRegion(GUIRegion newRegion)
        {                
            newRegion.CanActivate = true;
            if (newRegion.DwellTime == null)
            {
                newRegion.DwellTime = new TimeSpan(0, 0, DefaultTimeSpan);
            }

            newRegion.Enabled = false;

            // TODO: Figure out what these calls does
            // newRegion.AlwaysInteractive = true; 
            // newRegion.IsActivating = true; 
            // newRegion.UsesCoordinate = true; 
            try
            {
                Logger.Info("about to add a new region");
                Regions.Add(newRegion); // this is what actually makes tobii start tracking our region
            }
            catch (Exception)
            {
                // will occur if the region was already added, Interaction.AddRegion can throw stuff
                Logger.Warn("Failed to add a new region, region was most likely already added");
                return null;
            }

            Logger.Info("Successfully added a new region");
            newRegion.regionActivated += this.NewRegionRegionActivated;
            return newRegion.RegionIdentifier;
        }

        private void AddRightOffScreen(GUIRegion right)
        {
            this.right = right;
            this.AddRegion(this.right);
            this.right.Enabled = true;
            this.right.FocusEnter += this.FocusEnter;
        }

        private void AddTopOffScreen(GUIRegion top)
        {
            this.top = top;
            this.AddRegion(this.top);
            this.top.Enabled = true;
            this.top.FocusEnter += this.FocusEnter;
        }

        void FocusEnter(object sender, RegionFocusEventArgs e)
        {
            this.newActivatedRegion = (GUIRegion)sender;
        }

        // the TobiiController subscribes to all regions Activate event
        void NewRegionRegionActivated(object publisher, Event<GUIRegion> ev)
        {
            this.newActivatedRegion = (GUIRegion)publisher;
        }

        #endregion
    }
}