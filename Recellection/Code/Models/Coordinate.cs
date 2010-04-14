using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    [Obsolete("This is utterly useless, use Vector2 instead. The world map will use floats anyway.")]
    class Coordinate : IModel
    {
        public int x;
        public int y;
        
    }
}
