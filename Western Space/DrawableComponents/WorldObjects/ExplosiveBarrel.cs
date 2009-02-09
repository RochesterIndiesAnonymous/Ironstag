﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Physics;

using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Misc;
using WesternSpace.Collision;

namespace WesternSpace.DrawableComponents.WorldObjects
{
    /// <summary>
    /// A barrel... that EXPLODES!
    /// </summary>
    public class ExplosiveBarrel : WorldObject, ISpriteCollideable, ITileCollidable, IDamageable, IPhysical
    {
        private Texture2D texture;

        public ExplosiveBarrel(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            this.velocity = Vector2.Zero;
            this.netForce = Vector2.Zero;
            this.mass = DEFAULT_MASS;
            this.texture = ((ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService))).GetTexture("Textures\\TNTBarrel");
            this.halfWidth = this.texture.Width / 2;
            this.halfHeight = this.texture.Height / 2;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Draw(texture, Position-new Vector2(halfWidth, halfHeight), Color.White);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // Destroy self and spawn explosion if health is <0.
            if (currentHealth < 0)
            {
                World.RemoveWorldObject(this);
                World.AddWorldObject(new Explosion(World, SpriteBatch, Position));
                this.Dispose();
                return;
            }
            // Otherwise, apply some gravity for shits and grins
            base.Update(gameTime);
        }

        #region IDamageable Members

        private static readonly float MAX_HEALTH = 20; // This is one highly volatile barrel.

        public float MaxHealth
        {
            get { return MAX_HEALTH; }
        }

        private float currentHealth; 

        public float CurrentHealth
        {
            get { return currentHealth; }
        }

        public float MitigationFactor
        {
            get { return 1; }
        }

        public DamageCategory TakesDamageFrom
        {
            get { return DamageCategory.All; }
        }

        public void TakeDamage(IDamaging damageItem)
        {
            this.currentHealth -= damageItem.AmountOfDamage;
        }

        #endregion

        #region IPhysical Members

        public PhysicsHandler PhysicsHandler
        {
            get { return World.PhysicsHandler; }
        }

        private Vector2 velocity;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private Vector2 netForce;

        public Vector2 NetForce
        {
            get { return netForce; }
            set { netForce = value; }
        }

        private static readonly float DEFAULT_MASS = 2.0f;

        private float mass;

        public float Mass
        {
            get { return mass; }
            set { Mass = value; }
        } 


        #endregion

        #region ISpriteCollideable Members
        private int idNumber;

        public int IdNumber
        {
            get { return idNumber; }
            set { idNumber = value; }
        }

        private int halfWidth, halfHeight;

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X-halfWidth, (int)Position.Y-halfHeight, 
                                        halfWidth*2, halfHeight*2); }
        }

        public SpriteEffects collideableFacing
        {
            get
            {
                return SpriteEffects.None;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void OnSpriteCollision(ISpriteCollideable objectCollidedWith)
        {
            if (objectCollidedWith is IDamaging)
            {
                this.TakeDamage((IDamaging)objectCollidedWith);
            }
        }

        #endregion

        #region ITileCollidable Members

        public List<CollisionHotspot> Hotspots
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
