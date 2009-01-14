using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    class MouseWorldCoordinatesComponent : DrawableGameObject
    {
        private ICameraService camera;
        private IInputManagerService inputManager;

        private const int MIN_SIZE = 1;

        private SpriteFont font;
        private Vector2 fontPos;
        private Vector2 stringPos;

        private Vector2 mouseCoordinates;

        private SpriteBatch spriteBatch;

        public MouseWorldCoordinatesComponent(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            font = this.Game.Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE);
            stringPos = new Vector2(fontPos.X, fontPos.Y);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            mouseCoordinates = new Vector2(camera.Position.X + camera.Offset.X + inputManager.MouseState.X, camera.Position.Y + camera.Offset.Y + inputManager.MouseState.Y);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string output = "Mouse World Coordinates: " + mouseCoordinates.X + ", " + mouseCoordinates.Y;
            spriteBatch.DrawString(font, output, stringPos, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
