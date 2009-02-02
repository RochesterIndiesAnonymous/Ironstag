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
    /// <summary>
    /// The projectile that Bandit enemies fire at the player
    /// </summary>
    public class BanditNormalProjectile : Projectile
    {
        /// <summary>
        /// The animation object to to use for animating this projectile
        /// </summary>
        private static readonly Animation BanditProjectileAnimation = new Animation("ActorXML\\AnimationXML\\FlintBullet", "Bullet");
        
        /// <summary>
        /// The amount of damage a single projectile does to the player
        /// </summary>
        private static readonly int Damage = 50;
        
        /// <summary>
        /// The speed of the projectile
        /// </summary>
        private static readonly Vector2 Velocity = new Vector2(4f, 0f);

        /// <summary>
        /// Creates a new bandit projectile and adds it to the screen
        /// </summary>
        /// <param name="screen">The screen that this projectile belongs to</param>
        /// <param name="batch">The sprite batch object used to draw this projectile</param>
        /// <param name="position">The position of the projectile in world coordinates</param>
        /// <param name="owner">The character that fired this projectile</param>
        /// <param name="direction">The direction that is projectile is moving. 1 to the right, -1 to left</param>
        public BanditNormalProjectile(Screen screen, SpriteBatch batch, Vector2 position, object owner, short direction)
            : base(screen, batch, position, BanditNormalProjectile.BanditProjectileAnimation, direction, BanditNormalProjectile.Velocity,
                owner, DamageCategory.Player, BanditNormalProjectile.Damage)
        {
        }
    }
}

