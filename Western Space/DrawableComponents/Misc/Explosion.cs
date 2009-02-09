using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Interfaces;
using WesternSpace.Collision;
using WesternSpace.Physics;
using WesternSpace.AnimationFramework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.Misc
{
    class Explosion : WorldObject, IDamaging, ISpriteCollideable
    {
        public static readonly string EXPLOSION_ANIM_FILENAME = "ActorXML\\MiscXML\\Explosion";
        public static readonly float FORCE_AMOUNT = 5.0f;

        public static int INITIAL_EXPLOSION_RADIUS = 1;
        public static int EXPLOSION_RADIUS_ACCELLERATION = 1;

        private int radius;

        private Animation explosionAnimation;

        private AnimationPlayer animationPlayer;


        public Explosion(World world, SpriteBatch spriteBatch, Vector2 position)
            :base(world, spriteBatch, position)
        {
            this.explosionAnimation = new Animation(EXPLOSION_ANIM_FILENAME, "Explosion");
            this.animationPlayer = new AnimationPlayer(SpriteBatch, explosionAnimation);
            this.radius = INITIAL_EXPLOSION_RADIUS;
        }

        public override void Draw(GameTime gameTime)
        {
            animationPlayer.Draw(gameTime, SpriteBatch, Position - animationPlayer.Animation.CenterOffset, SpriteEffects.None);
            //PrimitiveDrawer.Instance.DrawRect(SpriteBatch, Rectangle, Color.Black);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (animationPlayer.isDonePlaying())
            {
                World.RemoveWorldObject(this);
                this.Dispose();
                return;
            }
            radius += EXPLOSION_RADIUS_ACCELLERATION;
            animationPlayer.Update(gameTime);
            base.Update(gameTime);
        }

        #region IDamaging Members

        public object Owner
        {
            get { throw new NotImplementedException(); }
        }

        public WesternSpace.Utility.DamageCategory DoesDamageTo
        {
            get { return WesternSpace.Utility.DamageCategory.All; }
        }

        float amountOfDamage = 15;
        public float AmountOfDamage
        {
            get { return amountOfDamage; }
            set { amountOfDamage = value; }
        }

        #endregion

        #region ISpriteCollideable Members

        private int idNumber;

        public int IdNumber
        {
            get
            {
                return idNumber;
            }
            set
            {
                idNumber = value;
            }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X - radius, (int)Position.Y - radius, radius * 2, radius * 2); }
        }

        /// <summary>
        /// I dunno what this is for. bullets? 
        /// Explosions are OMNIDIRECTIONALLY DEADLY.
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteEffects collideableFacing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void OnSpriteCollision(ISpriteCollideable objectCollidedWith)
        {
            if (objectCollidedWith is IPhysical)
            {
                Vector2 force = ((IPhysical)objectCollidedWith).Position - Position;
                force /= force.Length(); // Normalize vector to merely be directional.
                force *= FORCE_AMOUNT;
                ((IPhysical)objectCollidedWith).NetForce += force;
            }

            if (objectCollidedWith is IDamageable)
            {
                ((IDamageable)objectCollidedWith).TakeDamage(this);
            }
        }

        #endregion
        #region ISpriteCollideable Members

        Boolean removeFromCollisionRegistration;
        public bool removeFromRegistrationList
        {
            get
            {
                return removeFromCollisionRegistration;
            }
            set
            {
                removeFromCollisionRegistration = value;
            }
        }

        #endregion
    }
}
