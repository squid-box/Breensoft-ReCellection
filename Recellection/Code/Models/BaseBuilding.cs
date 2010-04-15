using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    public class BaseBuilding : ResourceBuilding // note that I inherit ResourceBuilding,
    {                                            // this makes sense as a BaseBuilding 
        LinkedList<Building> childBuildings;     // will have it's own production

        BaseBuilding(String name, int posX, int posY, int maxHealth,
            Player owner)
            : base(name, posX, posY, maxHealth, owner, null)
        {
            this.type = Globals.BuildingTypes.Base;
        }

        /// <summary>
        /// allows a AggressiveBuilding to add itself to this basebuildings list of buildings
        /// </summary>
        /// <param name="building"></param>
        public void Visit(Building building)
        {
            childBuildings.AddLast(building);    
        }

        ///// <summary>
        ///// allows a ResourceBuilding to add itself to this basebuildings list of buildings
        ///// </summary>
        ///// <param name="building"></param>
        //public void Visit(ResourceBuilding building)
        //{
        //    childBuildings.AddLast(building);
        //}

        ///// <summary>
        ///// allows a BarrierBuilding to add itself to this basebuildings list of buildings
        ///// </summary>
        ///// <param name="building"></param>
        //public void Visit(BarrierBuilding building)
        //{
        //    childBuildings.AddLast(building);
        //}

        /// <summary>
        /// Don't do it! it will break!
        /// </summary>
        /// <param name="building"></param>
        public void Visit(BaseBuilding building){
            throw new DivideByZeroException("");
        }

        /// <summary>
        /// Gets an enumerator to this base buildings child buildings
        /// </summary>
        /// <returns>
        /// The enumerator to the child buildings
        /// </returns>
        public LinkedList<Building>.Enumerator GetBuildings()
        {
            return childBuildings.GetEnumerator();
        }
    }
}
