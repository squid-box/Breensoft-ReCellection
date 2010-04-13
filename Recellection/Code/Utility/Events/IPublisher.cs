using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
	public delegate void Publish<T>(Object publisher, Event<T> ev) where T : IModel;
}
