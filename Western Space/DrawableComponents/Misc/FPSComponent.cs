using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Draws the current frames per second at the specific coordinates
    /// </summary>
    public class FPSComponent : DrawableGameObject, IDebugOutput
    {
        private string output;

        #region IDebugOutput Members

        /// <summary>
        /// The output that will be printed by the DebuggingOutputComponent
        /// </summary>
        public string Output
        {
            get { return output; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object that this component is part of</param>
        public FPSComponent(Game game)
            : base(game, null)
        {
        }

        public override void Initialize()
        {
            // turn off calling update
            this.Enabled = false;

            base.Initialize();
        }

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="gameTime">The amount of time elapsed since the last draw call</param>
        public override void Draw(GameTime gameTime)
        {
            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
            this.output = "FPS: " + Math.Ceiling(fps);

            base.Draw(gameTime);
        }


        
    }
}
