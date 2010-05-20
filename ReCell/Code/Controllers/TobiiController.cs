using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using Tobii.TecSDK.Client.Presentation;
using Recellection.Code.Models;
using Tobii.TecSDK.Client.Utilities;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;
using Tobii.TecSDK.Client.Interaction.Presentation;
using Tobii.TecSDK.Client.Interaction;
using Tobii.TecSDK.Core.Interaction;
using Tobii.TecSDK.Core.Utilities;
using Interaction = Tobii.TecSDK.Client.Utilities.Interaction;
using Recellection.Code.Utility.Logger;
using System.Drawing;
using Microsoft.Xna.Framework.Input;

namespace Recellection
{
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
        private const int DEFAULT_TIME_SPAN = 1;
        private static Logger logger = LoggerFactory.GetLogger();
        private GUIRegion newActivatedRegion = null;
        private static TobiiController _instance = null;
        private GUIRegion left = null, right = null, top = null, bot = null;       
        
        KeyboardState lastKBState, kBState;

        /// <summary>
        /// Main constructor for the controller,
        /// Remember to call Init or the controller will not work.
        /// </summary>
        private TobiiController()
        {       
            logger.Info("Created the Tobii Controller");
        }

        /// <summary>
        /// Provides singleton functionality
        /// </summary>
        /// <param name="xnaHandle"></param>
        /// <returns></returns>
        public static TobiiController GetInstance(IntPtr xnaHandle)
        {
            if (_instance == null)
            {
                //the xnaHandle was not used, so it has been removed
                //the constructor still takes it to not mess with other peoples code
                _instance = new TobiiController();
            }
            return _instance;
        }

        ///<summary>
        ///Initializes the Tobii Controller
        /// </summary>
        ///<returns>
        ///false if for any reason the initialization did not complete successfully, true otherwise.
        ///</returns>
        public bool Init()
        {
            if (!TecClient.IsInitialized)
            {
                try
                {
                    TecClient.Init("Recellection");

                    #region I'm too scared to remove these comments
                    Tobii.TecSDK.Core.Interaction.Contracts.UserProfileProperties newProfile = new Tobii.TecSDK.Core.Interaction.Contracts.UserProfileProperties("RecellectionProfile");
                    Tobii.TecSDK.Client.Utilities.UserProfile.Add("RecellectionProfile", "RecellectionProfile");
                    Tobii.TecSDK.Client.Utilities.UserProfile.SetCurrent("RecellectionProfile");
                    Tobii.TecSDK.Client.Utilities.UserProfile.Current.FeedbackSettings = new Tobii.TecSDK.Core.Interaction.Contracts.FeedbackSettings();
                    Tobii.TecSDK.Client.Utilities.UserProfile.Current.Enabled = true;

                    #endregion

                    //these lines will, it seems, create a new ClientApplicationProperties object
                    //based on the current client settings and current user profile.
                    //the new object sets OffWindowProcessing
                    //**there is probably an easier way to go about this**
                    //**have to wait and see if this will mess with the UserProfile outside of our software**
                    Tobii.TecSDK.Core.Interaction.Contracts.ClientApplicationProperties props = 
                        new Tobii.TecSDK.Core.Interaction.Contracts.ClientApplicationProperties
                            (TecClient.ClientSettings, Tobii.TecSDK.Client.Utilities.UserProfile.Current);
                    
                    props.OffWindowProcessing = true;                    
                    props.Enabled = true;
                    
                    //we copy our newly created ClientApplicationProperties object to our CurrentApplicationProfile
                    //the change is then applies by calling UpdateApplicationProfile
                    TecClient.CurrentApplicationProfile.CopyProperties(props);                    
                    TecClient.CurrentApplicationProfile.Enabled = true;
                    TecClient.UpdateApplicationProfile();
                    
                    TecClient.ClientSettings.OffWindowProcessing = true;
                    TecClient.ClientSettings.ApplySettings();
                    TecClient.SettingsManager.ApplySettings();

                    SetFeedbackColor(Microsoft.Xna.Framework.Graphics.Color.White);
                }
                catch (Exception)
                {
                    logger.Warn("The Tobii Controller did not initialize correctly");
                    return false;
                }

                #region uncomment me to test offscreen regions

                    //GUIRegion test = new GUIRegion(IntPtr.Zero, new Rect(1280, 0, 500, 1024));
                    //AddRightOffScreen(test);

                #endregion

                logger.Info("Successfully initialized the Tobii Controller");
                return true;
            }
            else
            {
                //the controller was already initialized
                return true;
            }
        }

