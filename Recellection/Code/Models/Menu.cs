using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/**
 * 
 * Author: co
 * 
 **/

namespace Recellection.Code.Models
{
    public class Menu : IModel
    {
        private List<GUIRegion> regions;
        private Texture2D menuPic;
        private Menu helpMenu; //om denna Menu är en helpmenu eller inte ska ha en helpMenu sets denna variabel till null duh.

        private Vector2 buttonDimension = new Vector2(Recellection.viewPort.Width / 3, Recellection.viewPort.Height / 3);
        private Vector2 confirmationButtonDimension = new Vector2(Recellection.viewPort.Width / 2, Recellection.viewPort.Height);

        private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
        private float fontSzInPx = 14;


        /**
         * 
         * min tanke är att varje meny ska vara ett Menu objekt. 
         * Denna klass kommer ha två konstruktorer den beskriver exakt hur menyn ska se ut och i den andra kan man välja bland några hårdkodade menyer.
         * Om du har synpunkter eller frågorom implementationen tveka inte att kontakta mig (co).
         * 
         * Jag tänker inte låta det vara möjligt att ändra menyer, man gör bara nya om så skulle behövas istället, om dett visar sig vara ineffektivt ändrar jag på det.
         * 
         * */


        //place holders, dem riktiga funktionerna ska faktiskt göra något :)
        public Menu()
        {
        }

        //här kommer menyerna hårdkodas, mest kod här
        public Menu(Globals.MenuTypes menuType, bool isHelpMenu)
        {
            if (!isHelpMenu)
            {
                switch (menuType)
                {
                    case Globals.MenuTypes.MainMenu:
                        CreateMainMenu();
                        break;

                    case Globals.MenuTypes.OptionsMenu:
                        CreateOptionsMenu();
                        break;

                    case Globals.MenuTypes.ConfirmationMenu:
                        CreateConfirmationMenu();
                        break;

                    case Globals.MenuTypes.CommandMenu:
                        CreateCommandMenu();
                        break;

                    case Globals.MenuTypes.SpecialCommandMenu:
                        CreateSpecialCommandMenu();
                        break;
                }
            }
            else
            {
                switch (menuType)
                {
                    case Globals.MenuTypes.MainMenu:
                        CreateMainMenuHelp();
                        break;

                    case Globals.MenuTypes.OptionsMenu:
                        CreateOptionsMenuHelp();
                        break;

                    case Globals.MenuTypes.ConfirmationMenu:
                        CreateConfirmationMenuHelp();
                        break;

                    case Globals.MenuTypes.CommandMenu:
                        CreateCommandMenuHelp();
                        break;

                    case Globals.MenuTypes.SpecialCommandMenu:
                        CreateSpecialCommandMenuHelp();
                        break;
                }
            }
        }

        #region Menu Creators

        private void CreateMainMenu()
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            textDrawer.Begin();
            //textDrawer.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.MainMenu), Vector2.Zero, Color.White);
            float textScale = 2;

            //This is the offset from the buttonvector where the text will be drawn
            Vector2 offset = new Vector2(buttonDimension.X / 4, (buttonDimension.Y / 2) - ((fontSzInPx / 2) * textScale));

            regions = new List<GUIRegion>();

            //Init the regions!
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu1"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu2"), new Vector2(buttonDimension.X, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu3"), new Vector2(buttonDimension.X * 2, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            /////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu4"), new Vector2(0, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            //The center region is not used
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu5"), new Vector2(buttonDimension.X * 2, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            ////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu6"), new Vector2(0, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu7"), new Vector2(buttonDimension.X, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu8"), new Vector2(buttonDimension.X * 2, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            textDrawer.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            menuPic = textRenderTex.GetTexture();

            helpMenu = new Menu(Globals.MenuTypes.CommandMenu, true);
        }

        private void CreateOptionsMenu()
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            textDrawer.Begin();
            //textDrawer.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.OptionsMenu), Vector2.Zero, Color.White);
            float textScale = 2;

            //This is the offset from the buttonvector where the text will be drawn
            Vector2 offset = new Vector2(buttonDimension.X / 4, (buttonDimension.Y / 2) - ((fontSzInPx / 2) * textScale));

            regions = new List<GUIRegion>();

