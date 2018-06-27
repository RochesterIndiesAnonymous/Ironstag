using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// Calculates the coordinate for a given map based on the mouse position.
    /// </summary>
    public class MapCoordinateComponent : GameObject, IDebugOutput
    {
        /// <summary>
        /// The map that we will be calculating coordinates from
        /// </summary>
        private IMapCoordinates map;

        /// <summary>
        /// The input manager that will have the state of the mouse and keyboard
        /// </summary>
        private IInputManagerService inputManager;

        /// <summary>
        /// The output that will be given to the DebuggingOutputComponent
        /// </summary>
        private string output;

        #region IDebugOutput Members

        /// <summary>
        /// Gets the output that will be printed to the debugging information
        /// </summary>
        public string Output
        {
            get { return output; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this component is tied to</param>
        /// <param name="map">The map we will be using to calculate the coordinates</param>
        public MapCoordinateComponent(Screen parentScreen, IMapCoordinates map)
            : base(parentScreen)
        {
            this.map = map;
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
        /// Gets called on every update cycle to refresh its debugging output
        /// </summary>
        /// <param name="gameTime">The amount of time elapsed relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            Vector2 mapCoordinates = map.CalculateMapCoordinatesFromScreenPoint(new Vector2(inputManager.MouseState.X, inputManager.MouseState.Y));

            this.output = "Map Coords: " + mapCoordinates.X + "," + mapCoordinates.Y;

            base.Update(gameTime);
        }
    }
}
