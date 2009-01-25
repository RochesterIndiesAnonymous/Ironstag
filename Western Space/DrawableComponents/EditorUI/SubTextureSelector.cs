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

        private SubTexture virtualTexture;

        public SubTexture CurrentTexture
        {
            get
            {
                if (this.Tile != null)
                {
                    return Tile.Textures[layerIndex, subLayerIndex];
                }
                else
                {
                    return virtualTexture;
                }
            }

            set
            {
                if (this.Tile != null)
                {
                    Tile.Textures[layerIndex, subLayerIndex] = value;
                }
                virtualTexture = value;
            }
        }

        // The index into textureService.SheetArray[] which determines
        //  what virtualSheet is.
        private int virtualSheetIndex;

        // A default sheet to switch to when we've selected
        //  a tile that has 'null' set as the SubTexture of 
        //  the sublayer associated with this selector.
        private SubTextureSheet virtualSheet
        {
            get { return textureService.SheetsArray[virtualSheetIndex]; }
        }

        public SubTexture hoveringTexture
        {
            get
            {
                Vector2 offsetMouse = Mouse.ScaledPosition - Position;

                int x = (int)(offsetMouse.X / currentSheet.SubTextureWidth);
                int y = (int)(offsetMouse.Y / currentSheet.SubTextureHeight);

                int index = (int)MathHelper.Clamp(y * currentSheet.Width + x, 0, currentSheet.Width * currentSheet.Height);
                return currentSheet.SubTextures[index];
            }
        }

        private SubTextureSheet currentSheet
        {
            get
            {
                if (CurrentTexture != null)
                {
                    return CurrentTexture.Sheet;
                }
                else
                {
                    return virtualSheet;
                }
            }
        }

        private int currentSheetIndex
        {
            get
            {
                if (CurrentTexture != null)
                {
                    return CurrentTexture.Index;
                }
                else
                {
                    return -1;
                }
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

        public override void OnMouseClick(int button)
        {
            if (!selectingSubTexture)
                SetSelectingSubTexture(true);
            base.OnMouseClick(button);
        }

        public override void WhileMouseOutside()
        {
            if (selectingSubTexture && OutsideTime > 300)
            {
                SetSelectingSubTexture(false);
            }
            base.WhileMouseOutside();
        }

        public override void OnMouseUnclick(int button)
        {
            if (selectingSubTexture)
            {
                if (button == 0) // Left click, select the subTexture we're hovering over
                {
                    CurrentTexture = hoveringTexture;
                }
                else if (button == 2) // Right click, clear the currentTexture
                {
                    CurrentTexture = null;
                }
                else // Ignore other mouse clicks.
                {
                    return;
                }
                Mouse.ButtonsUnclicked[button] = false;
                SetSelectingSubTexture(false);
            }
            else
            {
                SetSelectingSubTexture(true);
            }
            base.OnMouseUnclick(button);
        }

        public override void OnMouseScroll(int amount)
        { 
            // I'm thinking we could change the subTextureSheet
            //  from here.
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

            if (value)
            {
                if (currentSheet != null)
                {
                    this.Bounds = new RectangleF(Position.X, Position.Y, currentSheet.Texture.Width, currentSheet.Texture.Height);
                }
            }
            else
            {
                Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(0, 0, TileMap.tileWidth, TileMap.tileHeight);
                this.Bounds = new RectangleF(Position.X, Position.Y, rect.Width, rect.Height);
            }
        }

/*        public override void Update(GameTime gameTime)
        {
            if (Tile != null)
            {
                base.Update(gameTime);
            }
        }*/

        public SubTextureSelector(Game game, SpriteBatch spriteBatch, TileSelector tileSelector, int layerIndex, int subLayerIndex)
            :base(game, spriteBatch, new RectangleF())
        {
            this.selectingSubTexture = false;
            this.layerIndex = layerIndex;
            this.subLayerIndex = subLayerIndex;

            this.tileSelector = tileSelector;
            
            this.virtualSheetIndex = 0;

            // Generate our position based on the layer/subLayer-index we have.
            int padding = 10;
            int hzOffset = -(TileMap.tileWidth + padding);
            int oneDown = padding + TileMap.tileHeight;
            int vtOffset = layerIndex*(TileMap.SubLayerCount*oneDown + 2*padding) + (subLayerIndex * oneDown) + padding;
            Position = new Vector2(tileSelector.Bounds.Left + hzOffset, tileSelector.Bounds.Top + vtOffset);

            Microsoft.Xna.Framework.Rectangle rect = tileSelector.Tile.Textures[0, 0].Rectangle;
            this.Bounds = new RectangleF(Position.X, Position.Y, rect.Width, rect.Height);
        }

        public override void Initialize()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)

        {
            if (selectingSubTexture) // User is selecting a subtexture from us...
            {
                this.SpriteBatch.Draw(currentSheet.Texture, Position, Microsoft.Xna.Framework.Graphics.Color.White);
                if (MouseIsInside())
                {
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)Position.X + hoveringTexture.Rectangle.X,
                                                                 (int)Position.Y + hoveringTexture.Rectangle.Y,
                                                                 hoveringTexture.Rectangle.Width,
                                                                 hoveringTexture.Rectangle.Height);
                    PrimitiveDrawer.Instance.DrawRect(SpriteBatch, rect, Microsoft.Xna.Framework.Graphics.Color.White);
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
                    // Draw an X to indicate that there is *no* texture selected at all.
                    PrimitiveDrawer.Instance.DrawLine(SpriteBatch, new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.X+Bounds.Width, Bounds.Y+Bounds.Height),
                                Microsoft.Xna.Framework.Graphics.Color.White);
                }
            }
            base.Draw(gameTime);
        }
    }
}
