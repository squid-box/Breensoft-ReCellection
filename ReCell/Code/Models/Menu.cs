
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
                    CreateJapaneseFlagLayout(icons);
					break;
				case Globals.MenuLayout.FourMatrix:
                    CreateSwitzerlandFlagLayout(icons);
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
		
		public Texture2D GetMenuPic()
		{
			return menuPic;
		}
		
		public List<GUIRegion> GetRegions()
		{
			List<GUIRegion> regions = new List<GUIRegion>();
			foreach(MenuIcon mi in icons)
			{
				regions.Add(mi.region);
			}
			return regions;
		}
		
		private void CreatePrompt(List<MenuIcon> icons)
		{
			if (icons.Count != 2){
				throw new ArgumentException("Wrong amount of icons in menu");				
			}
			menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.PromptMenu);

			icons[0].region = new GUIRegion(Recellection.windowHandle, 
                new System.Windows.Rect(0, 0, Recellection.viewPort.Width * 2 / 5, Recellection.viewPort.Height));
            icons[0].targetRectangle  = 
                new Microsoft.Xna.Framework.Rectangle(0, 0, Recellection.viewPort.Width * 2 / 5, Recellection.viewPort.Height);

			icons[1].region = new GUIRegion(Recellection.windowHandle, 
                new System.Windows.Rect(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            icons[1].targetRectangle =
                new Microsoft.Xna.Framework.Rectangle(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height);

			this.icons = icons;
		}

        /// <summary>
        /// This method sets the eye tracking region for a N*M matrix menu, the number
        /// of icons in the list shall be N*M.
        /// </summary>
        /// <param name="cols">The number of cols of the matrix menu</param>
        /// <param name="rows">The number of rows of the matrix menu</param>
        /// <param name="icons">The list of icons</param>
        private void CreateNByMMatrix(int cols, int rows, List<MenuIcon> icons,Globals.TextureTypes menuTexture)
        {
            if (icons.Count != cols*rows)
            {
                throw new ArgumentException("Wrong amount of icons in menu");
            }
            int iconWidth = (int)(Recellection.viewPort.Width / cols);
            int iconHeight = (int)(Recellection.viewPort.Height / rows);

            menuPic = Recellection.textureMap.GetTexture(menuTexture);
            for(int i = 0; i < cols*rows; i++)
            {
                icons[i].targetRectangle = new Microsoft.Xna.Framework.Rectangle((i % cols) * iconWidth, (i / rows) * iconHeight, iconWidth, iconHeight);

                icons[i].region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect((i%cols)*iconWidth,(i/rows)*iconHeight,iconWidth,iconHeight));
            }
            this.icons = icons;
        }

        private void CreateSwitzerlandFlagLayout(List<MenuIcon> icons)
        {
            if (icons.Count != 4)
            {
                throw new ArgumentException("Wrong amount of icons in menu");
            }

            int windowWidth = Recellection.viewPort.Width;
            int windowHeight = Recellection.viewPort.Height;

            menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.TwoByTwo);

            for (int i = 0; i < 4; i++)
            {
                icons[i].targetRectangle =
                    new Microsoft.Xna.Framework.Rectangle((i % 2) * (windowWidth * 3 / 5), (i / 2) * (windowHeight * 3 / 5), (windowWidth * 2 / 5), (windowHeight * 2 / 5));

                icons[i].region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect((i % 2) * (windowWidth * 3 / 5), (i / 2) * (windowHeight * 3 / 5), (windowWidth * 2 / 5), (windowHeight * 2 / 5)));
            }
            this.icons = icons;
        }

        private void CreateJapaneseFlagLayout(List<MenuIcon> icons)
        {
            if (icons.Count != 8)
            {
                throw new ArgumentException("Wrong amount of icons in menu");
            }
            int iconWidth = (int)(Recellection.viewPort.Width / 3);
            int iconHeight = (int)(Recellection.viewPort.Height / 3);

            menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.ThreeByThree);
            for (int i = 0; i < 9; i++)
            {
                if (i == 5)
                {
                    continue;
                }
                icons[i].targetRectangle =
                    new Microsoft.Xna.Framework.Rectangle((i % 3) * iconWidth, (i / 3) * iconHeight, iconWidth, iconHeight);

                icons[i].region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect((i % 3) * iconWidth, (i / 3) * iconHeight, iconWidth, iconHeight));
            }
        }
    }
}
