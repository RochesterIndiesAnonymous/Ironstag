using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    public class MouseWorldCoordinatesComponent : GameObject, IDebugOutput
    {
        /// <summary>
        /// The camera that is used to calculate the mouse world coordinates
        /// </summary>
        private ICameraService camera;

        /// <summary>
        /// The input manager to get the state of the mouse
        /// </summary>
        private IInputManagerService inputManager;

        /// <summary>
        /// The output that will be printed to the debugging area
        /// </summary>
        private string output;

        #region IDebugOutput Members

        /// <summary>
        /// Gets the output that will be printed to the debugging area
        /// </summary>
        public string Output
        {
            get { return output; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game object this component is associated with</param>
        public MouseWorldCoordinatesComponent(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Initializes the internal state of the component
        /// </summary>
        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));

            base.Initialize();
        }


        /// <summary>
        /// Updates the output of the component
        /// </summary>
        /// <param name="gameTime">The time realative to the game</param>
        public override void Update(GameTime gameTime)
        {
            this.output = "Mouse World Coords: " + inputManager.BetterMouse.WorldPosition;

            base.Update(gameTime);
        }
    }
}
