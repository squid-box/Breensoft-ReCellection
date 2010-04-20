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
    /// Is a singleton class, only one object can be created!
    /// 
    /// Author: Joel Ahlgren
    /// Date: 2010-04-20
    /// 
    /// Signature: John Doe (yyyy-mm-dd)
    /// Signature: Jane Doe (yyyy-mm-dd)
    /// </summary>
    public sealed class UnitAccountant
    {
        #region Singleton-stuff

        // from http://www.yoda.arachsys.com/csharp/singleton.html
        static UnitAccountant instance = null;
        static readonly object padlock = new object();

        public static UnitAccountant Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new UnitAccountant();
                    }
                    return instance;
                }
            }
        }

        #endregion

        private HashSet<BaseBuilding> world;

        private UnitAccountant()
        {
            world = new HashSet<BaseBuilding>();
        }

        /// <summary>
        /// Called by building. Adds units to a building?
        /// </summary>
        /// <param name="units">A list of units.</param>
        public void addUnits(List<Unit> units)
        {
            foreach (Unit u in units)
            {
                // Din mamma
            }
        }

        /// <summary>
        /// Quite possibly a horribly slow way of adding units.
        /// </summary>
        private void ProduceUnits()
        {
            foreach (BaseBuilding b in world)
            {
                for (int i = 0; i < b.GetProductionRate(); i++)
                {
                    b.AddUnit(new Unit(b.posX,b.posY));
                }
            }
        }
    }
}
