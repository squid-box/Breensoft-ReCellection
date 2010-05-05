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
    /// 
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-05</date>
    /// 
    /// Signature: John Doe (yyyy-mm-dd)
    /// Signature: Jane Doe (yyyy-mm-dd)
    public sealed class UnitAccountant
    {
        private Player owner;

        private UnitAccountant(Player owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Called by building. Adds units to a building?
        /// </summary>
        /// <param name="units">A list of units.</param>
        public void addUnits(Building b, List<Unit> units)
        {
            b.AddUnits(units);
        }

        /// <summary>
        /// Quite possibly a horribly slow way of adding units.
        /// </summary>
        private void ProduceUnits()
        {   
            foreach (Graph g in owner.GetGraphs())
            {
                BaseBuilding b = g.baseBuilding;

                for (int i = 0; i < b.RateOfProduction; i++)
                {
                    b.AddUnit(new Unit(b.owner,b.coordinates));
                }
            }
        }
    }
}
