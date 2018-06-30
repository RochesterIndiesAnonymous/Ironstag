using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Screens;
using WesternSpace.Interfaces;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.Collision;
using WesternSpace.DrawableComponents.Misc;
using WesternSpace.Physics;

namespace WesternSpace
{
    public class World : GameObject, IXElementOutput
    {
        public static readonly int PLAYER_DRAW_ORDER = 0;

        private ISpriteBatchService batchService;

        private SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        private PhysicsHandler physicsHandler;

        public PhysicsHandler PhysicsHandler
        {
            get { return physicsHandler; }
        }


        private ConstructorInfo[] worldObjectCtorInfos;

        public ConstructorInfo[] WorldObjectCtorInfos
        {
            get { return worldObjectCtorInfos; }
            set { worldObjectCtorInfos = value; }
        }


        // The actual map that objects interact with.
        TileMap map;

        private SpriteSpriteCollisionManager spriteCollisionManager;

        public SpriteSpriteCollisionManager SpriteCollisionManager
        {
            get { return spriteCollisionManager; }
        }

        private Song bgm;

        public TileMap Map
        {
            get { return map; }
            set { map = value; }
        }

        private ICameraService camera;

        public ICameraService Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        // Secondary non-interactive tile maps that usually represent background layers/parallax.
        private List<TileMap> otherMaps;

        // int represents the Z-index the layer is drawn at.
        public Dictionary<int, TileMapLayer> interactiveLayers;
        
        // int represents the Z-index the layer is drawn at.
        public Dictionary<int, TileMapLayer> parallaxLayers;

        private Player player;

        internal Player Player
        {
            get { return player; }
            set { player = value; }
        }

        /// <summary>
        /// A list of all non-player Characters in the world.
        /// </summary>
        private List<WorldObject> worldObjects;

        public List<WorldObject> WorldObjects
        {
            get { return worldObjects; }
        }

        // Variables for Audio Engine
        private AudioEngine audioEngine;
        private SoundBank soundBank;
        private WaveBank waveBank;

        /// <summary>
        /// Function that can be called from anywhere to play any sound.
        /// </summary>
        public void playSound(string cue)
        {
            Cue newCue = soundBank.GetCue(cue);
            newCue.Play();
        }
        
        public void AddWorldObject(WorldObject worldObject)
        {
            worldObject.Enabled = !Paused;

            player.UpdateOrder = 3; // Need to standardize this.
            worldObject.DrawOrder = PLAYER_DRAW_ORDER; // ...and this.

            if(worldObject is ISpriteCollideable)
                spriteCollisionManager.addObjectToRegisteredObjectList((ISpriteCollideable)worldObject);

            worldObjects.Add(worldObject);
            ParentScreen.Components.Add(worldObject);
        }

        public void RemoveWorldObject(WorldObject worldObject)
        {
            /*if (worldObjects.Contains(worldObject))
            {*/                
                if (worldObject is ISpriteCollideable)
                    spriteCollisionManager.removeObjectFromRegisteredObjectList((ISpriteCollideable)worldObject);
                worldObjects.Remove(worldObject);
                ParentScreen.Components.Remove(worldObject);
            //}
            
        }

        /// <summary>
        /// Shift every world object in the world (including the Player)
        /// by a given offset. This is really only used by the editor.
        /// </summary>
        /// <param name="offset"></param>
        public void ShiftWorldObjects(Vector2 offset)
        {
            foreach (WorldObject worldObject in worldObjects)
            {
                worldObject.Position += offset;
            }
            Player.Position += offset;
        }

        private bool paused = false;

        /// <summary>
        /// Prevent any components that belong in the world (enemies, the player, etc)
        ///  from being Update()d. This will essentially set "Enabled = false" on all
        ///  characters in the world, when Paused is set to false.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set 
            {
                player.Enabled = !value;
                foreach (WorldObject worldObject in worldObjects)
                {
                    worldObject.Enabled = !value;
                }
                this.Enabled = !value;
                this.paused = value;
            }
        }


        private void LoadCtorInfos()
        {
            Type[] expectedCharacterArguments = new Type[] { typeof(World), typeof(SpriteBatch), typeof(Vector2) };

            IEnumerable<Type> types = from type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                                      where type.IsSubclassOf(typeof(WorldObject)) &&
                                           !(type.Name == "Player") &&
                                            type.GetConstructor(expectedCharacterArguments) != null &&
                                           !type.IsAbstract
                                      select type;
            worldObjectCtorInfos = new ConstructorInfo[types.Count<Type>()];

            Console.Out.WriteLine("World object type count: " + types.Count<Type>() + "\nTypes:\n");
            int index = 0;
            foreach (Type type in types)
            {
                worldObjectCtorInfos[index] = (type.GetConstructor(expectedCharacterArguments));
                ++index;
                Console.Out.WriteLine(index + " " + type.Name);
            }
        }

