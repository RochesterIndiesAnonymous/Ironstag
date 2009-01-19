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
    class SunsetSprite : AnimatedComponent
    {
        public static string XML_NAME = "SpriteXML\\SunSet";

        private ICameraService camera;

        public SunsetSprite(Game game, AnimationData data)
            : base(game, data)
        {

        }

        public override void Initialize()
        {
            this.SetFrame("Spin", 0);
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

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
            sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None, camera.CurrentViewMatrix);

            sb.Draw(this.AnimationData.SpriteSheet, new Vector2(200, 300), this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White);

            sb.End();

            base.Draw(gameTime);
        }
    }
}
