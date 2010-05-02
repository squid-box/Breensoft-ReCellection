using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// Handles state management for the GameOptions Model.
    /// 
    /// Author: Mattias Mikkola
    /// </summary>
    class Configurator
    {
        private void BuildMenu()
        {
            // Initialize a Menu.
            Menu menu = new Menu(Globals.MenuTypes.OptionsMenu, false);
            /// TODO: Add Options to menu.
            MenuController.LoadMenu(menu);
        }

        public void ChangeOptions()
        {
            BuildMenu();
            GUIRegion activatedRegion = MenuController.GetInput();
            
            /// There needs to be some way of distinguishing which menu item that triggered this region.
            /// Upon which the corresponding option in GameOptions will be changed using GameOptions.setOption().
        }

    }

}