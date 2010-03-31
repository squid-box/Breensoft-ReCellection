using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Models
{
    class SpriteMap
    {
        Texture2D[] textures;

        public Texture2D GetTexture(Globals.Texture texture, int curFrame, 
            int size)
        {
            return new Texture(textures[(int)texture], 
                new Rectangle(curFrame * size, 0, size, size));
        }

    }
}
