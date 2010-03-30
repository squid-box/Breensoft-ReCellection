using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

/*
 * BREENSOFT GAME OMG OMG OMG
 * 
 * Authors:
 */


namespace Recellection
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Recellection : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteFont screenFont;
        SpriteBatch spriteBatch;
        static Color breen = new Color(new Vector3(0.4f, 0.3f, 0.1f));
        
        //Sounds and music
        AudioPlayer audioPlayer;

        //Debug Input 
        KeyboardState lastKBState, kBState;
        MouseState lastMouseState, mouseState;

        public Recellection()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialize all classes and set renderstates
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Create models and load game data
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            screenFont = Content.Load<SpriteFont>("Fonts/ScreenFont");

            audioPlayer = new AudioPlayer(Content);
            audioPlayer.PlaySong(Globals.Songs.Theme);
        }

        /// <summary>
        /// Not Used
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleDebugInput();


            audioPlayer.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This method allows us to use the keyboard and mouse to test functionality in the program
        /// </summary>
        private void HandleDebugInput()
        {
            #region Update input states

            lastKBState = kBState;
            lastMouseState = mouseState;
            kBState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            #endregion

            if (kBState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (kBState.IsKeyDown(Keys.A) && lastKBState.IsKeyUp(Keys.A))
                audioPlayer.PlaySound("acid");

            if (kBState.IsKeyDown(Keys.B) && lastKBState.IsKeyUp(Keys.B))
                audioPlayer.PlaySound("boom");

            if (kBState.IsKeyDown(Keys.M) && lastKBState.IsKeyUp(Keys.M))
                audioPlayer.ToggleMusicMute();

            if (kBState.IsKeyDown(Keys.O) && lastKBState.IsKeyUp(Keys.O))
                audioPlayer.SetSoundVolume(1f);

            if (kBState.IsKeyDown(Keys.I) && lastKBState.IsKeyUp(Keys.I))
                audioPlayer.SetSoundVolume(0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(breen);

            PrintHelp();

            base.Draw(gameTime);
        }

        private void PrintHelp()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(screenFont, "M-> Toggle music\nI-> Turn SFX off\nO-> Turn SFX on\nA-> Acid sound\nB-> Explosion sound", Vector2.Zero, Color.White);
            spriteBatch.End();
        }

    }
}
