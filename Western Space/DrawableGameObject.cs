using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace
{
    /// <summary>
    /// Adds additional functionality to DrawableGameComponent necessary for our game
    /// </summary>
    public class DrawableGameObject : DrawableGameComponent
    {
        /// <summary>
        /// The spritebatch object this game object is to use to draw itself
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The spritebatch object this game object is to use to draw itself
        /// </summary>
        protected SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object this component is associated with</param>
        /// <param name="spriteBatch">The sprite batch that this component is to use to draw itself</param>
        public DrawableGameObject(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
        }
    }
}
