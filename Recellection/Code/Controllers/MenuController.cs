using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
	public class MenuController
	{
		private static Menu menuModel;
		private static TobiiController tobiiController;
		private static bool initiated = false;
		
		public static void Initiate(Menu model, TobiiController tobii)
		{
			menuModel = model;
			tobiiController = tobii;
			initiated = true;
		}
		
		// Lägg en meny på stacken
		public static void LoadMenu(Menu m)
		{
			// TODO: menuModel.Push(m);
			//menuModel.
			tobiiController.LoadMenu(m);
		}

		// Ta bort menyn överst på stacken
		public static void UnloadMenu()
		{
			// TODO: menuModel.Pop();
			// TODO: tobiiController.LoadMenu(menuModel.Peek());
		}

		/// <summary>
		/// Gets input from the Tobii controller
		/// </summary>
		/// <returns>An activated Region in a menu</returns>
		public static GUIRegion GetInput()
		{
			return tobiiController.GetActivatedRegion();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void ClearMenu()
		{
			// TODO: menuModel.Clear();
			// TODO: tobiiController.UnloadMenu();
		}
	}
}
