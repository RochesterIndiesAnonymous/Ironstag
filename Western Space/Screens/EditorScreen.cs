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
using WesternSpace.Utility;

namespace WesternSpace.Screens
{
    public class EditorScreen : Screen
    {
        public static readonly int MAX_TILEMAP_WIDTH = 500;
        public static readonly int MAX_TILEMAP_HEIGHT = 500;
        public static readonly int LAYER_COUNT = 2;
        public static readonly int SUB_LAYER_COUNT = 2;
        public static readonly int DEFAULT_TILE_WIDTH = 24;
        public static readonly int DEFAULT_TILE_HEIGHT = 24;
        public static readonly int DEFAULT_LAYER_SPACING = 10;

        public static readonly string WORLD_FILENAME = "WorldXML\\TestWorld";

        /// <summary>
        /// The resolution settings to use when the game is running in windowed mode
        /// </summary>
        private static ResolutionSettings windowedSettings = new ResolutionSettings(640, 480, 640, 480, false);

        public static ResolutionSettings WindowedSettings
        {
            get { return EditorScreen.windowedSettings; }
        }

        /// <summary>
        /// The full screen settings to use when using full screen. This is calculated based on the main display of the user
        /// </summary>
        private static ResolutionSettings fullScreenSettings;
        private bool isFullScreen;

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
            isFullScreen = false;
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

                fullScreenSettings = new ResolutionSettings(640, 480, 640,
                                                                      480, 
                                                                      true);

                ScreenManager.Instance.ResolutionService.CurrentResolutionSettings = windowedSettings;

                batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));

                // Create a new, empty world:
                world = new World(this, WORLD_FILENAME);

                
                // Create an empty, maximally-sized tilemap to center
                //  the loaded map onto:
                TileMap bigMap = new TileMap(MAX_TILEMAP_WIDTH, MAX_TILEMAP_HEIGHT, 
                                          DEFAULT_TILE_WIDTH, DEFAULT_TILE_HEIGHT,
                                          LAYER_COUNT, SUB_LAYER_COUNT);
                // Backup the original map:
                TileMap map = world.Map;

                // Compute the point where we want to blit it onto the empty map:
                int offsetX, offsetY;

                offsetX = (bigMap.Width / 2) - (map.Width / 2);
                offsetY = (bigMap.Height / 2) - (map.Height / 2);

                bigMap.BlitTileMap(map, offsetX, offsetY);

                world.Map = bigMap;
                foreach (TileMapLayer tml in world.interactiveLayers.Values)
                {
                    Components.Remove(tml);
                }
                world.interactiveLayers.Clear();
                world.Player.Position += new Vector2(world.Map.TileWidth * offsetX, world.Map.TileWidth * offsetY);

                for (int i = 0; i < world.Map.LayerCount; ++i)
                {
                    TileMapLayer tml = new TileMapLayer(this, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), world.Map, i);
                    if (i == 0)
                    {
                        tml.DrawOrder = World.PLAYER_DRAW_ORDER - DEFAULT_LAYER_SPACING;
                        tml.DrawBlanksEnabled = true;
                        tml.DrawEdgesEnabled = true;
                    }
                    else
                    {
                        tml.DrawOrder = World.PLAYER_DRAW_ORDER + i * DEFAULT_LAYER_SPACING;
                    }
                    world.interactiveLayers[tml.DrawOrder] = tml;
                    Components.Add(world.interactiveLayers[tml.DrawOrder]);
                }
                world.Initialize();
                world.Camera.Position = world.Player.Position;// -new Vector2(world.Camera.VisibleArea.Width / 2, world.Camera.VisibleArea.Height / 2);
                world.Pause();
                Components.Add(world);

                // Set up editor controls:
                inputMonitor = new InputMonitor(ScreenManager.Instance);
                inputMonitor.AssignKey("EditorLeft", Microsoft.Xna.Framework.Input.Keys.A);
                inputMonitor.AssignKey("EditorRight", Microsoft.Xna.Framework.Input.Keys.D);
                inputMonitor.AssignKey("EditorUp", Microsoft.Xna.Framework.Input.Keys.W);
                inputMonitor.AssignKey("EditorDown", Microsoft.Xna.Framework.Input.Keys.S);
                inputMonitor.AssignKey("EditorAppend", Microsoft.Xna.Framework.Input.Keys.LeftShift);
                inputMonitor.AssignKey("ToggleFullScreen", Microsoft.Xna.Framework.Input.Keys.F);
                Components.Add(inputMonitor);

                CreateUIComponents();

                // Initialize all components
                base.Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Let keypresses move the camera:
            if(inputMonitor.CheckKey("EditorLeft"))
            {
                world.Camera.Position -= new Vector2(CAM_SPEED, 0);
            }
            else if (inputMonitor.CheckKey("EditorRight"))
            {
                world.Camera.Position += new Vector2(CAM_SPEED, 0);
            }
            if (inputMonitor.CheckKey("EditorUp"))
            {
                world.Camera.Position -= new Vector2(0, CAM_SPEED);
            }
            else if (inputMonitor.CheckKey("EditorDown"))
            {
                world.Camera.Position += new Vector2(0, CAM_SPEED);
            } 
            else if (inputMonitor.CheckPressAndReleaseKey("ToggleFullScreen"))
            {
                if (isFullScreen)
                {
                    isFullScreen = false;
                    ScreenManager.Instance.ResolutionService.CurrentResolutionSettings = windowedSettings;
                }
                else
                {
                    isFullScreen = true;
                    ScreenManager.Instance.ResolutionService.CurrentResolutionSettings = fullScreenSettings;
                }
            }

            base.Update(gameTime);
        }

        private void CreateUIComponents()
        {
            SpriteBatch sb = batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName);

            // Where all the magic happens:
            TileSelector ts = new TileSelector(this, sb, 
                                               new RectangleF(40, 0, 600, 440),
                                               world.interactiveLayers.Values.First<TileMapLayer>(),
                                               inputMonitor);
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
