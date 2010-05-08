using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Recellection.Code.Views
{
    public interface IRenderable
    {
        List<DrawData> GetDrawData(ContentManager content);
    }
}
