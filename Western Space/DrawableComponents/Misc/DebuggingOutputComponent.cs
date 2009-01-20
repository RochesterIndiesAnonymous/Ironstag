using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.DrawableComponents.Misc
{
    public class DebuggingOutputComponent : DrawableGameObject
    {
        private static string spriteBatchName = "Static";

        public static string SpriteBatchName
        {
            get { return DebuggingOutputComponent.spriteBatchName; }
            set { DebuggingOutputComponent.spriteBatchName = value; }
        }

        /// <summary>
        /// The collection of lines to output
        /// </summary>
        private IList<IDebugOutput> debugLines;

        public IList<IDebugOutput> DebugLines
        {
            get { return debugLines; }
            set { debugLines = value; }
        }

        /// <summary>
        /// The x coordinate to start drawing the string.
        /// </summary>
        private const int XOFFSET = 1;

        /// <summary>
        /// The y coordinate to start drawing the string
        /// </summary>
        private const int YOFFSET = 1;

        /// <summary>
        /// The font to use to draw the string
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// The X Y coordinates to draw the font at
        /// </summary>
        private Vector2 fontPos;

        public DebuggingOutputComponent(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
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
            font = this.Game.Content.Load<SpriteFont>("Fonts\\Pala");
            fontPos = new Vector2(XOFFSET, YOFFSET);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < this.debugLines.Count; i++)
            {
                this.SpriteBatch.DrawString(this.font, this.debugLines[i].Output + "\n", new Vector2(XOFFSET, (i + 1) * font.LineSpacing), Color.Red);
            }

            base.Draw(gameTime);
        }
    }
}
