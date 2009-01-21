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
        /// Holds our single reference to our game
        /// </summary>
        private static ScreenManager instance;

        /// <summary>
        /// The game screen component to draw to the screen
        /// </summary>
        private GameScreen gameScreen;

        /// <summary>
        /// The graphics device manager our game uses
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// 
        /// </summary>
        private SpriteBatchService batchService;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public ScreenManager()
        {
            // XNA does not like it if this is not created here.
            graphics = new GraphicsDeviceManager(this);

            // Set the native resolution of the game
            graphics.PreferredBackBufferWidth = 320;
            graphics.PreferredBackBufferHeight = 240;
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

            // create our services
            CreateServices();

            // create our game screen
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

            batchService.Begin();

            base.Draw(gameTime);

            batchService.End();
        }

        /// <summary>
        /// Creates all the services our game will use
        /// </summary>
        private void CreateServices()
        {
            // create our TextureService
            ITextureService textureService = new TextureService();
            this.Services.AddService(typeof(ITextureService), textureService);

            // create our graphics device manager service
            IGraphicsDeviceManagerService graphicsService = new GraphicsDeviceMangerService(graphics);
            this.Services.AddService(typeof(IGraphicsDeviceManagerService), graphicsService);

            // create out input manager
            InputManagerService input = new InputManagerService(this);
            input.UpdateOrder = 0;
            this.Services.AddService(typeof(IInputManagerService), input);
            this.Components.Add(input);

            // create our layer service
            ILayerService layer = new LayerService();
            this.Services.AddService(typeof(ILayerService), layer);

            // create our animation data service
            IAnimationDataService animationDataService = new AnimationDataService();
            this.Services.AddService(typeof(IAnimationDataService), animationDataService);

            // create our camera service
            CameraService camera = new CameraService(this);
            camera.UpdateOrder = 1;
            this.Services.AddService(typeof(ICameraService), camera);
            this.Components.Add(camera);

            // create our batch service
            this.batchService = new SpriteBatchService(this);
            batchService.UpdateOrder = 2;
            this.Services.AddService(typeof(ISpriteBatchService), batchService);
            this.Components.Add(batchService);
        }
    }
}
