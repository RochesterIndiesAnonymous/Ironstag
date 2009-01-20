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
using WesternSpace.DrawableComponents.Sprites;

namespace WesternSpace.Screens
{
    public class GameScreen : DrawableGameObject
    {
        private TileEngine tileEngine;
        private IAnimationDataService animationDataService;
        private ISpriteBatchService batchService;
        private World world;

        public World World
        {
            get { return world; }
        }

        public GameScreen(Game game)
            : base(game, null)
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
            animationDataService = (IAnimationDataService)this.Game.Services.GetService(typeof(IAnimationDataService));
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));

            CreateLayerComponents();

            CreateSprites();

            CreateDebuggingInformationComponents();
            
            // Initialize all components
            base.Initialize();
        }

        private void CreateSprites()
        {
            AnimationData data = animationDataService.GetAnimationData(DiddyKongSprite.XmlAssetName);
            AnimatedComponent diddyComponent = new DiddyKongSprite(this.Game, batchService.GetSpriteBatch(DiddyKongSprite.SpriteBatchName), data);
            diddyComponent.UpdateOrder = 2;
            diddyComponent.DrawOrder = -20;
            this.Game.Components.Add(diddyComponent);

            AnimationData data2 = animationDataService.GetAnimationData(ToadManSprite.XmlAssetName);
            AnimatedComponent toadmanComponent = new ToadManSprite(this.Game, batchService.GetSpriteBatch(ToadManSprite.SpriteBatchName), data2);
            toadmanComponent.UpdateOrder = 2;
            toadmanComponent.DrawOrder = 300;
            this.Game.Components.Add(toadmanComponent);

            AnimationData data3 = animationDataService.GetAnimationData(GhastSprite.XmlAssetName);
            AnimatedComponent ghastComponent = new GhastSprite(this.Game, batchService.GetSpriteBatch(GhastSprite.SpriteBatchName), data3);
            ghastComponent.UpdateOrder = 2;
            ghastComponent.DrawOrder = 300;
            this.Game.Components.Add(ghastComponent);

            AnimationData data4 = animationDataService.GetAnimationData(SunsetSprite.XmlAssetName);
            AnimatedComponent sunsetComponent = new SunsetSprite(this.Game, batchService.GetSpriteBatch(SunsetSprite.SpriteBatchName), data4);
            sunsetComponent.UpdateOrder = 2;
            sunsetComponent.DrawOrder = 300;
            this.Game.Components.Add(sunsetComponent);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our Debugging output component
            DebuggingOutputComponent doc = new DebuggingOutputComponent(this.Game, batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName));
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
        }

        private void CreateLayerComponents()
        {
            // Create our World
            world = new World(this.Game, "WorldXML\\TestWorld");

            /*TileMapLayer background = new TileMapLayer(this.Game, World.Map, 0, 0.25f);
            background.DrawOrder = -10;

            this.Game.Components.Add(background);
            */

            MapCoordinateComponent mcc = new MapCoordinateComponent(this.Game, World.layers[0]);
            mcc.UpdateOrder = 3;
            this.Game.Components.Add(mcc);
        }
    }
}
