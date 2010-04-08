using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Media.Geometry;
using Tobii.TecSDK.Client.Presentation;
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
        //System.IO.StreamWriter myStream;
        List<GUIRegion>
        EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs> activationHandler;
        public TobiiController(IntPtr handle){
            
            try
            {
                TecClient.Init("Recellection");
                TecClient.IsMouseEngineOverrideEnabled = true;
                
                //Tobii.TecSDK.Client.Interaction;
                //Tobii.TecSDK.Client.Utilities.Interaction.
                //WindowsControl.Enabled = true;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(
                    "An error occured when trying to start the TecSDK client framework:{0}{0}{1}",
                    Environment.NewLine,
                    ex.Message
                );
                TecMessageBox.ShowError(errorMessage);
                return;
            }
            //WindowBoundInteractionRegionIdentifier testId = new WindowBoundInteractionRegionIdentifier(handle,new Rect(200,0,200,200));
            //WindowBoundInteractionRegion testWindow = new WindowBoundInteractionRegion(testId);
            //testWindow.Enabled = true;
            //testWindow.CanActivate = true;
            //testWindow.AlwaysInteractive = true;
            //activationHandler = new EventHandler<Tobii.TecSDK.Client.Interaction.ActivateEventArgs>(testWindow_Activate);
            //testWindow.Activate += activationHandler;
            //testWindow.DwellTime = new System.TimeSpan(0, 0, 2);
            //Interaction.AddRegion(testWindow);
            //Interaction.Regions.ElementAt(0).Value.Enabled = true;
            //Interaction.Regions.ElementAt(0).Value.CanActivate = true;
            //Interaction.Regions.ElementAt(0).Value.AlwaysInteractive = true;

        }

        public void testWindow_Activate(object sender, Tobii.TecSDK.Client.Interaction.ActivateEventArgs e)
        {
            if (activationHandler != null)
            {
                isActivated = !isActivated;
            }
        }

    
        /*
         * Initializes the Tobii Controller
         * returns false if for any reason the initialization did not
         * complete successfully, true otherwise.
         * */
        public bool Init(){
            
            return false;        
        }

        public bool listen()
        {
            return isActivated;                
        }
    }
}
