using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;


namespace WesternSpace.DrawableComponents.EditorUI
{
    public class TextureSelector : EditorUIComponent
    {
        private ITextureService textureService;
        private Dictionary<string, Texture2D>.KeyCollection.Enumerator currentTexture;

        private Tile tile;
        private int layerIndex;
        private int subLayerIndex;

        public Tile Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        public override void OnMouseClick(int button)
        {
            base.OnMouseClick(button);
        }

        public override void OnMouseUnclick(int button)
        {
            base.OnMouseUnclick(button);
        }

        public override void OnMouseScroll(int amount)
        { 
            // Basically we just want to switch what texture tile.Textures[layerIndex, subLayerIndex] points
            //  to here.
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
        }

        public TextureSelector(Game game, SpriteBatch spriteBatch, RectangleF bounds)
            : base(game, spriteBatch, bounds)
        {
        }

        public TextureSelector(Game game, SpriteBatch spriteBatch, RectangleF bounds, Tile tile, int layerIndex, int subLayerIndex)
            :base(game, spriteBatch, bounds)
        {
            this.tile = tile;
            this.layerIndex = layerIndex;
            this.subLayerIndex = subLayerIndex;
        }

        public override void Initialize()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
            currentTexture = textureService.Textures.Keys.GetEnumerator();
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)

        {
            if (tile != null)
            {
                SubTexture sub = tile.Textures[layerIndex, subLayerIndex];
                this.SpriteBatch.Draw(sub.Texture, Position, sub.Rectangle, Microsoft.Xna.Framework.Graphics.Color.White);
            }
            base.Draw(gameTime);
        }
    }
}
