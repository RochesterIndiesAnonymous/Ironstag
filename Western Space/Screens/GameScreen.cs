﻿using System;
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
    class GameScreen : DrawableGameObject
    {
        private TileEngine tileEngine;
        private ILayerService layerService;
        private IAnimationDataService animationDataService;
        private ITextureService textureService;
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

            CreateServices();

            // CreateLayerComponents needs the layer service
            layerService = (ILayerService)this.Game.Services.GetService(typeof(ILayerService));

            // CreateSprites needs the animation data service and texture service
            animationDataService = (IAnimationDataService)this.Game.Services.GetService(typeof(IAnimationDataService));
            textureService = (ITextureService)this.Game.Services.GetService(typeof(ITextureService));

            CreateLayerComponents();

            CreateSprites();

            CreateDebuggingInformationComponents();
            
            // Initialize all components
            base.Initialize();
        }

        private void CreateSprites()
        {
            AnimationData data = animationDataService.GetAnimationData(DiddyKongSprite.XML_NAME);
            AnimatedComponent diddyComponent = new DiddyKongSprite(this.Game, data);
            diddyComponent.UpdateOrder = 2;
            diddyComponent.DrawOrder = 2;
            this.Game.Components.Add(diddyComponent);

            AnimationData data2 = animationDataService.GetAnimationData(ToadManSprite.XML_NAME);
            AnimatedComponent toadmanComponent = new ToadManSprite(this.Game, data2);
            toadmanComponent.UpdateOrder = 2;
            toadmanComponent.DrawOrder = 2;
            this.Game.Components.Add(toadmanComponent);

            AnimationData data3 = animationDataService.GetAnimationData(GhastSprite.XML_NAME);
            AnimatedComponent ghastComponent = new GhastSprite(this.Game, data3);
            ghastComponent.UpdateOrder = 2;
            ghastComponent.DrawOrder = 20;
            this.Game.Components.Add(ghastComponent);

            AnimationData data4 = animationDataService.GetAnimationData(SunsetSprite.XML_NAME);
            AnimatedComponent sunsetComponent = new SunsetSprite(this.Game, data4);
            sunsetComponent.UpdateOrder = 2;
            sunsetComponent.DrawOrder = 2;
            this.Game.Components.Add(sunsetComponent);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our FPSComponent
            FPSComponent fps = new FPSComponent(this.Game);
            fps.DrawOrder = 200;
            this.Game.Components.Add(fps);

            MouseScreenCoordinatesComponent mscc = new MouseScreenCoordinatesComponent(this.Game);
            mscc.UpdateOrder = 3;
            mscc.DrawOrder = 200;
            this.Game.Components.Add(mscc);

            MouseWorldCoordinatesComponent mwcc = new MouseWorldCoordinatesComponent(this.Game);
            mwcc.UpdateOrder = 3;
            mwcc.DrawOrder = 200;
            this.Game.Components.Add(mwcc);
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
            mcc.UpdateOrder = 2;
            mcc.DrawOrder = 200;
            this.Game.Components.Add(mcc);
        }

        private void CreateServices()
        {
            InputManagerService input = new InputManagerService(this.Game);
            input.UpdateOrder = 0;
            this.Game.Services.AddService(typeof(IInputManagerService), input);
            this.Game.Components.Add(input);

            ILayerService layer = new LayerService();
            this.Game.Services.AddService(typeof(ILayerService), layer);

            IAnimationDataService animationDataService = new AnimationDataService();
            this.Game.Services.AddService(typeof(IAnimationDataService), animationDataService);            

            // create and add any necessary components
            CameraService camera = new CameraService(this.Game);
            camera.UpdateOrder = 1;
            this.Game.Services.AddService(typeof(ICameraService), camera);
            this.Game.Components.Add(camera);
        }
    }
}
