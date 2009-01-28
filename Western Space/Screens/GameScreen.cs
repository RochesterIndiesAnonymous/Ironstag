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
using System.Xml.Linq;
using WesternSpace.Collision;

namespace WesternSpace.Screens
{
    public class GameScreen : Screen
    {
        public static readonly string ScreenName = "Game";

        private TileEngine tileEngine;
        private ISpriteBatchService batchService;
        private World world;
        public SpriteTileCollisionManager tileCollisionManager;
        public SpriteSpriteCollisionManager spriteCollisionManager;

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

                CreateDebuggingInformationComponents();

                // Initialize all components
                base.Initialize();
            }
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

            ToadMan toadMan = new ToadMan(this, batchService.GetSpriteBatch(Character.SpriteBatchName), new Vector2(100, 150), "SpriteXML\\FlintIronstag");
            toadMan.UpdateOrder = 3;
            toadMan.DrawOrder = 0;
            this.Components.Add(toadMan);

            Player flint = new Player(this, batchService.GetSpriteBatch(Character.SpriteBatchName), new Vector2(251, 79), "SpriteXML\\ToadMan");
            flint.UpdateOrder = 3;
            flint.DrawOrder = 0;
            this.Components.Add(flint);
            
            tileCollisionManager = new SpriteTileCollisionManager(this.Game, this.world);
            tileCollisionManager.addObjectToList(flint);
            this.Components.Add(tileCollisionManager);

            
            spriteCollisionManager = new SpriteSpriteCollisionManager(this.Game, new Point(40, 40));
            spriteCollisionManager.RegisterGameObject(flint);
            spriteCollisionManager.RegisterGameObject(toadMan);
            this.Components.Add(spriteCollisionManager);
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

            doc.DebugLines.Add(spriteCollisionManager);

        }

        private void CreateLayerComponents()
        {
            // Create our World
            world = new World(this, "WorldXML\\TestWorld");

            /*TileMapLayer background = new TileMapLayer(this.Game, World.Map, 0, 0.25f);
            background.DrawOrder = -10;

            this.Game.Components.Add(background);
            */
        }
    }
}
