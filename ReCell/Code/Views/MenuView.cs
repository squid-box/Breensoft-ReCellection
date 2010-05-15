
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Content;

namespace Recellection
{

	public sealed class MenuView : IView
	{
		/// <summary>
		/// author: co
		/// </summary>
	
		//private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        //private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
		List<DrawData> graphics;
        static readonly object padlock = new object();
        static MenuView instance = null;
        
        private Menu currentMenu = null;

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


		private MenuView()
		{
			MenuModel.Instance.MenuEvent += menuEventFunction;
			graphics = new List<DrawData>();
		}
		
		public void menuEventFunction(Object publisher, Event<Menu> ev)
		{
			currentMenu = ev.subject;
		}

		override public void Update(GameTime passedTime)
		{
		}
		
		override public void Draw(SpriteBatch spriteBatch)
		{
			Layer = 1.0f;
			this.drawTexture(spriteBatch, currentMenu.GetMenuPic(), new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));

			Layer = 0.0f;
            spriteBatch.DrawString(Recellection.screenFont, currentMenu.explanation, currentMenu.explanationDrawPos, currentMenu.explanationColor);

			foreach (MenuIcon mi in currentMenu.GetIcons())
			{
				if (mi.texture != null)
				{
					Layer = 0.5f;
					this.drawTexture(spriteBatch, mi.texture, mi.targetTextureRectangle);
				}
				if (mi.label != null)
				{
					Layer = 0.25f;
                    spriteBatch.DrawString(Recellection.screenFont, mi.label, new Vector2(mi.targetLabelRectangle.X, mi.targetLabelRectangle.Y), mi.labelColor);
				}
			}
		}

        
	}
}