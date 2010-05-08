using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Views
{
    public class OptionsView : IRenderable
    {
        private Menu optionsMenu;

        private OptionsView()
        {
            
        }

        private void drawMenu()
        {
            MenuIcon one = new MenuIcon("one", null);
            MenuIcon two = new MenuIcon("two", null);
            MenuIcon three = new MenuIcon("three", null);
            MenuIcon four = new MenuIcon("four", null);

            List<MenuIcon> listOfMenuIcons = new List<MenuIcon>();
            listOfMenuIcons.Add(one);
            listOfMenuIcons.Add(two);
            listOfMenuIcons.Add(three);
            listOfMenuIcons.Add(four);
            optionsMenu = new Menu(Globals.MenuLayout.FourMatrix, listOfMenuIcons, "Options Menu");
        }
    }
}
