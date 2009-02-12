using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        public static readonly int MAX_TILEMAP_WIDTH = 501;
        public static readonly int MAX_TILEMAP_HEIGHT = 501;
        public static readonly int LAYER_COUNT = 2;
        public static readonly int SUB_LAYER_COUNT = 2;
        public static readonly int DEFAULT_TILE_WIDTH = 24;
        public static readonly int DEFAULT_TILE_HEIGHT = 24;
        public static readonly int DEFAULT_LAYER_SPACING = 10;

        public static readonly string WORLD_FILENAME = "WorldXML\\TestWorld";

        public enum EditMode : int
        {
            TileEdit = 0,
            SpriteEdit
        }

        private static int modeCount = Enum.GetValues(typeof(EditMode)).Length;

        private EditMode mode;

        public EditMode Mode
        {
            get { return mode; }
            set 
            { 
                mode = value;
            }
        }

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

        private TileSelector tileSelector;

        public TileSelector TileSelector
        {
            get { return tileSelector; }
        }

        private List<SubTextureSelector> subTextureSelectors;

        public List<SubTextureSelector> SubTextureSelectors
        {
            get { return subTextureSelectors; }
        }

        private EdgeToggler edgeToggler;

        public EdgeToggler EdgeToggler
        {
            get { return edgeToggler; }
        }

        private DestructableToggler destructableToggler;

        public DestructableToggler DestructableToggler
        {
            get { return destructableToggler; }
        }

        private WorldObjectMover playerMover;

        public WorldObjectMover PlayerMover
        {
            get { return playerMover; }
        }


        private WorldObjectPlacer worldObjectPlacer;

        public WorldObjectPlacer WorldObjectPlacer
        {
            get { return worldObjectPlacer; }
        }

        private List<WorldObjectMover> worldObjectMovers;

        public List<WorldObjectMover> WorldObjectMovers
        {
            get { return worldObjectMovers; }

        }

        private InputMonitor inputMonitor;

        private TileEngine tileEngine;

        private ISpriteBatchService batchService;

        private World world;

        public World World
        {
            get { return world; }
        }

        public EditorScreen(Game game, string name)
            : base(game, name)
        {
            isFullScreen = false;
            Game.IsMouseVisible = true;
        }

        private int offsetX, offsetY;

        public Vector2 Offset
        {
            get { return new Vector2(offsetX * World.Map.TileWidth, offsetY * World.Map.TileHeight); }
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

                // We need to disable the SpriteSpriteCollisionManager because it makes some assumptions about
                //  the gameScreen...
                world.SpriteCollisionManager.Enabled = false;
                
                // Create an empty, maximally-sized tilemap to center
                //  the loaded map onto:
                TileMap bigMap = new TileMap(MAX_TILEMAP_WIDTH, MAX_TILEMAP_HEIGHT, 
                                          DEFAULT_TILE_WIDTH, DEFAULT_TILE_HEIGHT,
                                          LAYER_COUNT, SUB_LAYER_COUNT);
                bigMap.FileName = world.Map.FileName;

                // Backup the original map:
                TileMap map = world.Map;

                // Compute the point where we want to blit it onto the empty map:
                this.offsetX = (bigMap.Width / 2) - (map.Width / 2);
                this.offsetY = (bigMap.Height / 2) - (map.Height / 2);

                bigMap.BlitTileMap(map, offsetX, offsetY);

                world.Map = bigMap;
                foreach (TileMapLayer tml in world.interactiveLayers.Values)
                {
                    Components.Remove(tml);
                }
                world.interactiveLayers.Clear();
                world.ShiftWorldObjects(new Vector2(world.Map.TileWidth * offsetX, world.Map.TileWidth * offsetY));

                for (int i = 0; i < world.Map.LayerCount; ++i)
                {
                    TileMapLayer tml = new TileMapLayer(this, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), world.Map, i);
                    if (i == 0)
                    {
                        tml.DrawOrder = World.PLAYER_DRAW_ORDER - DEFAULT_LAYER_SPACING;
                    }
                    else
                    {
                        if (i == world.Map.LayerCount - 1)
                        {
                            tml.DrawBlanksEnabled = true;
                            tml.DrawEdgesEnabled = true;
                            tml.DrawDestructablesEnabled = true;
                        }
                        tml.DrawOrder = World.PLAYER_DRAW_ORDER + i * DEFAULT_LAYER_SPACING;
                    }
                    world.interactiveLayers[tml.DrawOrder] = tml;
                    Components.Add(world.interactiveLayers[tml.DrawOrder]);
                }

                world.Initialize();
                world.Camera.Position = world.Player.Position;// -new Vector2(world.Camera.VisibleArea.Width / 2, world.Camera.VisibleArea.Height / 2);
                world.Paused = true;

                Components.Add(world);

                // Set up editor controls:
                inputMonitor = InputMonitor.Instance;
                inputMonitor.AssignPressable("EditorLeft", new PressableKey(Keys.A));
                inputMonitor.AssignPressable("EditorRight", new PressableKey(Keys.D));
                inputMonitor.AssignPressable("EditorUp", new PressableKey(Keys.W));
                inputMonitor.AssignPressable("EditorDown", new PressableKey(Keys.S));
                inputMonitor.AssignPressable("EditorAppend", new PressableKey(Keys.LeftShift));
                inputMonitor.AssignPressable("ToggleFullScreen", new PressableKey(Keys.F));
                inputMonitor.AssignPressable("EditorCycleMode", new PressableKey(Keys.Tab));
                Components.Add(inputMonitor);

                CreateUIComponents();

                Mode = EditMode.SpriteEdit;
                CycleMode();

                // Initialize all components
                base.Initialize();
            }
        }

        private void CreateUIComponents()
        {
            SpriteBatch sb = batchService.GetSpriteBatch(DebuggingOutputComponent.SpriteBatchName);

            // Where all the magic happens:
            tileSelector = new TileSelector(this, sb, 
                                               new RectangleF(40, 0, 600, 480),
                                               world.interactiveLayers.Values.First<TileMapLayer>(),
                                               inputMonitor);
            tileSelector.DrawOrder = 0;
            this.Components.Add(tileSelector);

            this.subTextureSelectors = new List<SubTextureSelector>();

            for (int i = 0; i < world.Map.LayerCount; ++i)
            {
                for (int j = 0; j < world.Map.SubLayerCount; ++j)
                {
                    int index = i * world.Map.SubLayerCount + j;

                    SubTextureSelector subTexSel = new SubTextureSelector(this, sb, TileSelector, i, j);
                    tileSelector.TilePropertyComponents.Add(subTexSel);
                    subTexSel.DrawOrder = 25;
                    this.Components.Add(subTexSel);
                    SubTextureSelectors.Add(subTexSel);
                }
            }

            this.worldObjectMovers = new List<WorldObjectMover>();
            foreach (WorldObject wo in World.WorldObjects)
            {
                WorldObjectMover wom = new WorldObjectMover(this, sb, wo);
                WorldObjectMovers.Add(wom);
                this.Components.Add(wom);
            }

            RectangleF tmp = ((SubTextureSelector)tileSelector.TilePropertyComponents.Last<ITilePropertyComponent>()).Bounds;
            tmp.Y += 20 + world.Map.TileHeight;
            this.edgeToggler = new EdgeToggler(this, sb, tmp, TileSelector);
            EdgeToggler.DrawOrder = 25;
            TileSelector.TilePropertyComponents.Add(EdgeToggler);
            this.Components.Add(EdgeToggler);

            tmp.Y += 20 + world.Map.TileHeight;
            this.destructableToggler = new DestructableToggler(this, sb, tmp, TileSelector);
            DestructableToggler.DrawOrder = 25;
            TileSelector.TilePropertyComponents.Add(DestructableToggler);
            this.Components.Add(DestructableToggler);


            SaveButton saveButton = new SaveButton(this, sb, new RectangleF(5, 400, 30, 15), World);
            saveButton.DrawOrder = 20;
            this.Components.Add(saveButton);

            worldObjectPlacer = new WorldObjectPlacer(this, sb, new RectangleF(40, 0, 600, 480), World);
            worldObjectPlacer.DrawOrder = 25;
            this.Components.Add(worldObjectPlacer);

            playerMover = new WorldObjectMover(this, sb, World.Player);
            this.Components.Add(playerMover);
        }

        public void CycleMode()
        {
            switch (Mode)
            {
                case EditMode.TileEdit:
                    Console.Out.WriteLine("Switching to SpriteEdit mode..");
                    Mode = EditMode.SpriteEdit;
                    Game.IsMouseVisible = false;
                    tileSelector.Enabled = false;
                    tileSelector.Visible = false;
                    foreach (SubTextureSelector sts in SubTextureSelectors)
                    {
                        sts.Enabled = false;
                        sts.Visible = false;
                    }
                    foreach (WorldObjectMover wom in worldObjectMovers)
                    {
                        wom.Enabled = true;
                        wom.Visible = true;
                    }
                    EdgeToggler.Enabled = false;
                    EdgeToggler.Visible = false;
                    worldObjectPlacer.Enabled = true;
                    worldObjectPlacer.Visible = true;
                    playerMover.Enabled = true;
                    playerMover.Visible = true;
                    break;
                case EditMode.SpriteEdit:
                    Console.Out.WriteLine("Switching to TileEdit mode..");
                    Mode = EditMode.TileEdit;
                    Game.IsMouseVisible = true;
                    playerMover.Enabled = false;
                    playerMover.Visible = false;
                    tileSelector.Enabled = true;
                    tileSelector.Visible = true;
                    foreach (WorldObjectMover wom in worldObjectMovers)
                    {
                        wom.Enabled = false;
                        wom.Visible = false;
                    }
                    foreach (SubTextureSelector sts in SubTextureSelectors)
                    {
                        sts.Enabled = true;
                        sts.Visible = true;
                    }
                    worldObjectPlacer.Enabled = false;
                    worldObjectPlacer.Visible = false;
                    EdgeToggler.Enabled = true;
                    EdgeToggler.Visible = true;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (inputMonitor.WasJustPressed("EditorCycleMode"))
            {
                CycleMode();
            }

            // Let keypresses move the camera:
            if (inputMonitor.IsPressed("EditorLeft"))
            {
                world.Camera.Position -= new Vector2(CAM_SPEED, 0);
            }
            else if (inputMonitor.IsPressed("EditorRight"))
            {
                world.Camera.Position += new Vector2(CAM_SPEED, 0);
            }
            if (inputMonitor.IsPressed("EditorUp"))
            {
                world.Camera.Position -= new Vector2(0, CAM_SPEED);
            }
            else if (inputMonitor.IsPressed("EditorDown"))
            {
                world.Camera.Position += new Vector2(0, CAM_SPEED);
            }
            else if (inputMonitor.WasJustPressed("ToggleFullScreen"))
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
