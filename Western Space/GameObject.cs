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
using WesternSpace.Interfaces;

namespace WesternSpace
{
    /// <summary>
    /// Adds additional functionality to GameComponent necessary for our game
    /// </summary>
    public class GameObject : GameComponent, IScreenComponent
    {
        private Screen parentScreen;

        public GameObject(Screen parentScreen)
            : base(ScreenManager.Instance)
        {
            ParentScreen = parentScreen;
        }

        #region IScreenComponent Members

        public Screen ParentScreen
        {
            get
            {
                return parentScreen;
            }
            set
            {
                parentScreen = value;
            }
        }

        #endregion
    }
}