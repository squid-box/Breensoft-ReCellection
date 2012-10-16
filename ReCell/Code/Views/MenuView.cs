namespace Recellection
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Views;

    public class MenuView : IView
	{
        #region Static Fields

        static readonly object padlock = new object();
        static MenuView instance;

        #endregion

        #region Fields

        private Menu currentMenu;

        /// <summary>
        /// author: co
        /// </summary>
        // private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        // private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
        List<DrawData> graphics;

        #endregion

        #region Constructors and Destructors

        private MenuView()
		{
			MenuModel.Instance.MenuEvent += this.menuEventFunction;
			this.graphics = new List<DrawData>();
		}

        #endregion

        #region Public Properties

        public static MenuView Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new MenuView();
                    }

                    return instance;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        override public void Draw(SpriteBatch spriteBatch)
		{
			this.Layer = 1.0f;
			this.DrawTexture(spriteBatch, this.currentMenu.GetMenuPic(), new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			this.Layer = 0.0f;
            this.DrawCenteredString(spriteBatch, this.currentMenu.explanation, this.currentMenu.explanationDrawPos, this.currentMenu.explanationColor);
			foreach (MenuIcon mi in this.currentMenu.GetIcons())
			{
				if (mi.texture != null)
				{
					this.Layer = 0.5f;
					this.DrawTexture(spriteBatch, mi.texture, mi.targetTextureRectangle);
				}

				if (mi.label != null)
				{
					this.Layer = 0.25f;
                    this.DrawCenteredString(spriteBatch, mi.label, new Vector2(mi.targetLabelRectangle.X, mi.targetLabelRectangle.Y), mi.labelColor);
				}
			}
		}

        override public void Update(GameTime passedTime)
        {
        }

        public void menuEventFunction(object publisher, Event<Menu> ev)
        {
            this.currentMenu = ev.subject;
        }

        #endregion
	}
}