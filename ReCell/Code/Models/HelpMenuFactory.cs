using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    /// <summary>
    /// This is actually meant to be a very big class of very ugly code,
    /// will probably be pretty short, due to circumstances which lead to events.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    public static class HelpMenuFactory
    {
        public enum MenuType
        {
            Generic, Main, Options
        }
        
        /// <summary>
        /// Returns a help menu.
        /// </summary>
        public static Menu GetHelpMenu(MenuType type)
        {
            switch (type)
            {
                case(MenuType.Generic):
                {
                    return new Menu(Globals.MenuLayout.NineMatrix, CreateGenericHelpMenu(), "Generic help menu.");
                }
                case (MenuType.Main):
                {
                    return new Menu(Globals.MenuLayout.NineMatrix, CreateMainHelpMenu(), "Main Menu help menu.");
                }
                case (MenuType.Options):
                {
                    return new Menu(Globals.MenuLayout.NineMatrix, CreateOptionsHelpMenu(), "Options menu help menu.");
                }
            }

            throw new ArgumentException("What the hell did you just try?");
        }

        private static List<MenuIcon> CreateGenericHelpMenu()
        {
            List<MenuIcon> list = new List<MenuIcon>();

            // Create and add all buttons for menu.
            list.Add(new MenuIcon("This is a unit.",Recellection.textureMap.GetTexture(Globals.TextureTypes.Unit)));
            list.Add(new MenuIcon("This is a base building.", Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding)));
            list.Add(new MenuIcon("This is an offensive building.", Recellection.textureMap.GetTexture(Globals.TextureTypes.AggressiveBuilding)));
            list.Add(new MenuIcon("This is a defensive building.", Recellection.textureMap.GetTexture(Globals.TextureTypes.BarrierBuilding)));
            list.Add(new MenuIcon("This is a resource building.", Recellection.textureMap.GetTexture(Globals.TextureTypes.ResourceBuilding)));
            list.Add(new MenuIcon("This is a helpfull text which tells me what this button does."));
            list.Add(new MenuIcon("This is a helpfull text which tells me what this button does."));
            list.Add(new MenuIcon("Back", Recellection.textureMap.GetTexture(Globals.TextureTypes.No)));
            
            return list;
        }

        [Obsolete("Not yet implemented")]
        private static List<MenuIcon> CreateMainHelpMenu()
        {
            throw new NotImplementedException();
        }

        [Obsolete("Not yet implemented")]
        private static List<MenuIcon> CreateOptionsHelpMenu()
        {
            throw new NotImplementedException();
        }
    }
}
