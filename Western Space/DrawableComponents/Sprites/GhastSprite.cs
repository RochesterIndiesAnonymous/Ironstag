using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Utility;
using WesternSpace.Services;

namespace WesternSpace.DrawableComponents.Sprites
{
    /// <summary>
    /// Animates Ghast using the appropriate animation data
    /// </summary>
    class GhastSprite : AnimatedComponent
    {
        /// <summary>
        /// The name of the asset that has the animation data for this sprite
        /// </summary>
        private static string xmlAssetName = "SpriteXML\\Ghast";

        public static string XmlAssetName
        {
            get { return GhastSprite.xmlAssetName; }
        }

        /// <summary>
        /// The name of the sprite batch that this sprite needs to use to draw.
        /// </summary>
        private static string spriteBatchName = "Camera Sensitive";

        /// <summary>
        /// The name of the sprite batch that this sprite needs to use to draw.
        /// </summary>
        public static string SpriteBatchName
        {
            get { return GhastSprite.spriteBatchName; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this sprite is associated with</param>
        /// <param name="spriteBatch">The SpriteBatch this sprite is to use to draw itself</param>
        /// <param name="data">The animation data used to animate the sprite</param>
        public GhastSprite(Game game, SpriteBatch spriteBatch, AnimationData data)
            : base(game, spriteBatch, data)
        {

        }

        /// <summary>
        /// Sets up the initial animation
        /// </summary>
        public override void Initialize()
        {
            this.SetFrame("Slash", 0);

            base.Initialize();
        }

        /// <summary>
        /// Updates the animation sequence
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.IsTimeForNextFrame)
            {
                this.IncrementFrame();
            }
        }

        /// <summary>
        /// Draws the sprite to the screen
        /// </summary>
        /// <param name="gameTime">The time realtive to the game</param>
        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(this.AnimationData.SpriteSheet, new Vector2(400, 300), 
                this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White);

            base.Draw(gameTime);
        }
    }
}
