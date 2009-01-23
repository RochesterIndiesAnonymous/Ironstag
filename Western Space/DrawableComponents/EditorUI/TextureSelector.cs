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
            if (button == 0) // Left click
            {
            }
            else if(button == 2) // Right click
            {
            }
            base.OnMouseClick(button);
        }
        public override void OnMouseScroll(int amount)
        { 
            // Basically we just want to switch what texture tile.Textures[layerIndex, subLayerIndex] points
            //  to here.
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
            textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
            currentTexture = textureService.Textures.Keys.GetEnumerator();
            base.Initialize();
        }
    }
}
