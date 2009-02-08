using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Input;
using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace.Screens
{
    public class TitleScreen : Screen
    {
        public static readonly string ScreenName = "TitleScreen";

        public TitleScreen()
            : base(ScreenManager.Instance, ScreenName)
        {
        }

        public TitleScreen(Game game, string name)
            : base(game, name)
        {

        }

        public override void Initialize()
        {
            ISpriteBatchService batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            TitleScreenContents tsc = new TitleScreenContents(this, batchService.GetSpriteBatch(TitleScreenContents.SpriteBatchName), new Vector2(0, 0));
            this.Components.Add(tsc);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputMonitor.Instance.WasJustReleased(InputMonitor.PAUSE) || InputMonitor.Instance.WasJustReleased(InputMonitor.JUMP))
            {
                ScreenTransition sts = new ScreenTransition("TitleScreen", "Game", 0.01f, 0.01f, false);
                ScreenManager.Instance.Transition(sts);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
