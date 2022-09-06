using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Screens;
using WesternSpace.Interfaces;

namespace WesternSpace
{
    /// <summary>
    /// Adds additional functionality to DrawableGameComponent necessary for our game
    /// </summary>
    public class DrawableGameObject : DrawableGameComponent, IScreenComponent
    {
        /// <summary>
        /// Used for the implementation of IScreenComponent
        /// </summary>
        private Screen parentScreen;

        /// <summary>
        /// The spritebatch object this game object is to use to draw itself
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The spritebatch object this game object is to use to draw itself
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            protected set { spriteBatch = value; }
        }

        /// <summary>
        /// The coordinates that this component is to be drawn at
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// The coordinates that this component is to be drawn at
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object this component is associated with</param>
        /// <param name="spriteBatch">The sprite batch that this component is to use to draw itself</param>
        /// <param name="position">The coordinates that this component is to be drawn at</param>
        public DrawableGameObject(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position)
            : base(ScreenManager.Instance)
        {
            ParentScreen = parentScreen;
            this.spriteBatch = spriteBatch;
            this.position = position;
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
