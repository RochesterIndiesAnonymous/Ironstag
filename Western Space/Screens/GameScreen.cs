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

namespace WesternSpace.Screens
{
    public class GameScreen : GameObject
    {
        private TileEngine tileEngine;
        private ISpriteBatchService batchService;
        private World world;

        public World World
        {
            get { return world; }
        }

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
            tileEngine = new TileEngine();

            // CreateSprites needs the animation data service
           // animationDataService = (IAnimationDataService)this.Game.Services.GetService(typeof(IAnimationDataService));
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));

            CreateLayerComponents();

            CreateSprites();

            CreateDebuggingInformationComponents();
            
            // Initialize all components
            base.Initialize();
        }

        private void CreateSprites()
        {
            /*
            AnimationData data = animationDataService.GetAnimationData(DiddyKongSprite.XmlAssetName);
            Vector2 position = new Vector2(500, 500);
            AnimatedComponent diddyComponent = new DiddyKongSprite(this.Game, batchService.GetSpriteBatch(DiddyKongSprite.SpriteBatchName), position, data);
            diddyComponent.UpdateOrder = 3;
            diddyComponent.DrawOrder = 0;
            this.Game.Components.Add(diddyComponent);

            AnimationData data2 = animationDataService.GetAnimationData(ToadManSprite.XmlAssetName);
            position = new Vector2(1000, 200);
            AnimatedComponent toadmanComponent = new ToadManSprite(this.Game, batchService.GetSpriteBatch(ToadManSprite.SpriteBatchName), position, data2);
            toadmanComponent.UpdateOrder = 3;
            toadmanComponent.DrawOrder = 0;
            this.Game.Components.Add(toadmanComponent);

            AnimationData data3 = animationDataService.GetAnimationData(GhastSprite.XmlAssetName);
            position = new Vector2(400, 300);
            AnimatedComponent ghastComponent = new GhastSprite(this.Game, batchService.GetSpriteBatch(GhastSprite.SpriteBatchName), position, data3);
            ghastComponent.UpdateOrder = 3;
            ghastComponent.DrawOrder = 0;
            this.Game.Components.Add(ghastComponent);

            AnimationData data4 = animationDataService.GetAnimationData(SunsetSprite.XmlAssetName);
            position = new Vector2(200, 1500);
            AnimatedComponent sunsetComponent = new SunsetSprite(this.Game, batchService.GetSpriteBatch(SunsetSprite.SpriteBatchName), position, data4);
            sunsetComponent.UpdateOrder = 3;
            sunsetComponent.DrawOrder = 0;
            this.Game.Components.Add(sunsetComponent);
             */
            ToadMan toadMan = new ToadMan(this.Game, batchService.GetSpriteBatch(Character.SpriteBatchName), new Vector2(100, 100), "SpriteXML\\ToadMan");
            toadMan.UpdateOrder = 3;
            toadMan.DrawOrder = 0;
            this.Game.Components.Add(toadMan);

            Player flint = new Player(this.Game, batchService.GetSpriteBatch(Character.SpriteBatchName), new Vector2(300, 100), "SpriteXML\\ToadMan");
            flint.UpdateOrder = 3;
            flint.DrawOrder = 0;
            this.Game.Components.Add(flint);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our Debugging output component
            Vector2 position = new Vector2(1,1);
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

        private void CreateLayerComponents()
        {
            // Create our World
            world = new World(this.Game, "WorldXML\\TestWorld");

            /*TileMapLayer background = new TileMapLayer(this.Game, World.Map, 0, 0.25f);
            background.DrawOrder = -10;

            this.Game.Components.Add(background);
            */
        }
    }
}
