namespace Recellection.Code.Models
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This class should be instantiated once, preferably in the Initializer. 
    /// When constructed it will load all the image textures the game will use
    /// to represent models graphically. The only method it has is the method
    /// to retrieve one of these textures by providing a enum from
    /// Globals.TextureTypes.
    /// 
    /// Author: John Forsberg
    /// </summary>
    public class SpriteTextureMap : IModel
    {
        #region Fields

        private readonly ContentManager content;

        private readonly Dictionary<Globals.TextureTypes, Texture2D> loadedTextures;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor for the SpriteTextureMap
        /// </summary>
        /// <param name="content">The ContentManager the
        /// SpriteTextureMap will use to load the images</param>
        public SpriteTextureMap(ContentManager content)
        {
            this.content = content;
            this.loadedTextures = new Dictionary<Globals.TextureTypes, Texture2D>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// This method returns the Texture2D specified by the enum
        /// Globals.TextureTypes.
        /// </summary>
        /// <param name="texture">The Globals.TextureTypes enum which 
        /// specifies which Texture2D to return</param>
        /// <returns>The requested Texture2D</returns>
        public Texture2D GetTexture(Globals.TextureTypes texture)
        {
            if(this.loadedTextures.ContainsKey(texture))
            {
                return this.loadedTextures[texture];
            }

            return this.loadedTextures[texture] = this.content.Load<Texture2D>("Graphics/" + texture);
        }

        #endregion
    }
}
