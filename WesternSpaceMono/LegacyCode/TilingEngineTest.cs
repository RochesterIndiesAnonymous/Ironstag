//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

//using WesternSpace.TilingEngine;
//using WesternSpace.Services;
//using WesternSpace.ServiceInterfaces;

//namespace WesternSpace
//{
//    /*
//     * Default Game Class
//     */
//    public class TilingEngineTest : Microsoft.Xna.Framework.Game
//    {
//        GraphicsDeviceManager graphics;
//        SpriteBatch spriteBatch;

//        TileEngine tileEngine;
//        Layer background;

//        /*
//         * Default Constructor
//         */
//        public TilingEngineTest()
//        {
//            Content.RootDirectory = "Content";
//        }

//        /// <summary>
//        /// Allows the game to perform any initialization it needs to before starting to run.
//        /// This is where it can query for any required services and load any non-graphic
//        /// related content.  Calling base.Initialize will enumerate through any components
//        /// and initialize them as well.
//        /// </summary>
//        protected override void Initialize()
//        {
//            //Initialization Logic

//            // create our TextureService
//            ITextureService textureService = new TextureService();
//            this.Services.AddService(typeof(ITextureService), textureService);

//            // create our graphics device manager service
//            //IGraphicsDeviceManagerService graphicsService = new GraphicsDeviceMangerService();
//            //this.Services.AddService(typeof(IGraphicsDeviceManagerService), graphics);
            
//            // Initialize all components
//            base.Initialize();
//        }

//        /*
//         * LoadContent will be called once per game and is the place to load
//         * all of your content.
//         */
//        protected override void LoadContent()
//        {
//            ITextureService ts = (ITextureService)this.Services.GetService(typeof(ITextureService));
//            tileEngine = new TileEngine(this, ts.Textures);

//            // Load a layer
//            background = tileEngine.LoadLayer("Layers\\TestLayer", "LayerXML\\TestLayer.xml");
//            this.Components.Add(background);
//        }
//        /*
//         * UnloadContent will be called once per game and is the place to unload
//         * all content.
//         */
//        protected override void UnloadContent()
//        {
//            // TODO: Unload any non ContentManager content here
//        }

//        /*
//         * Allows the game to run logic such as updating the world,
//         * checking for collisions, gathering input, and playing audio.
//         *
//         * <param name="gameTime">Provides a snapshot of timing values.</param>
//         */
//        protected override void Update(GameTime gameTime)
//        {
//            // Allows the game to exit
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
//                this.Exit();

            
//            base.Update(gameTime);
//        }

//        /*
//         * This is called when the game should draw itself.
//         *
//         * <param name="gameTime">Provides a snapshot of timing values.</param>
//         */
//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.CornflowerBlue);

//            base.Draw(gameTime);
//        }
//    }
//}
