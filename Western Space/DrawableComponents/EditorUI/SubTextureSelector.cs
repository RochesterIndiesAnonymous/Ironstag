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
using System.Collections;

namespace WesternSpace.DrawableComponents.EditorUI
{
    public class SubTextureSelector : EditorUIComponent, ITilePropertyComponent
    {
        private TileSelector tileSelector;

        private ITextureService textureService;

        private SubTexture subTexture;

        public SubTexture SubTexture
        {
            get { return subTexture; }
            set { subTexture = value; }
        }

        // Set to true if the user is currently
        //  selecting a SubTexture. Changes how this behaves
        //  quite a bit.
        private bool isSelectingSubTexture;

        // The index into the textureService SheetArray property that gets us the current
        //  sheet we're selecting from:
        private int sheetIndex;

        /// <summary>
        /// The sheet we're currently selecting from.
        /// </summary>
        private SubTextureSheet selectingSheet
        {
            get { return textureService.SheetsArray[sheetIndex]; }
            set
            {
                int index;
                for (index = 0; index < textureService.SheetsArray.Count<SubTextureSheet>(); ++index)
                {
                    if (value == textureService.SheetsArray[index])
                    {
                        sheetIndex = index;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Used to get the SubTexture from the Sheet we're currently hovering over.
        /// </summary>
        public SubTexture hoveringTexture
        {
            get
            {
                Vector2 offsetMouse = Mouse.ScaledPosition - Position;

                int x = (int)(offsetMouse.X / selectingSheet.SubTextureWidth);
                int y = (int)(offsetMouse.Y / selectingSheet.SubTextureHeight);

                int index = (int)MathHelper.Clamp(y * selectingSheet.Width + x, 0, selectingSheet.Width * selectingSheet.Height);
                return selectingSheet.SubTextures[index];
            }
        }

        public TileMap TileMap
        {
            get { return tileSelector.TileMap; }
        }

        private int layerIndex;

        public int LayerIndex
        {
            get { return layerIndex; }
            set { layerIndex = value; }
        }

        private int subLayerIndex;

        public int SubLayerIndex
        {
            get { return subLayerIndex; }
            set { subLayerIndex = value; }
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseUnclick(int button)
        {
            if (!isSelectingSubTexture && button == 0)
                SetSelectingSubTexture(true);
            else if (isSelectingSubTexture)
            {
                if (button == 0) // Left click, select the subTexture we're hovering over
                {
                    SubTexture = hoveringTexture;
                    tileSelector.SetSubTexture(SubTexture, LayerIndex, SubLayerIndex);
                }

                // Right/middle click will cancel.
                Mouse.ButtonsUnclicked[button] = false;
                SetSelectingSubTexture(false);
            }
            else if (button == 2) // Right click, clear the currentTexture
            {
                SubTexture = null;
                tileSelector.SetSubTexture(SubTexture, LayerIndex, SubLayerIndex);
            }
            
            base.OnMouseUnclick(button);
        }

        protected override void OnMouseUnClickOutside(int button)
        {
            if (isSelectingSubTexture)
            {
                SetSelectingSubTexture(false);
                //Mouse.ButtonsClicked[button] = false;
            }
            base.OnMouseUnClickOutside(button);
        }

        protected override void OnMouseScroll(int amount)
        { 
            // I'm thinking we could change the subTextureSheet
            //  from here.
            if (isSelectingSubTexture)
            {
                this.sheetIndex = (int)MathHelper.Clamp((this.sheetIndex + (amount > 0 ? 1 : -1)),
                                                                 0,
                                                                 textureService.SheetsArray.Count<SubTextureSheet>() - 1);
                this.Bounds = new RectangleF(Position.X, Position.Y, selectingSheet.Texture.Width, selectingSheet.Texture.Height);
            }
            base.OnMouseScroll(amount);
        }

        #endregion

        private void SetSelectingSubTexture(bool value)
        {
            isSelectingSubTexture = value;
            tileSelector.Enabled = !value;
            tileSelector.Visible = !value;
            this.DrawOrder = value ? -500 : 0;
            foreach (SubTextureSelector sts in tileSelector.SubTextureSelectors)
            {
                if (sts != this)
                {
                    sts.Enabled = !value;
                    sts.DrawOrder = value ? 500 : 0;
                }
            }

            if (isSelectingSubTexture)
            {
                // We're now selecting a texture.
                if (subTexture != null)
                    this.selectingSheet = subTexture.Sheet;
                this.Bounds = new RectangleF(Position.X, Position.Y, selectingSheet.Texture.Width, selectingSheet.Texture.Height);
            }
            else
            {
                Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(0, 0, TileMap.TileWidth, TileMap.TileHeight);
                this.Bounds = new RectangleF(Position.X, Position.Y, rect.Width, rect.Height);
            }
        }

        public SubTextureSelector(Screen parentScreen, SpriteBatch spriteBatch, TileSelector tileSelector, int layerIndex, int subLayerIndex)
            :base(parentScreen, spriteBatch, new RectangleF())
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
            this.isSelectingSubTexture = false;
            this.layerIndex = layerIndex;
            this.subLayerIndex = subLayerIndex;

            this.tileSelector = tileSelector;

            // We make the silly assumption that there is at least one Sheet loaded. he-he-he.
            this.selectingSheet = textureService.SheetsArray[0];

            // Generate our position based on the layer/subLayer-index we have.
            int padding = 10;
            int hzOffset = -(TileMap.TileWidth + padding);
            int oneDown = padding + TileMap.TileHeight;
            int vtOffset = layerIndex*(TileMap.SubLayerCount*oneDown + 2*padding) + (subLayerIndex * oneDown) + padding;
            Position = new Vector2(tileSelector.Bounds.Left + hzOffset, tileSelector.Bounds.Top + vtOffset);

            Microsoft.Xna.Framework.Rectangle rect = tileSelector.TileMap.Sheets[0].Rectangles[0];
            this.Bounds = new RectangleF(Position.X, Position.Y, rect.Width, rect.Height);
        }

        public override void Draw(GameTime gameTime)

        {
            if (isSelectingSubTexture) // User is selecting a subtexture from us...
            {
                this.SpriteBatch.Draw(selectingSheet.Texture, Position, Microsoft.Xna.Framework.Graphics.Color.White);
                if (MouseIsInside())
                {
                    // Highlight the subTexture our mouse is hovering over:
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)Position.X + hoveringTexture.Rectangle.X,
                                                                 (int)Position.Y + hoveringTexture.Rectangle.Y,
                                                                 hoveringTexture.Rectangle.Width,
                                                                 hoveringTexture.Rectangle.Height);
                    PrimitiveDrawer.Instance.DrawRect(SpriteBatch, rect, Microsoft.Xna.Framework.Graphics.Color.White);
                }
                if (subTexture != null && selectingSheet == subTexture.Sheet)
                { 
                    // Highlight the texture that's actually selected currently:
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)Position.X + subTexture.Rectangle.X,
                                                                 (int)Position.Y + subTexture.Rectangle.Y,
                                                                 subTexture.Rectangle.Width,
                                                                 subTexture.Rectangle.Height);
                    PrimitiveDrawer.Instance.DrawRect(SpriteBatch, rect, Microsoft.Xna.Framework.Graphics.Color.CornflowerBlue);
                }
            }
            else // Waiting for user to activate us...
            {
                if (subTexture != null)
                {
                    this.SpriteBatch.Draw(subTexture.Texture, Position, subTexture.Rectangle,
                        Microsoft.Xna.Framework.Graphics.Color.White);
                }
                else
                {
                    // Draw a slash to indicate that there is *no* texture selected at all.
                    PrimitiveDrawer.Instance.DrawLine(SpriteBatch, new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.X+Bounds.Width, Bounds.Y+Bounds.Height),
                                Microsoft.Xna.Framework.Graphics.Color.White);
                }
            }
            base.Draw(gameTime);
        }

