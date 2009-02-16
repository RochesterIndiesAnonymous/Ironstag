using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace.TilingEngine
{
    class ShatterEffect : IDestructionEffect
    {
        #region IDestructionEffect Members

        public void OnDestruct(DestructableTile destructable)
        {
            SubTexture texture = destructable.Textures[destructable.Map.LayerCount-1,destructable.Map.SubLayerCount-1];
            Vector2 f1Pos = new Vector2(destructable.X*destructable.Map.TileWidth, destructable.Y*destructable.Map.TileHeight);
            Rectangle f1Rect = new Rectangle(0,0, texture.Rectangle.Width/2, texture.Rectangle.Height/2);
            Fragment f1 = new Fragment(destructable.World, destructable.World.SpriteBatch, f1Pos, texture, f1Rect);

            /*
            Vector2 f2Pos = new Vector2(f1Rect.X, destructable.Y * destructable.Map.TileHeight);
            Rectangle f2Rect = new Rectangle(f1Rect.Width, 0, texture.Rectangle.Width / 2, texture.Rectangle.Height / 2);
            Fragment f2 = new Fragment(destructable.World, destructable.World.SpriteBatch, f2Pos, texture, f2Rect);

            Vector2 f3Pos = new Vector2(destructable.X * destructable.Map.TileWidth, destructable.Y * destructable.Map.TileHeight + f1Rect.Height);
            Rectangle f3Rect = new Rectangle(f1Rect.X, f1Rect.Y, texture.Rectangle.Width / 2, texture.Rectangle.Height / 2);
            Fragment f3 = new Fragment(destructable.World, destructable.World.SpriteBatch, f1Pos, texture, f1Rect);
             * */
            destructable.World.AddWorldObject(f1);
        }

        #endregion
    }
}
