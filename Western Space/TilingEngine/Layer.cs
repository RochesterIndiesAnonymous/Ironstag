using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.TilingEngine
{
    public class Layer : DrawableGameComponent
    {
        private SpriteBatch sb;

        private Dictionary<string, Texture2D> textures;

        private Color[] colors;

        private Dictionary<Color, string> lookupTable;

        private int layerImageHeight;

        private int layerImageWidth;

        private int textureHeight;

        private int textureWidth;

        public Layer(Game game, Dictionary<string, Texture2D> textures, Color[] colors, Dictionary<Color, string> lookupTable, int layerHeight, int layerWidth, int textureHeight, int textureWidth)
            : base(game)
        {
            this.colors = colors;
            this.lookupTable = lookupTable;
            this.layerImageHeight = layerHeight;
            this.layerImageWidth = layerWidth;
            this.textures = textures;
            sb = new SpriteBatch(game.GraphicsDevice);
            this.textureHeight = textureHeight;
            this.textureWidth = textureWidth;

            this.Enabled = false;
            this.Visible = true;
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin();

            for (int i = 0; i < colors.Length; i++)
            {
                int x = i % layerImageWidth;
                int y = i / layerImageWidth;

                Vector2 position = new Vector2(x*textureWidth, y*textureHeight);

                string name = lookupTable[colors[i]];
                Texture2D textureToDraw = textures[name];

                sb.Draw(textureToDraw, position, Color.White);
            }

            sb.End();

            base.Draw(gameTime);
        }
    }
}