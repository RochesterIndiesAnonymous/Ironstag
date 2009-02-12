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
    /// <summary>
    /// A component who's main purpose is to toggle the destructability of a tile.
    /// </summary>
    public class DestructableToggler : EditorUIComponent, ITilePropertyComponent
    {
        private TileSelector tileSelector;

        private ITextureService textureService;

        public DestructableToggler(EditorScreen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileSelector tileSelector)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileSelector = tileSelector;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));

            destructable = false;
            conflict = false;
        }

        private bool destructable;

        public bool Destructable
        {
            get { return destructable; }
        }

        private bool conflict;

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClickAndUnclick(int button)
        {
            switch (button)
            {
                case 0: // Left click: turn edges on.
                    tileSelector.SetDestructable(true);
                    break;

                case 2: // Right click: turn edges off.
                    tileSelector.SetDestructable(false);
                    break;

            }
            base.OnMouseClickAndUnclick(button);
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            bool mouseInside = MouseIsInside();

            // For now we just pretend tiles can be either solid or not, no in-between:
            if (conflict)
            {
                Color = new Microsoft.Xna.Framework.Graphics.Color(255, 0, 255);
            }
            else if (destructable)
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

            base.Draw(gameTime);
        }

        #region ITilePropertyComponent Members


        public void OnTileSelectionChange()
        {
            List<Tile> selectedTiles = tileSelector.SelectedTiles;
            if (selectedTiles.Count > 0)
            {
                if (selectedTiles.First<Tile>() is DestructableTile)
                {
                    IEnumerable<Tile> destructables = (from tile in selectedTiles
                                                       where tile is DestructableTile
                                                       select tile);
                    if (destructables.Count() == selectedTiles.Count())
                    {
                        destructable = true;
                        conflict = false;
                    }
                    else
                    {
                        conflict = true;
                    }
                }
                else
                {
                    IEnumerable<Tile> nonDestructables = (from tile in selectedTiles
                                                       where !(tile is DestructableTile)
                                                       select tile);
                    if (nonDestructables.Count() == selectedTiles.Count())
                    {
                        destructable = false;
                        conflict = false;
                    }
                    else
                    {
                        conflict = true;
                    }
                }
            }
            else
            {
                conflict = false;
                destructable = false;
            }
        }

        #endregion
    }
}
