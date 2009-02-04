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
    // Let's you change the starting position of the player.
    public class SaveButton : EditorUIComponent
    {
        private World world;

        private ITextureService textureService;

        public SaveButton(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, World world)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.world = world;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseUnclick(int button)
        {
            if (button == 0) // Only handle left clicks, ignore others.
            {
                TileMap orig = world.Map;

                // Save TileMap here.
                TileMap copy = new TileMap(orig.Width, orig.Height,
                                           orig.TileWidth, orig.TileHeight,
                                           orig.LayerCount, orig.SubLayerCount);


                Tile tile;
                for (int i = 0; i < orig.Width; ++i)
                {
                    for (int j = 0; j < orig.Height; ++j)
                    {
                        tile = orig[i,j];
                        if (tile != null)
                            copy[i, j] = new Tile(tile);
                    }
                }
                copy.Minimize();
                Vector2 worldOffset = new Vector2(((orig.Width - copy.Width) * orig.TileWidth) / 2, 
                                                  ((orig.Height - copy.Height) * orig.TileHeight) / 2);

                world.Player.Position -= worldOffset;
                
                XDocument tileDoc = new XDocument(copy.ToXElement());
                tileDoc.Save("..\\..\\..\\Content\\TileMapXML\\BigTileMap.xml");

                XDocument worldDoc = new XDocument(world.ToXElement());
                worldDoc.Save("..\\..\\..\\Content\\WorldXML\\TestWorld.xml");

                world.Player.Position += worldOffset;
            }
            base.OnMouseUnclick(button);
        }

        #endregion

    }
}
