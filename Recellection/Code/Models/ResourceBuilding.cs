using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
   
    public class ResourceBuilding : Building
    {
        private int rateOfProduction;

        public ResourceBuilding(String name, int posX, int posY, int maxHealth,
            Player owner, BaseBuilding baseBuilding)
            : base(name, posX, posY, maxHealth, owner, Globals.BuildingTypes.Resource, baseBuilding)
        {

        }

        /// <summary>
        /// Sets the producion rate of this ResourceBuilding in terms of units / turns
        /// </summary>
        /// <param name="rate"></param>
        public void SetProductionRate(int rate)
        {
            rateOfProduction = rate;
        }

        /// <summary>
        /// Gets the production rate of this ResourceBuilding in terms of units / turns
        /// </summary>
        /// <returns>
        /// ProductionRate as an int
        /// </returns>
        public int GetProductionRate()
        {
            return rateOfProduction;
        }

    }
}
