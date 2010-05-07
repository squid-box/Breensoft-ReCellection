using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Views
{
    public interface IRenderable
    {
        List<DrawData> GetDrawData();
    }
}
