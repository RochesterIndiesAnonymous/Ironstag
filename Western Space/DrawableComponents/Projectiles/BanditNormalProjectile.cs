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
    public class BanditNormalProjectile : Projectile
    {
        private static readonly Animation BanditProjectileAnimation = new Animation("ActorXML\\AnimationXML\\FlintBullet", "Bullet");
        private static readonly int Damage = 50;
        private static readonly Vector2 Velocity = new Vector2(4f, 0f);

        public BanditNormalProjectile(Screen screen, SpriteBatch batch, Vector2 position, object owner, short direction)
            : base(screen, batch, position, BanditNormalProjectile.BanditProjectileAnimation, direction, BanditNormalProjectile.Velocity,
                owner, DamageCategory.Enemy, BanditNormalProjectile.Damage)
        {
        }
    }
}

