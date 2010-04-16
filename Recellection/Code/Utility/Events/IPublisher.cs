using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
	public delegate void Publish<T, E>(Object publisher, E ev)
		where E : Event<T> where T : IModel;
}
