using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WesternSpace.TilingEngine;
using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.AnimationFramework;
using WesternSpace.DrawableComponents.Actors;
using System.Xml.Linq;

using WesternSpace.Collision;
using WesternSpace.DrawableComponents.GameUI;
using WesternSpace.Utility;
using WesternSpace.Input;

namespace WesternSpace.Screens
{
    public class GameScreen : Screen
    {
        public static readonly string ScreenName = "Game";

        private TileEngine tileEngine;
        private ISpriteBatchService batchService;
        private World world;

        public World World
        {
            get { return world; }
        }

        public GameScreen(Game game, string name)
            : base(game, name)
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
            if (!this.IsInitialized)
            {
                tileEngine = new TileEngine();

                // CreateSprites needs the animation data service
                // animationDataService = (IAnimationDataService)this.Game.Services.GetService(typeof(IAnimationDataService));
                batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));

                CreateLayerComponents();

                CreateSprites();

                //CreateDebuggingInformationComponents();

                // Initialize all components
                base.Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void CreateSprites()
        {
            GameUI ui = new GameUI(this, batchService.GetSpriteBatch(GameUI.SpriteBatchName), new Vector2(0f, 0f), world.Player);
            ui.DrawOrder = -10;
            this.Components.Add(ui);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our Debugging output component
            Vector2 position = new Vector2(1,1);
            DebuggingOutputComponent doc = new DebuggingOutputComponent(this, batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName), position);
            doc.UpdateOrder = 4;
            doc.DrawOrder = 400;
            this.Components.Add(doc);
            
            // Create our FPSComponent
            FPSComponent fps = new FPSComponent(this);
            fps.DrawOrder = 3;
            this.Components.Add(fps);
            doc.DebugLines.Add(fps);

            MouseScreenCoordinatesComponent mscc = new MouseScreenCoordinatesComponent(this);
            mscc.UpdateOrder = 3;
            this.Components.Add(mscc);
            doc.DebugLines.Add(mscc);

            MouseWorldCoordinatesComponent mwcc = new MouseWorldCoordinatesComponent(this);
            mwcc.UpdateOrder = 3;
            this.Components.Add(mwcc);
            doc.DebugLines.Add(mwcc);

            MapCoordinateComponent mcc = new MapCoordinateComponent(this, World.interactiveLayers.Values.First<TileMapLayer>());
            mcc.UpdateOrder = 3;
            this.Components.Add(mcc);
            doc.DebugLines.Add(mcc);

            /* What was this for?
             doc.DebugLines.Add(spriteCollisionManager);
             */

        }

        public override void TransitionComplete()
        {
            World.Paused = false;
            base.TransitionComplete();
        }

        private void CreateLayerComponents()
        {
            // Create our World
            world = new World(this, "WorldXML\\TestWorld");
            this.Components.Add(world);
        }
    }
}
