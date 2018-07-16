using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Interfaces;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Outputs debugging information to the given position on the screen.
    /// </summary>
    public class DebuggingOutputComponent : DrawableGameObject
    {
        /// <summary>
        /// The name of the sprite batch that this class uses to draw itself
        /// </summary>
        private static string spriteBatchName = "Static";

        /// <summary>
        /// The name of the sprite batch that this class uses to draw itself
        /// </summary>
        public static string SpriteBatchName
        {
            get { return DebuggingOutputComponent.spriteBatchName; }
            set { DebuggingOutputComponent.spriteBatchName = value; }
        }

        /// <summary>
        /// The collection of lines to output
        /// </summary>
        private IList<IDebugOutput> debugLines;

        /// <summary>
        /// The collection of lines to output
        /// </summary>
        public IList<IDebugOutput> DebugLines
        {
            get { return debugLines; }
            set { debugLines = value; }
        }

        /// <summary>
        /// The font to use to draw the string
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object this component is associated with</param>
        /// <param name="spriteBatch">The sprite batch instance to use to draw on the screen</param>
        /// <param name="position">The position of this component on the screen</param>
        public DebuggingOutputComponent(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position)
            : base(parentScreen, spriteBatch, position)
        {
            debugLines = new List<IDebugOutput>();
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        public override void Initialize()
        {
            // turn off calling update
            this.Enabled = false;

            //Setup the font
            font = this.Game.Content.Load<SpriteFont>("System\\Fonts\\Pala");

            base.Initialize();
        }

        /// <summary>
        /// Draws the debugging information to the screen
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < this.debugLines.Count; i++)
            {
                this.SpriteBatch.DrawString(this.font, this.debugLines[i].Output + "\n", new Vector2(this.Position.X, (i + 1) * font.LineSpacing), Color.Red);
            }

            base.Draw(gameTime);
        }
    }
}
