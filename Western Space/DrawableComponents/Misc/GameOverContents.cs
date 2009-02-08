using Microsoft.Xna.Framework;
using WesternSpace.Input;
using WesternSpace.Screens;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.DrawableComponents.Misc
{
    public class GameOverContents : DrawableGameObject
    {
        public static readonly string SpriteBatchName = "Static";

        private static string pressStartString = "Press Space or the A Button";

        private Vector2 pressStartPosition = new Vector2(50, 180);

        private Texture2D titleScreenTexture;

        private SpriteFont font;

        private bool displayStartString;

        public GameOverContents(Screen parentScreen, SpriteBatch batch, Vector2 position)
            : base(parentScreen, batch, position)
        {
            displayStartString = true;

            pressStartPosition += position;
        }

        public override void Initialize()
        {
            ITextureService textureService = (ITextureService)this.Game.Services.GetService(typeof(ITextureService));

            titleScreenTexture = textureService.GetTexture("System\\Window Skin\\GameOverImage");

            font = this.Game.Content.Load<SpriteFont>("System\\Fonts\\WesternSpaceFont");

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputMonitor.Instance.WasJustReleased(InputMonitor.PAUSE) || InputMonitor.Instance.WasJustReleased(InputMonitor.JUMP))
            {
                displayStartString = false;

                ScreenTransition sts = new ScreenTransition("Game", "Game", 0.01f, 0.01f, true);

                ScreenManager.Instance.Transition(sts);
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
