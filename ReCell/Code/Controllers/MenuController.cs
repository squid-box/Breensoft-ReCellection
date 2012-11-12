namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
	/// Provides functionality to other controllers to access a menu.
	/// 
	/// Author: Martin Nycander
	/// </summary>
	public class MenuController
    {
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        private static bool initiated;

        private static MenuModel menuModel;
		private static TobiiController tobiiController;

        #endregion

        #region Constructors and Destructors

        /// <summary>
		/// Initiates the MenuController with an empty menu model and the provided tobii controller.
		/// </summary>
		/// <param name="tobii">The tobii controller associated with the application.</param>
		/// <param name="initialMenu">The opening menu</param>
		/// <exception cref="InvalidOperationException">If the MenuController is instantiated a second time.</exception>
		public MenuController(TobiiController tobii, Menu initialMenu)
		{
			if (initiated)
			{
				throw new InvalidOperationException("The MenuController has already been initiated.");
			}

            logger.Info("Initializing MenuController.");
			menuModel = MenuModel.Instance;
            logger.Info("Got " + tobii +" from constructor.");
			tobiiController = tobii;
            logger.Info("MenuController.tobiiController is now: "+ tobiiController +".");
			initiated = true;
			
			LoadMenu(initialMenu);
		}

        #endregion

        #region Public Methods and Operators

        public static void DisableMenuInput()
		{
			tobiiController.SetRegionsEnabled(false);
		}

        /// <summary>
		/// Gets input from the Tobii controller.
		/// </summary>
		/// <returns>An activated Region in the current menu</returns>
		public static MenuIcon GetInput()
		{
			while (true)
			{

				tobiiController.SetRegionsEnabled(true);
				GUIRegion activated = tobiiController.GetActivatedRegion();
				tobiiController.SetRegionsEnabled(false);

				// tobiiController.UnloadMenu(menuModel.Peek());
				Menu m = menuModel.Peek();
				List<MenuIcon> options = m.GetIcons();
				foreach (MenuIcon mi in options)
				{
					if (mi.region.RegionIdentifier == activated.RegionIdentifier)
						return mi;
				}

				if (m.leftOff != null && m.leftOff.region.RegionIdentifier == activated.RegionIdentifier)
				{
					return m.leftOff;
				}
				else if (m.rightOff != null && m.rightOff.region.RegionIdentifier == activated.RegionIdentifier)
				{
					return m.rightOff;
				}
				else if (m.topOff != null && m.topOff.region.RegionIdentifier == activated.RegionIdentifier)
				{
					return m.topOff;
				}
				else if (m.botOff != null && m.botOff.region.RegionIdentifier == activated.RegionIdentifier)
				{
					return m.botOff;
				}
			}
		}

        /// <summary>
        /// Opens up and loads a specified menu.
        /// </summary>
        /// <param name="m">The menu to load.</param>
        public static void LoadMenu(Menu m)
        {
            // for the case where there is nothing on the stack yet
            if (menuModel.Peek() != null)
            {
                tobiiController.UnloadMenu(menuModel.Peek());
            }

            menuModel.Push(m);
            tobiiController.LoadMenu(m);
        }

        /// <summary>
        /// Removes the currently opened menu, and reveals the one below.
        /// </summary>
        public static void UnloadMenu()
        {
            tobiiController.UnloadMenu(menuModel.Pop());
            
            if (menuModel.Peek() != null)
            {			
                tobiiController.LoadMenu(menuModel.Peek());
            }
        }

        #endregion
    }
	
	public class NonExistantInputException : Exception
	{
	}
}
