using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Util
{
    public interface IObserver
    {
        void Update(Observable observable, Object argument);
    }
}
