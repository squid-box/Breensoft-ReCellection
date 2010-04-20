using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// The purpose of the Unit Accountant is to insert new units into its
    /// graph as they are created by the buildings.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-19</date>
    public class UnitAccountant
    {
        private BaseBuilding[] world;
        
        public UnitAccountant()
        {

        }

        /// <summary>
        /// Get all Base buildings and have them produce units.
        /// </summary>
        private void ProduceUnits()
        {

        }

        /// <summary>
        /// Called by building. Adds units to a building?
        /// </summary>
        /// <param name="units">A list of units.</param>
        public void addUnits(Unit[] units)
        {

        }

        /// <summary>
        /// Unknown.
        /// </summary>
        private void ProduceUnits()
        {
            // for all BaseBuilding b in bases
                // for all graphs g in p
                    // for all buildings b in g.GetBuildings()
                        //b.AddUnits(b.Production())
        }
    }
}
