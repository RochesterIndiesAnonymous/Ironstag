using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.EditorUI
{
    // Quick hack to allow saving of your changes.
    public class SaveButton : EditorUIComponent
    {
        private TileSelector tileSelector;

        private ITextureService textureService;

        public SaveButton(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileSelector tileSelector)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileSelector = tileSelector;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseUnclick(int button)
        {
            if (button == 0) // Only handle left clicks, ignore others.
            {
                TileMap orig = tileSelector.TileMap;

                // Save TileMap here.
                TileMap copy = new TileMap(orig.Width, orig.Height,
                                           orig.TileWidth, orig.TileHeight,
                                           orig.LayerCount, orig.SubLayerCount);

                Tile tile;
                for (int i = 0; i < orig.Width; ++i)
                {
                    for (int j = 0; j < orig.Height; ++j)
                    {
                        tile = orig.Tiles[i,j];
                        if (tile != null)
                            copy.Tiles[i, j] = new Tile(tile);
                    }
                }
                copy.Minimize();
                
                XDocument doc = new XDocument(copy.ToXElement());
                doc.Save("..\\..\\..\\Content\\TileMapXML\\BigTileMap.xml");
            }
            base.OnMouseUnclick(button);
        }

        #endregion

    }
}
