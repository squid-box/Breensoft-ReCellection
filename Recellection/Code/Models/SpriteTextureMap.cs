using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Recellection.Code.Models
{
    //Signed by Viktor!
    /// <summary>
    /// This class should be instantiated once, preferably in the Initializer. 
    /// When constructed it will load all the image textures the game will use
    /// to represent models graphically. The only method it has is the method
    /// to retrieve one of these textures by providing a enum from
    /// Globals.TextureTypes.
    /// </summary>
    class SpriteTextureMap : IModel
    {
        //Image file format, currently only accepts one format
        public const String IMAGE_FORMAT = "png";

        //The map which each of the texture image file will be placed
        public const String TEXTURE_FOLDER = "Textures";

        //The array containing each of the Texture2D.
        private Texture2D[] textures;

        /// <summary>
        /// Constructor for the SpriteTextureMap
        /// </summary>
        /// <param name="content">The ContentManager the
        /// SpriteTextureMap will use to load the images</param>
        public SpriteTextureMap(ContentManager content)
        {
            textures = new Texture2D[
                Enum.GetValues(typeof(Globals.TextureTypes)).Length];

            //Get all the names for the enum TextureTypes.

            String[] textureNames = 
                Enum.GetNames(typeof(Globals.TextureTypes));

            for(int i = 0; i < textures.Length; i++) 
            {
                //For each of the enum name load the same texture image file.
                textures[i] =
                    content.Load<Texture2D>(

                    TEXTURE_FOLDER+"/"+textureNames[i] + "." + IMAGE_FORMAT);

            }

        }
        
        /// <summary>
        /// This method returns the Texture2D specified by the enum
        /// Globals.TextureTypes.
        /// </summary>
        /// <param name="texture">The Globals.TextureTypes enum which 
        /// specifies which Texture2D to return</param>
        /// <returns>The requested Texture2D</returns>
        public Texture2D GetTexture(Globals.TextureTypes texture)
        {
            return textures[(int)texture];
        }
    }
}
