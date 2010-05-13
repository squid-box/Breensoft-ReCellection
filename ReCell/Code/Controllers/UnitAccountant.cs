using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// The purpose of the Unit Accountant is to insert new units into its
    /// graph as they are created by the buildings.
    /// 
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-07</date>
    /// 
    /// Signature: John Doe (yyyy-mm-dd)
    /// Signature: Jane Doe (yyyy-mm-dd)
    public sealed class UnitAccountant
    {
        private Player owner;

        /// <summary>
        /// Constructs an UnitAccountant.
        /// </summary>
        /// <param name="owner">This UnitAccountant will belong to the player 'owner'.</param>
        public UnitAccountant(Player owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Called by building, adds units to a building.
        /// </summary>
        /// <param name="b">The building to add units to.</param>
        /// <param name="units">A list of units.</param>
        public void addUnits(Building b, List<Unit> units)
        {
            b.AddUnits(units);
        }

        /// <summary>
        /// Quite possibly a horribly slow way of adding units.
        /// </summary>
        public void ProduceUnits()
        {
            Random randomer = new Random();
            List<Unit> res;
            Unit temp;

            foreach (Graph g in owner.GetGraphs())
            {
                res = new List<Unit>();
                
                BaseBuilding b = g.baseBuilding;

                for (int i = 0; i < b.RateOfProduction; i++)
                {
                    // Places them randomly around the building. - John
                    // No, it does not. - Martin
                    
                    /*Vector2 coords = b.position;
                    coords.X += (float)randomer.NextDouble();
                    coords.Y += (float)randomer.NextDouble();*/

                    temp = new Unit(b.owner, b.position, b);
                    temp.targetEntity = b;

                    res.Add(temp);
                    
                    
                }
                b.AddUnits(res);
            }
        }
    }
}
