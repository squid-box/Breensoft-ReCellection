using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Utility
{
    public interface IObserver
    {
        void Update(Publisher observable, Object argument);
    }
}
