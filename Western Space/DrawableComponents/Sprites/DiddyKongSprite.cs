using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.DrawableComponents.Sprites
{
    class DiddyKongSprite : AnimatedComponent
    {
        public static string XML_NAME = "SpriteXML\\DiddyKong";

        public DiddyKongSprite(Game game, AnimationData data)
            : base(game, data)
        {

        }

        public override void Initialize()
        {
            this.SetFrame("Walk", 0);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.IsTimeForNextFrame)
            {
                this.IncrementFrame();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sb = new SpriteBatch(this.Game.GraphicsDevice);
            sb.Begin();

            sb.Draw(this.AnimationData.SpriteSheet, new Vector2(500, 500),this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White);

            sb.End();

            base.Draw(gameTime);
        }
    }
}
