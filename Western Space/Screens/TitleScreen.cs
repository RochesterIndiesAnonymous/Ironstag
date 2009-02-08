using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
    }
}
