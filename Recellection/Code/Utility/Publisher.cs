using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Utility
{
    
   /// <summary>
   /// The super class of all classes that implement the Publisher functionality.
   ///  
   /// An Publisher object may have a number of subscribed Observer objects which 
   /// are notified when changes are made.
   /// 
   /// Observers may define what method will be invoked upon notification by providing
   /// a delegate, but will default on the void Update() function required by the 
   /// Publisher interface.
   /// </summary>
   public class Publisher
    {
        
        /// <summary>
        /// The internal list of observers that have subscribed for updates on this Publisher.
        /// </summary>
        private Dictionary<IObserver, ObserverDelegate> observers;
        
        /// <summary>
        /// An ObserverDelegate wraps a function in an observable object, and allows for
        /// invokation of a specified method upon notification of changes.
        /// </summary>
        /// <param name="observable">The observable that will invokde this delegate</param>
        /// <param name="argument">The argument tha twill be passed on upon invokation</param>
        public delegate void ObserverDelegate(Publisher observable, Object argument);

        /// <summary>
        /// The constructor for Publisher objects. Intializes the internal dictionary
        /// of observers.
        /// </summary>
        public Publisher()
        {
            observers = new Dictionary<IObserver, ObserverDelegate>();
        }

        /// <summary>
        /// Allows an Observer o to subscribe for updates from this Publisher.
        /// Any Observers that subscribed using this method will have their 
        /// Update-method invoked when being notified.
        /// </summary>
        /// <param name="o">The observer to be added to the subscription list.</param>
        public void Subscribe(IObserver o)
        {
            ObserverDelegate m = new ObserverDelegate(o.Update);
            observers.Add(o, m);
        }

        
        /// <summary>
        /// Allows an IObserver o to subscribe for updates from this Publisher.
        /// Any IObservers that subscribes using this method provides 
        /// Update-method invoked when being notified.
        /// </summary>
        /// <param name="o">The Observer to be added to the list of subscribers</param>
        /// <param name="m">The ObserverDelegate to be associated with the Observer defining the 
        /// method to be invoked upon notification
        /// </param>
        public void Subscribe(IObserver o, ObserverDelegate m)
        {
            observers.Add(o, m);
        }

        /// <summary>
        /// Notify all subscribed Observers of a change
        /// </summary>
        /// <param name="argument">The argument to be passed to be observer</param>
        public void Notify(Object argument)
        {
            foreach (IObserver o in observers.Keys)
            {
                ObserverDelegate m = observers[o];
                m.Invoke(this, argument);
            }
        }

        /// <summary>
        /// Unsubscribe this observer from this observable object. Any unsubscribed Observer
        /// will henceforth recieve no notifications from this observer.
        /// </summary>
        /// <param name="o">The Observer to be removed</param>
        public void Unsubscribe(IObserver o)
        {
            observers.Remove(o);
        }
    }
}
