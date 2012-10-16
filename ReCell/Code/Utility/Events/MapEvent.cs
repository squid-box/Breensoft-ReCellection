namespace Recellection.Code.Utility.Events
{
    using global::Recellection.Code.Models;

    class MapEvent : Event<World>
    {
        #region Fields

        readonly World.Map map;
        Tile tile;

        #endregion

        #region Constructors and Destructors

        public MapEvent(World w, int x, int y, EventType type)
            : base(w, type)
        {
            this.x = x;
            this.y = y;
            this.map = w.GetMap();
            this.tile = this.map.GetTile(x, y);
        }

        #endregion

        #region Public Properties

        public int x { get; private set;}
        public int y { get; private set;}

        #endregion
    }
}
