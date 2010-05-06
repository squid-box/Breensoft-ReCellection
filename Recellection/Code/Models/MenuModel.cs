using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Models
{
    public class MenuModel : IModel
	{
		#region Singleton-stuff

		// from http://www.yoda.arachsys.com/csharp/singleton.html
		static MenuModel instance = null;
		static readonly object padlock = new object();

		public static MenuModel Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new MenuModel();
					}
					return instance;
				}
			}
		}

		#endregion

        private Stack<Menu> menuStack;
        private Dictionary<Menu, Vector2> menuPositions;

        public event Publish<Menu> MenuEvent;
        public event Publish<Menu> MenuClearedEvent;

		private MenuModel()
		{
		}
		
        /// <summary>
        /// check which menu is on top of the stack
        /// and return it without popping.
        /// i.e the "current menu"
        /// </summary>
        /// <returns></returns>
        public Menu Peek()
        {
            Menu m = menuStack.Peek();
            return m;
        }

        /// <summary>
        /// Pop a menu that is no longer needed on the stack
        /// </summary>
        /// <returns></returns>
        public Menu Pop()
        {
            Menu m = menuStack.Pop();
            if (MenuEvent != null)
            {
                MenuEvent(this, new Event<Menu>(m, EventType.REMOVE));
            }
            return m;
        }

        /// <summary>
        /// Push a new menu onto the stack
        /// </summary>
        /// <param name="m"></param>
        public void Push(Menu m)
        {
            menuStack.Push(m);
            if (MenuEvent != null)
            {
                MenuEvent(this, new Event<Menu>(m, EventType.ADD));
            }
        }

        /// <summary>
        /// clears the entire stack
        /// probably not really needed
        /// </summary>
        public void Clear()
        {
            menuStack.Clear();
            if (MenuClearedEvent != null)
            {
                MenuClearedEvent(this, new Event<Menu>(null, EventType.REMOVE));
            }

        }

        /// <summary>
        /// Gets the position of the menu provided
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Vector2 GetPosition(Menu m)
        {
            return menuPositions[m];
        }
    }
}
