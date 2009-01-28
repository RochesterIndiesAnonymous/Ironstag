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

namespace WesternSpace
{
    public class World : GameObject
    {
        private ISpriteBatchService batchService;

        // The actual map that objects interact with.
        TileMap map;

        public TileMap Map
        {
            get { return map; }
        }

        // Secondary non-interactive tile maps that usually represent background layers/parallax.
        private TileMap[] otherMaps;

        // int represents the Z-index the layer is drawn at.
        public Dictionary<int, TileMapLayer> interactiveLayers;
        
        // int represents the Z-index the layer is drawn at.
        public Dictionary<int, TileMapLayer> parallaxLayers;

        /// <summary>
        /// Create an empty parent screen.
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        public World(Screen parentScreen)
            : base(parentScreen)
        {
            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentScreen">The screen this world will be updated in.</param>
        /// <param name="fileName">The name of the XML file that contains this World's information.</param>
        public World(Screen parentScreen, string fileName)
            : base(parentScreen)
        {
            #region FOR TESTING ONLY - REMOVE ME LATER
            /*
            // Convert our tilemap images to giant XML files.
            TileEngine te = new TileEngine();
            TileMap btm = te.LoadTileMap("Layers\\BigTestLayer", "LayerXML\\TestLayer");
            TileMap tm = te.LoadTileMap("Layers\\TestLayer", "LayerXML\\TestLayer");

            XDocument doc = new XDocument(tm.ToXElement());
            doc.Save("TestTileMap.xml", SaveOptions.DisableFormatting);

            XDocument doc2 = new XDocument(btm.ToXElement());
            doc2.Save("BigTileMap.xml", SaveOptions.DisableFormatting);
            */
            #endregion

            this.interactiveLayers = new Dictionary<int, TileMapLayer>();
            this.parallaxLayers = new Dictionary<int, TileMapLayer>();
            batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
            LoadWorldXmlFile(fileName);
        }

        // TODO: add proper support for parallax layers. This will require a new class that derives from
        //        TileMapLayer and overrides Draw()
        private void LoadWorldXmlFile(string fileName)
        {
            // For now, we use tileEngine to load image data for our maps.
            // This will change later when TileMap can be stored as XML.
            TileEngine tileEngine = new TileEngine();

            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);

            // Load the interactive TileMap:
            map = new TileMap(fileContents.Root.Attribute("InteractiveMapFileName").Value);

            map.ToXElement();

            // The Camera currently looks at the layers contained in layerService.
            // This can be removed later when the camera is smarter.
            ILayerService layerService = (ILayerService)Game.Services.GetService(typeof(ILayerService));

            IEnumerable<XElement> allMapLayers = fileContents.Descendants("MapLayer");

            foreach (XElement mapLayer in allMapLayers)
            {
                string LayerName;
                int LayerIndex, ZIndex;

                LayerName = mapLayer.Attribute("LayerName").Value;
                Int32.TryParse(mapLayer.Attribute("LayerIndex").Value, out LayerIndex);
                Int32.TryParse(mapLayer.Attribute("ZIndex").Value, out ZIndex);

                interactiveLayers[ZIndex] = new TileMapLayer(ParentScreen, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), map, LayerIndex);
                interactiveLayers[ZIndex].DrawOrder = ZIndex;

                ParentScreen.Components.Add(interactiveLayers[ZIndex]);
                layerService.Layers[LayerName] = interactiveLayers[ZIndex];
            }

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

                float.TryParse(parallaxMap.Attribute("ScrollSpeed").Value, out ScrollSpeed);

                foreach (XElement parallaxLayer in allParallaxLayers)
                {
                    string LayerName;
                    int LayerIndex, ZIndex;
                    
                    LayerName = parallaxLayer.Attribute("LayerName").Value;
                    Int32.TryParse(parallaxLayer.Attribute("LayerIndex").Value, out LayerIndex);
                    Int32.TryParse(parallaxLayer.Attribute("ZIndex").Value, out ZIndex);

                    parallaxLayers[ZIndex] = new TileMapLayer(ParentScreen, batchService.GetSpriteBatch(TileMapLayer.SpriteBatchName), tileMap, LayerIndex, ScrollSpeed);
                    parallaxLayers[ZIndex].DrawOrder = ZIndex;

                    ParentScreen.Components.Add(parallaxLayers[ZIndex]);
                    layerService.Layers[LayerName] = parallaxLayers[ZIndex];
                }
            }
        }
    }
}
