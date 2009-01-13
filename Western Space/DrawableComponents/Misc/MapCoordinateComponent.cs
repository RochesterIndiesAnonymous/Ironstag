using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    class MapCoordinateComponent : DrawableGameObject
    {
        private IMapCoordinates map;
        private ICamera camera;

        private const int MIN_SIZE = 1;

        private SpriteFont font;
        private Vector2 fontPos;
        private Vector2 stringPos;

        private Vector2 mapCoordinates;

        private SpriteBatch spriteBatch;

        public MapCoordinateComponent(Game game, IMapCoordinates map)
            : base(game)
        {
            this.map = map;
        }

        public override void Initialize()
        {
            font = this.Game.Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(MIN_SIZE, MIN_SIZE + font.LineSpacing);
            stringPos = new Vector2(fontPos.X, fontPos.Y + font.LineSpacing);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            camera = (ICamera)this.Game.Services.GetService(typeof(ICamera));

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            mapCoordinates = camera.GetMapCoordinates(map);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string output = "Map Coordinates: " + mapCoordinates.X + ", " + mapCoordinates.Y;
            spriteBatch.DrawString(font, output, stringPos, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
