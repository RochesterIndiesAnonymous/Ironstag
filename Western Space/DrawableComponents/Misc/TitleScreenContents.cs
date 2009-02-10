using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Input;
using Microsoft.Xna.Framework.Media;

namespace WesternSpace.DrawableComponents.Misc
{
    public class TitleScreenContents : DrawableGameObject
    {
        public static readonly string SpriteBatchName = "Static";

        private static string pressStartString = "Press Space or the A Button";

        private Vector2 pressStartPosition = new Vector2(45, 160);

        private Texture2D titleScreenTexture;

        private SpriteFont font;

        private Song bgm;

        private bool displayStartString;

        public TitleScreenContents(Screen parentScreen, SpriteBatch batch, Vector2 position)
            : base(parentScreen, batch, position)
        {
            displayStartString = true;

            pressStartPosition += position;
        }

        public override void Initialize()
        {
            ITextureService textureService = (ITextureService)this.Game.Services.GetService(typeof(ITextureService));

            titleScreenTexture = textureService.GetTexture("System\\Window Skin\\TitleImage");

            font = this.Game.Content.Load<SpriteFont>("System\\Fonts\\WesternSpaceFont");

            bgm = this.Game.Content.Load<Song>("System\\Music\\TitleScreenBGM");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputMonitor.Instance.WasJustReleased(InputMonitor.PAUSE) || InputMonitor.Instance.WasJustReleased(InputMonitor.JUMP))
            {
                displayStartString = false;
                MediaPlayer.Stop();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(titleScreenTexture, this.Position, Color.White);

            if (this.displayStartString)
            {
                this.SpriteBatch.DrawString(font, pressStartString, this.pressStartPosition, Color.Red);
            }

            base.Draw(gameTime);
        }
    }
}
