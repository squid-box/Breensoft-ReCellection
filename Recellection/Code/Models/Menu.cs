
using System;
using System.Collections.Generic;
namespace Recellection
{
	/// <summary>
	/// author: co
	/// </summary>
	using System.Collections.Generic;


	public class Menu
	{
	    private List<MenuIcon> icons;
        private Texture2D menuPic;
		
		public Menu(Texture2D menuPic, List<MenuIcon> icons)
		{
			this.menuPic = menuPic;
			this.icons = icons;
		}
		
		public Menu(MenuLayout layout, List<MenuIcon> icons)
		{
			switch(layout)
			{
				case MenuLayout.Prompt:
					CreatePrompt(icons);
					break;
				case MenuLayout.NineMatrix:
					//code
					break;
				case MenuLayout.FourMatrix:
					//code
					break;
			}
		}
		private void CreatePrompt(List<MenuIcon> icons)
		{
			if (icons.Count != 2){
				throw new Exception("Wrong amount of icons in menu");				
			}
			icons[0].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, Recellection.viewPort.Width * 2 / 5, Recellection.viewPort.Height)));
			icons[1].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height)));
			this.icons = icons;
		}
	}
}
