using System;
using System.IO;
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

using WesternSpace.TilingEngine;

namespace WesternSpace
{
    /*
     * Default Game Class
     */
    public class TilingEngineTest : Microsoft.Xna.Framework.Game
    {
        private const int MIN_SIZE = 1;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Dictionary<string, Texture2D> textures;

        SpriteFont font;
        double fps;
        Vector2 fontPos;
        Vector2 fpsStringPos;

        TileEngine tileEngine;
        Layer background;

        /*
         * Default Constructor
         */
        public TilingEngineTest()
        {
            graphics = new GraphicsDeviceManager(this);
            textures = new Dictionary<String, Texture2D>();
            Content.RootDirectory = "Content";
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
            base.Initialize();
        }

        /*
         * LoadContent will be called once per game and is the place to load
         * all of your content.
         */
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Setup the font
            font = Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE);
            fpsStringPos = new Vector2(fontPos.X, fontPos.Y + font.LineSpacing);

            tileEngine = new TileEngine(this, textures);

            // Load a layer
            background = tileEngine.LoadLayer("Layers\\TestLayer", "LayerXML\\TestLayer.xml");
            this.Components.Add(background);
        }
        /*
         * UnloadContent will be called once per game and is the place to unload
         * all content.
         */
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            spriteBatch.Begin();

            string output = "FPS: " + fps;
            spriteBatch.DrawString(font, output, fpsStringPos, Color.Red);

            spriteBatch.End();
        }
    }
}
