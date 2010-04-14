using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    [Obsolete("This is retarded, use Vector2 instead. It won't correctly work either.")]
    class Coordinate : IModel
    {
        public int x;
        public int y;
        
    }
}
