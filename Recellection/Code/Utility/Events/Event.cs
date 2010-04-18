using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
	/// <summary>
	/// Different types of events, might have to be altered at a later stage.
	/// </summary>
	public enum EventType
	{
		ADD, REMOVE, ALTER
	}
	
	/// <summary>
	/// The base class for all events in the application.
	/// 
	/// Author: Martin Nycander
	/// </summary>
	/// <typeparam name="T">The type of object which is updated.</typeparam>
	public class Event<T> : EventArgs where T : IModel
	{
		
		/// <summary>
		/// The object responsible for generating the event.
		/// </summary>
		public T subject { get; protected set; }
		
		/// <summary>
		/// The type of event.
		/// </summary>
		public EventType type { get; protected set; }
		
		/// <summary>
		/// Constructor, initializes internals.
		/// </summary>
		/// <param name="subject">The object responsible for generating the event.</param>
		/// <param name="type">The type of event.</param>
		public Event(T subject, EventType type)
		{
			this.subject = subject;
			this.type = type;
		}
	}
}
