namespace Recellection
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Models;

    public class MenuIcon
    {
        #region Constructors and Destructors

        public MenuIcon(string label)
            : this(label, null, Color.Black)
        {
        }

        public MenuIcon(string label, Texture2D texture)
            : this(label, texture, Color.Black)
        {
        }

        public MenuIcon(string label, Texture2D texture, Color color)
        {
            this.region = null;
            this.label = label;
            this.texture = texture;
            this.labelColor = color;
        }

        // use only with offscreenregion!
        public MenuIcon(GUIRegion region)
        {
            this.region = region;
        }

        #endregion

        #region Public Properties

        public string label { get; set; }

        public Color labelColor { get; set; }

        public GUIRegion region { get; set; }

        public Rectangle targetLabelRectangle { get; set; }

        public Rectangle targetTextureRectangle { get; set; }

        public Texture2D texture { get; private set; }

        #endregion
    }
}
