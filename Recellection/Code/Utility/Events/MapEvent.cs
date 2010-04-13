using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
    class MapEvent : Event<Tile[, ]>
    {
        public int row {get ; private set;}
        public int col { get; private set;}

        public MapEvent(Tile[,] map, int row, int col, EventType type)
            : base(map, type)
        {

        }
    }
}
