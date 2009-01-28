using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using WesternSpace.Interfaces;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Measures the current frames per second and reports it to the debugging component
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
        /// TODO
        /// </summary>
        /// <param name="parentScreen"></param>
        public FPSComponent(Screen parentScreen)
            : base(parentScreen, null, Vector2.Zero)
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
