using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Windows.Media.Geometry;
using Tobii.TecSDK.Client.Presentation;
using Recellection.Code.Models;
using Tobii.TecSDK.Client.Utilities;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;
using Tobii.TecSDK.Client.Interaction.Presentation;
using Tobii.TecSDK.Core.Interaction;
using Tobii.TecSDK.Core.Utilities;
using Interaction = Tobii.TecSDK.Client.Utilities.Interaction;

namespace Recellection.Code.Controllers
{
    class TobiiController
    {
        List<GUIRegion> regions;
        EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs> activationHandler;
        EventHandler<Tobii.TecSDK.Client.Interaction.RegionFocusEventArgs> focusHandler;
        IntPtr XNAHandle;

        //can only be instantiated with a handle to a window
        public TobiiController(IntPtr handle){
            XNAHandle = handle;
            Init();
        }
    
        /*
         * Initializes the Tobii Controller
         * returns false if for any reason the initialization did not
         * complete successfully, true otherwise.
         * */
        public bool Init(){
            try
            {
                TecClient.Init("Recellection");
               //probably more stuff to do here, like loading eye tracking preferences e.t.c
            }
            catch (Exception)
            {
                return false;
            }
            return true;        
        }

        //will return the currently focused region, if any.
        public GUIRegion GetRegion()
        {
            foreach(GUIRegion region in regions)
            {
                if (region.HasFocus)
                {
                    return region;
                }
            }
            return null;
        }

        //Attempts to add a region, will return false if the region was already added, otherwise true
        public bool AddRegion(GUIRegion newRegion)
        {
            try
            {
                Interaction.AddRegion(newRegion);
            }
            catch (Exception)
            {
                return false;
            }
            regions.Add(newRegion);
            return false;
        }

        public void Update()
        {
            //check all registered event handlers for all regions in the game
        }
    }
}



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






