using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.Services;
using WesternSpace.AnimationFramework;
using WesternSpace.Collision;
using WesternSpace.TilingEngine;
using WesternSpace.Screens;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.Actors
{
    public abstract class Character : WorldObject, ITileCollidable
    {
        public static readonly string XMLPATH = "ActorXML";
        /// <summary>
        /// The name used to identify a specific character.
        /// </summary>
        protected String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The current Role of a Character. The character can
        /// only be one role at a time, but may change roles.
        /// </summary>
        protected Role currentRole;

        public Role CurrentRole
        {
            get { return currentRole; }
        }

        /// <summary>
        /// A mapping of the possible roles a character can
        /// take on to a string representing the role's name.
        /// </summary>
        protected Dictionary<string, Role> roleMap;

        public Dictionary<string, Role> RoleMap
        {
            get { return roleMap; }
        }

        /// <summary>
        /// The character's current state. A state determines
        /// which animation the character will be displaying
        /// and what actions they can take at any point in
        /// time.
        /// </summary>
        protected String currentState;

        public String CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        /// <summary>
        /// The character's current animation. The animation
        /// class contains a list of frames and is played
        /// by the character's AnimationPlayer object.
        /// </summary>
        protected Animation currentAnimation;

        public Animation CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }

        /// <summary>
        /// The character's animation player. This object
        /// is responsible for playing the character's
        /// current animation.
        /// </summary>
        protected AnimationPlayer animationPlayer;

        public AnimationPlayer AnimationPlayer
        {
            get { return animationPlayer; }
        }

        public Rectangle Rectangle
        {
            get 
            { 
                return new Rectangle((int)Position.X, (int)Position.Y, animationPlayer.Animation.FrameWidth, animationPlayer.Animation.FrameHeight);
            }
        }

        // The character's velocity vector. Determine's the
        // character's movement direction.

        /// <summary>
        /// The character's Velocity vector. Determine's the
        /// character's movement direction and speed.
        /// </summary>
        public Vector2 velocity;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// The name of the sprite batch this character will use to draw.
        /// </summary>
        protected static string spriteBatchName = "Camera Sensitive";

        public static string SpriteBatchName
        {
            get { return Character.spriteBatchName; }
        }

        /// <summary>
        /// The direction the player is facing.
        /// </summary>
        protected SpriteEffects facing;

        public SpriteEffects Facing
        {
            get { return facing; }
        }

        /// <summary>
        /// Determines if the character is currently on the ground. True if
        /// the character is on the ground, and false otherwise.
        /// </summary>
        public bool isOnGround = true;


        //static int idNumberCount = 0;
        //protected int idNumber;
        //public int IdNumber
        //{
        //    get { return idNumber; }
        //}

        /// <summary>
        /// Character constructor.
        /// </summary>
        /// <param name="parentScreen">The screen this character belongs to.</param>
        /// <param name="spriteBatch">The sprite batch used to draw this object.</param>
        /// <param name="position">The character's position in the world.</param>
        /// <param name="xmlFile">The XML file name which stores the Character's data.</param>
        public Character(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            this.Position = position;
            this.hotspotsFacingRight = new List<CollisionHotspot>();
            this.hotspotsFacingLeft = new List<CollisionHotspot>();
            this.roleMap = new Dictionary<string, Role>();
	        //idNumber = idNumberCount;
            //idNumberCount++;

            //Set up the Roles for this Character
        }

        /// <summary>
        /// Changes both the Character's state and animation to the given state.
        /// If the character is already in a given state, no change is to be made.
        /// </summary>
        /// <param name="newState">The new state to change to.</param>
        public void ChangeState(String newState)
        {
            if (!currentState.Equals(newState))
            {
                try
                {
                    currentState = newState;
                    animationPlayer.PlayAnimation(currentRole.AnimationMap[newState]);
                }
                catch(KeyNotFoundException knfe)
                {
                }
                
            }
        }

        /// <summary>
        /// Changes both the Character's state and animation to a given state.
        /// The character's new animation starts on the same frame as the 
        /// previously playing animation and returns to that animation after
        /// three frames.
        /// </summary>
        /// <param name="newState"></param>
        public void ContinueAnimationNewState(String newState)
        {
            if (!currentState.Equals(newState))
            {
                try
                {
                    int start = 0;
                    int end = 0;

                    currentState = newState;

                    start = animationPlayer.CurrentFrame.FrameIndex;
                    end = start + 3;

                    if (end > animationPlayer.CurrentFrame.FrameIndex)
                    {
                        end = end % animationPlayer.Animation.FrameCount;
                    }

                    animationPlayer.PlayAnimation(currentRole.AnimationMap[newState], start, end);
                }
                catch (KeyNotFoundException knfe)
                {
                }
            }
        }

        /// <summary>
        /// Creates the role objects for a given character.
        /// </summary>
        /// <param name="xmlFile">The xml file which stores role information.</param>
        public abstract void SetUpRoles();

        #region ITileCollideable Members
        protected List<CollisionHotspot> hotspotsFacingLeft;
        protected List<CollisionHotspot> hotspotsFacingRight;

        public List<CollisionHotspot> Hotspots
        {
            // If you set one list of hot spots, mirror it for the other.
            set { hotspotsFacingRight = value;
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
                    CollisionHotspot newHotspot = new CollisionHotspot(this,new Vector2(this.CurrentAnimation.FrameWidth - hotspot.Offset.X,hotspot.Offset.Y),h);

                    hotspotsFacingLeft.Add(newHotspot);
                }
            }
            // Get the hotspot list based upon which direction the player is facing.
            get { return facing.Equals(SpriteEffects.FlipHorizontally) ? hotspotsFacingLeft : hotspotsFacingRight; }
        }

        public override void Update(GameTime gameTime)
        {
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
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position, facing);

            /*
            #region FOR DEBUGGING COLLISION
            foreach (CollisionHotspot hotspot in Hotspots)
            {
                PrimitiveDrawer.Instance.DrawLine(SpriteBatch, hotspot.WorldPosition, hotspot.WorldPosition + new Vector2(1, 1), Color.Black);
            }
            #endregion
            */
        }

        #endregion
    }
}