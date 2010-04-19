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

namespace Recellection
{
    /// <summary>
    /// The TobiiController serves the purpose of simplifying region creation and handling
    /// for our software.
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class TobiiController
    {        
        //utan ett internt litet uppslagsverk så har controllern ingen aning om vad den ska kontrollera för något =(
        Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>> regionCategories;

        private const int DEFAULT_TIME_SPAN = 1;
        private static Logger logger = LoggerFactory.GetLogger();
        IntPtr xnaHandle;


        /// <summary>
        /// Main constructor for the controller,
        /// 
        /// Important: only instantiate it once as it uses
        /// static classes from the tobii SDK
        /// 
        /// Remember to call Init or the controller will not work.
        /// </summary>
        public TobiiController(IntPtr xnaHandle)
        {
            this.xnaHandle = xnaHandle;
            regionCategories = new Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>>();
            logger.Info("Created the Tobii Controller");
        }

        ///<summary>
        ///Initializes the Tobii Controller
        /// </summary>
        ///<returns>
        ///false if for any reason the initialization did not complete successfully, true otherwise.
        ///</returns>
        public bool Init()
        {
            try
            {
                TecClient.Init("Recellection");
                //TODO: more stuff?, like loading eye tracking preferences e.t.c?
            }
            catch (Exception)
            {
                logger.Warn("The Tobii Controller did not initialize correctly");
                return false;
            }
            logger.Info("Successfully initialized the Tobii Controller");
            return true;
        }

        /// <summary>
        /// Enable/disable a group of regions specified by the region ID
        /// If more region IDs are wanted just add more to Globals.RegionCategories
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="value"></param>
        public void SetRegionsEnabled(Globals.RegionCategories regionID, bool value)
        {
            for (int i = 0; i < regionCategories[regionID].Count; i++)
            {
                Interaction.Regions[regionCategories[regionID].ElementAt(i)].Enabled = value;
            }
        }

        /// <summary>
        /// Will return a list of all the GUIRegions that are part of the specified category
        /// Will implement when a GUIRegion has a getter for, instead of inheriting from, 
        /// WindowBoundInteractionRegion
        /// </summary>
        /// <param name="regionID"></param>
        /// <returns>List of GUIRegion</returns>
        public List<GUIRegion> GetRegionsByCategory(Globals.RegionCategories regionID)
        {
            List<GUIRegion> ret = new List<GUIRegion>();
            for (int i = 0; i < regionCategories[regionID].Count; i++)
            {                
                ret.Add((GUIRegion)Interaction.FindRegion(regionCategories[regionID].ElementAt(i)));
            }
            return ret;
        }

        /// <summary>
        /// Will return a region given it's identifier        
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// The region asked for
        /// </returns>
        public GUIRegion GetRegionByIdentifier(WindowBoundInteractionRegionIdentifier id){
            return (GUIRegion)Interaction.Regions[id];
            //uncertain if this is castable.
        }

        #region subscribers, awaiting confirmation to remove
        /// <summary>
        /// This will, when it works, allow you to subsribe to an event from a region, given it's identifier
        /// Disabled for now
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private void SubscribeToActivation(WindowBoundInteractionRegionIdentifier id, EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs> handler)
        {
            //Interaction.FindRegion(id).Activate += handler;
        }

        /// <summary>
        /// Subscribe to events genereted by focus entering the specified region
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        private void SubscribeToFocusEnter(WindowBoundInteractionRegionIdentifier id,EventHandler<RegionFocusEventArgs> handler){
            Interaction.FindRegion(id).FocusEnter += handler;
        }

        /// <summary>
        /// Subscribe to events generated by focus leaving the specified region
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        private void SubscribeToFocusLeave(WindowBoundInteractionRegionIdentifier id, EventHandler<RegionFocusEventArgs> handler)
        {
            Interaction.FindRegion(id).FocusLeave += handler;
        }
        #endregion

        /// <summary>
        /// Attempts to add a region
        /// Note that a region will be disabled by default
        /// </summary>
        /// <param name="newRegion"></param>
        /// <returns>
        /// will return an identifier to your newly created region
        /// this can be used to find your region with 
        /// "GetRegionByIdentifer"
        /// should you want to fiddle with the GUIRegion later.
        /// </returns>
        public WindowBoundInteractionRegionIdentifier AddRegion(Rect pos,Globals.RegionCategories regionID)
        {
                GUIRegion newRegion = new GUIRegion(xnaHandle, pos);
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

            if(regionCategories.ContainsKey(regionID))
            {
                regionCategories[regionID].Add(newRegion.RegionIdentifier);
            }
            else
            {
                List<WindowBoundInteractionRegionIdentifier> temp = new List<WindowBoundInteractionRegionIdentifier>();
                temp.Add(newRegion.RegionIdentifier);
                regionCategories.Add(regionID, temp);
            }

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
            return newRegion.RegionIdentifier;
        }

        /// <summary>
        /// Odds are that we won't want to really remove a region (we can just disable them by category instead)
        /// But if one need to be removed then so be it
        /// </summary>
        /// <param name="id"></param>
        public void RemoveRegionByIdentifier(WindowBoundInteractionRegionIdentifier id) {
            Interaction.RemoveRegion(id); 
            //assuming nothing funky happens if trying to remove a non existing region
        }

        /// <summary>
        /// Awaiting confirmation
        /// </summary>
        /// <returns></returns>
        public List<GUIRegion> GetActivatedRegions()
        {
            //will implement if this function is needed here, 
            return null;           
        }
    }
}