            //Init the regions!
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu1"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu2"), new Vector2(buttonDimension.X, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu3"), new Vector2(buttonDimension.X * 2, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            /////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu4"), new Vector2(0, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            //The center region is not used
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu5"), new Vector2(buttonDimension.X * 2, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            ////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu6"), new Vector2(0, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu7"), new Vector2(buttonDimension.X, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("OptionsMenu8"), new Vector2(buttonDimension.X * 2, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            textDrawer.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            menuPic = textRenderTex.GetTexture();

            helpMenu = new Menu(Globals.MenuTypes.OptionsMenu, true);
        }

        private void CreateConfirmationMenu()
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            textDrawer.Begin();
            //textDrawer.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.ConfirmationMenu), Vector2.Zero, Color.White);
            float textScale = 6;

            //This is the offset from the buttonvector where the text will be drawn
            Vector2 offset = new Vector2(buttonDimension.X / 4, (buttonDimension.Y / 2) - ((fontSzInPx / 2) * textScale));

            regions = new List<GUIRegion>();

            //Init the regions!
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, confirmationButtonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("ConfirmationMenuYes"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(confirmationButtonDimension.X, 0, confirmationButtonDimension.X, confirmationButtonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("ConfirmationMenuNo"), new Vector2(confirmationButtonDimension.X, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            textDrawer.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            menuPic = textRenderTex.GetTexture();

            helpMenu = new Menu(Globals.MenuTypes.ConfirmationMenu, true);
        }

        private void CreateCommandMenu()
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            textDrawer.Begin();
            //textDrawer.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.CommandMenu), Vector2.Zero, Color.White);
            float textScale = 2;

            //This is the offset from the buttonvector where the text will be drawn
            Vector2 offset = new Vector2(buttonDimension.X / 4, (buttonDimension.Y / 2) - ((fontSzInPx / 2) * textScale));

            regions = new List<GUIRegion>();

            //Init the regions!
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu1"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu2"), new Vector2(buttonDimension.X, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu3"), new Vector2(buttonDimension.X * 2, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            /////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu4"), new Vector2(0, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            //The center region is not used
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu5"), new Vector2(buttonDimension.X * 2, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            ////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu6"), new Vector2(0, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu7"), new Vector2(buttonDimension.X, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("CommandMenu8"), new Vector2(buttonDimension.X * 2, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            textDrawer.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            menuPic = textRenderTex.GetTexture();

            helpMenu = new Menu(Globals.MenuTypes.CommandMenu, true);
        }

        private void CreateSpecialCommandMenu()
        {
            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            textDrawer.Begin();
            //textDrawer.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.SpecialCommandMenu), Vector2.Zero, Color.White);
            float textScale = 2;

            //This is the offset from the buttonvector where the text will be drawn
            Vector2 offset = new Vector2(buttonDimension.X / 4, (buttonDimension.Y / 2) - ((fontSzInPx / 2) * textScale));

            regions = new List<GUIRegion>();

            //Init the regions!
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu1"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu2"), new Vector2(buttonDimension.X, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, 0, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu3"), new Vector2(buttonDimension.X * 2, 0) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            /////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu4"), new Vector2(0, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            //The center region is not used
            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu5"), new Vector2(buttonDimension.X * 2, buttonDimension.Y) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            ////////////

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(0, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu6"), new Vector2(0, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu7"), new Vector2(buttonDimension.X, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            regions.Add(new GUIRegion(Recellection.windowHandle, new System.Windows.Rect(buttonDimension.X * 2, buttonDimension.Y * 2, buttonDimension.X, buttonDimension.Y)));
            textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("SpecialCommandMenu8"), new Vector2(buttonDimension.X * 2, buttonDimension.Y * 2) + offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            textDrawer.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);

            menuPic = textRenderTex.GetTexture();

            helpMenu = new Menu(Globals.MenuTypes.SpecialCommandMenu, true);
        }

        
        #region Help Menu Creators

        private void CreateMainMenuHelp()
        {
            helpMenu = null;
        }

        private void CreateOptionsMenuHelp()
        {
            helpMenu = null;
        }

        private void CreateConfirmationMenuHelp()
        {
            helpMenu = null;
        }

        private void CreateCommandMenuHelp()
        {
            helpMenu = null;
        }

        private void CreateSpecialCommandMenuHelp()
        {
            helpMenu = null;
        }

        #endregion

        #endregion

        //och så några get metoder:
        public List<GUIRegion> GetRegions()
        {
            return regions;
        }
        public Texture2D GetMenuPic()
        {
            return menuPic;
        }
        public Menu GetHelp() //seriously dude, you need help...
        {
            return helpMenu;
        }

    }
}
