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
        // Constants

        // The Move speed for the normal Flint Ironstag
        const int MOVE_SPEED = 100;

        // Move speed for Flint when transformed
        const int TRANS_MOVE_SPEED = 200;

        // Class Variables

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
        public void Jump()
        {
            if (!isTransformed)
            {
                if (!currentState.Equals("Jumping") || !currentState.Equals("Jump-Shooting") ||
                    !currentState.Equals("Falling"))
                {
                    //Change state and animation
                    ChangeState("Jumping");

                    //JUMP PHYSICS AND COLLISION DETECTION LOGIC GO HERE :^D
                }
            }
            else
            {
                if (!currentState.Equals("TJumping") || !currentState.Equals("TJump-Shooting") ||
                    !currentState.Equals("TFalling"))
                {
                    //Change state and animation
                    ChangeState("TJumping");

                    //TRANSFORMED JUMP PHYSICS AND COLLISION DETECTION LOGIC GO HERE :^D
                }
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

        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            //Input Logic / State Changing

            if (isTransformed)
            {
                //Deplete Transform Gauge
            }
            else
            {
                //Replenish Transform Gauge
            }

            /* ======= EXAMPLE TEST CODE BELOW ========= */
            if (currentKeyboardState.IsKeyDown(Keys.A) == true)
            {
                Jump();
            }
            else if (currentKeyboardState.IsKeyDown(Keys.S) == true)
            {
                ChangeState("Walking");
            }
            /* ======= END OF EXAMPLE TEST CODE ========= */

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
