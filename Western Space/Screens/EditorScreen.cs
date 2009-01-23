using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Misc;
using Microsoft.Xna.Framework;
using WesternSpace.TilingEngine;
using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.AnimationFramework;
using WesternSpace.DrawableComponents.Actors;

using WesternSpace.DrawableComponents.EditorUI;
using System.Drawing;

namespace WesternSpace.Screens
{
    public class EditorScreen : GameObject
    {
        private TileEngine tileEngine;
        private ISpriteBatchService batchService;
        private World world;

        public World World
        {
            get { return world; }
        }

        public EditorScreen(Game game)
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
            tileEngine = new TileEngine();

            // CreateSprites needs the animation data service
            // animationDataService = (IAnimationDataService)this.Game.Services.GetService(typeof(IAnimationDataService));
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));

            world = new World(this.Game, "WorldXML\\TestWorld");

            CreateDebuggingInformationComponents();
            CreateUIComponents();

            // Initialize all components
            base.Initialize();
        }

        private void CreateUIComponents()
        {
            SpriteBatch sb = batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName);
            TextureSelector ts = new TextureSelector(this.Game, sb, new RectangleF(20, 20, 100, 100));
            ts.DrawOrder = 400;
            this.Game.Components.Add(ts);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our Debugging output component
            Vector2 position = new Vector2(1, 1);
            DebuggingOutputComponent doc = new DebuggingOutputComponent(this.Game, batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName), position);
            doc.UpdateOrder = 4;
            doc.DrawOrder = 400;
            this.Game.Components.Add(doc);
            // Create our FPSComponent
            FPSComponent fps = new FPSComponent(this.Game);
            fps.DrawOrder = 3;
            this.Game.Components.Add(fps);
            doc.DebugLines.Add(fps);

            MouseScreenCoordinatesComponent mscc = new MouseScreenCoordinatesComponent(this.Game);
            mscc.UpdateOrder = 3;
            this.Game.Components.Add(mscc);
            doc.DebugLines.Add(mscc);

            MouseWorldCoordinatesComponent mwcc = new MouseWorldCoordinatesComponent(this.Game);
            mwcc.UpdateOrder = 3;
            this.Game.Components.Add(mwcc);
            doc.DebugLines.Add(mwcc);

            MapCoordinateComponent mcc = new MapCoordinateComponent(this.Game, World.layers[0]);
            mcc.UpdateOrder = 3;
            this.Game.Components.Add(mcc);
            doc.DebugLines.Add(mcc);
        }

    }
}
