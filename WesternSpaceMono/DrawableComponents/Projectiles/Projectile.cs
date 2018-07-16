using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Collision;
using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.AnimationFramework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Actors;
using System.Diagnostics;
using WesternSpace.TilingEngine;

namespace WesternSpace.DrawableComponents.Projectiles
{
    /// <summary>
    /// Manages the the creation, updating, drawing, and destruction of a projectile
    /// </summary>
    public class Projectile : WorldObject, ISpriteCollideable, IDamaging, IDisposable, ITileCollidable
    {
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

        public Projectile(World world, SpriteBatch batch, Vector2 position, Texture2D texture, short direction,
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

            //Initializes the hat's hotspots.
            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(6, 0), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, 3), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(9, 3), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(6, 6), HOTSPOT_TYPE.bottom));
            Hotspots = hotspots;

            this.World.AddWorldObject(this);
        }

        /// <summary>
        /// Registers this projectile with the collision manager
        /// </summary>
        public override void Initialize()
        {

            //Debug.Print("Added Projectile");
            base.Initialize();
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
            this.Position += (direction * this.velocity);

            // DO ALL COLLISIONS HERE
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                IDamageable tile = hotspot.Tile as IDamageable;
                if (tile != null)
                {
                    tile.TakeDamage(this);
                }
                hotspot.Collide();
                if (hotspot.DidCollide)
                {
                    switch (hotspot.HotSpotType)
                    {
                        default:
                            this.World.RemoveWorldObject(this);
                            this.Dispose();
                            break;
                    }
                }
            }


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


            #region FOR DEBUGGING COLLISION
         /*   
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                PrimitiveDrawer.Instance.DrawLine(SpriteBatch, hotspot.WorldPosition, hotspot.WorldPosition + new Vector2(1, 1), Color.Black);
            }
            PrimitiveDrawer.Instance.DrawRect(SpriteBatch, Rectangle, Color.Black);
           */ 
            #endregion

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

        #region ITileCollideable Members
        protected List<CollisionHotspot> hotspotsFacingLeft = new List<CollisionHotspot>();
        protected List<CollisionHotspot> hotspotsFacingRight;

        public List<CollisionHotspot> Hotspots
        {
            // If you set one list of hot spots, mirror it for the other.
            set
            {
                hotspotsFacingRight = value;
                foreach (CollisionHotspot hotspot in hotspotsFacingRight)
                {
                    HOTSPOT_TYPE h = hotspot.HotSpotType;
                    if (hotspot.HotSpotType == HOTSPOT_TYPE.left)
                    {
                        h = HOTSPOT_TYPE.right;
                    }
                    else if (hotspot.HotSpotType == HOTSPOT_TYPE.right)
                    {
                        h = HOTSPOT_TYPE.left;
                    }
                    //CollisionHotspot newHotspot = new CollisionHotspot(this,new Vector2(this.CurrentAnimation.FrameWidth - hotspot.Offset.X,hotspot.Offset.Y),h);
                    CollisionHotspot newHotspot = new CollisionHotspot(this, new Vector2((-1) * hotspot.Offset.X, hotspot.Offset.Y), h);
                    hotspotsFacingLeft.Add(newHotspot);
                }
            }
            // Get the hotspot list based upon which direction the player is facing.
            get { return facing.Equals(SpriteEffects.FlipHorizontally) ? hotspotsFacingLeft : hotspotsFacingRight; }
        }

        #endregion
    }
}
