using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Misc;
using Microsoft.Xna.Framework;
using WesternSpace.TilingEngine;
using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;

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
            fps.DrawOrder = 2;
            this.Game.Components.Add(fps);

            MouseScreenCoordinatesComponent mscc = new MouseScreenCoordinatesComponent(this.Game);
            mscc.UpdateOrder = 2;
            mscc.DrawOrder = 4;
            this.Game.Components.Add(mscc);

            // Create our tilemap
            TileMap tm = tileEngine.LoadLayer("Layers\\TestLayer", "LayerXML\\TestLayer.xml");
            tm.DrawOrder = 1;
            this.Game.Components.Add(tm);

            MapCoordinateComponent mcc = new MapCoordinateComponent(this.Game, tm);
            mcc.UpdateOrder = 1;
            mcc.DrawOrder = 3;
            this.Game.Components.Add(mcc);

            MouseWorldCoordinatesComponent mwcc = new MouseWorldCoordinatesComponent(this.Game);
            mwcc.UpdateOrder = 2;
            mwcc.DrawOrder = 4;
            this.Game.Components.Add(mwcc);

            Camera camera = new Camera(this.Game);
            camera.UpdateOrder = 0;
            this.Game.Services.AddService(typeof(ICamera), camera);
            this.Game.Components.Add(camera);

            // Initialize all components
            base.Initialize();
        }
    }
}
