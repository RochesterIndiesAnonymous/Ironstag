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
using WesternSpace.Utility;

namespace WesternSpace
{
    /// <summary>
    /// This is a singleton class for our game.
    /// </summary>
    public class ScreenManager : Game
    {
        /// <summary>
        /// The resolution settings to use when the game is running in windowed mode
        /// </summary>
        private static ResolutionSettings windowedSettings = new ResolutionSettings(640, 480, 640, 480);

        /// <summary>
        /// The full screen settings to use when using full screen. This is calculated based on the main display of the user
        /// </summary>
        private static ResolutionSettings fullScreenSettings;

        /// <summary>
        /// A pointer to the current resolution settings the game is using.
        /// </summary>
        private static ResolutionSettings currentResolutionSettings;

        /// <summary>
        /// The intermediate render target we use to provide scaling of the game screen resolution
        /// </summary>
        private RenderTarget2D renderTarget;

        /// <summary>
        /// The camera that is used to move around the editor and game world
        /// </summary>
        private CameraService cameraService;

        /// <summary>
        /// The graphics device manager our game uses for managing the graphics card
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// The service that manages all the sprite batches in the game
        /// </summary>
        private SpriteBatchService batchService;

        /// <summary>
        /// The list of screens that the screen manager is to manage.
        /// </summary>
        private IList<Screen> screenList;

        /// <summary>
        /// The list of screens that the screen manager is to manage.
        /// </summary>
        public IList<Screen> ScreenList
        {
            get { return screenList; }
        }

        /// <summary>
        /// True if the current game is in full screen. Used to determine which resolution to use for rendering
        /// </summary>
        private bool isFullScreen;

        /// <summary>
        /// True if the current game is in full screen. Used to determine which resolution to use for rendering
        /// </summary>
        public bool IsFullScreen
        {
            get { return isFullScreen; }
        }

        /// <summary>
        /// The service that manages the current resolution that the application runs in
        /// </summary>
        private ScreenResolutionService resolutionService;

        /// <summary>
        /// The service that manages the current resolution that the application runs in
        /// </summary>
        public ScreenResolutionService ResolutionService
        {
            get { return resolutionService; }
        }

        /// <summary>
        /// Holds our single reference to our game
        /// </summary>
        private static ScreenManager instance;

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
            screenList = new List<Screen>();
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

            fullScreenSettings = new ResolutionSettings(320, 240, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);

            resolutionService = new ScreenResolutionService(graphics, windowedSettings.RenderTargetWidth, windowedSettings.RenderTargetHeight);
            this.Services.AddService(typeof(IScreenResolutionService), resolutionService);

            SetScreenMode(false);

            // create our services
            CreateServices();

            // create our screens
            Screen editorScreen = new EditorScreen(this, EditorScreen.ScreenName);
            this.screenList.Add(editorScreen);
            
            Screen gameScreen = new GameScreen(this, GameScreen.ScreenName);
            this.screenList.Add(gameScreen);

            this.AddScreen(gameScreen);

            // Initialize all components
            base.Initialize();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the current set of screens to the render target
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        protected override void Draw(GameTime gameTime)
        {            
            graphics.GraphicsDevice.SetRenderTarget(0, renderTarget);

            GraphicsDevice.Clear(Color.Black);

            batchService.Begin();

            base.Draw(gameTime);

            batchService.End();

            GraphicsDevice.SetRenderTarget(0, null);

            Texture2D screen = renderTarget.GetTexture();

            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 0.0f, 0);

            SpriteBatch sb = new SpriteBatch(GraphicsDevice);
            sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
            sb.Draw(screen, resolutionService.ScaleRectangle, Color.White);
            sb.End();
        }

        /// <summary>
        /// Adds a screen to be rendered on every draw and updated every update
        /// </summary>
        /// <param name="screen">The screen to add to the currently drawn screens</param>
        public void AddScreen(Screen screen)
        {
            if (!this.Components.Contains(screen))
            {
                this.Components.Add(screen);
            }
        }

