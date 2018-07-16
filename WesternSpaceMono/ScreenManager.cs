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
using WesternSpace.Input;
using System.IO;

namespace WesternSpace
{
    /// <summary>
    /// This is a singleton class for our game.
    /// </summary>
    public class ScreenManager : Game
    {
        /// <summary>
        /// The graphics device manager our game uses for managing the graphics card
        /// </summary>
        private GraphicsDeviceManager graphics;

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        /// <summary>
        /// The camera that determines what is displayed
        /// </summary>
        private CameraService cameraService;

        /// <summary>
        /// The service that manages all the sprite batches in the game
        /// </summary>
        private SpriteBatchService batchService;

        /// <summary>
        /// The spritebatch used to draw the scaled version of our game:
        /// </summary>
        private SpriteBatch sb;

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
        /// The game is currently in full screen mode
        /// </summary>
        private bool isFullScreen;

        /// <summary>
        /// The resolution settings to use when the game is running in windowed mode
        /// </summary>
        private static ResolutionSettings windowedSettings = new ResolutionSettings(320, 240, 640, 480, false);

        public static ResolutionSettings WindowedSettings
        {
            get { return ScreenManager.windowedSettings; }
        }

        /// <summary>
        /// Indicates whether to use the sprite batch service.
        /// We want to disable it when we are using HLSL functions such as 
        /// Storyboard screens
        /// </summary>
        private bool useSpriteBatchService;

        public bool UseSpriteBatchService
        {
            get { return useSpriteBatchService; }
            set { useSpriteBatchService = value; }
        }

        /// <summary>
        /// The full screen settings to use when using full screen. This is calculated based on the main display of the user
        /// </summary>
        private static ResolutionSettings fullScreenSettings;

        /// <summary>
        /// The current state of the ongoing fade transition
        /// </summary>
        private ScreenTransition transitionState;

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

            this.isFullScreen = false;

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
            fullScreenSettings = new ResolutionSettings(320, 240, this.GraphicsDevice.DisplayMode.Width, this.GraphicsDevice.DisplayMode.Height, true);

            //Initialization Logic

            // Set our XNA content directory
            Content.RootDirectory = "Content";

            // create our services
            CreateServices();
#if !XBOX
            // create our screens
            Screen editorScreen = new EditorScreen(this, EditorScreen.ScreenName);
            this.screenList.Add(editorScreen);
#endif
            Screen gameScreen = new GameScreen(this, GameScreen.ScreenName);
            this.screenList.Add(gameScreen);

            Screen titleScreen = new StoryboardScreen("TitleScreen", "Introduction", @"StoryboardXML\TitleScreenStoryboard");
            this.screenList.Add(titleScreen);

            Screen introScreen = new StoryboardScreen("Introduction", "Game", @"StoryboardXML\IntroductionStoryboard");
            this.screenList.Add(introScreen);

            Screen midpoint = new StoryboardScreen("MidPoint", "Game", @"StoryboardXML\MidPointStoryboard");
            this.screenList.Add(midpoint);

            Screen endScreen = new StoryboardScreen("Ending", "Game", @"StoryboardXML\EndingStoryboard");
            this.screenList.Add(endScreen);

            this.AddScreenToDisplay(titleScreen);

            resolutionService = new ScreenResolutionService(graphics, ScreenManager.WindowedSettings);
            this.Services.AddService(typeof(IScreenResolutionService), resolutionService);

            sb = new SpriteBatch(GraphicsDevice);

            byte[] bytecode = File.ReadAllBytes("Content/System/Effects/SetAlphaValue.mgfxd");
            this.alphaEffect = new Effect(this.graphics.GraphicsDevice, bytecode);
      
            //For profiling:
            /*
            this.IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            */

