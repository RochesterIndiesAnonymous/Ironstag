using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Services;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace
{
    /// <summary>
    /// This is a singleton class for our game.
    /// </summary>
    public class ScreenManager : Game
    {
        /// <summary>
        /// holds our single reference to our game
        /// </summary>
        private static ScreenManager instance;
        private GameScreen gameScreen;

        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Gets the current instance of the game.
        /// </summary>
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScreenManager();
                }

                return instance;
            }
        }

        public ScreenManager()
        {
            // XNA does not like it if this is not created here.
            graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialization Logic
            this.IsMouseVisible = true;

            // Set our XNA content directory
            Content.RootDirectory = "Content";

            // create our TextureService
            ITextureService textureService = new TextureService();
            this.Services.AddService(typeof(ITextureService), textureService);

            // create our graphics device manager service
            IGraphicsDeviceManagerService graphicsService = new GraphicsDeviceMangerService(graphics);
            this.Services.AddService(typeof(IGraphicsDeviceManagerService), graphicsService);

            // create and add any necessary components if needed
            gameScreen = new GameScreen(this);
            this.Components.Add(gameScreen);

            // Initialize all components
            base.Initialize();
        }

        /*
         * Allows the game to run logic such as updating the world,
         * checking for collisions, gathering input, and playing audio.
         *
         * <param name="gameTime">Provides a snapshot of timing values.</param>
         */
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            base.Update(gameTime);
        }

        /*
         * This is called when the game should draw itself.
         *
         * <param name="gameTime">Provides a snapshot of timing values.</param>
         */
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
