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

namespace WesternSpace
{
    /*
     * Default Game Class
     */
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
        PlayerObject megaFly; 
        List<FlyingObject> flyObjList;

        // lra4691 - All sprite will be contained in the following
        //            dictionary as Texture2Ds where the key for them
        //            is their filename without the "Sprites\\" part
        //            or extension.
        public Dictionary<String, Texture2D> textures;
        

        // tjw6445 - Variables for maintaining and displaying framerate. 
        double fps;
        Vector2 fpsStringPos;

        /*
         * Default Constructor
         */
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            flyObjList = new List<FlyingObject>();
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

            //Get "old" keyboard state
            oldState = Keyboard.GetState();

            //Setup cursor
            mouseCursor = new Cursor(textures["1down"]);
        }

        /*
         * LoadContent will be called once per game and is the place to load
         * all of your content.
         */ 
        protected override void LoadContent()
        {
            // lra4691 - Load each image from the "Sprites" content directory.
            //           NOTE: Here we assume all files in "Sprites" can be loaded
            //                  properly with Content.Load<Texture2D>()!

            DirectoryInfo di = new DirectoryInfo("Content\\Sprites");
            FileInfo[] files = di.GetFiles("*.xnb");

            foreach (FileInfo file in files) 
            {
                String path = file.Name.Substring(0, file.Name.Length - 4);
                textures.Add(path, Content.Load<Texture2D>("Sprites\\"+path));
            }

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Setup the font
            font = Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE);
            fpsStringPos = new Vector2(fontPos.X, fontPos.Y + font.LineSpacing);

            megaFly = new PlayerObject(this, "rushFrown");

            //Create 5000 game objects and add them to a list.
            for (int count = 0; count < 2500; count++)
            {
                GameObject megaSmile = new GameObject(this, "1up");
                GameObject megaFrown = new GameObject(this, "1down");
                megaSmile.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                 random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                megaFrown.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                 random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                gameObjList.Add(megaSmile);
                gameObjList.Add(megaFrown);
            }

            //Create 20 flying graphics and add them to a list.
            for (int count = 0; count < 10; count++)
            {
                FlyingObject metaFly = new FlyingObject(this, "flyingMeta", new Vector2(0, 0), new Vector2(-1, 0),
                    Content.Load<SoundEffect>("Sounds\\hBump"), Content.Load<SoundEffect>("Sounds\\wBump") );
                metaFly.position = new Vector2(random.Next(MIN_SIZE + 1, graphics.GraphicsDevice.Viewport.Width-(metaFly.size.Width-1)),
                                               random.Next(MIN_SIZE + 1, graphics.GraphicsDevice.Viewport.Height-(metaFly.size.Height-1)));
                FlyingObject copterJoe = new FlyingObject(this, "CopterJoe", new Vector2(random.Next(MIN_SIZE, graphics.GraphicsDevice.Viewport.Width),
                                                                      random.Next(MIN_SIZE, graphics.GraphicsDevice.Viewport.Height)), new Vector2(0, -1),
                                                                      Content.Load<SoundEffect>("Sounds\\hBump"), Content.Load<SoundEffect>("Sounds\\wBump"));
                copterJoe.position = new Vector2(random.Next(MIN_SIZE+1, graphics.GraphicsDevice.Viewport.Width-(copterJoe.size.Width-1)), 
                                                 random.Next(MIN_SIZE+1, graphics.GraphicsDevice.Viewport.Height-(copterJoe.size.Height-1)));
                flyObjList.Add(metaFly);
                flyObjList.Add(copterJoe);
            }

            //Setup the music
            rave = Content.Load<Song>("Music\\rave");
            MediaPlayer.Play(rave);
            MediaPlayer.Pause();
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

            //Get new input from the user
            updateInput();

            //Update Mouse Position
            mouseCursor.Update();

            //Update the player character
            megaFly.Update(gameTime);

            //Should be handled in the Update method of the particular object, but I did not pass the graphics object to that class.
            megaFly.position.X = MathHelper.Clamp(megaFly.position.X, MIN_SIZE, graphics.GraphicsDevice.Viewport.Width - megaFly.size.Width);
            megaFly.position.Y = MathHelper.Clamp(megaFly.position.Y, MIN_SIZE, graphics.GraphicsDevice.Viewport.Height - megaFly.size.Height);

            //Update the movement for all of the Flying objects --Clamp should probably go in their Update as well.
            foreach (FlyingObject currObj in flyObjList)
            {
                currObj.UpdateMovement(gameTime, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                currObj.position.Y = MathHelper.Clamp(currObj.position.Y, MIN_SIZE, graphics.GraphicsDevice.Viewport.Height - currObj.size.Height);
            }
            
            base.Update(gameTime);
        }

        /*
         * This is called when the game should draw itself.
         *
         * <param name="gameTime">Provides a snapshot of timing values.</param>
         */
        protected override void Draw(GameTime gameTime)
        {
            // tjw6445 - Calculate frames per second. 
            fps = 1000/gameTime.ElapsedGameTime.TotalMilliseconds;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw the 5000 background sprites
            foreach (GameObject currObj in gameObjList)
            {
                if (raveParty)
                {
                    //If it is time for an amazing rave party, randomly update the positions for each sprite
                    currObj.position = new Vector2(random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Width),
                                                   random.Next(MIN_SIZE, this.graphics.GraphicsDevice.Viewport.Height));
                }
                    currObj.Draw(this.spriteBatch);
            }

            //Draw the Font
            String output = mouseCursor.getPosition().X +", "+ mouseCursor.getPosition().Y;
            spriteBatch.DrawString(font, output, fontPos, Color.Red);

            // tjw6445 - Display the framerate.
            output = "FPS: " + fps;
            spriteBatch.DrawString(font, output, fpsStringPos, Color.Red);

            //Draw all of the flying objects in their list
            foreach (FlyingObject currObj in flyObjList)
            {
                currObj.Draw(this.spriteBatch);
            }

            //Draw the player character object
            megaFly.Draw(spriteBatch);

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
                mouseCursor.Sprite = (Content.Load<Texture2D>("Sprites\\1up"));
                megaFly.Sprite = (Content.Load<Texture2D>("Sprites\\rushSmile"));
                MediaPlayer.Resume();
            }
            else
            {
                mouseCursor.Sprite = (Content.Load<Texture2D>("Sprites\\1down"));
                megaFly.Sprite = (Content.Load<Texture2D>("Sprites\\rushFrown"));
                MediaPlayer.Pause();
            }
        }
    }
}
