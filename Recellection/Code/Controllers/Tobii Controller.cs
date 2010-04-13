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

namespace Recellection
{
    public class TobiiController
    {
        //debug prylar
        private bool DEBUG = true;
        private StreamWriter log_out = null;
        
        //utan ett internt litet uppslagsverk så har controllern ingen aning om vad den ska kontrollera för något =(
        //Det ser ut att finnas regiongrupper redan färdiga att använda från tobii
        //men jag fick det inte att fungera, kan hända att de var till för något annat.
        //TODO: senare bör den mappa mot en lista av GUIRegions
        Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>> regionCategories;

        /// <summary>
        /// Main constructor for the controller, 
        /// Important: only instantiate it once as it uses
        /// static classes from the tobii SDK
        /// Remember to call Init or the controller will not work.
        /// </summary>
        public TobiiController(){
            regionCategories = new Dictionary<Globals.RegionCategories,List<WindowBoundInteractionRegionIdentifier>>();
            #region debug
            if (DEBUG)
            {
                try
                {
                    log_out = new StreamWriter("tobiiController_logfile.txt");
                }
                catch (IOException exc)
                {
                    Console.WriteLine(exc.Message + "Cannot open file.");
                    return;
                }
                Console.SetOut(log_out);
            }
            #endregion
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
                TecClient.IsMouseEngineOverrideEnabled = true;
                //TecClient.BroadcastActivate();
                //probably more stuff to do here, like loading eye tracking preferences e.t.c
            }
            catch (Exception)
            {
                return false;
            }
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
            //THIS USED TO BE UGLY WITH IFS AND STUFF BUT I MADE IT SMARTSER Ü
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
                //TODO this row is just waiting to be finished, oh yes
                //ret.Add(Interaction.FindRegion(regionCategories[regionID].ElementAt(i)));
            }
            return ret;
        }

        public WindowBoundInteractionRegion GetRegionByIdentifier(WindowBoundInteractionRegionIdentifier id){
            return (WindowBoundInteractionRegion)Interaction.Regions[id];
        }

        #region subscribers, wrong place for these though?
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
        /// </summary>
        /// <param name="newRegion"></param>
        /// <returns>
        /// will return false if the region was already added, otherwise true
        /// </returns>
        public bool AddRegion(GUIRegion newRegion,Globals.RegionCategories regionID)
        {
            #region doing stuff with the GUIRegion

            //TODO will have to change to something like
                //newRegion.GetWindowBoundInteractionRegion
                newRegion.CanActivate = true;
                newRegion.DwellTime = new TimeSpan(0, 0, 1);
                newRegion.Enabled = false;
                newRegion.AlwaysInteractive = true; //what does this call really mean?
                //newRegion.IsActivating = true; //and this one?
                newRegion.UsesCoordinate = true; //and this one?

                //newRegion.FocusEnter += new EventHandler<RegionFocusEventArgs>(newRegion_FocusEnter);
                //newRegion.FocusLeave += new EventHandler<RegionFocusEventArgs>(newRegion_FocusLeave);
                //newRegion.Activate += new EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs>(newRegion_Activate);

            #endregion

            if(regionCategories.ContainsKey(regionID))
            {
                regionCategories[regionID].Add(newRegion.RegionIdentifier);
            }else
            {
                List<WindowBoundInteractionRegionIdentifier> temp = new List<WindowBoundInteractionRegionIdentifier>();
                temp.Add(newRegion.RegionIdentifier);
                regionCategories.Add(regionID, temp);
            }
            try
            {
                Interaction.AddRegion(newRegion); //this is what actually makes tobii start tracking our region
            }
            catch (Exception e) //will occur if the region was already added, Interaction.AddRegion can throw stuff
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Odds are that we won't want to really remove a region (we can just disable them by category instead)
        /// But if one need to be removed then so be it
        /// </summary>
        /// <param name="id"></param>
        public void RemoveRegionsByIdentifier(WindowBoundInteractionRegionIdentifier id) {
            Interaction.RemoveRegion(id);
        }

        #region dummymethods
        //this method is a dummy and won't be needed
        private void newRegion_Activate(object sender, Tobii.TecSDK.Client.Interaction.ActivateEventArgs e)
        {
            Console.WriteLine("Activated");
        }

        //this method is a dummy and won't be needed
        private void newRegion_FocusEnter(object sender, RegionFocusEventArgs e)
        {
            GUIRegion s = (GUIRegion)sender;
            Console.Write("hello from: ");
            Console.WriteLine(s.Name);
        }

        /// <summary>
        /// this method is a dummy and won't be needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newRegion_FocusLeave(object sender, RegionFocusEventArgs e)
        {
            Console.WriteLine("good bye");

        }

        #endregion

        /// <summary>
        /// this function will return null, checking for activated regions does not work yet
        /// will this be handled elsewhere?
        /// </summary>
        /// <returns></returns>
        public GUIRegion GetActivatedRegions()
        {
            return null;           
        }
    }
}
#region old code stuffs
//Wrong place for this =(
///// <summary>
///// will return the currently focused region, if any.
///// does not currently work
///// </summary>
///// <returns>
///// The currently focused GUIRegion
///// </returns>
//public GUIRegion GetFocusedRegion()
//{
//    for(int i = 0; i < Interaction.Regions.Values.Count; i++)
//    {
//        if (Interaction.Regions.Values.ElementAt(i).HasFocus)
//        {
//            return Interaction.Regions.Values.ElementAt(i);
//        }
//    }
//    return null;
//}

//WindowBoundInteractionRegionIdentifier testId = new WindowBoundInteractionRegionIdentifier(XNAHandle, new Rect(0, 0, 200, 200));
//WindowBoundInteractionRegion testWindow = new WindowBoundInteractionRegion(testId);
//testWindow.Enabled = true;
//testWindow.CanActivate = true;
//testWindow.AlwaysInteractive = true;
//testWindow.DwellTime = new System.TimeSpan(0, 0, 2);
//Interaction.AddRegion(testWindow);
//Interaction.Regions.ElementAt(0).Value.Enabled = true;
//Interaction.Regions.ElementAt(0).Value.CanActivate = true;
//Interaction.Regions.ElementAt(0).Value.AlwaysInteractive = true;
#endregion






