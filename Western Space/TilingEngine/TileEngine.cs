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

namespace WesternSpace.TilingEngine
{
    public class TileEngine
    {
        private Game game;
        private Dictionary<string, Texture2D> loadedTextures;

        public TileEngine(Game game, Dictionary<string, Texture2D> loadedTextures)
        {
            this.game = game;
            this.loadedTextures = loadedTextures;
        }

        public Layer LoadLayer(string imageFileName, string settingsFileName)
        {
            Texture2D layer = game.Content.Load<Texture2D>(imageFileName);

            Color[] allPixels = new Color[(layer.Width * layer.Height)];

            layer.GetData<Color>(allPixels);
            Dictionary<Color, string> lookupTable = LoadSettingsFile(settingsFileName);

            Layer result = new Layer(game, loadedTextures, allPixels, lookupTable, layer.Height, layer.Width, 100, 100);

            return result;
        }

        private Dictionary<Color, string> LoadSettingsFile(string settingsFileName)
        {
            Dictionary<Color, string> lookupTable = new Dictionary<Color, string>();

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

                lookupTable.Add(li.Color, li.Name);
            }

            return lookupTable;
        }
    }
}
