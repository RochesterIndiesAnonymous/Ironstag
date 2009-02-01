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
    public abstract class Character : DrawableGameObject, ITileCollideable, ISpriteCollideable
    {

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
        /// Bounding Rectangle used for the character's current sprite.
        /// </summary>
        protected Rectangle rectangle;
        public Rectangle Rectangle
        {
            get
            {
                rectangle = new Rectangle((int)this.position.X, (int)this.position.Y,
                    this.animationPlayer.Animation.FrameWidth,
                    this.animationPlayer.Animation.FrameHeight);
                return rectangle;
            }
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


        static int idNumberCount = 0;
        protected int idNumber;
        public int IdNumber
        {
            get { return idNumber; }
        }

        /// <summary>
        /// Character constructor.
        /// </summary>
        /// <param name="parentScreen">The screen this character belongs to.</param>
        /// <param name="spriteBatch">The sprite batch used to draw this object.</param>
        /// <param name="position">The character's position in the world.</param>
        /// <param name="xmlFile">The XML file name which stores the Character's data.</param>
        public Character(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, position)
        {
            this.Position = position;
            this.collisionHotSpots = new List<CollisionHotspot>();
            this.roleMap = new Dictionary<string, Role>();
	        idNumber = idNumberCount;
            idNumberCount++;

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
        /// Creates the role objects for a given character.
        /// </summary>
        /// <param name="xmlFile">The xml file which stores role information.</param>
        public abstract void SetUpRoles(String xmlFile);

        #region ITileCollideable Members
        protected List<CollisionHotspot> collisionHotSpots;
        public List<CollisionHotspot> Hotspots
        {
            set { collisionHotSpots = value; }
            get { return collisionHotSpots; }
        }
        public Vector2 OnTileColision(Tile tile, CollisionHotspot hotSpot, Rectangle tileRectangle)
        {
            Vector2 newPosition = hotSpot.HostPosition;
            this.isOnGround = hotSpot.IsOnGround;
            // Default Collision Actions
            //if (tile.TopEdge && hotSpot.HotSpotType == HOTSPOT_TYPE.bottom)
            //{
            ////    // Puts the sprite above the tile;      
            //    //newPosition = new Vector2(hotSpot.HostPosition.X,
            //    //    hotSpot.HostPosition.Y - (hotSpot.WorldPosition.Y - tileRectangle.Top));
            //    //isOnGround = true;
            //}
            //if (tile.BottomEdge && hotSpot.HotSpotType == HOTSPOT_TYPE.top)
            //{
            //    newPosition = new Vector2(hotSpot.HostPosition.X,
            //        hotSpot.HostPosition.Y + (tileRectangle.Bottom - hotSpot.WorldPosition.Y));
            //    isOnGround = false;
            //}
            //if (tile.LeftEdge && hotSpot.HotSpotType == HOTSPOT_TYPE.right)
            //{
            //    newPosition = new Vector2(hotSpot.HostPosition.X - (hotSpot.WorldPosition.X - tileRectangle.Left),
            //        hotSpot.HostPosition.Y);
            //}
            //if (tile.RightEdge && hotSpot.HotSpotType == HOTSPOT_TYPE.left)
            //{
            //    newPosition = new Vector2(hotSpot.HostPosition.X + (tileRectangle.Right - hotSpot.WorldPosition.X),
            //     hotSpot.HostPosition.Y);
            //}

            //if (this.Position == newPosition)
            //{
            //    isOnGround = false;
            //}

            //this.Position = newPosition;
            return newPosition;
        }

        #endregion

        #region ISpriteCollideable Members

        public void OnSpriteCollision(Character characterCollidedWith)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}