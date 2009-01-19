using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.Services;
using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using System.Xml.Linq;

namespace WesternSpace
{
    class World : GameObject
    {
        // The actual map that objects interact with.
        TileMap map;

        public TileMap Map
        {
            get { return map; }
        }

        // Secondary non-interactive tile maps that usually represent background layers/parallax.
        private TileMap[] otherMaps;

        // int represents the Z-index the layer is drawn at.
        public Dictionary<int, TileMapLayer> layers;

        public World(Game game, string fileName)
            : base(game)
        {
            //this.camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this.layers = new Dictionary<int, TileMapLayer>();
            LoadWorldXmlFile(fileName);
        }

        // TODO: add support for parallax layers. This will require a new class that derives from
        //        TileMapLayer and overrides Draw()
        private void LoadWorldXmlFile(string fileName)
        {
            // For now, we use tileEngine to load image data for our maps.
            // This will change later when TileMap can be stored as XML.
            TileEngine tileEngine = new TileEngine();

            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);

            // Load the interactive TileMap:
            map = tileEngine.LoadLayer(fileContents.Root.Attribute("MapFileName").Value,
                                       fileContents.Root.Attribute("MapSettingsFileName").Value);

            // The Camera currently looks at the layers contained in layerService.
            // This can be removed later when the camera is smarter.
            ILayerService layerService = (ILayerService)Game.Services.GetService(typeof(ILayerService));

            IEnumerable<XElement> allMapLayers = from world in fileContents.Descendants("MapLayer")
                                                 select world;

            foreach (XElement mapLayer in allMapLayers)
            {
                string LayerName;
                int LayerIndex, ZIndex;

                LayerName = mapLayer.Attribute("LayerName").Value;
                Int32.TryParse(mapLayer.Attribute("LayerIndex").Value, out LayerIndex);
                Int32.TryParse(mapLayer.Attribute("ZIndex").Value, out ZIndex);

                layers[ZIndex] = new TileMapLayer(this.Game, map, LayerIndex);
                layers[ZIndex].DrawOrder = ZIndex;

                Game.Components.Add(layers[ZIndex]);
                layerService.Layers[LayerName] = layers[ZIndex];
            }

            IEnumerable<XElement> allParallaxMaps = from world in fileContents.Descendants("Parallax")
                                                    select world;

            // A parallaxMap is made up of a tileMap just like the interactive map,
            //  so it can have multiple layers in and of itself. This might not be common
            //  but the functionality is there.
            foreach (XElement parallaxMap in allParallaxMaps)
            {
                IEnumerable<XElement> allParallaxLayers = from world in parallaxMap.Descendants("Layer")
                                                     select world;
                float ScrollSpeed;
                TileMap tileMap;

                tileMap = tileEngine.LoadLayer(parallaxMap.Attribute("MapFileName").Value,
                                                       parallaxMap.Attribute("MapSettingsFileName").Value);
                float.TryParse(parallaxMap.Attribute("ScrollSpeed").Value, out ScrollSpeed);

                foreach (XElement parallaxLayer in allParallaxLayers)
                {
                    string LayerName;
                    int LayerIndex, ZIndex;
                    
                    LayerName = parallaxLayer.Attribute("LayerName").Value;
                    Int32.TryParse(parallaxLayer.Attribute("LayerIndex").Value, out LayerIndex);
                    Int32.TryParse(parallaxLayer.Attribute("ZIndex").Value, out ZIndex);

                    layers[ZIndex] = new TileMapLayer(this.Game, map, LayerIndex, ScrollSpeed);
                    layers[ZIndex].DrawOrder = ZIndex;

                    Game.Components.Add(layers[ZIndex]);
                    layerService.Layers[LayerName] = layers[ZIndex];
                }
            }
        }
    }
}
