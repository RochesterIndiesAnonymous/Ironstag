using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Misc;
using Microsoft.Xna.Framework;
using WesternSpace.TilingEngine;

namespace WesternSpace.Screens
{
    class GameScreen : DrawableGameObject
    {
        private TileEngine tileEngine;

        public GameScreen(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            tileEngine = new TileEngine(this.Game);

            // create and add any necessary components

            // Create our FPSComponent
            FPSComponent fps = new FPSComponent(this.Game);
            fps.DrawOrder = 1;
            this.Game.Components.Add(fps);

            // Create our tilemap
            TileMap tm = tileEngine.LoadLayer("Layers\\TestLayer", "LayerXML\\TestLayer.xml");
            tm.DrawOrder = 0;
            this.Game.Components.Add(tm);

            // Initialize all components
            base.Initialize();
        }
    }
}
