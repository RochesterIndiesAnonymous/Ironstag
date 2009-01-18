using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Draws the current frames per second at the specific coordinates
    /// </summary>
    class FPSComponent : DrawableGameObject
    {
        /// <summary>
        /// The coordinates to draw the string at.
        /// </summary>
        private const int MIN_SIZE = 1;

        /// <summary>
        /// The font to use to draw the string
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// The X Y coordinates to draw the font at
        /// </summary>
        private Vector2 fontPos;

        /// <summary>
        /// The calculated position of string
        /// </summary>
        private Vector2 fpsStringPos;

        /// <summary>
        /// The batch used to draw the string to the screen
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object that this component is part of</param>
        public FPSComponent(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        public override void Initialize()
        {
            // turn off calling update
            this.Enabled = false;

            //Setup the font
            font = this.Game.Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE);
            fpsStringPos = new Vector2(fontPos.X, fontPos.Y + font.LineSpacing);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="gameTime">The amount of time elapsed since the last draw call</param>
        public override void Draw(GameTime gameTime)
        {
            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;

            spriteBatch.Begin();

            string output = "FPS: " + Math.Ceiling(fps);
            spriteBatch.DrawString(font, output, fpsStringPos, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
    }
}
