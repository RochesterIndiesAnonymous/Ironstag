using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    class MouseScreenCoordinatesComponent : DrawableGameObject
    {
        private IInputManagerService inputManager;

        private const int MIN_SIZE = 1;

        private SpriteFont font;
        private Vector2 fontPos;
        private Vector2 stringPos;

        private Vector2 mouseCoordinates;

        private SpriteBatch spriteBatch;

        public MouseScreenCoordinatesComponent(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            font = this.Game.Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE + font.LineSpacing + font.LineSpacing);
            stringPos = new Vector2(fontPos.X, fontPos.Y + font.LineSpacing);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            mouseCoordinates = new Vector2(inputManager.MouseState.X, inputManager.MouseState.Y);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string output = "Screen Coordinates: " + mouseCoordinates.X + ", " + mouseCoordinates.Y;
            spriteBatch.DrawString(font, output, stringPos, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
