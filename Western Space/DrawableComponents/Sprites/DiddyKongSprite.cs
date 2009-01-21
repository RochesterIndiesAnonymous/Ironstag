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
    /// Animates Diddy Kong using the appropriate animation data
    /// </summary>
    public class DiddyKongSprite : AnimatedComponent
    {
        /// <summary>
        /// The name of the asset that has the animation data for this sprite
        /// </summary>
        private static string xmlAssetName = "SpriteXML\\DiddyKong";

        /// <summary>
        /// The name of the asset that has the animation data for this sprite
        /// </summary>
        public static string XmlAssetName
        {
            get { return DiddyKongSprite.xmlAssetName; }
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
            get { return DiddyKongSprite.spriteBatchName; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game that this sprite is associated with</param>
        /// <param name="spriteBatch">The sprite batch that this sprite is to use to draw itself</param>
        /// <param name="position">The world coordinates of this sprite</param>
        /// <param name="data">The animation information that is used to animate this sprite</param>
        public DiddyKongSprite(Game game, SpriteBatch spriteBatch, Vector2 position, AnimationData data)
            : base(game, spriteBatch, position, data)
        {
            
        }

        /// <summary>
        /// Initializes the animation state
        /// </summary>
        public override void Initialize()
        {
            this.SetFrame("Walk", 0);

            base.Initialize();
        }

        /// <summary>
        /// Manages which frame to show next in the animation sequence
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
        /// <param name="gameTime">The time relative to the game</param>
        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(this.AnimationData.SpriteSheet, this.Position,
                this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White);

            base.Draw(gameTime);
        }
    }
}
