using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Linq;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.TilingEngine
{
    public class TileEngine
    {
        private ITextureService textureService;

        public TileEngine()
        {
            textureService = (ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService));
        }

        public TileMap LoadTileMap(string imageFileName, string settingsFileName)
        {
            Texture2D layer = ScreenManager.Instance.Content.Load<Texture2D>(imageFileName);

            Color[] allPixels = new Color[(layer.Width * layer.Height)];

            layer.GetData<Color>(allPixels);
            Dictionary<Color, Tile> lookupTable = LoadSettingsFile(settingsFileName);
            TileMap tm = new TileMap(layer.Width, layer.Height, 100, 100, 1, 1); // For now just 1 layer and 1 sublayer. (1 total texture per tile)

            for (int i = 0; i < allPixels.Length; i++)
            {
                int x = i % layer.Width;
                int y = i / layer.Width;

                Tile t = lookupTable[allPixels[i]];

                tm.SetTile(t, x, y);
            }

            return tm;
        }

        private Dictionary<Color, Tile> LoadSettingsFile(string settingsFileName)
        {
            Dictionary<Color, Tile> lookupTable = new Dictionary<Color, Tile>();
            
            lookupTable.Add(Color.Black, null);
            XDocument rootLayerElement = ScreenManager.Instance.Content.Load<XDocument>(settingsFileName);

            IEnumerable<LayerInformation> textureInformation = from texture in rootLayerElement.Descendants("Texture")
                                                                 select new LayerInformation
                                                                 {
                                                                     Name = texture.Attribute("FileName").Value,
                                                                     Color = new Color(Byte.Parse(texture.Attribute("ColorRed").Value), 
                                                                                       Byte.Parse(texture.Attribute("ColorGreen").Value), 
                                                                                       Byte.Parse(texture.Attribute("ColorBlue").Value))
                                                                 };

            foreach (LayerInformation li in textureInformation)
            {
                Texture2D texture = textureService.GetTexture(li.Name);
                Texture2D[,] textures = new Texture2D[1,1];
                textures[0,0] = texture;
                lookupTable.Add(li.Color, new Tile(textures));
            }

            return lookupTable;
        }
    }
}
