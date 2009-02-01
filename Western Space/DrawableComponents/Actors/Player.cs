using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WesternSpace.Input;
using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using WesternSpace.Collision;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Audio;
using WesternSpace.Physics;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.Actors
{
    class Player : Character
    {
        /// Constants ///
        private static readonly string COWBOY = "Cowboy";
        private static readonly string SPACE_COWBOY = "SpaceCowboy";

        PhysicsHandler playerPhysics;

        /// <summary>
        /// Temporary use of InputMonitor until it becomes a service.
        /// </summary>
        InputMonitor input;

        /// <summary>
        /// Sound Effect will be moved later on.
        /// </summary>
        SoundEffect gunShot;

        /// <summary>
        /// The maximum value of the transformation guage for
        /// Flint Ironstag.
        /// </summary>
        private int maxGauge;

        public int MaxGuage
        {
            get { return maxGauge; }
            set { MaxGuage = value; }
        }

        /// <summary>
        /// The current value of the transformation gauge.
        /// </summary>
        private int currentGauge;

        public int CurrentGauge
        {
            get { return currentGauge; }
            set { currentGauge = value; }
        }

        /// <summary>
        /// Value which returns true if Flint is currently in
        /// his transformed state. False if not.
        /// </summary>
        private bool isTransformed = false;

        public bool IsTransformed
        {
            get { return isTransformed; }
            set { isTransformed = value; }
        }

        /// <summary>
        /// True if the player has pressed the jump button. False otherwise.
        /// </summary>
        public bool pressedJump = false;


        /// <summary>
        /// Constructor for Flint Ironstag.
        /// </summary>
        /// <param name="parentScreen">The screen which this object is a part of.</param>
        /// <param name="spriteBatch">The sprite batch which handles drawing this object.</param>
        /// <param name="position">The initial position of this character.</param>
        /// <param name="xmlFile">The XML file which houses the information for this character.</param>
        public Player(Screen parentScreen , SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, position, xmlFile)
        {
            //Set the character's Name
            name = "Flint Ironstag";

            //Load the player information from the XML file
            LoadPlayerXmlFile(xmlFile);

            //Load the Player's Roles
            SetUpRoles(xmlFile);

            //Set current health
            currentHealth = maxHealth;

            //Instantiate the Physics Handler
            playerPhysics = new PhysicsHandler();

            //Set the character's transformation guage
            currentGauge = maxGauge;

            //Create the Animation Player and give it the Idle Animation
            this.animationPlayer = new AnimationPlayer(spriteBatch, currentRole.AnimationMap["Idle"], currentRole.AnimationMap["Idle"]);

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

            //Initializes the player's hotspots.
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(16, 0), HOTSPOT_TYPE.top));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(0, 30), HOTSPOT_TYPE.left));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(36, 30), HOTSPOT_TYPE.right));
            this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(7, 60), HOTSPOT_TYPE.bottom));
            //this.collisionHotSpots.Add(new CollisionHotspot(this, new Vector2(27, 60), HOTSPOT_TYPE.bottom));

            //Temp: Loads the gunshot sound.
            gunShot = this.Game.Content.Load<SoundEffect>("System\\Sounds\\flintShot");

            //Temp: Sets the input monitor up.
            input = new InputMonitor(this.Game);
        }

        /// <summary>
        /// Sets up the Character's individual Roles.
        /// </summary>
        /// <param name="xmlFile">The xml file containing the role information.</param>
        public override void SetUpRoles(string xmlFile)
        {
            Cowboy cowboy = new Cowboy(xmlFile, COWBOY);
            //SpaceCowboy spaceCowboy = new SpaceCowboy(xmlFile, SPACE_COWBOY);

            this.roleMap.Add(COWBOY, cowboy);
            //this.roleMap.Add(SPACE_COWBOY, spaceCowboy);

            this.currentRole = cowboy;
        }

        /// <summary>
        /// Called when the player presses the jump button. If the player is already
        /// in a jumping state then no action is to occurr.
        /// </summary>
        public void Jump()
        {
            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                if (!currentState.Contains("Jumping") && !currentState.Contains("Falling"))
                {
                    playerPhysics.ApplyJump();
                    ChangeState("JumpingAscent");
                    isOnGround = false;
                }
            }
        }

        /// <summary>
        /// Called when the player presses a movement button.
        /// </summary>
        public void Move()
        {
            int direction = 1;

            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                //Calculate Facing
                if(facing.Equals(SpriteEffects.None))
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }

                if (isOnGround)
                {
                    playerPhysics.ApplyGroundMove(direction);
                    if (!currentState.Contains("Shooting"))
                    {
                        ChangeState("Running");
                    }
                    else if (!currentState.Contains("Up"))
                    {
                        ChangeState("RunningShooting");
                    }
                    else
                    {
                        ChangeState("RunningShootingUp");
                    }
                }
                else
                {
                    playerPhysics.ApplyAirMove(direction);
                }
            }
        }

        /// <summary>
        /// Called when the player presses the shoot button.
        /// </summary>
        public void Shoot()
        {
            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {

                if (!currentState.Contains("Shooting"))
                {
                    if (currentState.Contains("Jumping"))
                    {
                        //Change state and animation
                        //ChangeState("JumpingShooting");
                    }
                    else if (currentState.Contains("Running"))
                    {
                        //Change state and animation
                        //ChangeState("RunningShooting");
                    }
                    else
                    {
                        //Change state and animation
                        ChangeState("Shooting");

                        gunShot.Play();
                    }

                    //Generate a Bullet

                }
            }
        }

        /// <summary>
        /// Called when the player attempts to dodge roll. This action can only
        /// be performed if the player is in a transformed state.
        /// </summary>
        public void DodgeRoll()
        {
            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                if (isTransformed)
                {
                    //Change state
                    ChangeState("Rolling");

                    //Logic for moving character backwards
                    //playerPhysics.Roll(direction);
                }
            }
        }

        /// <summary>
        /// Called when the player presses the transformation button.
        /// </summary>
        public void Transform()
        {
            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                if (isTransformed)
                {
                    //Change state
                    ChangeState("Transforming");
                }
                else
                {
                    //Change state and animaton
                    ChangeState("Transforming");
                }
            }
        }

        /// <summary>
        /// Called every Update
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        public override void Update(GameTime gameTime)
        {
            /// -- Get User Input -- ///
            if (input.CheckKey(InputMonitor.RIGHT))
            {
                facing = SpriteEffects.None;
                Move();
            }
            if (input.CheckKey(InputMonitor.LEFT))
            {
                facing = SpriteEffects.FlipHorizontally;
                Move();
            }
            if (input.CheckPressAndReleaseKey(InputMonitor.SHOOT))
            {
                Shoot();
            }
            if (input.CheckPressAndReleaseKey(InputMonitor.JUMP))
            {
                Jump();
            }

            /// -- Handle Physics -- ///
            velocity = playerPhysics.ApplyPhysics(velocity);

            /// -- Update Position -- ///
            position += velocity;

            /// --Reset the Velocity -- ///
            playerPhysics.ResetVelocity();

            /// --- Check For Max Ascent of Jump -- ///
            if (currentState.Contains("Jumping"))
            {
                if ((-0.5 <= velocity.Y) || (velocity.Y <= 0.8))
                {
                    ChangeState("JumpingDescent");
                }
            }

            /// -- Check for Final State Changes -- ///
            if ((velocity.X == 0) && isOnGround && !currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                if(!currentState.Contains("Shooting"))
                {
                    ChangeState("Idle");
                }
                else if (this.currentState.Contains("Shooting"))
                {
                    if (animationPlayer.Animation.animationName.Equals("Idle"))
                    {
                        ChangeState("Idle");
                    }
                }
            }

            /// -- Animation Player Update Frames -- ///
            animationPlayer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position, facing);
        }

        /// <summary>
        /// Loads a Character's information from a specified XML file.
        /// </summary>
        /// <param name="fileName">The name of the xml file housing the character's information.</param>
        private void LoadPlayerXmlFile(string fileName)
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);

            Int32.TryParse(fileContents.Root.Element("Health").Attribute("MaxHealth").Value, out this.maxHealth);
            Int32.TryParse(fileContents.Root.Element("Transformation").Attribute("MaxGauge").Value, out this.maxGauge);
        }


        /// <summary>
        /// Called on a Sprite Collision?
        /// </summary>
        public void OnSpriteCollision()
        {
        }
    }
}
