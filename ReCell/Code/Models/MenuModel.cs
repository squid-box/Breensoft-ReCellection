namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Utility.Events;

    public class MenuModel : IModel
	{
        // from http://www.yoda.arachsys.com/csharp/singleton.html
        #region Static Fields

        static readonly object padlock = new object();

        static MenuModel instance;

        #endregion

        #region Fields

        private readonly Dictionary<Menu, Vector2> menuPositions;

        private readonly Stack<Menu> menuStack;

        #endregion

        #region Constructors and Destructors

        private MenuModel()
		{
            this.menuStack = new Stack<Menu>();
            this.menuPositions = new Dictionary<Menu, Vector2>();
		}

        #endregion

        #region Public Events

        public event Publish<Menu> MenuClearedEvent;

        public event Publish<Menu> MenuEvent;

        #endregion

        #region Public Properties

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

        #region Public Methods and Operators

        /// <summary>
        /// clears the entire stack
        /// probably not really needed
        /// </summary>
        public void Clear()
        {
            this.menuStack.Clear();
            if (this.MenuClearedEvent != null)
            {
                this.MenuClearedEvent(this, new Event<Menu>(null, EventType.REMOVE));
            }

        }

        /// <summary>
        /// Gets the position of the menu provided
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Vector2 GetPosition(Menu m)
        {
            return this.menuPositions[m];
        }

        /// <summary>
        /// check which menu is on top of the stack
        /// and return it without popping.
        /// x.e the "current menu"
        /// </summary>
        /// <returns></returns>
        public Menu Peek()
        {
            Menu m = null;
            
            try
            {
                m = this.menuStack.Peek();
            }
            catch (InvalidOperationException)
            {
                // nothing on the stack yet
                return m;
            }

            return m;
        }

        /// <summary>
        /// Pop a menu that is no longer needed on the stack
        /// </summary>
        /// <returns></returns>
        public Menu Pop()
        {
            Menu m = this.menuStack.Pop();
            if (this.MenuEvent != null && this.menuStack.Count() > 0)
            {
                this.MenuEvent(this, new Event<Menu>(this.menuStack.Peek(), EventType.REMOVE));
            }

            return m;
        }

        /// <summary>
        /// Push a new menu onto the stack
        /// </summary>
        /// <param name="m"></param>
        public void Push(Menu m)
        {
            this.menuStack.Push(m);
            if (this.MenuEvent != null)
            {
                this.MenuEvent(this, new Event<Menu>(m, EventType.ADD));
            }
        }

        #endregion
	}
}
