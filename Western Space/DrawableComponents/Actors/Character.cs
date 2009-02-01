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

namespace WesternSpace.DrawableComponents.Actors
{
    public abstract class Character : DrawableGameObject, ITileCollideable, ISpriteCollideable
    {

        // The name used to identify the specific character.
        protected String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
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
	static int idNumberCount = 0;
        protected int idNumber;
        public int IdNumber
        {
            get { return idNumber; }
        }



        // The character's maximum health. All characters
        // will have health, but non-enemy NPCs will not be able to
        // have their health drained.
        protected int maxHealth;

        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        //The character's current health.
        protected int currentHealth;

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        // The character's current state. A state determines
        // which animation the character will be displaying
        // and what actions they can take at any point in
        // time.
        protected String currentState;

        public String CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        // The character's current animation. The animation
        // class contains a list of frames and is played
        // by the character's AnimationPlayer object.
        protected Animation currentAnimation;

        public Animation CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }

        // The character's animation player. This object 
        // is responsible for playing the character's 
        // current animation.
        protected AnimationPlayer animationPlayer;

        public AnimationPlayer AnimationPlayer
        {
            get { return animationPlayer; }
        }

        // A collection mapping a character's animation objects
        // to their available states.
        protected Dictionary<String, Animation> animationMap;

        public Dictionary<String, Animation> AnimationMap
        {
            get { return animationMap; }
        }

        // The character's velocity vector. Determine's the
        // character's movement direction.
        protected Vector2 velocity;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        // The name of the sprite batch that this sprite needs to use to draw.
        protected static string spriteBatchName = "Camera Sensitive";

        // The name of the sprite batch that this sprite needs to use to draw.
        public static string SpriteBatchName
        {
            get { return Character.spriteBatchName; }
        }

        // True if the character is currently on the ground
        public bool isOnGround = true;

        // Character constructor.
        // param: game - The over-arching Game object.
        // param: spriteBatch - The spriteBatch used to draw this object.
        // param: position - The character's position in the world. May not be visible by the camera's
        //                   current position.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public Character(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, position)
        {
            this.Position = position;
            this.collisionHotSpots = new List<CollisionHotspot>();
            animationMap = new Dictionary<string, Animation>();
	    idNumber = idNumberCount;
            idNumberCount++;
        }

        // Sets up all of the Animations associated with the particular character
        // and adds them to the collection mapping states to animations.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public abstract void SetUpAnimation(String xmlFile);

        // Modifies the Character's current Health based on the value given. The given
        // value may either be positive or negative, denoting healing and taking damage
        // respectively.
        // param: modifier - Amount to change health by; Positive if healing
        //                   Negative if taking damage.
        public void ChangeHealth(int modifier)
        {
            this.currentHealth = currentHealth + modifier;
        }

        // Changes both the Character's state and animation to the given state.
        // If the character is already in the given state, no change is to be
        // made.
        // param: state - The state to change to.
        public void ChangeState(String newState)
        {
            if (!currentState.Equals(newState))
            {
                currentState = newState;
                animationPlayer.PlayAnimation(animationMap[newState]);
            }
        }

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