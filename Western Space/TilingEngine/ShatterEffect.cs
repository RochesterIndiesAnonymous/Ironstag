using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace.TilingEngine
{
    // TODO:
    class ShatterEffect : IDestructionEffect
    {
        static Random rand;
        #region IDestructionEffect Members

        public void OnDestruct(DestructableTile destructable)
        {
            if (rand == null)
                rand = new Random();
            SubTexture texture = destructable.Textures[destructable.Map.LayerCount-1,destructable.Map.SubLayerCount-1];
            if (texture != null)
            {
                int xslice, yslice, xinc, yinc;
                xslice = yslice = 0;

                List<Fragment> fragments = new List<Fragment>();

                while (xslice < destructable.Map.TileWidth) 
                {
                    yslice = 0;
                    xinc = Math.Min(rand.Next(4, 8), destructable.Map.TileWidth - xslice);
                    while (yslice < destructable.Map.TileHeight)
                    {
                        yinc = Math.Min(rand.Next(4, 8), destructable.Map.TileHeight - yslice);
                        Vector2 pos = new Vector2(destructable.X * destructable.Map.TileWidth + xslice, destructable.Y * destructable.Map.TileHeight + yslice);
                        Rectangle rect = new Rectangle(xslice, yslice, xinc, yinc);
                        destructable.World.AddWorldObject(new Fragment(destructable.World, destructable.World.SpriteBatch, pos, texture, rect));
                        yslice += yinc;
                    }
                    xslice += xinc;
                }
                //destructable.World.AddWorldObject(f1);
            }

            /*
            Vector2 f2Pos = new Vector2(f1Rect.X, destructable.Y * destructable.Map.TileHeight);
            Rectangle f2Rect = new Rectangle(f1Rect.Width, 0, texture.Rectangle.Width / 2, texture.Rectangle.Height / 2);
            Fragment f2 = new Fragment(destructable.World, destructable.World.SpriteBatch, f2Pos, texture, f2Rect);

            Vector2 f3Pos = new Vector2(destructable.X * destructable.Map.TileWidth, destructable.Y * destructable.Map.TileHeight + f1Rect.Height);
            Rectangle f3Rect = new Rectangle(f1Rect.X, f1Rect.Y, texture.Rectangle.Width / 2, texture.Rectangle.Height / 2);
            Fragment f3 = new Fragment(destructable.World, destructable.World.SpriteBatch, f1Pos, texture, f1Rect);
             * */
        }

        #endregion
    }
}