            // Initialize all components
            base.Initialize();
        }
        public Effect alphaEffect;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        protected override void Update(GameTime gameTime)
        {
            if (InputMonitor.Instance.WasJustPressed("ToggleFullScreen"))
            {
                if (isFullScreen)
                {
                    isFullScreen = false;
                    ResolutionService.CurrentResolutionSettings = windowedSettings;
                }
                else
                {
                    isFullScreen = true;
                    ResolutionService.CurrentResolutionSettings = fullScreenSettings;
                }
            }

            if (transitionState != null)
            {
                transitionState.Update();

                if (transitionState.IsTransitionComplete)
                {
                    Screen s = (from sc in this.ScreenList
                                where sc.Name == transitionState.ToScreenName
                                select sc).FirstOrDefault();

                    s.TransitionComplete();

                    transitionState = null;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the current set of screens to the render target
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.SetRenderTarget(resolutionService.RenderTarget);

            GraphicsDevice.Clear(Color.Black);

            if (this.UseSpriteBatchService)
            {
                batchService.Begin();
            }

            base.Draw(gameTime);

            if (this.UseSpriteBatchService)
            {
                batchService.End();
            }

            GraphicsDevice.SetRenderTarget(null);

            Texture2D screen = resolutionService.RenderTarget;

            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 0.0f, 0);

            if (transitionState != null)
            {
                transitionState.BeginTransition();
                Effect tEffect = transitionState.GetEffect();
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, tEffect);


            }
            else
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

            }

            sb.Draw(screen, resolutionService.ScaleRectangle, Color.White);
            sb.End();
            if (transitionState != null)
            {
                transitionState.EndTransition();
            }
            /*
                        alphaEffect.Parameters["AlphaValue"].SetValue(0.25f);

                        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, this.alphaEffect);
                       // sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, null);
                        if (transitionState != null)
                        {
                            transitionState.BeginTransition();
                        }

                        // GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
                        //GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
                        //GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
                        sb.Draw(screen, resolutionService.ScaleRectangle, Color.White);

                        if (transitionState != null)
                        {
                            transitionState.EndTransition();
                        }

                        sb.End();
                        */
        }

        /// <summary>
        /// Adds a screen to be rendered on every draw and updated every update
        /// </summary>
        /// <param name="screen">The screen to add to the currently drawn screens</param>
        public void AddScreenToDisplay(Screen screen)
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
        public void AddScreenToDisplay(string name)
        {
            Screen screenToAdd = (from screen in screenList
                                  where screen.Name == name
                                  select screen).FirstOrDefault();

            if (screenToAdd != null && !this.Components.Contains(screenToAdd))
            {
                screenToAdd.Enabled = true;
                this.Components.Add(screenToAdd);
            }
        }

        /// <summary>
        /// Removes a screen from being drawn and updated
        /// </summary>
        /// <param name="screen">The screen to be removed</param>
        public void RemoveScreenFromDisplay(Screen screen)
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
        public void RemoveScreenFromDisplay(string name)
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
#if !XBOX
            InputManagerService input = new InputManagerService(this);
            input.UpdateOrder = 0;
            this.Services.AddService(typeof(IInputManagerService), input);
            this.Components.Add(input);
#endif

#if !XBOX
            InputMonitor.Instance.AssignPressable("ToggleFullScreen", new PressableKey(Keys.F));
#endif
            this.Components.Add(InputMonitor.Instance);

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

        public void Transition(ScreenTransition transition)
        {
            Screen s = (from sc in this.ScreenList
                        where sc.Name == transition.FromScreenName
                        select sc).FirstOrDefault();

            if (s == null)
            {
                throw new ArgumentException("FromScreenName is not a known screen", "TransitionState.FromScreenName");
            }

            if (!this.Components.Contains(s))
            {
                throw new ArgumentException("FromScreenName is currently not being drawn to the screen", "TransitionState.FromScreenName");
            }

            s = (from sc in this.ScreenList
                 where sc.Name == transition.ToScreenName
                 select sc).FirstOrDefault();

            if (s == null)
            {
                throw new ArgumentException("ToScreenName is not a known screen", "TransitionState.ToScreenName");
            }

            if (!transition.ResetGame && this.Components.Contains(s))
            {
                throw new ArgumentException("ToScreenName is currently being drawn to the screen", "TransitionState.ToScreenName");
            }

            this.transitionState = transition;
        }
    }
}
