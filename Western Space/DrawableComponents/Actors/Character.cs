using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Services;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.AnimationFramework;

namespace WesternSpace.DrawableComponents.Actors
{
    public abstract class Character : DrawableGameObject
    {
        // The name used to identify the specific character.
        protected String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
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
        // character's movement.
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

        // Character constructor.
        // param: game - The over-arching Game object.
        // param: spriteBatch - The spriteBatch used to draw this object.
        // param: position - The character's position in the world. May not be visible by the camera's
        //                   current position.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public Character(Game game, SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(game, spriteBatch, position)
        {
            this.Position = position;
            animationMap = new Dictionary<string, Animation>();

        }

        // Sets up all of the Animations associated with the particular character
        // and adds them to the collection mapping states to animations.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public abstract void setUpAnimation(String xmlFile);

    }
}