        /// <summary>
        /// Create an empty world.
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        public World(Screen parentScreen, Player player)
            : base(parentScreen)
        {
            this.worldObjects = new List<WorldObject>();
            this.player = player;
            ParentScreen.Components.Add(player);
            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            otherMaps = new List<TileMap>();

            //Set up Sound systems
            audioEngine = new AudioEngine("Content\\System\\Sounds\\Win\\SoundFX.xgs");
            soundBank = new SoundBank(audioEngine, "Content\\System\\Sounds\\Win\\GameSoundBank.xsb");
            waveBank = new WaveBank(audioEngine, "Content\\System\\Sounds\\Win\\GameWavs.xwb");

            // Set up our collision systems:
            spriteCollisionManager = new SpriteSpriteCollisionManager(this.Game, batchService, 40, 40);
            ParentScreen.Components.Add(spriteCollisionManager);

           // bgm = this.Game.Content.Load<Song>("System\\Music\\DesertBGM");
            String songFilename = @"Content\System\\Music\DesertBGM.mp3";
            bgm = Song.FromUri(songFilename, new Uri(songFilename, UriKind.Relative));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);

            LoadCtorInfos();
        }

        /// <summary>
        /// Load a world from an XML file.
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        /// <param name="fileName">The name of the XML file that contains this World's information.</param>
        public World(Screen parentScreen, string fileName)
            : base(parentScreen)
        {
            this.worldObjects = new List<WorldObject>();
            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            otherMaps = new List<TileMap>();

            // Set up our collision systems:
            spriteCollisionManager = new SpriteSpriteCollisionManager(this.Game, batchService, 40, 40);
            ParentScreen.Components.Add(spriteCollisionManager);

            //Set up Sound systems
            // audioEngine = new AudioEngine("Content\\System\\Sounds\\SoundFX.xgs");
            //soundBank = new SoundBank(audioEngine, "Content\\System\\Sounds\\GameSoundBank.xsb");
            //waveBank = new WaveBank(audioEngine, "Content\\System\\Sounds\\GameWavs.xwb");

            // bgm = this.Game.Content.Load<Song>("System\\Music\\DesertBGM");
            String songFilename = @"Content\System\\Music\DesertBGM.mp3";
            bgm = Song.FromUri(songFilename, new Uri(songFilename, UriKind.Relative));
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgm);

            // The spritebatch to be used when creating all of our worldObjects:
            spriteBatch = batchService.GetSpriteBatch(Character.SpriteBatchName);
            LoadCtorInfos();

            // Load the contents of the world from the specified XML file:
            LoadWorldXmlFile(fileName);
        }

        public override void Initialize()
        {
            camera = (ICameraService)ScreenManager.Instance.Services.GetService(typeof(ICameraService));
            base.Initialize();
            this.physicsHandler = new PhysicsHandler(this);
        }

        public override void Update(GameTime gameTime)
        {
            // Update the camera's position. For now, just a very simple smooth-centering algorithm:
            float cam_vx, cam_vy;
            if (player.Facing == SpriteEffects.FlipHorizontally)
            { // Facing left
                cam_vx = (Player.Position.X - camera.VisibleArea.Width * 0.7f - camera.RealPosition.X) / 20;
                cam_vy = (Player.Position.Y - camera.VisibleArea.Height / 2 - camera.RealPosition.Y) / 20;
            }
            else
            { // Facing Right
                cam_vx = (Player.Position.X - camera.VisibleArea.Width * 0.3f - camera.RealPosition.X) / 20;
                cam_vy = (Player.Position.Y - camera.VisibleArea.Height / 2 - camera.RealPosition.Y) / 20;
            }

            camera.Position += new Vector2(cam_vx, cam_vy);

           // audioEngine.Update();

            base.Update(gameTime);
        }

        // TODO: add proper support for parallax layers. This will require a new class that derives from
        //        TileMapLayer and overrides Draw()
        private void LoadWorldXmlFile(string fileName)
        {
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);


            #region LOAD INTERACTIVE MAP LAYERS

            // Load the interactive TileMap:
            map = new TileMap(this, fileContents.Root.Attribute("InteractiveMapFileName").Value);

            for (int i = 0; i < map.Width; ++i)
            {
                for (int j = 0; j < map.Height; ++j)
                {
                    if (map[i, j] != null && map[i, j] is ISpriteCollideable)
                    {
                        spriteCollisionManager.addObjectToRegisteredObjectList((ISpriteCollideable)map[i, j]);
                    }
                }
            }

            IEnumerable<XElement> allMapLayers = fileContents.Descendants("MapLayer");

            foreach (XElement mapLayer in allMapLayers)
            {
                string LayerName;
                int LayerIndex, ZIndex;

                LayerName = mapLayer.Attribute("LayerName").Value;
                LayerIndex = Int32.Parse(mapLayer.Attribute("LayerIndex").Value);
                ZIndex = Int32.Parse(mapLayer.Attribute("ZIndex").Value);

                interactiveLayers[ZIndex] = new TileMapLayer(ParentScreen, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), map, LayerIndex);
                interactiveLayers[ZIndex].DrawOrder = ZIndex;

                ParentScreen.Components.Add(interactiveLayers[ZIndex]);
            }

            #endregion


            #region LOAD CHARACTERS

            // Add the player:
            Vector2 playerPosition = new Vector2(float.Parse(fileContents.Root.Attribute("PlayerPositionX").Value),
                                                 float.Parse(fileContents.Root.Attribute("PlayerPositionY").Value));

            player = new Player(this, spriteBatch, playerPosition);
            player.UpdateOrder = 3;
            player.DrawOrder = PLAYER_DRAW_ORDER;
            ParentScreen.Components.Add(player);
            spriteCollisionManager.addObjectToRegisteredObjectList(player);

            foreach (XElement woElement in fileContents.Descendants("o"))
            {
                string name = woElement.Attribute("n").Value;
                Vector2 position = new Vector2();
                position.X = float.Parse(woElement.Attribute("x").Value);
                position.Y = float.Parse(woElement.Attribute("y").Value);

                ConstructorInfo ci = (from CI in WorldObjectCtorInfos
                                      where CI.DeclaringType.Name == name
                                      select CI).First<ConstructorInfo>();
                AddWorldObject((WorldObject)ci.Invoke(new object[]{this, SpriteBatch, position}));
            }

            #endregion


            #region LOAD PARALLAX LAYERS

            IEnumerable<XElement> allParallaxMaps = fileContents.Descendants("Parallax");

            // A parallaxMap is made up of a tileMap just like the interactive map,
            //  so it can have multiple layers in and of itself. This might not be common
            //  but the functionality is there.
            foreach (XElement parallaxMap in allParallaxMaps)
            {
                IEnumerable<XElement> allParallaxLayers = parallaxMap.Descendants("Layer");
                float ScrollSpeed;
                TileMap tileMap;

                tileMap = new TileMap(this, parallaxMap.Attribute("MapFileName").Value);
                otherMaps.Add(tileMap);

                ScrollSpeed = Single.Parse(parallaxMap.Attribute("ScrollSpeed").Value);

                foreach (XElement parallaxLayer in allParallaxLayers)
                {
                    string LayerName;
                    int LayerIndex, ZIndex;
                    
                    LayerName = parallaxLayer.Attribute("LayerName").Value;
                    LayerIndex = Int32.Parse(parallaxLayer.Attribute("LayerIndex").Value);
                    ZIndex = Int32.Parse(parallaxLayer.Attribute("ZIndex").Value);

                    parallaxLayers[ZIndex] = new TileMapLayer(ParentScreen, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), tileMap, LayerIndex, ScrollSpeed);
                    parallaxLayers[ZIndex].DrawOrder = ZIndex;

                    ParentScreen.Components.Add(parallaxLayers[ZIndex]);
                }
            }

            #endregion
        }


        #region IXElementOutput Members

        public XElement ToXElement()
        {
            XAttribute interactiveMapFileName = new XAttribute("InteractiveMapFileName", Map.FileName);

            XAttribute playerPositionX = new XAttribute("PlayerPositionX", Player.Position.X);
            XAttribute playerPositionY = new XAttribute("PlayerPositionY", Player.Position.Y);

            XElement ret = new XElement("World", interactiveMapFileName, playerPositionX, playerPositionY);

            foreach (int zIndex in interactiveLayers.Keys)
            {
                TileMapLayer tml = interactiveLayers[zIndex];
                XElement imlElement = new XElement("MapLayer",
                                                   new XAttribute("LayerName",""),
                                                   new XAttribute("LayerIndex", tml.LayerIndex),
                                                   new XAttribute("ZIndex", zIndex));
                ret.Add(imlElement);
            }

            foreach (TileMap otherMap in otherMaps)
            {
                XElement parallax = new XElement("Parallax", new XAttribute("MapFileName", otherMap.FileName), new XAttribute("ScrollSpeed", ""));
                ret.Add(parallax);
            }

            foreach (WorldObject wo in worldObjects)
            {
                ret.Add(wo.ToXElement());
            }

            foreach (int zIndex in parallaxLayers.Keys)
            { 
                TileMapLayer pml = parallaxLayers[zIndex];
                XElement pmlElement = new XElement("Layer",
                                                   new XAttribute("LayerName",""),
                                                   new XAttribute("LayerIndex", pml.LayerIndex),
                                                   new XAttribute("ZIndex", zIndex));


                IEnumerable<XElement> plaxs = from plax in ret.Descendants("Parallax") 
                                              where plax.Attribute("MapFileName").Value == pml.TileMap.FileName
                                              select plax;

                XElement plaxX = plaxs.First<XElement>();
                plaxX.Attribute("ScrollSpeed").Value = pml.ScrollSpeed.ToString();
                plaxX.Add(pmlElement);
            }


            return ret;
        }

        #endregion
    }
}
