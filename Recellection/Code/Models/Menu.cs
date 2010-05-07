
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;


namespace Recellection
{
	/// <summary>
	/// author: co
	/// </summary>

	public class Menu
	{
	    private List<MenuIcon> icons;
        private Texture2D menuPic;
		private String text;
		
		
		public Menu(Texture2D menuPic, List<MenuIcon> icons)
		{
			this.menuPic = menuPic;
			this.icons = icons;
		}
		
		public List<MenuIcon> GetIcons()
		{
			return icons;
		}
		
		public Texture2D getMenuPic()
		{
			return menuPic;
		}
		
		public List<GUIRegion> GetRegions()
		{
			List<GUIRegion> regions = new List<GUIRegion>();
			foreach(MenuIcon mi in icons)
			{
				regions.Add(mi.getRegion());
			}
			return regions;
		}
		
		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String text)
		{
			this.text = text;
			switch(layout)
			{
				case Globals.MenuLayout.Prompt:
					CreatePrompt(icons);
					break;
				case Globals.MenuLayout.NineMatrix:
					//code
					break;
				case Globals.MenuLayout.FourMatrix:
					//code
					break;
			}
		}
		private void CreatePrompt(List<MenuIcon> icons)
		{
			if (icons.Count != 2){
				throw new Exception("Wrong amount of icons in menu");				
			}
			menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes(PromptMenu));
			icons[0] = new MenuIcon("Yes", null);
			icons[1] = new MenuIcon("No", null);
			icons[0].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, Recellection.viewPort.Width * 2 / 5, Recellection.viewPort.Height)));
			icons[1].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height)));
			this.icons = icons;
		}
	}
}
