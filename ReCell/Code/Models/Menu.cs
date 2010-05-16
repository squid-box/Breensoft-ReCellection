
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
        private const int FONT_SIZE = 40;
        private const int FONT_WIDTH = 20;
        private const int CHARS_PER_ROW = 15;
        
	    private List<MenuIcon> icons;
        private Texture2D menuPic;
        public String explanation { get; private set; }
        public Vector2 explanationDrawPos { get; private set; }
        public Color explanationColor { get; private set; }

		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation)
			 : this(layout, icons, explanation, Color.Black)
		{
		}
		
		public Menu(Globals.MenuLayout layout, List<MenuIcon> icons, String explanation, Color explanationColor)
		{
            this.explanation = insertLineBreaksForString(explanation);
            this.explanationColor = explanationColor;
            this.explanationDrawPos = calculateDrawCoordinates(new Vector2(Recellection.viewPort.Width / 2, Recellection.viewPort.Height / 2),this.explanation);
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

        private Vector2 calculateDrawCoordinates(Vector2 middlePointOfString, String text)
        {
            int textWidth = 0;

            if (text.IndexOf('\n') != -1)
            {
                textWidth = text.IndexOf('\n') * FONT_WIDTH;
            }
            else
            {
                textWidth = text.Length * FONT_WIDTH;
            }

            float x = middlePointOfString.X - textWidth / 2;
            float y = middlePointOfString.Y - (FONT_SIZE / 2 * (text.Split('\n').Length+1));

            return new Vector2(x, y);
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
                if( icons[i].label != null)
                {
                    icons[i].label = insertLineBreaksForString(icons[i].label);

                    int textWidth = icons[i].label.Length * FONT_WIDTH;
                    Vector2 temp = calculateDrawCoordinates(new Vector2(
                        i * Recellection.viewPort.Width * 3 / 5 + Recellection.viewPort.Width * 1 / 5, Recellection.viewPort.Height / 2), icons[i].label);

                    icons[i].targetLabelRectangle =
                    new Microsoft.Xna.Framework.Rectangle((int)temp.X, (int)temp.Y, textWidth, FONT_SIZE);
                }

                if (icons[i].label != null && icons[i].texture != null)
                {

                    icons[i].targetLabelRectangle = new Rectangle(
                        icons[i].targetLabelRectangle.Location.X, icons[i].targetLabelRectangle.Location.Y + icons[i].texture.Height + 5,
                        icons[i].texture.Width, icons[i].texture.Height);

                }
                
            }
			

			/*icons[1].region = new GUIRegion(Recellection.windowHandle, 
                new System.Windows.Rect(, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            icons[1].targetRectangle =
                new Microsoft.Xna.Framework.Rectangle(Recellection.viewPort.Width * 3 / 5, 0, Recellection.viewPort.Width, Recellection.viewPort.Height);
            */
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
            //This will not work, waiting for better way to implement.
            /*if (scrollZone)
            {

                for (int i = cols + 1; i < cols * rows; i++)
                {
                    //Hax calc, ignores the edges
                    if (i % cols != 0 && (i+1) % cols != 0 && i < (cols - 1) * rows)
                    {
                        icons[i].targetRectangle = new Microsoft.Xna.Framework.Rectangle((i % cols) * iconWidth, (i / rows) * iconHeight, iconWidth, iconHeight);

                        icons[i].region = new GUIRegion(Recellection.windowHandle,
                            new System.Windows.Rect((i % cols) * iconWidth, (i / rows) * iconHeight, iconWidth, iconHeight));
                    }
                }

            }*/
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
                if (icons[i].label != null)
                {
                    icons[i].label = insertLineBreaksForString(icons[i].label);

                    int textWidth = icons[i].label.Length * FONT_WIDTH;

                    Vector2 temp = calculateDrawCoordinates(new Vector2(
                       (i % 2) * (windowWidth * 3 / 5) + (windowWidth * 1 / 5), (i / 2) * (windowHeight * 3 / 5) + (windowHeight * 1 / 5)), icons[i].label);

                    icons[i].targetLabelRectangle =
                        new Microsoft.Xna.Framework.Rectangle((int) temp.X, (int) temp.Y, (textWidth), (FONT_SIZE));
                }

                if (icons[i].label != null && icons[i].texture != null)
                {

                    icons[i].targetLabelRectangle = new Rectangle(
                        icons[i].targetLabelRectangle.Location.X, icons[i].targetLabelRectangle.Location.Y + icons[i].texture.Height + 5,
                        icons[i].texture.Width,icons[i].texture.Height);

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
							(position / 3) * iconHeight + (iconWidth / 2)  - (mi.texture.Height / 2),
							(mi.texture.Width),
							(mi.texture.Height));

                }
				if (mi.label != null)
				{
                    mi.label = insertLineBreaksForString(mi.label);

					int textWidth = mi.label.Length * FONT_WIDTH;

                    Vector2 temp = calculateDrawCoordinates(new Vector2(
					   (position % 3) * (iconWidth) + (iconWidth / 2), 
					   (position / 3) * (iconHeight) + (iconHeight / 2)), 
					   mi.label);

					mi.targetLabelRectangle = new Rectangle((int)temp.X, (int)temp.Y, textWidth, FONT_SIZE);
                }
                if (mi.label != null && mi.texture != null)
                {
                    mi.targetLabelRectangle = new Rectangle(
                        mi.targetLabelRectangle.Location.X, mi.targetLabelRectangle.Location.Y + mi.texture.Height + 5,
                        mi.texture.Width, mi.texture.Height);
                }

                icons[i].region = new GUIRegion(Recellection.windowHandle,
                    new System.Windows.Rect(
                        (position % 3) * iconWidth,
                        (position / 3) * iconHeight,
                        iconWidth, iconHeight));
            }
            
            this.icons = icons;
        }
    }
}
