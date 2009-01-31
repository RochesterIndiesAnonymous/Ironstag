using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using WesternSpace.Collision;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Actors
{
    class Player : Character
    {
        // --- Constants ---
       
        // -Horizontal Movement Constants

        // Acceleration Constant
        private const float moveAcceleration = 14000.0f;

        // Maximum Movement Speed
        private const float maxMoveSpeed = 200.0f;

        // Friction of the Ground
        private const float groundFriction = 0.58f;

        // Friction of the Air
        private const float airFriction = 0.65f;

        // -Vertical Movement Constants

        // Longest amount of time a jump can last
        private const float maxJumpTime = 0.7f;

        // Acceleration due to Gravity
        private const float gravity = 3500.0f;

        // Maximum Falling SPeed
        private const float maxFallSpeed = 60.0f;

        // Helps to give the player more control of the character
        // in the air.
        private const float jumpControl = 0.14f;

        // Helps to give the player more control of the character
        // in the air.
        private const float jumpVelocity = -4000.0f;

        // --- Class Variables ---

        private SpriteEffects facing;

        /// <summary>
        /// The direction the player is facing.
        /// </summary>
        public SpriteEffects Facing
        {
            get { return facing; }
        }

        // The maximum value of the transformation guage for
        // Flint Ironstag.
        private int maxGauge;

        public int MaxGuage
        {
            get { return maxGauge; }
            set { MaxGuage = value; }
        }

        // The current value of the transformation guage.
        private int currentGauge;

        public int CurrentGauge
        {
            get { return currentGauge; }
            set { currentGauge = value; }
        }

        // Value which returns true if Flint is currently in
        // his transformed state, and false if not.
        private bool isTransformed = false;

        public bool IsTransformed
        {
            get { return isTransformed; }
            set { isTransformed = value; }
        }

        // -Movement Variables

        // Direction in which the player is going. -1 for Left, 0 for not moving and
        // +1 for right.
        public float direction = 0f;

        // True if the user pressed the jump button
        public bool pressedJump = false;

        // The time the character has been jumping
        private float jumpTime;

        // The constructor for Flint Ironstag
        public Player(Screen parentScreen , SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, position, xmlFile)
        {
            //Set the character's Name
            name = "Flint Ironstag";

            //Set character's max and current health
            maxHealth = 100;
            currentHealth = maxHealth;

            //Set the character's transformation guage
            maxGauge = 100;
            currentGauge = maxGauge;

            //Create all of the Animations for Flint Ironstag
            SetUpAnimation(xmlFile);

            //Create the Animation Player and give it the Idle Animation
            this.animationPlayer = new AnimationPlayer(spriteBatch, animationMap["Idle"]);

            //Set the current animation
            currentAnimation = animationPlayer.Animation;

            //Set the current state
            currentState = "Idle";

            //Set the Velocity
            velocity = new Vector2(0, 0);

            //Set the position
            this.Position = position;

            //Set the facing
            facing = SpriteEffects.None;

            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(16, 0), HOTSPOT_TYPE.top));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(0, 16), HOTSPOT_TYPE.left));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(36, 16), HOTSPOT_TYPE.right));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(16, 39), HOTSPOT_TYPE.bottom));
        }

        // Called when the player presses the jump button. If the player is already
        // in a jumping state (or jumping and shooting) then no action is to occurr.
        public void Jump(GameTime gameTime)
        {
            //If the jump button was pressed.
            if (pressedJump)
            {
                //Start a Jump or continue jumping
                if ( ((!currentState.Equals("JumpingAscent") || !currentState.Equals("JumpingDescent")) && isOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                    {
                        //Play Sound if necessary
                        ChangeState("JumpingAscent");
                        isOnGround = false;
                    }

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //If in jump ascent
                if (0 < jumpTime && jumpTime <= maxJumpTime)
                {
                    //Override the vertical velocity w/ curve to give Player more control in the jump
                    velocity.Y = jumpVelocity * (1.0f - (float)Math.Pow(jumpTime / maxJumpTime, jumpControl));
                }
                else
                {
                    //Reached the top of the jump
                    jumpTime = 0.0f;
                    ChangeState("JumpingDescent");
                }
            }
            else
            {
                //Not Jumping
                jumpTime = 0.0f;
            }
        }

        // Called when the player presses the shoot button. If the player is already
        // in a shooting state, then no action is to occurr.
        public void Shoot()
        {
            if (!isTransformed)
            {
                if (!currentState.Equals("Shooting") || !currentState.Equals("Jump-Shooting"))
                {
                    if (currentState.Equals("JumpingAscent") || currentState.Equals("JumpingDescent"))
                    {
                        //Change state and animation
                        //ChangeState("Jump-Shooting");
                    }
                    else
                    {
                        //Change state and animation
                        ChangeState("Shooting");
                    }

                    //Generate a Bullet
                }
            }
            else
            {
                if (!currentState.Equals("TShooting") || !currentState.Equals("TJump-Shooting"))
                {
                    if (currentState.Equals("TJumping"))
                    {
                        //Change state and animation
                        ChangeState("TJump-Shooting");
                    }
                    else
                    {
                        //Change state and animation
                        ChangeState("TShooting");
                    }

                    //Generate a Bullet
                }
            }
        }

        // Called when the player attempts to dodge roll. This action can only be performed
        // if the player is in a transformed state.
        public void DodgeRoll()
        {
            if (isTransformed)
            {
                //Change state
                ChangeState("TDodging");

                //Logic for moving character backwards
            }
        }

        //Called when the player transforms from normal to a space cowboy.
        public void Transform()
        {
            if (isTransformed)
            {
                //Change state
                ChangeState("Transforming");
            }
            else
            {
                //Change state and animaton
                ChangeState("TTransforming");
            }
        }

        // Sets up all of the Animations associated with the particular character
        // and adds them to the collection mapping states to animations.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public override void SetUpAnimation(String xmlFile)
        {
            Animation idle = new Animation(xmlFile, "Idle");
            Animation walking = new Animation(xmlFile, "Walking");
            Animation jumpingAscent = new Animation(xmlFile, "JumpingAscent");
            Animation jumpingDescent = new Animation(xmlFile, "JumpingDescent");
            Animation shooting = new Animation(xmlFile, "Shooting");

            this.animationMap.Add("Idle", idle);
            this.animationMap.Add("Walking", walking);
            this.animationMap.Add("JumpingAscent", jumpingAscent);
            this.animationMap.Add("JumpingDescent", jumpingDescent);
            this.animationMap.Add("Shooting", shooting);
        }

        // Handles the physics for moving a character. 
        public void HandlePhysics(GameTime gameTime)
        {
            float timePassed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            //Update the Velocity vector
            velocity.X += direction * moveAcceleration * timePassed;
            velocity.Y = MathHelper.Clamp(velocity.Y + gravity * timePassed, -maxFallSpeed, maxFallSpeed);

            Jump(gameTime);

            //Apply Ground Drag
            if (isOnGround)
            {
                velocity.X *= groundFriction;
            }
            else
            {
                velocity.X *= airFriction;
            }

            //Check top speed of player
            velocity.X = MathHelper.Clamp(velocity.X, -maxMoveSpeed, maxMoveSpeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -maxMoveSpeed, maxMoveSpeed);

            //Add the Velocity to the Position
            Position += velocity * timePassed;
            //System.Diagnostics.Debug.WriteLine("Position is: "+Position);
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
           // System.Diagnostics.Debug.WriteLine("New Position is: " + Position);

            //Collision Detection Time I guess?

            //If a collision stopped movement, change the velocity
            if (Position.X == previousPosition.X)
            {
                velocity.X = 0;
            }
            if (Position.Y == previousPosition.Y)
            {
                velocity.Y = 0;
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            // Get the Input Here
            if (currentKeyboardState.IsKeyDown(Keys.O))
            {
                direction = -1.0f;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.P))
            {
                direction = 1.0f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.J))
            {
                pressedJump = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.K))
            {
                Shoot();
            }

            //Handle the Physics to update Movement and Position
            HandlePhysics(gameTime);

            if (velocity.X < 0)
            {
                facing = SpriteEffects.FlipHorizontally;
            }
            else if(velocity.X > 0 && (facing == SpriteEffects.FlipHorizontally))
            {
                facing = SpriteEffects.None;
            }

            //Handle Transformation Gauge
            if (isTransformed)
            {
                //Deplete Transform Gauge
            }
            else
            {
                //Replenish Transform Gauge
            }

            /* ======= EXAMPLE TEST CODE BELOW ========= */
            if (pressedJump)
            {
                Jump(gameTime);
            }
            /* ======= END OF EXAMPLE TEST CODE ========= */

            if (currentHealth >= 0 && isOnGround)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    ChangeState("Walking");
                }
                else
                {
                   ChangeState("Idle");
                }
            }

            //Reset Input
            direction = 0;
            pressedJump = false;

            //Let the Animation Player Update the Frame
            animationPlayer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position, facing);
        }

    }
}
