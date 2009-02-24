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
using WesternSpace.ServiceInterfaces;
using WesternSpace.Collision;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.Projectiles
{
    /// <summary>
    /// The projectile that Bandit enemies fire at the player
    /// </summary>
    public class BossProjectile : WorldObject, ISpriteCollideable, IDamaging, IDisposable
    {
        /// <summary>
        /// The animation object to to use for animating this projectile
        /// </summary>
        internal static readonly Texture2D Texture = ((ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService))).GetTexture("Textures\\Enemies\\BanditBullet");
        
        /// <summary>
        /// The amount of damage a single projectile does to the player
        /// </summary>
        internal static readonly int Damage = 25;
        
        /// <summary>
        /// The speed of the projectile
        /// </summary>
        internal static readonly Vector2 Velocity = new Vector2(4f, 0f);

        /// <summary>
        /// The speed at which this projectile moves across the screen
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// The character that fired this projectile
        /// </summary>
        private object owner;
        
        /// <summary>
        /// the type of character that this projectile is attempting to hit
        /// </summary>
        private DamageCategory doesDamageTo;

        /// <summary>
        /// The amount of damage this projectile will do when it hits an enemy
        /// </summary>
        private float amountOfDamage;

        /// <summary>
        /// The direction constant to apply to the velocity vector
        /// </summary>
        private short direction;

        /// <summary>
        /// The direction to draw the projectile in
        /// </summary>
        private SpriteEffects facing;

        protected Texture2D texture;

        /// <summary>
        /// The collision manager that this projectile needs to register with
        /// </summary>
        private SpriteSpriteCollisionManager collision;

        /// <summary>
        /// The game screen this object belongs to
        /// This is used to get access to the camera and collision manager
        /// </summary>
        private GameScreen gameScreen;

        public BossProjectile(World world, SpriteBatch batch, Vector2 position, Texture2D texture, short direction,
            Vector2 velocity, object owner, DamageCategory doesDamageTo, int amountOfDamage)
            : base(world, batch, position)
        {

            this.texture = texture;
            this.direction = direction;
            this.velocity = velocity;
            this.owner = owner;
            this.doesDamageTo = doesDamageTo;
            this.amountOfDamage = amountOfDamage;

            facing = SpriteEffects.None;

            if (direction == 1)
            {
                facing = SpriteEffects.FlipHorizontally;
            }

            this.World.AddWorldObject(this);
        }

        /// <summary>
        /// Checks to see if the projectile is off the screen, updates the animation state, and updates the position
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            // check to see if we are outside the camera.
            if (this.Position.X > this.World.Camera.VisibleArea.X + this.World.Camera.VisibleArea.Width || this.Position.X + this.texture.Width < this.World.Camera.VisibleArea.X)
            {
                // This allows the bullet to be garbage collected. Verified that garbage collection happens on 2/1/2009
                this.World.RemoveWorldObject(this);
                this.Dispose();
                return;
            }

            // update the position of the projectile
            this.Position += new Vector2(direction * this.velocity.X, this.velocity.Y);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the projectile to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // draw the projectile
            SpriteBatch.Draw(texture, Position, Color.White);

            base.Draw(gameTime);
        }

        #region IDamaging Members

        public object Owner
        {
            get { return owner; }
        }

        public DamageCategory DoesDamageTo
        {
            get { return doesDamageTo; }
        }

        public float AmountOfDamage
        {
            get { return amountOfDamage; }
            set { amountOfDamage = value; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Called to flag that this projectile is ready to be garbage collected.
        /// Might not be needed :/
        /// </summary>
        void IDisposable.Dispose()
        {
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
            get 
            {
                int x = (int)(this.Position.X);
                int y = (int)(this.Position.Y);
                return new Rectangle(x, y, this.texture.Width, this.texture.Height);
            }
        }

        public void OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
            IDamageable damage = characterCollidedWith as IDamageable;

            if (damage != null && damage.TakesDamageFrom != this.DoesDamageTo)
            {
                // we hit
               /* this.gameScreen.World.SpriteCollisionManager.removeObjectFromRegisteredObjectList(this);
                this.ParentScreen.Components.Remove(this);
                this.Dispose(); */
            }
        }

        public SpriteEffects collideableFacing
        {
            get { return SpriteEffects.None; }
            set { }
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
    }
}