        //keeping this function to quickly test, if needed, that off screen regions still work
        void test_Activate(object sender, Tobii.TecSDK.Client.Interaction.ActivateEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Enable/disable all currently loaded regions
        /// false will disable, true will enable
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="value"></param>       
        public void SetRegionsEnabled(bool value)
        {
            foreach (IInteractionRegion region in Interaction.Regions.Values)
            {
                region.Enabled = value;
            }        
        }

        //We'll just do them like this for now
        private void AddLeftOffScreen(GUIRegion left)
        {
            this.left = left;
            AddRegion(left);
            this.left.Enabled = true;
        }
        private void AddRightOffScreen(GUIRegion right)
        {
            this.right = right;
            AddRegion(this.right);
            this.right.Enabled = true;
        }
        private void AddTopOffScreen(GUIRegion top)
        {
            this.top = top;
            AddRegion(this.top);
            this.top.Enabled = true;
        }
        private void AddBotOffScreen(GUIRegion bot)
        {
            this.bot = bot;
            AddRegion(this.bot);
            this.bot.Enabled = true;     
        }

        /// <summary>
        /// Reads the GUIRegions from the Menu parameter
        /// and makes sure they get tracked.
        /// </summary>
        /// <param name="menu"></param>
        public void LoadMenu(Menu menu)
        {
            //Interaction.Regions.Clear();    // This seems to break, at least on my laptop.          
            foreach(GUIRegion region in menu.GetRegions())
            {
                AddRegion(region);
            }
            if (menu.leftOff != null)
            {
                AddLeftOffScreen(menu.leftOff.region);
            }
            if (menu.rightOff != null)
            {
                AddRightOffScreen(menu.rightOff.region);
            }
            if (menu.botOff != null)
            {
                AddTopOffScreen(menu.botOff.region);
            }
            if (menu.topOff != null)
            {
                AddBotOffScreen(menu.topOff.region);
            }
        }

        /// <summary>
        /// Unloads a Menu, preferably only provide this
        /// function with the menu that is currently loaded
        /// </summary>
        /// <param name="menu"></param>
        public void UnloadMenu(Menu menu)
        {            
            foreach (GUIRegion region in menu.GetRegions())
            {
                Interaction.RemoveRegion(region.RegionIdentifier);
            }
            if (menu.leftOff != null)
            {
                Interaction.RemoveRegion(menu.leftOff.region.RegionIdentifier);
            }
            if (menu.rightOff != null)
            {
                Interaction.RemoveRegion(menu.rightOff.region.RegionIdentifier);
            }
            if (menu.botOff != null)
            {
                Interaction.RemoveRegion(menu.botOff.region.RegionIdentifier);
            }
            if (menu.topOff != null)
            {
                Interaction.RemoveRegion(menu.topOff.region.RegionIdentifier);
            }
        }       

        /// <summary>
        /// Attempts to add a region
        /// Note that a region will be disabled by default
        /// </summary>
        /// <param name="newRegion"></param>
        /// <returns>
        /// will return an identifier to your newly created region
        /// should you want to fiddle with it later
        /// </returns>
        private WindowBoundInteractionRegionIdentifier AddRegion(GUIRegion newRegion)
        {                
                newRegion.CanActivate = true;
                if (newRegion.DwellTime == null)
                {
                    newRegion.DwellTime = new TimeSpan(0, 0, DEFAULT_TIME_SPAN);
                }
                newRegion.Enabled = false;

                //TODO: Figure out what these calls does
                //newRegion.AlwaysInteractive = true; 
                //newRegion.IsActivating = true; 
                //newRegion.UsesCoordinate = true; 

            try
            {
                logger.Info("about to add a new region");
                Interaction.AddRegion(newRegion); //this is what actually makes tobii start tracking our region
            }
            catch (Exception) //will occur if the region was already added, Interaction.AddRegion can throw stuff
            {
                logger.Warn("Failed to add a new region, region was most likely already added");
                return null;
            }
            logger.Info("Successfully added a new region");
            newRegion.regionActivated += newRegion_regionActivated;
            return newRegion.RegionIdentifier;
        }

        // the TobiiController subscribes to all regions Activate event
        void newRegion_regionActivated(object publisher, global::Recellection.Code.Utility.Events.Event<GUIRegion> ev)
        {
            newActivatedRegion = (GUIRegion)publisher;
        }

        /// <summary>
        /// Blocking function that will, eventually, return an Event
        /// consisting of the GUIRegion that Published the event, and any EventArgs
        /// </summary>
        /// <returns></returns>
        public GUIRegion GetActivatedRegion()
        {
            for (; ; )
            {
                System.Threading.Thread.Sleep(200); // so I heard you like hogging cpu time                
                lastKBState = kBState;
                kBState = Recellection.publicKeyBoardState;
#if DEBUG
                if (kBState.IsKeyDown(Keys.W) && lastKBState.IsKeyUp(Keys.W))
                {
                    if (top != null)
                    {
                        top.Publish(top, new global::Recellection.Code.Utility.Events.EventType());
                    }
                }
                else if(kBState.IsKeyDown(Keys.S) && lastKBState.IsKeyUp(Keys.S))
                {
                    if (bot != null)
                    {                    
                        bot.Publish(bot, new global::Recellection.Code.Utility.Events.EventType());
                    }
                }
                else if(kBState.IsKeyDown(Keys.A) && lastKBState.IsKeyUp(Keys.A))
                {
                    if (left != null)
                    {
                        left.Publish(left, new global::Recellection.Code.Utility.Events.EventType());
                    }
                        
                }
                else if(kBState.IsKeyDown(Keys.D) && lastKBState.IsKeyUp(Keys.D))
                {
                    if (right != null)
                    {
                        right.Publish(right, new global::Recellection.Code.Utility.Events.EventType());
                    }
                }
#endif
                if (newActivatedRegion != null)
                {
                    GUIRegion temp = newActivatedRegion;
                    newActivatedRegion = null;
                    return temp;
                }
            }
        }

        /// <summary>
        /// change color of the dwell indicator
        /// </summary>
        /// <param name="color">Microsoft.Xna.Framework.Graphics.Color</param>
        public void SetFeedbackColor(Microsoft.Xna.Framework.Graphics.Color color)
        {
            System.Windows.Media.Color col = new System.Windows.Media.Color();
            col.A = color.A;
            col.B = color.B;
            col.G = color.G;
            col.R = color.R;
            TecClient.CurrentApplicationProfile.FeedbackSettings.DwellFeedbackColor = col;
            TecClient.UpdateApplicationProfile();
        }
    }
}