        #region ITilePropertyComponent Members

        public void OnTileSelectionChange()
        {
            List<Tile> tiles = tileSelector.SelectedTiles;

            int count = tiles.Count;

            if (count > 0)
            {
                // We only change our texture if the tileSelector has one or more
                //  tiles selected.
                Tile first = tiles.First<Tile>();

                // Special case: we may be selecting nothing but empty tiles:
                if (first == null)
                {
                    // If every other one is also null:
                    if ((from other in tiles where other == null select other).Count<Tile>() == count)
                    { 
                        // Set our SubTexture to null! Yay!
                        SubTexture = null;
                    }
                }
                else
                {
                    // We compare the subTexture of each tile in the list.
                    // If they're all the same, we change ours to whatever theirs is.
                    // Otherwise, we change ours to "unknown" to illustrate that 
                    //  the tiles differ in the subTexture resulting from Tile.Textures[layerIndex,subLayerIndex].
                    if (
                        (from other in tiles 
                         where (
                            other != null && 
                            other.Textures[LayerIndex,SubLayerIndex] == first.Textures[LayerIndex,SubLayerIndex]
                         ) select other).Count<Tile>() == count) // How you like 'dem apples?
                    { 
                        // All subTextures are the same! Set our subTexture to the same thing too!
                        SubTexture = first.Textures[LayerIndex, SubLayerIndex];
                    }
                }

            }
            // Otherwise, we ignore "deselection" of tiles; we keep our current texture.
        }

        #endregion
    }
}