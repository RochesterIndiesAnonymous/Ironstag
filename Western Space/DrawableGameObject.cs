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
        /// The coordinates that this component is to be drawn at
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The coordinates that this component is to be drawn at
        /// </summary>
        protected Vector2 Position
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
        public DrawableGameObject(Game game, SpriteBatch spriteBatch, Vector2 position)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.position = position;
        }
    }
}
