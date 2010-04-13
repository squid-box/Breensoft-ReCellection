using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;

namespace Recellection.Code.Models
{
    /*
     * klassen "WindowBoundInteractionRegion" representerar de regioner vi vill använda
     * den behöver i sin konstruktor en "WindowBoundInteractionRegionIdentifier" som i sin tur
     * behöver en IntPtr, som ska vara en handle till det XNA fönster vi kör i( i Recellection.cs så är det "this.Window.Handle") 
     * och en Rect för att definera sin position
     * Ovannämnda klasser finns i "Tobii.TecSDK.Client.Interaction.RegionImplementations"
     * andra paket kanske behövs
     * Det är inte mitt jobb att skriva den här klassen, men ovan är säkert till någon form av hjälp.
     *
     * */
    public class GUIRegion : WindowBoundInteractionRegion
    {
        public GUIRegion(WindowBoundInteractionRegionIdentifier id ):base(id){ 
        
        }
    }
}
