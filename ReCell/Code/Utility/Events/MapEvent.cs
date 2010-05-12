using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    class MapEvent : Event<World>
    {
        public int x { get; private set;}
        public int y { get; private set;}

        World.Map map;
        Tile tile;

        public MapEvent(World w, int x, int y, EventType type)
            : base(w, type)
        {
            this.x = x;
            this.y = y;
            map = w.GetMap();
            tile = map.GetTile(x, y);
        }
    }
}
