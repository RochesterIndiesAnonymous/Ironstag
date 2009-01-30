using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.AnimationFramework;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.DrawableComponents.EditorUI;
using WesternSpace.DrawableComponents.Misc;
using WesternSpace.Screens;
using WesternSpace.TilingEngine;
using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Input;

namespace WesternSpace.Screens
{
    public class EditorScreen : Screen
    {
        public static readonly float CAM_SPEED = 14.0f;

        public static readonly string ScreenName = "Editor";

        private TileEngine tileEngine;
        private ISpriteBatchService batchService;
        private World world;

        private InputMonitor inputMonitor;

        public World World
        {
            get { return world; }
        }

        public EditorScreen(Game game, string name)
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

                world = new World(this, "WorldXML\\TestWorld");
                world.interactiveLayers.Values.Last<TileMapLayer>().DrawBlanksEnabled = true;
                world.interactiveLayers.Values.Last<TileMapLayer>().DrawEdgesEnabled = true;
                world.Pause();

                Components.Add(world);

                //CreateDebuggingInformationComponents();
                CreateUIComponents();

                inputMonitor = new InputMonitor(ScreenManager.Instance);
                inputMonitor.assignKey("EditorLeft", Microsoft.Xna.Framework.Input.Keys.A);
                inputMonitor.assignKey("EditorRight", Microsoft.Xna.Framework.Input.Keys.D);
                inputMonitor.assignKey("EditorUp", Microsoft.Xna.Framework.Input.Keys.W);
                inputMonitor.assignKey("EditorDown", Microsoft.Xna.Framework.Input.Keys.S);
                Components.Add(inputMonitor);

                // Initialize all components
                base.Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Let keypresses move the camera:
            if(inputMonitor.checkKey("EditorLeft"))
            {
                world.Camera.Position -= new Vector2(CAM_SPEED, 0);
            }
            else if (inputMonitor.checkKey("EditorRight"))
            {
                world.Camera.Position += new Vector2(CAM_SPEED, 0);
            }
            if (inputMonitor.checkKey("EditorUp"))
            {
                world.Camera.Position -= new Vector2(0, CAM_SPEED);
            }
            else if (inputMonitor.checkKey("EditorDown"))
            {
                world.Camera.Position += new Vector2(0, CAM_SPEED);
            }

            base.Update(gameTime);
        }

        private void CreateUIComponents()
        {
            SpriteBatch sb = batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName);

            // Where all the magic happens:
            TileSelector ts = new TileSelector(this, sb, new RectangleF(40, 0, 600, 440), world.interactiveLayers.Values.First<TileMapLayer>());
            ts.DrawOrder = 400;
            this.Components.Add(ts);

            SaveButton saveButton = new SaveButton(this, sb, new RectangleF(5, 400, 30, 15), ts);
            saveButton.DrawOrder = 401;
            this.Components.Add(saveButton);
        }

        private void CreateDebuggingInformationComponents()
        {
            // Create our Debugging output component
            Vector2 position = new Vector2(1, 1);
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

            MapCoordinateComponent mcc = new MapCoordinateComponent(this, World.interactiveLayers[0]);
            mcc.UpdateOrder = 3;
            this.Components.Add(mcc);
            doc.DebugLines.Add(mcc);
        }
    }
}