        /// <summary>
        /// Adds a screen by its name.
        /// </summary>
        /// <param name="name">The name of the screen to add</param>
        public void AddScreen(string name)
        {
            Screen screenToAdd = (from screen in screenList
                                  where screen.Name == name
                                  select screen).FirstOrDefault();

            if (screenToAdd != null && !this.Components.Contains(screenToAdd))
            {
                this.Components.Add(screenToAdd);
            }
        }

        /// <summary>
        /// Removes a screen from being drawn and updated
        /// </summary>
        /// <param name="screen">The screen to be removed</param>
        public void RemoveScreen(Screen screen)
        {
            if (this.Components.Contains(screen))
            {
                this.Components.Remove(screen);
            }
        }

        /// <summary>
        /// Removes a screen by its name.
        /// </summary>
        /// <param name="name">The ame of the screen to remove</param>
        public void RemoveScreen(string name)
        {
            IEnumerable<Screen> screens = (from component in this.Components
                                           where component is Screen
                                           select component).Cast<Screen>();

            Screen screenToRemove = (from screen in screens
                                    where screen.Name == name
                                    select screen).FirstOrDefault();

            if (screenToRemove != null && this.Components.Contains(screenToRemove))
            {
                this.Components.Remove(screenToRemove);
            }
        }

        /// <summary>
        /// Sets the game to use full screen or not
        /// </summary>
        /// <param name="fullScreen">True for full screen, false for windowed mode</param>
        public void SetScreenMode(bool fullScreen)
        {
            if (fullScreen)
            {
                currentResolutionSettings = fullScreenSettings;
      
                graphics.PreferredBackBufferWidth = fullScreenSettings.BackBufferWidth;
                graphics.PreferredBackBufferHeight = fullScreenSettings.BackBufferHeight;

                Viewport vp = GraphicsDevice.Viewport;
                vp.Width = fullScreenSettings.BackBufferWidth;
                vp.Height = fullScreenSettings.BackBufferHeight;
                GraphicsDevice.Viewport = vp;

                graphics.IsFullScreen = true;

                isFullScreen = true;
            }
            else
            {
                currentResolutionSettings = windowedSettings;

                graphics.PreferredBackBufferWidth = windowedSettings.BackBufferWidth;
                graphics.PreferredBackBufferHeight = windowedSettings.BackBufferHeight;

                Viewport vp = GraphicsDevice.Viewport;
                vp.Width = windowedSettings.BackBufferWidth;
                vp.Height = windowedSettings.BackBufferHeight;
                GraphicsDevice.Viewport = vp;

                graphics.IsFullScreen = false;

                isFullScreen = false;
            }

            resolutionService.StartTextureWidth = currentResolutionSettings.RenderTargetWidth;
            resolutionService.StartTextureHeight = currentResolutionSettings.RenderTargetHeight;
            resolutionService.ScaleRectangle = resolutionService.CalculateResolution(graphics, currentResolutionSettings.RenderTargetWidth, currentResolutionSettings.RenderTargetHeight);

            if (cameraService != null)
            {
                cameraService.UpdateVisibleArea();
                cameraService.CreateViewTransformationMatrix();
            }

            graphics.ApplyChanges();

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, currentResolutionSettings.RenderTargetWidth, currentResolutionSettings.RenderTargetHeight, 1, SurfaceFormat.Color);
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

            // create our camera service
            cameraService = new CameraService(this);
            cameraService.UpdateOrder = 1;
            this.Services.AddService(typeof(ICameraService), cameraService);
            this.Components.Add(cameraService);

            // create our batch service
            this.batchService = new SpriteBatchService(this);
            batchService.UpdateOrder = 2;
            this.Services.AddService(typeof(ISpriteBatchService), batchService);
            this.Components.Add(batchService);
        }
    }
}
