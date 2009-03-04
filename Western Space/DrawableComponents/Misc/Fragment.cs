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
    /// <summary>
    /// A simple fragment of a texture that bounces around the world.
    /// </summary>
    class Fragment : WorldObject, ITileCollidable, ISpriteCollideable, IPhysical
    {
        public static readonly int LIFETIME = 800;

        private int timeToLive = LIFETIME;

        private SubTexture texture;

        public SubTexture Texture
        {
            get { return texture; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, SubRectangle.Width, SubRectangle.Height); }
        }


        private Rectangle subRectangle;

        public Rectangle SubRectangle
        {
            get { return subRectangle; }
        }

        public Fragment(World world, SpriteBatch spriteBatch, Vector2 position, SubTexture texture, Rectangle rectangle)
            :base(world, spriteBatch, position)
        {
            this.texture = texture;

            // Mass is proportional to the percentage of the texture in area.
            this.mass = ((subRectangle.Width * subRectangle.Height) / (texture.Rectangle.Width * texture.Rectangle.Height)) * 0.025f;
            this.subRectangle = new Rectangle(texture.Rectangle.X + rectangle.X, texture.Rectangle.Y + rectangle.Y,
                                              rectangle.Width, rectangle.Height);

            hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, subRectangle.Height / 2), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(subRectangle.Width, subRectangle.Height / 2), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(subRectangle.Width/2, 0), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(subRectangle.Width / 2, subRectangle.Height), HOTSPOT_TYPE.bottom));
        }

        private float rotation = 0.0f;

        public override void Draw(GameTime gameTime)
        {
            Color col = new Color(255.0f, 255.0f, 255.0f, (float)Math.Pow((double)timeToLive/(double)LIFETIME, 0.5));
            rotation += Velocity.X/10.0f;
            SpriteBatch.Draw(texture.Texture, Position, SubRectangle, col);
            SpriteBatch.Draw(texture.Texture,

                //this is the destination point we want to render at
                Position,

                //this is the texture surface area we want to use when rendering
                SubRectangle,

                //By using White we are basically saying just use the texture color
                col,

                //Here is the rotation in radians
                rotation,

                //This is the center point of the rotation
                new Vector2(SubRectangle.Width / 2, SubRectangle.Height / 2),

                //This is the scaling of the sprite. I want it a bit larger so you dont
                //see the edges as it spins around
                1.5f,

                //We are not doing any fancy effects
                SpriteEffects.None, 0);

            base.Draw(gameTime);
        }


        private float SLIDE_FRICTION = 0.7f;
        private float BOUNCE_FRICTION = 0.7f;

        public override void Update(GameTime gameTime)
        {
            timeToLive -= gameTime.ElapsedGameTime.Milliseconds;

            if (timeToLive <= 0)
            {
                World.RemoveWorldObject(this);
                this.Dispose();
                return;
            }

            NetForce += (new Vector2(0, 0.2f));
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

        #region ITileCollidable Members

        List<CollisionHotspot> hotspots;

        public List<CollisionHotspot> Hotspots
        {
            get { return hotspots; }
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

        private float mass;

        public float Mass
        {
            get
            {
                return 1;
            }
            set
            {
                mass = value;
            }
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

        bool removeFromCollisionRegistration;

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

        public void OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
            // Do nothing :)
            if(characterCollidedWith is Explosion)
                this.removeFromCollisionRegistration = true;
        }

        #endregion
    }
}
