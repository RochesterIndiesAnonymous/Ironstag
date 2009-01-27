using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using WesternSpace.Screens;

namespace WesternSpace
{
    /// <summary>
    /// Adds additional functionality to GameComponent necessary for our game
    /// </summary>
    public class GameObject : GameComponent
    {
        private Screen parentScreen;

        public Screen ParentScreen
        {
            get { return parentScreen; }
            set { parentScreen = value; }
        }

        public GameObject(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}