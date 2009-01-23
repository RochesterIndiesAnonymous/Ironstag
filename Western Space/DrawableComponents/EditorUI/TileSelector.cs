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
    public class TileSelector : EditorUIComponent
    {
        private TileMap tileMap;

        public TileMap TileMap
        {
            get { return tileMap; }
        }

        private int tileX;

        public int TileX
        {
            get { return tileX; }
        }

        private int tileY;

        public int TileY
        {
            get { return tileY; }
        }

        public void SetTile(int x, int y)
        {
            tileX = x;
            tileY = y;
            // Notify all TextureSelectors of change
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
            base.OnMouseEnter();
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
        }

        public TileSelector(Game game, SpriteBatch spriteBatch, RectangleF bounds, TileMap tileMap)
            : base(game, spriteBatch, bounds)
        {
            this.tileMap = tileMap;
        }

        public override void Initialize()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
