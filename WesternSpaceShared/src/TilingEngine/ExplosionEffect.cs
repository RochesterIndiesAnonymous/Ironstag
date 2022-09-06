using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace.TilingEngine
{
    class ExplosionEffect : IDestructionEffect
    {
        #region IDestructionEffect Members

        public void OnDestruct(DestructableTile destructable)
        {
            Explosion expl = new Explosion(destructable.World, destructable.World.SpriteBatch,
                new Vector2(destructable.X * destructable.Map.TileWidth + destructable.Map.TileWidth/2,
                            destructable.Y * destructable.Map.TileHeight + destructable.Map.TileHeight / 2));
            destructable.World.AddWorldObject(expl);
        }

        #endregion

        public ExplosionEffect() 
        {
        }
    }
}
