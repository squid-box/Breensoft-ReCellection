
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using System.Text;


namespace Recellection
{
	/// <summary>
	/// author: co
	/// </summary>

	public class Menu
	{
        private const int FONT_SIZE = 33;
        private const int FONT_WIDTH = 20;
        private const int CHARS_PER_ROW = 15;
        
	    private List<MenuIcon> icons;
        private Texture2D menuPic;
        public String explanation { get; private set; }
        public Vector2 explanationDrawPos { get; private set; }
        public Color explanationColor { get; private set; }
        public MenuIcon leftOff { get; private set; }
        public MenuIcon rightOff { get; private set; }
        public MenuIcon topOff { get; private set; }
        public MenuIcon botOff { get; private set; }

		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation)
			 : this(layout, icons, explanation, Color.Black)
		{
		}
		
		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation, Color explanationColor)
		{
            this.explanation = insertLineBreaksForString(explanation);
            this.explanationColor = explanationColor;
            this.explanationDrawPos = new Vector2(Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2);
			switch (layout)
			{
				case Globals.MenuLayout.Prompt:
					CreatePrompt(icons);
					break;
				case Globals.MenuLayout.NineMatrix:
                    CreateNineMatrix(icons);
					break;
				case Globals.MenuLayout.FourMatrix:
                    CreateFourMatrix(icons);
					break;
			}
		}

        public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation, Color explanationColor, MenuIcon leftOff, MenuIcon rightOff, MenuIcon topOff, MenuIcon botOff)
        {
            this.explanation = insertLineBreaksForString(explanation);
            this.explanationColor = explanationColor;
            this.explanationDrawPos = new Vector2(Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2);
            this.leftOff = leftOff;
            this.rightOff = rightOff;
            this.topOff = topOff;
            this.botOff = botOff;
            switch (layout)
            {
                case Globals.MenuLayout.Prompt:
                    CreatePrompt(icons);
                    break;
                case Globals.MenuLayout.NineMatrix:
                    CreateNineMatrix(icons);
                    break;
                case Globals.MenuLayout.FourMatrix:
                    CreateFourMatrix(icons);
                    break;
            }
        }

        public Menu(List<MenuIcon> icons)


        {
            this.icons = icons;
        }


        public Menu(List<MenuIcon> icons, MenuIcon leftOff, MenuIcon rightOff, MenuIcon topOff, MenuIcon botOff)
        {
            this.leftOff = leftOff;
            this.rightOff = rightOff;
            this.topOff = topOff;
            this.botOff = botOff;
            this.icons = icons;
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

        private String insertLineBreaksForString(String text)
        {
            int lineBreaksToAdd = text.Length / CHARS_PER_ROW;

            StringBuilder buffer = new StringBuilder(text.Length);

            if (lineBreaksToAdd == 0)
            {
                return text;

            }
            String[] words = text.Split(' ');

            //Some line breaks are needed in this string.
            int i = 0;
            while (lineBreaksToAdd >= 0)
            {
                int charsLeftToAdd = CHARS_PER_ROW;
                while ( i < words.Length && charsLeftToAdd > words[i].Length )
                {
                    buffer.Append(words[i]);
                    buffer.Append(' ');

                    charsLeftToAdd -= words[i].Length;

                    i++;
                }
                
                buffer.Append('\n');
                lineBreaksToAdd--;
            }
            return buffer.ToString();
        }
		
		private void CreatePrompt(List<MenuIcon> icons)
		{
			if (icons.Count != 2){
				throw new ArgumentException("Wrong amount of icons in menu");				
			}
			menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.PromptMenu);

            for (int i = 0; i < 2; i++)
            {
                icons[i].region = new GUIRegion(Recellection.windowHandle,
                new System.Windows.Rect(i*Recellection.viewPort.Width * 3 / 5, 0, 
                    Recellection.viewPort.Width * 2 / 5 + i*Recellection.viewPort.Width* 3 / 5, Recellection.viewPort.Height));

                if ( icons[i].texture != null)
                {
                    icons[i].targetTextureRectangle =
                    new Microsoft.Xna.Framework.Rectangle(i*Recellection.viewPort.Width * 3 / 5+Recellection.viewPort.Width * 1 / 5 - icons[i].texture.Width / 2,
                        Recellection.viewPort.Height / 2 - icons[i].texture.Height / 2,
                        icons[i].texture.Width,
                        icons[i].texture.Height);
                }
                int textWidth = 0;
                if( icons[i].label != null)
                {
                    icons[i].label = insertLineBreaksForString(icons[i].label);

                    textWidth = icons[i].label.Length * FONT_WIDTH;

                    Vector2 temp = new Vector2(
                        i * Recellection.viewPort.Width * 3 / 5 + Recellection.viewPort.Width * 1 / 5, Recellection.viewPort.Height / 2);

                    icons[i].targetLabelRectangle =
                    new Microsoft.Xna.Framework.Rectangle((int)temp.X, (int)temp.Y, textWidth, FONT_SIZE);
                }

                if (icons[i].label != null && icons[i].texture != null)
                {

                    icons[i].targetTextureRectangle = new Rectangle(
                        icons[i].targetTextureRectangle.Location.X, icons[i].targetTextureRectangle.Location.Y - 40,
                        icons[i].texture.Width, icons[i].texture.Height);

                    icons[i].targetLabelRectangle = new Rectangle(
                        icons[i].targetLabelRectangle.Location.X, icons[i].targetLabelRectangle.Location.Y + icons[i].texture.Height - 15,
                        textWidth, FONT_SIZE);

                }
                
            }
			this.icons = icons;
		}

        /// <summary>
        /// This method sets the eye tracking region for a N*M matrix menu, the number
        /// of icons in the list shall be N*M.
        /// </summary>
        /// <param name="cols">The number of cols of the matrix menu</param>
        /// <param name="rows">The number of rows of the matrix menu</param>
        /// <param name="icons">The list of icons</param>
        private void FreeStyle(int cols, int rows, List<MenuIcon> icons,Globals.TextureTypes menuTexture, bool scrollZone)
        {
            if (icons.Count != cols*rows)
            {
                throw new ArgumentException("Wrong amount of icons in menu");
            }
            int iconWidth = (int)(Recellection.viewPort.Width / cols);
            int iconHeight = (int)(Recellection.viewPort.Height / rows);

            if (menuTexture != Globals.TextureTypes.NoTexture)
            {
                menuPic = Recellection.textureMap.GetTexture(menuTexture);
            }
            else
            {
                for (int i = 0; i < cols * rows; i++)
                {
                    icons[i].targetTextureRectangle = new Microsoft.Xna.Framework.Rectangle((i % cols) * iconWidth, (i / rows) * iconHeight, iconWidth, iconHeight);

                    icons[i].region = new GUIRegion(Recellection.windowHandle,
                        new System.Windows.Rect((i % cols) * iconWidth, (i / rows) * iconHeight, iconWidth, iconHeight));
                }
            }
            this.icons = icons;
        }

        private void CreateFourMatrix(List<MenuIcon> icons)
        {
            int windowWidth = Recellection.viewPort.Width;
            int windowHeight = Recellection.viewPort.Height;

            menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.TwoByTwo);

            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].texture != null)
                {
                    icons[i].targetTextureRectangle =
                        new Microsoft.Xna.Framework.Rectangle(
                            (i % 2) * (windowWidth * 3 / 5) + (windowWidth * 1 / 5) - icons[i].texture.Width / 2,
                            (i / 2) * (windowHeight * 3 / 5) + (windowHeight * 1 / 5) - icons[i].texture.Height / 2,
                            (icons[i].texture.Width),
                            (icons[i].texture.Height));

                }
                int textWidth = 0;
                if (icons[i].label != null)
                {
                    icons[i].label = insertLineBreaksForString(icons[i].label);

                   textWidth = icons[i].label.Length * FONT_WIDTH;

                    Vector2 temp = new Vector2(
                       (i % 2) * (windowWidth * 3 / 5) + (windowWidth * 1 / 5), (i / 2) * (windowHeight * 3 / 5) + (windowHeight * 1 / 5));

                    icons[i].targetLabelRectangle =
                        new Microsoft.Xna.Framework.Rectangle((int) temp.X, (int) temp.Y, (textWidth), (FONT_SIZE));
                }

                if (icons[i].label != null && icons[i].texture != null)
                {

                    icons[i].targetTextureRectangle = new Rectangle(
                        icons[i].targetTextureRectangle.Location.X, icons[i].targetTextureRectangle.Location.Y - 40,
                        icons[i].texture.Width, icons[i].texture.Height);

                    icons[i].targetLabelRectangle = new Rectangle(
                        icons[i].targetLabelRectangle.Location.X, icons[i].targetLabelRectangle.Location.Y + icons[i].texture.Height - 15,
                        textWidth, FONT_SIZE);

                }
                
                icons[i].region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect((i % 2) * (windowWidth * 3 / 5), (i / 2) * (windowHeight * 3 / 5), (windowWidth * 2 / 5), (windowHeight * 2 / 5)));
            }
            this.icons = icons;
        }

        private void CreateNineMatrix(List<MenuIcon> icons)
        {
            
            int iconWidth = (int)(Recellection.viewPort.Width / 3);
            int iconHeight = (int)(Recellection.viewPort.Height / 3);

            menuPic = Recellection.textureMap.GetTexture(Globals.TextureTypes.ThreeByThree);
            for (int i = 0; i < icons.Count; i++)
            {
				MenuIcon mi = icons[i];
				int position = (i >= 4 ? i+1 : i);

				
				if (mi.texture != null)
                {
					mi.targetTextureRectangle =
                        new Microsoft.Xna.Framework.Rectangle(
							(position % 3) * iconWidth  + (iconWidth / 2)  - (mi.texture.Width / 2),
							(position / 3) * iconHeight + (iconHeight / 2)  - (mi.texture.Height / 2),
							(mi.texture.Width),
							(mi.texture.Height));

                }
                int textWidth = 0;
                
				if (mi.label != null)
				{
                    mi.label = insertLineBreaksForString(mi.label);

					textWidth = mi.label.Length * FONT_WIDTH;

                    Vector2 temp = new Vector2(
                       (position % 3) * (iconWidth) + (iconWidth / 2),
                       (position / 3) * (iconHeight) + (iconHeight / 2));

					mi.targetLabelRectangle = new Rectangle((int)temp.X, (int)temp.Y, textWidth, FONT_SIZE);
                }
                if (mi.label != null && mi.texture != null)
                {
                    mi.targetTextureRectangle = new Rectangle(
                        mi.targetTextureRectangle.Location.X, mi.targetTextureRectangle.Location.Y - 40,
                        mi.texture.Width, mi.texture.Height);

                    mi.targetLabelRectangle = new Rectangle(
                        mi.targetLabelRectangle.Location.X, mi.targetLabelRectangle.Location.Y + mi.texture.Height - 15,
                        textWidth, FONT_SIZE);
                }

                mi.region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect(
                        (position % 3) * iconWidth,
                        (position / 3) * iconHeight,
                        iconWidth, iconHeight));
            }
            
            this.icons = icons;
        }

        [Obsolete("Generates null-references currently")]
        /// <summary>
        /// Magic Constructor for Menu!
        /// </summary>
        /// <param name="iconList">A List of MenuIcons to display.</param>
        /// <param name="explanation">A explanation for the menu to give to the user.</param>
        public Menu(List<MenuIcon> iconList, String explanation)
        {
            if (iconList.Count < 3)
            {
                new Menu(Globals.MenuLayout.Prompt, iconList, explanation);
            }
            else if (2 < iconList.Count && iconList.Count < 5)
            {
                new Menu(Globals.MenuLayout.FourMatrix, iconList, explanation);
            }
            else if (4 < iconList.Count && iconList.Count < 10)
            {
                new Menu(Globals.MenuLayout.NineMatrix, iconList, explanation);
            }
            else
            {
                new Menu(Globals.MenuLayout.FreeStyle, iconList, explanation);
            }
        }
    }
}
