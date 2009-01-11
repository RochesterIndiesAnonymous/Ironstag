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
        private Game game;
        private Dictionary<string, Texture2D> loadedTextures;

        public TileEngine(Game game)
        {
            this.game = game;

            ITextureService textureService = (ITextureService)game.Services.GetService(typeof(ITextureService));
            this.loadedTextures = textureService.Textures;
        }

        public TileMap LoadLayer(string imageFileName, string settingsFileName)
        {
            Texture2D layer = game.Content.Load<Texture2D>(imageFileName);

            Color[] allPixels = new Color[(layer.Width * layer.Height)];

            layer.GetData<Color>(allPixels);
            Dictionary<Color, Tile> lookupTable = LoadSettingsFile(settingsFileName);
            TileMap tm = new TileMap(game, layer.Width, layer.Height, 100, 100);

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

            XElement rootLayerElement = XElement.Load(settingsFileName);

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
                if (!loadedTextures.ContainsKey(li.Name))
                {
                    loadedTextures.Add(li.Name, game.Content.Load<Texture2D>(li.Name));
                }

                lookupTable.Add(li.Color, new Tile(loadedTextures[li.Name]));
            }

            return lookupTable;
        }
    }
}
