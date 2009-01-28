using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.EditorUI
{
    public class EdgeToggler : EditorUIComponent, ITilePropertyComponent
    {
        private TileSelector tileSelector;

        private ITextureService textureService;

        public EdgeToggler(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileSelector tileSelector)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileSelector = tileSelector;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
        }

        #region MOUSE EVENT HANDLERS
/*
        protected override void OnMouseUnclick(int button)
        {
            if (button == 0) // Only handle left clicks, ignore others.
            {
                if (tileSelector.TileX > 0 && tileSelector.TileY > 0)
                {
                    tileSelector.TileMap.SetSolid(!Tile.IsSolid(), tileSelector.TileX, tileSelector.TileY);
                }
                else
                {
                    tileSelector.VirtualTile.SetSolid(!tileSelector.VirtualTile.IsSolid());
                }
            }
            base.OnMouseUnclick(button);
        }
*/
        #endregion

        public override void Draw(GameTime gameTime)
        {
            /*
            if(Tile != null ) 
            {
                for (int i = 0; i < Tile.LayerCount; ++i)
                {
                    for (int j = 0; j < Tile.SubLayerCount; ++j)
                    {
                        SubTexture subTex = Tile.Textures[i, j];
                        if (subTex != null)
                            this.SpriteBatch.Draw(subTex.Texture, Position, subTex.Rectangle, Microsoft.Xna.Framework.Graphics.Color.White);
                    }
                }
                bool mouseInside = MouseIsInside();
                if (Tile.IsSolid())
                {
                    if (mouseInside)
                    {
                        Color = new Microsoft.Xna.Framework.Graphics.Color(255, 127, 127);
                    }
                    else
                    {
                        Color = new Microsoft.Xna.Framework.Graphics.Color(255, 0, 0);
                    }
                }
                else
                {
                    Color = new Microsoft.Xna.Framework.Graphics.Color(1.0f, 1.0f, 1.0f, (mouseInside ? 0.8f : 0.5f));
                }
            }*/

            base.Draw(gameTime);
        }


        #region ITilePropertyComponent Members

        public void OnTileSelectionChange()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
