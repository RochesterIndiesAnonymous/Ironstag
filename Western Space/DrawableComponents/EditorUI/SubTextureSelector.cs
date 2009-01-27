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

namespace WesternSpace.DrawableComponents.EditorUI
{
    public class SubTextureSelector : EditorUIComponent
    {
        private TileSelector tileSelector;

        public Tile Tile
        {
            get { return tileSelector.Tile; }
        }

        private ITextureService textureService;

        // Set to true if the user is currently
        //  selecting a SubTexture. Changes how this behaves
        //  quite a bit.
        private bool selectingSubTexture;

        public SubTexture CurrentTexture
        {
            get
            {
                if (Tile != null)
                {
                    return Tile.Textures[layerIndex, subLayerIndex];
                }
                else { return null; }
            }

            set
            {
                Tile.Textures[layerIndex, subLayerIndex] = value;
            }
        }

        private int selectingSheetIndex;

        private SubTextureSheet selectingSheet
        {
            get { return textureService.SheetsArray[selectingSheetIndex]; }
            set
            {
                int index;
                for (index = 0; index < textureService.SheetsArray.Count<SubTextureSheet>(); ++index)
                {
                    if (value == textureService.SheetsArray[index])
                    {
                        selectingSheetIndex = index;
                        return;
                    }
                }
            }
        }

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

        protected override void OnMouseClick(int button)
        {
            
            base.OnMouseClick(button);
        }

        protected override void WhileMouseOutside()
        {
            /*
            if (selectingSubTexture && OutsideTime > 300)
            {
                SetSelectingSubTexture(false);
            }
            */
            base.WhileMouseOutside();
        }

        protected override void OnMouseUnclick(int button)
        {
            if (!selectingSubTexture && button == 0)
                SetSelectingSubTexture(true);
            else if (selectingSubTexture)
            {
                if (button == 0) // Left click, select the subTexture we're hovering over
                {
                    CurrentTexture = hoveringTexture;
                    selectingSheet = CurrentTexture.Sheet;
                }
                // Right/middle click will cancel.
                Mouse.ButtonsUnclicked[button] = false;
                SetSelectingSubTexture(false);
            }
            else if (button == 2) // Right click, clear the currentTexture
            {
                CurrentTexture = null;
                if (tileSelector.TileX >= 0 && tileSelector.TileY >= 0)
                {
                    TileMap.SetTile(Tile, tileSelector.TileX, tileSelector.TileY);
                }
                SetSelectingSubTexture(false);
            }
            base.OnMouseUnclick(button);
        }

        protected override void OnMouseClickOutside(int button)
        {
            if (selectingSubTexture)
            {
                SetSelectingSubTexture(false);
                Mouse.ButtonsClicked[button] = false;
            }
            base.OnMouseClickOutside(button);
        }

        protected override void OnMouseScroll(int amount)
        { 
            // I'm thinking we could change the subTextureSheet
            //  from here.
            if (selectingSubTexture)
            {
                this.selectingSheetIndex = (int)MathHelper.Clamp((this.selectingSheetIndex + (amount > 0 ? 1 : -1)),
                                                                 0,
                                                                 textureService.SheetsArray.Count<SubTextureSheet>() - 1);
                this.Bounds = new RectangleF(Position.X, Position.Y, selectingSheet.Texture.Width, selectingSheet.Texture.Height);
            }
            base.OnMouseScroll(amount);
        }

        #endregion

        private void SetSelectingSubTexture(bool value)
        {
            selectingSubTexture = value;
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

            if (selectingSubTexture)
            {
                // We're now selecting a texture.
                if (CurrentTexture != null)
                    this.selectingSheet = CurrentTexture.Sheet;
                this.Bounds = new RectangleF(Position.X, Position.Y, selectingSheet.Texture.Width, selectingSheet.Texture.Height);
            }
            else
            {
                Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(0, 0, TileMap.TileWidth, TileMap.TileHeight);
                this.Bounds = new RectangleF(Position.X, Position.Y, rect.Width, rect.Height);
            }
        }

        public SubTextureSelector(Game game, SpriteBatch spriteBatch, TileSelector tileSelector, int layerIndex, int subLayerIndex)
            :base(game, spriteBatch, new RectangleF())
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
            this.selectingSubTexture = false;
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
            if (selectingSubTexture) // User is selecting a subtexture from us...
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
                if (CurrentTexture != null && selectingSheet == CurrentTexture.Sheet)
                { 
                    // Highlight the texture that's actually selected currently:
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)Position.X + CurrentTexture.Rectangle.X,
                                                                 (int)Position.Y + CurrentTexture.Rectangle.Y,
                                                                 CurrentTexture.Rectangle.Width,
                                                                 CurrentTexture.Rectangle.Height);
                    PrimitiveDrawer.Instance.DrawRect(SpriteBatch, rect, Microsoft.Xna.Framework.Graphics.Color.CornflowerBlue);
                }
            }
            else // Waiting for user to activate us...
            {
                if (CurrentTexture != null)
                {
                    this.SpriteBatch.Draw(CurrentTexture.Texture, Position, CurrentTexture.Rectangle,
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
    }
}