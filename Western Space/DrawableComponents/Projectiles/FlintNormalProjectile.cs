using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;
using WesternSpace.Screens;
using WesternSpace.Utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Physics;

namespace WesternSpace.DrawableComponents.Projectiles
{
    public class FlintNormalProjectile : Projectile
    {
        private static readonly Animation FlintProjectileAnimation = new Animation("ActorXML\\AnimationXML\\FlintBullet", "Bullet");
        private static readonly int Damage = 50;
        private static readonly Vector2 Velocity = new Vector2(1f, 0f);

        public FlintNormalProjectile(Screen screen, SpriteBatch batch, Vector2 position, object owner, short direction)
            : base(screen, batch, position, FlintNormalProjectile.FlintProjectileAnimation, direction, FlintNormalProjectile.Velocity,
                owner, DamageCategory.Enemy, FlintNormalProjectile.Damage)
        {
        }
    }
}
