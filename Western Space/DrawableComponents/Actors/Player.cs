using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using WesternSpace.AnimationFramework;

namespace WesternSpace.DrawableComponents.Actors
{
    class Player : Character
    {
        // --- Constants ---

        // -Horizontal Movement Constants

        // Acceleration Constant
        private const float moveAcceleration = 14000.0f;

        // Maximum Movement Speed
        private const float maxMoveSpeed = 2000.0f;

        // Friction of the Ground
        private const float groundFriction = 0.58f;

        // Friction of the Air
        private const float airFriction = 0.65f;

        // -Vertical Movement Constants

        // Longest amount of time a jump can last
        private const float maxJumpTime = 0.35f;

        // Acceleration due to Gravity
        private const float gravity = 3500.0f;

        // Maximum Falling SPeed
        private const float maxFallSpeed = 600.0f;

        // Helps to give the player more control of the character
        // in the air.
        private const float jumpControl = 0.14f;

        // Helps to give the player more control of the character
        // in the air.
        private const float jumpVelocity = -4000.0f;

        // --- Class Variables ---

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

        // True if the character is currently on the ground
        private bool isOnGround = true;

        // The constructor for Flint Ironstag
        public Player(Game game, SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(game, spriteBatch, position, xmlFile)
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
            this.animationPlayer = new AnimationPlayer(game, spriteBatch, animationMap["Idle"]);

            //Set the current animation
            currentAnimation = animationPlayer.Animation;

            //Set the current state
            currentState = "Idle";

            //Set the Velocity
            velocity = new Vector2(0, 0);

            //Set the position
            this.Position = position;
        }

        // Called when the player presses the jump button. If the player is already
        // in a jumping state (or jumping and shooting) then no action is to occurr.
        public void Jump(GameTime gameTime)
        {
            //If the jump button was pressed.
            if (pressedJump)
            {
                //Start a Jump or continue jumping
                if ((currentState.Equals("Jumping") && isOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                    {
                        //Play Sound if necessary
                    }

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    ChangeState("Jumping");
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
                    if (currentState.Equals("Jumping"))
                    {
                        //Change state and animation
                        ChangeState("Jump-Shooting");
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
            Animation jumping = new Animation(xmlFile, "Jumping");
            // Animation shooting = new Animation(xmlFile, "Shooting");

            this.animationMap.Add("Idle", idle);
            this.animationMap.Add("Walking", walking);
            this.animationMap.Add("Jumping", jumping);
            // this.animationMap.Add("Shooting", shooting);
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

            //Add the Velocity to the Position
            Position += velocity * timePassed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

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
            else if (currentKeyboardState.IsKeyDown(Keys.J))
            {
                pressedJump = true;
            }

            //Handle the Physics to update Movement and Position
            HandlePhysics(gameTime);

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
            else if (currentKeyboardState.IsKeyDown(Keys.K) == true)
            {
                ChangeState("Walking");
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
            animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position);
        }

    }
}
