using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Views
{
    public interface IDrawable
    {
        List<DrawData> GetDrawData();
    }
}
