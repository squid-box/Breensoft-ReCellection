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
    public class Menu : IModel
    {
        private List<GUIRegion> regions;

        public List<GUIRegion> Regions
        {
            get { return regions; }
            set { regions = value; }
        }
        private Texture2D menuPic;

        public Texture2D MenuPic
        {
            get { return menuPic; }
            set { menuPic = value; }
        }
        private Menu helpMenu; //om denna Menu är en helpmenu eller inte ska ha en HelpMenu sets denna variabel till null duh.

        public Menu HelpMenu
        {
            get { return helpMenu; }
            set { helpMenu = value; }
        }


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
    }
}
