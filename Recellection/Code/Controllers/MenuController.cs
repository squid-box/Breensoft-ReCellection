using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
	/// <summary>
	/// Provides functionality to other controllers to access a menu.
	/// 
	/// Author: Martin Nycander
	/// </summary>
	public class MenuController
	{
		private static MenuModel menuModel;
		private static TobiiController tobiiController;
		private static bool initiated = false;
		
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
			
			menuModel = MenuModel.Instance;
			tobiiController = tobii;
			initiated = true;
			
			LoadMenu(initialMenu);
		}
		
		/// <summary>
		/// Opens up and loads a specified menu.
		/// </summary>
		/// <param name="m">The menu to load.</param>
		public static void LoadMenu(Menu m)
		{
			menuModel.Push(m);
			tobiiController.LoadMenu(m);
		}

		/// <summary>
		/// Removes the currently opened menu, and reveals the one below.
		/// </summary>
		public static void UnloadMenu()
		{
			menuModel.Pop();
			tobiiController.LoadMenu(menuModel.Peek());
		}

		/// <summary>
		/// Gets input from the Tobii controller.
		/// </summary>
		/// <returns>An activated Region in the current menu</returns>
		public static MenuIcon GetInput()
		{
			GUIRegion activated = tobiiController.GetActivatedRegion();
			List<MenuIcon> options = menuModel.Peek().GetIcons();
			foreach(MenuIcon mi in options)
			{
				if (mi.getRegion() == activated)
					return mi;
			}
			throw new NonExistantInputException();
		}
	}
	
	public class NonExistantInputException : Exception
	{
	}
}
