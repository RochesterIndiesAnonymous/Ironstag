using System;
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
        public static readonly int LIFETIME = 3000; // Time to live in milliseconds.
        
        private int timeToLive = LIFETIME;

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
            this.currentHealth = MaxHealth;

            hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, -halfHeight), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-halfWidth, 0), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(halfWidth, 0), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, halfHeight), HOTSPOT_TYPE.bottom));
        }

        public override void Draw(GameTime gameTime)
        {
            Color col = (timeToLive < 1000 && timeToLive >= 500 && ((timeToLive / 60) % 2 == 0)) || (timeToLive < 500 && ((timeToLive / 30) % 2 == 0)) ? Color.Red : Color.White;
            SpriteBatch.Draw(texture, Position-new Vector2(halfWidth, halfHeight), col);
            base.Draw(gameTime);
        }

        private float SLIDE_FRICTION = 0.7f;
        private float BOUNCE_FRICTION = 0.7f;

        public override void Update(GameTime gameTime)
        {
            timeToLive -= gameTime.ElapsedGameTime.Milliseconds;

            // Destroy self and spawn explosion if health is <0.
            if (currentHealth < 0 || timeToLive <= 0)
            {
                World.RemoveWorldObject(this);
                World.AddWorldObject(new Explosion(World, SpriteBatch, Position));
                this.Dispose();
                return;
            }

            NetForce += (new Vector2(0, 0.2f)) * Mass;
            World.PhysicsHandler.ApplyPhysics(this);

            // DO ALL COLLISIONS HERE
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                hotspot.Collide();
                if (hotspot.DidCollide)
                {
                    switch (hotspot.HotSpotType)
                    {
                        case HOTSPOT_TYPE.bottom:
                            velocity.Y *= -BOUNCE_FRICTION;
                            velocity.X *= SLIDE_FRICTION;
                            break;
                        case HOTSPOT_TYPE.top:
                            velocity.Y *= -BOUNCE_FRICTION;
                            velocity.X *= SLIDE_FRICTION;
                            break;

                        case HOTSPOT_TYPE.left:
                            velocity.X *= -BOUNCE_FRICTION;
                            velocity.Y *= SLIDE_FRICTION;
                            break;
                        case HOTSPOT_TYPE.right:
                            velocity.X *= -BOUNCE_FRICTION;
                            velocity.Y *= SLIDE_FRICTION;
                            break;
                    }
                }
            }
            base.Update(gameTime);
        }

        #region IDamageable Members

        private static readonly float MAX_HEALTH = 90; // This is one highly volatile barrel.

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

            if (objectCollidedWith is IPhysical)
            {
                Vector2 force = ((IPhysical)objectCollidedWith).Position - Position;
                float force_amount = 8.0f;
                force /= force.Length(); // Normalize vector to merely be directional.
                force *= force_amount;
                //((IPhysical)objectCollidedWith).NetForce += force;
                //this.NetForce -= force;
            }
        }

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

        #region ITileCollidable Members

        private List<CollisionHotspot> hotspots;

        public List<CollisionHotspot> Hotspots
        {
            get { return hotspots; }
        }

        #endregion
    }
}
