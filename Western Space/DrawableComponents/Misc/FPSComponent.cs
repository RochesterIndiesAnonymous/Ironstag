using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.DrawableComponents.Misc
{
    class FPSComponent : DrawableGameObject
    {
        private const int MIN_SIZE = 1;

        private SpriteFont font;
        private Vector2 fontPos;
        private Vector2 fpsStringPos;

        private SpriteBatch spriteBatch;

        public FPSComponent(Game game)
            : base(game)
        {
        }

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
