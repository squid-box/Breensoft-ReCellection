using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

/**
 * 
 * Author: co
 * 
 **/

namespace Recellection.Code.Models
{
    class Menu : IModel
    {
        private List<GUIRegion> regions;
        private Texture2D menuPic;
        private Menu helpMenu; //om denna Menu är en helpmenu eller inte ska ha en helpMenu sets denna variabel till null duh.


        /**
         * 
         * min tanke är att varje meny ska vara ett Menu objekt. 
         * Denna klass kommer ha två konstruktorer den beskriver exakt hur menyn ska se ut och i den andra kan man välja bland några hårdkodade menyer.
         * Om du har synpunkter eller frågorom implementationen tveka inte att kontakta mig (co).
         * 
         * Jag tänker inte låta det vara möjligt att ändra menyer, man gör bara nya om så skulle behövas istället, om dett visar sig vara ineffektivt ändrar jag på det.
         * 
         * */


        //place holders, dem riktiga funktionerna ska faktiskt göra något :)
        public Menu()
        {
        }

        //här kommer menyerna hårdkodas, mest kod här
        public Menu(String menuType)
        {
        }


        //och så några get metoder:
        public List<GUIRegion> GetRegions()
        {
            return regions;
        }
        public Texture2D GetMenuPic()
        {
            return menuPic;
        }
        public Menu GetHelp() //seriously dude, you need help...
        {
            return helpMenu;
        }
        
    }
}
