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

namespace WesternSpace
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        // CONSTANT DECLARATION //
        const int MIN_SIZE = 0;

        // CLASS VARIABLES //
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random random = new Random();
        KeyboardState oldState;
        List<GameObject> gameObjList = new List<GameObject>();
        bool raveParty = false;
        Cursor mouseCursor;
        Song rave;
        SpriteFont font;
        Vector2 fontPos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here
            base.Initialize();

            //Get "old" keyboard state
            oldState = Keyboard.GetState();

            //Setup cursor
            mouseCursor = new Cursor(Content.Load<Texture2D>("Sprites\\1down"));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Initialize

            // TODO: use this.Content to load your game content here

            //Setup the font
            font = Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE);

            //Create 5000 game objects and add them to a list.
            for (int count = 0; count < 2500; count++)
            {
                GameObject megaSmile = new GameObject(Content.Load<Texture2D>("Sprites\\1up"));
                GameObject megaFrown = new GameObject(Content.Load<Texture2D>("Sprites\\1down"));
                megaSmile.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                 random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                megaFrown.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                 random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                gameObjList.Add(megaSmile);
                gameObjList.Add(megaFrown);
            }

            //Setup the music
            rave = Content.Load<Song>("Music\\rave");
            MediaPlayer.Play(rave);
            MediaPlayer.Pause();
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Get new input from the user
            updateInput();

            //Update Mouse Position
            mouseCursor.Update();

            // Toggles between fullscreen and windowed mode with the 'F' key


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draw the list of game objects
            foreach (GameObject currObj in gameObjList)
            {
                if (raveParty)
                {
                    currObj.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                   random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                }
                    currObj.Draw(this.spriteBatch);
            }

            //Draw the Font
            String output = mouseCursor.getPosition().X +", "+ mouseCursor.getPosition().Y;
            spriteBatch.DrawString(font, output, fontPos, Color.Red);

            //Draw the Mouse Cursor
            mouseCursor.Draw(this.spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /*
         * Updates user input every frame
         */
        private void updateInput()
        {
#if !XBOX
            KeyboardState newState = Keyboard.GetState();

            //Check if the F key is down
            if (newState.IsKeyDown(Keys.F))
            {
                if (!oldState.IsKeyDown(Keys.F))
                {
                    //Key just pressed
                }
            }
            else if(oldState.IsKeyDown(Keys.F))
            {
                //Key was just released
                this.graphics.ToggleFullScreen();
            }

            //Check if the R key is down
            if (newState.IsKeyDown(Keys.R))
            {
                if (!oldState.IsKeyDown(Keys.R))
                {
                    //Key just pressed
                }
            }
            else if (oldState.IsKeyDown(Keys.R))
            {
                //Key just released
                engageRaveParty();
            }

            //Update Key state
            oldState = newState;

#endif
        }

        /*
         * Begins and ends an absolutely insane RAVE PARTY!
         */
        public void engageRaveParty()
        {
            raveParty = !raveParty;

            if (raveParty)
            {
                mouseCursor.setSprite(Content.Load<Texture2D>("Sprites\\1up"));
                MediaPlayer.Resume();
            }
            else
            {
                mouseCursor.setSprite(Content.Load<Texture2D>("Sprites\\1down"));
                MediaPlayer.Pause();
            }
        }
    }
}
