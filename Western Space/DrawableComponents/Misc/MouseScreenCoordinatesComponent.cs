using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Outputs the mouse coordinates to the debugging information
    /// </summary>
    public class MouseScreenCoordinatesComponent : GameObject, IDebugOutput
    {
        /// <summary>
        /// The inputmanager to get the state of the mouse
        /// </summary>
        private IInputManagerService inputManager;

        /// <summary>
        /// The string that will be printed to the debugging area
        /// </summary>
        private string output;

        #region IDebugOutput Members

        /// <summary>
        /// Gets the string that will be printed out to the screen.
        /// </summary>
        public string Output
        {
            get { return output; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this component is associated with</param>
        public MouseScreenCoordinatesComponent(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Sets up the internal state of the component
        /// </summary>
        public override void Initialize()
        {
            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));

            base.Initialize();
        }

        /// <summary>
        /// Updates the output to the debugging area on every update cycle
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            Vector2 mouseCoordinates = new Vector2(inputManager.MouseState.X, inputManager.MouseState.Y);
            this.output = "Screen Coords: " + mouseCoordinates.X + ", " + mouseCoordinates.Y;

            base.Update(gameTime);
        }
    }
}
