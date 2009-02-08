using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using WesternSpace.Collision;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Audio;
using WesternSpace.Physics;
using WesternSpace.Utility;
using WesternSpace.DrawableComponents.Projectiles;
using WesternSpace.Interfaces;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.DrawableComponents.WorldObjects
{
    class FlintHat : WorldObject, ITileCollidable, IPhysical
    {

        // -- Constants -- //
        public static readonly string FALL = "Fall";
        public static readonly string XMLFILENAME = "ActorXML\\MiscXML\\" + typeof(FlintHat).Name;

        /// <summary>
        /// Vector representing the Velocity of moving in the air.
        ///</summary>
        readonly Vector2 airVelocity = new Vector2(2f, 0f);

        /// <summary>
        /// Vector representing the Acceleration due to gravity.
        /// </summary>
        public readonly Vector2 gravity = new Vector2(0f, 0.05f);

        /// <summary>
        /// The world object's animation player. This player
        /// is responsible for playing the object's
        /// current animation.
        /// </summary>
        protected AnimationPlayer animationPlayer;

        public AnimationPlayer AnimationPlayer
        {
            get { return animationPlayer; }
        }

        /// <summary>
        /// A collection mapping a character's animation objects
        /// to their available states.
        /// </summary>
        protected Dictionary<String, Animation> animationMap;

        public Dictionary<String, Animation> AnimationMap
        {
            get { return animationMap; }
        }

        bool isOnGround = false;

        String currentState;

        /// <summary>
        /// The game screen this object belongs to
        /// This is used to get access to the camera and collision manager
        /// </summary>
        private Screen gameScreen;

        SpriteEffects facing = SpriteEffects.None;

        Character owner;

        Vector2 velocity;

        Animation currentAnimation;

        World world;

        public Vector2 UpperLeft
        {
            get
            {
                if (facing.Equals(SpriteEffects.None))
                {
                    return new Vector2((position.X - currentAnimation.CenterOffsetX), (position.Y - currentAnimation.CenterOffsetY));
                }
                else if (facing.Equals(SpriteEffects.FlipHorizontally))
                {
                    return new Vector2((position.X - (currentAnimation.FrameWidth - currentAnimation.CenterOffsetX)), (position.Y - currentAnimation.CenterOffsetY));
                }
                else if (facing.Equals(SpriteEffects.FlipVertically))
                {
                    return Vector2.Zero;
                }
                else
                {
                    return Vector2.Zero;
                }
            }
        }

        /// <summary>
        /// Constructor for Flint's Hat.
        /// </summary>
        /// <param name="parentScreen">The screen which this object is a part of.</param>
        /// <param name="spriteBatch">The sprite batch which handles drawing this object.</param>
        /// <param name="position">The initial position of this character.</param>
        public FlintHat(Screen gameScreen, World world, SpriteBatch spriteBatch, Vector2 position, Character owner)
            : base(world, spriteBatch, position)
        {
            mass = 1.0f;
            //Set the Hat's owner
            this.owner = owner;

            this.gameScreen = gameScreen;

            //Set up the Animation Map
            animationMap = new Dictionary<string, Animation>();

            //Set up the Hat's Animations
            SetUpAnimation();

            //Set the current Animation
            currentAnimation = animationMap["Fall"];

            animationPlayer = new AnimationPlayer(spriteBatch, animationMap["Fall"]);

            currentState = "None";

            //Set the Hat's positon
            position = owner.Position + new Vector2(60, 0);

            //Set the hat's velocity
            velocity = new Vector2(0, 0);

            //Initializes the hat's hotspots.
            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(-1, -10), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-7, 1), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(5, 1), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-1, 12), HOTSPOT_TYPE.bottom));
            Hotspots = hotspots;

            this.world = world;
            world.AddWorldObject(this);
            //this.ParentScreen.Components.Add(this);
        }

        /// <summary>
        /// Registers this hat with the collision manager
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Sets up all of this object's animations and adds them to the animation map.
        public void SetUpAnimation()
        {
            Animation fall = new Animation(XMLFILENAME, FALL);

            this.animationMap.Add(FALL, fall);
        }

        /// <summary>
        /// Changes both the hat's state and animation to the given state.
        /// If the hat is already in a given state, no change is to be made.
        /// </summary>
        /// <param name="newState">The new state to change to.</param>
        public void ChangeState(String newState)
        {

            if (!currentState.Equals(newState))
            {
                try
                {
                    currentState = newState;
                    animationPlayer.PlayAnimation(animationMap[newState]);
                }
                catch (KeyNotFoundException knfe)
                {
                }

            }
        }

        /// <summary>
        /// Applies the set air velocity to a velocity vector.
        /// </summary>
        /// <param name="direction">The direction the object is facing. Right = 1, Left = -1</param>
        public void ApplyAirMove(int direction)
        {
            velocity.X = (direction * airVelocity.X);
        }

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


        public override void Update(GameTime gameTime)
        {
            facing = owner.Facing;

            if (currentState != "None")
            {
                int direction = 1;
                if (this.facing == SpriteEffects.FlipHorizontally)
                {
                    direction = -1;
                }

                NetForce += gravity / Mass;

                ApplyAirMove(direction);

                if (isOnGround)
                {
                    velocity = Vector2.Zero;
                }

                PhysicsHandler.ApplyPhysics(this);

            }
            else
            {
            }


            // DO ALL COLLISIONS HERE
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                hotspot.Collide();
            }

            isOnGround = false;
            IEnumerable<CollisionHotspot> hotspots = from hotspot in Hotspots
                                                     where hotspot.HotSpotType == HOTSPOT_TYPE.bottom && hotspot.DidCollide
                                                     select hotspot;
            if (hotspots.Count<CollisionHotspot>() > 0)
            {
                isOnGround = true;
            }

            animationPlayer.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            if (currentState != "None")
            {
                animationPlayer.Draw(gameTime, this.SpriteBatch, UpperLeft, facing);
            }

            #region FOR DEBUGGING COLLISION
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                PrimitiveDrawer.Instance.DrawLine(SpriteBatch, hotspot.WorldPosition, hotspot.WorldPosition + new Vector2(1, 1), Color.Black);
            }
            #endregion
        }

        #endregion

        #region IPhysical Members

        public PhysicsHandler PhysicsHandler
        {
            get { return World.PhysicsHandler; }
        }

        private Vector2 netForce;

        public Vector2 NetForce
        {
            get { return netForce; }
            set { netForce = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private float mass;

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        #endregion
    }
}