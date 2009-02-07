using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Screens;
using WesternSpace.Interfaces;
using WesternSpace.DrawableComponents.Actors;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.Collision;
using Microsoft.Xna.Framework.Media;
using WesternSpace.DrawableComponents.Misc;
using WesternSpace.Physics;

namespace WesternSpace
{
    public class World : GameObject, IXElementOutput
    {
        public static readonly int PLAYER_DRAW_ORDER = 0;

        private ISpriteBatchService batchService;


        private PhysicsHandler physicsHandler;

        public PhysicsHandler PhysicsHandler
        {
            get { return physicsHandler; }
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
        private List<Character> characters;

        public List<Character> Characters
        {
            get { return characters; }
        }

        /// <summary>
        /// Prevent any components that belong in the world (enemies, the player, etc)
        ///  from being Update()d. This will essentially set "Enabled = false" on all
        ///  characters in the world.
        /// </summary>
        public void Pause()
        {
            player.Enabled = false;
            this.Enabled = false;
        }

        /// <summary>
        /// Create an empty world.
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        public World(Screen parentScreen, Player player)
            : base(parentScreen)
        {
            this.player = player;
            ParentScreen.Components.Add(player);
            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            otherMaps = new List<TileMap>();

            // Set up our collision systems:
            spriteCollisionManager = new SpriteSpriteCollisionManager(this.Game, batchService, new Point(40, 40));
            ParentScreen.Components.Add(spriteCollisionManager);

            bgm = this.Game.Content.Load<Song>("System\\Music\\DesertBGM");
            MediaPlayer.Play(bgm);
        }

        /// <summary>
        /// Load a world from an XML file.
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        /// <param name="fileName">The name of the XML file that contains this World's information.</param>
        public World(Screen parentScreen, string fileName)
            : base(parentScreen)
        {
            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            otherMaps = new List<TileMap>();

            // Set up our collision systems:
            spriteCollisionManager = new SpriteSpriteCollisionManager(this.Game, batchService, new Point(40, 40));
            ParentScreen.Components.Add(spriteCollisionManager);

            bgm = this.Game.Content.Load<Song>("System\\Music\\DesertBGM");
            MediaPlayer.Play(bgm);

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

            base.Update(gameTime);
        }

        // TODO: add proper support for parallax layers. This will require a new class that derives from
        //        TileMapLayer and overrides Draw()
        private void LoadWorldXmlFile(string fileName)
        {
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);


            #region LOAD INTERACTIVE MAP LAYERS

            // Load the interactive TileMap:
            map = new TileMap(fileContents.Root.Attribute("InteractiveMapFileName").Value);

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
            // The spritebatch to be used when creating all of our characters:
            SpriteBatch sb = batchService.GetSpriteBatch(Character.SpriteBatchName);

            // Add the player:
            Vector2 playerPosition = new Vector2(float.Parse(fileContents.Root.Attribute("PlayerPositionX").Value),
                                                 float.Parse(fileContents.Root.Attribute("PlayerPositionY").Value));

            player = new Player(this, sb, playerPosition);
            player.UpdateOrder = 3;
            player.DrawOrder = PLAYER_DRAW_ORDER;

            EBandit bandit1 = new EBandit(this, sb, new Vector2(500, 79));
            bandit1.UpdateOrder = 3;
            bandit1.DrawOrder = PLAYER_DRAW_ORDER;

            //SmallCactus smallCactus1 = new SmallCactus(this, sb, new Vector2(700, 336));
            //smallCactus1.UpdateOrder = 3;
            //smallCactus1.DrawOrder = PLAYER_DRAW_ORDER;

            spriteCollisionManager.addObjectToRegisteredObjectList(player);
            spriteCollisionManager.addObjectToRegisteredObjectList(bandit1);
            //spriteCollisionManager.addObjectToRegisteredObjectList(smallCactus1);

            ParentScreen.Components.Add(player);
            ParentScreen.Components.Add(bandit1);
   

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

                tileMap = new TileMap(parallaxMap.Attribute("MapFileName").Value);
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
                XElement parallax = new XElement("Parallax", new XAttribute("MapFileName", otherMap.FileName));
                ret.Add(parallax);
            }

            foreach (int zIndex in parallaxLayers.Keys)
            { 
                TileMapLayer pml = parallaxLayers[zIndex];
                XElement pmlElement = new XElement("MapLayer",
                                                   new XAttribute("LayerName",null),
                                                   new XAttribute("LayerIndex", pml.LayerIndex),
                                                   new XAttribute("ZIndex", zIndex));
                ret.Add(pmlElement);

                IEnumerable<XElement> plaxs = from plax in ret.Descendants("Parallax") 
                                              where plax.Attribute("MapFileName").Value == pml.TileMap.FileName
                                              select plax;
                plaxs.First<XElement>().Attribute("ScrollSpeed").Value = pml.ScrollSpeed.ToString();
            }

            return ret;
        }

        #endregion
    }
}
