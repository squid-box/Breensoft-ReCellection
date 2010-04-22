using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{

    /// <summary>
    /// The base building class serves the purpose of keeping track of 
    /// all the other buildings associated with it.
    /// A base building should never in this way be connected to another base building
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class BaseBuilding : Building 

        private int rateOfProduction;

        public int RateOfProduction
        {
            get { return rateOfProduction; }
            set { rateOfProduction = value; }
        }
        private LinkedList<Building> childBuildings;    
        public LinkedList<Building>.Enumerator ChildBuildings
        {
            get { return childBuildings.GetEnumerator(); }            
        }

		public event Publish<Building> buildingsChanged;

        /// <summary>
        /// Constructs a new base building
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="owner"></param>
        public BaseBuilding(String name, int posX, int posY,Player owner)
               :base(name, posX, posY, BASE_BUILDING_HEALTH,owner,Globals.BuildingTypes.Base , null)
        {
            this.type = Globals.BuildingTypes.Base;
            childBuildings = new LinkedList<Building>();
            baseBuilding = this;
        }

        /// <summary>
        /// Allows any building except a BaseBuilding to add itself to this basebuildings list of buildings
        /// </summary>
        /// <param name="building"></param>
        public void Visit(Building building)
        {
            childBuildings.AddLast(building);
            buildingsChanged(this, new BuildingAddedEvent(building,EventType.ADD));
        }

        /// <summary>
        /// This function will prevent the real Visit function from being called
        /// with a base building.
        /// </summary>
        /// <param name="building"></param>
        public void Visit(BaseBuilding building)
        {
            throw new ArgumentException("A BaseBuilding should not be added as a child building to another BaseBuilding");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding);
        }
    }
}
