using System.Linq;
using System.Text;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;
using System;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Recellection
{


	public class MenuIcon
	{

        public GUIRegion region { get; set; }
        public String label { get; set; }
        public Texture2D texture { get; private set; }
        public Rectangle targetTextureRectangle { get; set; }
        public Rectangle targetLabelRectangle { get; set; }
        public Color labelColor { get; set; }

        public MenuIcon(String label)
         : this(label, null, Color.Black)
        {
        }
        
        public MenuIcon(String label, Texture2D texture)
         : this(label, texture, Color.Black)
        {
        }
        
        public MenuIcon(String label, Texture2D texture, Color color)
		{
			region = null;
			this.label = label;
			this.texture = texture;
            this.labelColor = color;
		}
        //use only with offscreenregion!
        public MenuIcon(GUIRegion region)
        {
            this.region = region;
        }
	}
}
