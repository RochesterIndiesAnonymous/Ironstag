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
            Dictionary<Color, SubTexture> lookupTable = LoadSettingsFile(settingsFileName);
            TileMap tm = new TileMap(layer.Width, layer.Height, 20, 20, 1, 1); // For now just 1 layer and 1 sublayer. (1 total texture per tile)

            for (int i = 0; i < allPixels.Length; i++)
            {
                int x = i % layer.Width;
                int y = i / layer.Width;

                SubTexture[,] textures = new SubTexture[1,1];
                textures[0, 0] = lookupTable[allPixels[i]];
                Tile t;
                t = textures[0,0] != null ? new Tile(textures) : null;

                tm.SetTile(t, x, y);
            }

            return tm;
        }


        private Dictionary<Color, SubTexture> LoadSettingsFile(string settingsFileName)
        {
            Dictionary<Color, SubTexture> lookupTable = new Dictionary<Color, SubTexture>();
            
            lookupTable.Add(Color.Black, null);
            XDocument rootLayerElement = ScreenManager.Instance.Content.Load<XDocument>(settingsFileName);

            IEnumerable<LayerInformation> textureInformation = from texture in rootLayerElement.Descendants("Texture")
                                                                 select new LayerInformation
                                                                 {
                                                                     Name = texture.Attribute("FileName").Value,
                                                                     Index = Int32.Parse(texture.Attribute("Index").Value),
                                                                     Color = new Color(Byte.Parse(texture.Attribute("ColorRed").Value), 
                                                                                       Byte.Parse(texture.Attribute("ColorGreen").Value), 
                                                                                       Byte.Parse(texture.Attribute("ColorBlue").Value))
                                                                 };

            foreach (LayerInformation li in textureInformation)
            {
                lookupTable.Add(li.Color, textureService.GetSheet(li.Name).SubTextures[li.Index]);
            }

            return lookupTable;
        }
    }
}
