
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
        private String explanation;

		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation)
		{
            this.explanation = explanation;
			switch (layout)
			{
				case Globals.MenuLayout.Prompt:
					CreatePrompt(icons);
					break;
				case Globals.MenuLayout.NineMatrix:
                    CreateNByMMatrix(3, 3, icons);
					break;
				case Globals.MenuLayout.FourMatrix:
                    CreateNByMMatrix(2, 2, icons);
					break;
			}
		}
		
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
		
		private void CreatePrompt(List<MenuIcon> icons)
		{
			if (icons.Count != 2){
				throw new ArgumentException("Wrong amount of icons in menu");				
			}
			menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.PromptMenu);
			icons[0] = new MenuIcon("Yes", null);
			icons[1] = new MenuIcon("No", null);
			icons[0].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, Recellection.viewPort.Width * 2 / 5, Recellection.viewPort.Height)));
			icons[1].setRegion(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height)));
			this.icons = icons;
		}

        /// <summary>
        /// This method sets the eye tracking region for a N*M matrix menu, the number
        /// of icons in the list shall be N*M.
        /// </summary>
        /// <param name="cols">The number of cols of the matrix menu</param>
        /// <param name="rows">The number of rows of the matrix menu</param>
        /// <param name="icons">The list of icons</param>
        private void CreateNByMMatrix(int cols, int rows, List<MenuIcon> icons)
        {
            if (icons.Count != cols*rows)
            {
                throw new ArgumentException("Wrong amount of icons in menu");
            }
            int iconWidth = (int)(Recellection.viewPort.Width / cols);
            int iconHeight = (int)(Recellection.viewPort.Height / rows);

            menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.ThreeByThreeMenu);
            for(int i = 0; i < cols*rows; i++)
            {
                icons[i].setRegion(new GUIRegion(Recellection.windowHandle, 
                    new System.Windows.Rect((i%cols)*iconWidth,(i/rows)*iconHeight,(1+(i%cols))*iconWidth,(1+(i/rows))*iconHeight)));
            }
            this.icons = icons;
        }
	}
}
