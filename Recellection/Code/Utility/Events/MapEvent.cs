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
        public int row { get; private set;}
        public int col { get; private set;}

        World.Map map;
        Tile tile;

        public MapEvent(World w, int row, int col, EventType type)
            : base(w, type)
        {
            this.row = row;
            this.col = col;
            map = w.GetMap();
            tile = map.GetTile(row, col);
        }
    }
}
