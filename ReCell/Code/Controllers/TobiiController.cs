﻿using System;
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
    /// Signature: Marco Ahumada Juntunen (2010-05-06)
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public sealed class TobiiController
    {        
        //utan ett internt litet uppslagsverk så har controllern ingen aning om vad den ska kontrollera för något =(
        private Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>> regionCategories;      
        private const int DEFAULT_TIME_SPAN = 1;
        private static Logger logger = LoggerFactory.GetLogger();
        private IntPtr xnaHandle;
        private GUIRegion newActivatedRegion = null;
        private static TobiiController _instance = null;

        /// <summary>
        /// Main constructor for the controller,
        /// 
        /// Important: only instantiate it once as it uses
        /// static classes from the tobii SDK
        /// 
        /// Remember to call Init or the controller will not work.
        /// </summary>
        private TobiiController(IntPtr xnaHandle)
        {
            this.xnaHandle = xnaHandle;
            regionCategories = new Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>>();
            logger.Info("Created the Tobii Controller");
        }

        public static TobiiController GetInstance(IntPtr xnaHandle)
        {
            if (_instance == null)
            {
                _instance = new TobiiController(xnaHandle);
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
        /// Enable/disable a group of Regions specified by the region ID
        /// If more region IDs are wanted just add more to Globals.RegionCategories
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="value"></param>
        [Obsolete("No longer supported functionality, use LoadMenu instead")]
        public void SetRegionsEnabled(Globals.RegionCategories regionID, bool value)
        {
            for (int i = 0; i < regionCategories[regionID].Count; i++)
            {
                Interaction.Regions[regionCategories[regionID].ElementAt(i)].Enabled = value;
            }
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

        }

        /// <summary>
        /// Will return a list of all the GUIRegions that are part of the specified category
        /// Will implement when a GUIRegion has a getter for, instead of inheriting from, 
        /// WindowBoundInteractionRegion
        /// </summary>
        /// <param name="regionID"></param>
        /// <returns>List of GUIRegion</returns>
        [Obsolete("GUIRegion categories are no longer supported, do not use")]
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
        [Obsolete("The way things work now, you most likely won't have a correct identifier, consider not using")]
        public GUIRegion GetRegionByIdentifier(WindowBoundInteractionRegionIdentifier id)
        {
            return (GUIRegion)Interaction.Regions[id];
            //uncertain if this is castable.
        }

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
        private WindowBoundInteractionRegionIdentifier AddRegion(Rect pos)
        {
            GUIRegion newRegion = new GUIRegion(xnaHandle, pos);
            return AddRegion(newRegion);
        }
        private WindowBoundInteractionRegionIdentifier AddRegion(GUIRegion newRegion)
        {                
                newRegion.CanActivate = true;
                if (newRegion.DwellTime == null)
                {
                    newRegion.DwellTime = new TimeSpan(0, 0, DEFAULT_TIME_SPAN);
                }
                newRegion.Enabled = true;

                //TODO: Figure out what these calls does
                //newRegion.AlwaysInteractive = true; 
                //newRegion.IsActivating = true; 
                //newRegion.UsesCoordinate = true; 

            
            /*
             * keeping this commented stub for now because things are undecided.
             */
            //if(regionCategories.ContainsKey(regionID))
            //{
            //    regionCategories[regionID].Add(newRegion.RegionIdentifier);
            //}
            //else
            //{
            //    List<WindowBoundInteractionRegionIdentifier> temp = new List<WindowBoundInteractionRegionIdentifier>();
            //    temp.Add(newRegion.RegionIdentifier);
            //    regionCategories.Add(regionID, temp);
            //}

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


        void newRegion_regionActivated(object publisher, global::Recellection.Code.Utility.Events.Event<GUIRegion> ev)
        {
            newActivatedRegion = (GUIRegion)publisher;
        }


        /// <summary>
        /// Odds are that we won't want to really remove a region (we can just disable them by category instead)
        /// But if one need to be removed then so be it
        /// </summary>
        /// <param name="id"><
        /// /param>
        [Obsolete("You probably don't want to make this call")]
        public bool RemoveRegionByIdentifier(WindowBoundInteractionRegionIdentifier id) {
           return Interaction.RemoveRegion(id);//throws exceptions if id did not exist               
            //assuming nothing funky happens if trying to remove a non existing region
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
                System.Threading.Thread.Sleep(10); // so I heard you like hogging cpu time
                if (newActivatedRegion != null)
                {
                    return newActivatedRegion;
                }
            }
        }
    }
}