namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// The base fromBuilding class serves the purpose of keeping track of 
    /// all the other buildings associated with it.
    /// A base fromBuilding should never in this way be connected to another base fromBuilding
    /// 
    /// Author: Viktor Eklund
    /// </summary>
    public class BaseBuilding : Building{
        #region Constants

        private const int BASE_PRODUCTION = 5;

        #endregion

        #region Fields

        private readonly LinkedList<Building> childBuildings;

        private Logger logger = LoggerFactory.GetLogger();

        private int rateOfProduction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new base fromBuilding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="owner"></param>
        public BaseBuilding(string name, int posX, int posY, Player owner, LinkedList<Tile> controlZone)
               :base(name, posX, posY, BASE_BUILDING_HEALTH, owner, Globals.BuildingTypes.Base , null, controlZone)
        {
            this.type = Globals.BuildingTypes.Base;
            this.childBuildings = new LinkedList<Building>();
            this.baseBuilding = this;
            this.rateOfProduction = BASE_PRODUCTION;
        }

        #endregion

        #region Public Events

        public event Publish<Building> buildingsChanged;

        #endregion

        #region Public Properties

        public LinkedList<Building>.Enumerator ChildBuildings
        {
            get { return this.childBuildings.GetEnumerator(); }            
        }

        public int RateOfProduction
        {
            get { return this.rateOfProduction; }
            set { this.rateOfProduction = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The sprite!</returns>
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.BaseBuilding);
        }

        /// <summary>
        /// A fromBuilding may remove itself with itself as identifier
        /// </summary>
        /// <param name="fromBuilding"></param>
        public bool RemoveBuilding(Building building)
        {
            if (building.type == Globals.BuildingTypes.Resource)
            {
                var rb = (ResourceBuilding)building;
                this.rateOfProduction -= rb.RateOfProduction;
            }

            return this.childBuildings.Remove(building);
        }

        /// <summary>
        /// Allows any fromBuilding except a BaseBuilding to add itself to this basebuildings list of buildings
        /// </summary>
        /// <param name="fromBuilding"></param>
        public void Visit(Building building)
        {
            this.childBuildings.AddLast(building);
            if (this.buildingsChanged != null)
            {
                this.buildingsChanged(this, new BuildingAddedEvent(building, EventType.ADD));
            }
        }

        /// <summary>
        /// This function will prevent the real Visit function from being called
        /// with a base fromBuilding.
        /// </summary>
        /// <param name="fromBuilding"></param>
        public void Visit(BaseBuilding building)
        {
            throw new ArgumentException("A BaseBuilding should not be added as a child building to another BaseBuilding");
        }

        #endregion
    }
}
