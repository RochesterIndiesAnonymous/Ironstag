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
using WesternSpace.DrawableComponents.Projectiles;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.Actors
{
    public class Player : Character, IDamageable, ISpriteCollideable
    {
        /// Constants ///
        private static readonly string COWBOY = "Cowboy";
        private static readonly string SPACE_COWBOY = "SpaceCowboy";

        /// <summary>
        /// Calculates the necessary changes in the player's
        /// velocity depending upong the player's actions.
        /// </summary>
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
        /// The velocity to move the player back with when hit.
        /// </summary>
        private Vector2 hitPushBack;

        /// <summary>
        /// The amount of time before the player can shoot again.
        /// </summary>
        private int shotCoolDown;

        /// <summary>
        /// Keeps track of the time, in milliseconds, until the next shot can be fired.
        /// </summary>
        private int shotDelay;

        /// <summary>
        /// Constructor for Flint Ironstag.
        /// </summary>
        /// <param name="parentScreen">The screen which this object is a part of.</param>
        /// <param name="spriteBatch">The sprite batch which handles drawing this object.</param>
        /// <param name="position">The initial position of this character.</param>
        /// <param name="xmlFile">The XML file which houses the information for this character.</param>
        public Player(Screen parentScreen , SpriteBatch spriteBatch, World world, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, world, position, xmlFile)
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
            this.animationPlayer = new AnimationPlayer(spriteBatch, currentRole.AnimationMap["Idle"]);

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
            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(32, 3), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(20, 24), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(20, 42), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(36, 24), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(36, 42), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(30, 60), HOTSPOT_TYPE.bottom));
            Hotspots = hotspots;

            //Set up the ShotCoolDown Timer
            UpdateShotCoolDown();

            //Initialize the last fired shot time
            shotDelay = 0;

            //Sets the hitback Vector
            hitPushBack = new Vector2(-7f, 0f);

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
        /// Updates the shotCoolDown Timer to that of its current Role.
        /// </summary>
        private void UpdateShotCoolDown()
        {
            if (isTransformed)
            {
                //shotCoolDown = ((SpaceCowboy)currentRole).shootCoolDown;
            }
            else
            {
                shotCoolDown = ((Cowboy)currentRole).shootCoolDown;
            }
        }

        /// <summary>
        /// Called when the player presses the jump button. If the player is already
        /// in a jumping state then no action is to occurr.
        /// </summary>
        private void Jump()
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
        private void Move()
        {
            int direction = 1;

            if (!currentState.Equals("Dead") && !currentState.Equals("Hit") && !currentState.Equals("Shooting"))
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
                    if (!currentState.Contains("Jumping"))
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
        private void Shoot(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - shotDelay > shotCoolDown)
            {

                if (!currentState.Equals("Dead") && !currentState.Equals("Hit") && !currentState.Contains("Shooting"))
                {
                    if (currentState.Contains("Jumping"))
                    {
                        //Change state and animation
                        if (currentState.Equals("JumpingAscent"))
                        {
                            ContinueAnimationNewState("JumpingAscentShooting");
                        }
                        else
                        {
                            ContinueAnimationNewState("JumpingDescentShooting");
                        }
                    }
                    else if (currentState.Contains("Falling"))
                    {
                        ContinueAnimationNewState("FallingShooting");
                    }
                    else if (currentState.Contains("Running"))
                    {

                        //Change state and animation
                        ContinueAnimationNewState("RunningShooting");
                    }
                    else
                    {
                        //Change state and animation
                        ChangeState("Shooting");
                    }

                    //Generate a Bullet
                    gunShot.Play();
                    GenerateBullet();
                    shotDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }

        /// <summary>
        /// Called when the player attempts to dodge roll. This action can only
        /// be performed if the player is in a transformed state.
        /// </summary>
        private void DodgeRoll()
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
        private void Transform()
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
            // -- Get User Input -- //
            if (input.CheckKey(InputMonitor.RIGHT) || input.CheckButton(InputMonitor.RIGHT) || input.CheckLeftJoystickOnXAxis(InputMonitor.RIGHT))
            {
                facing = SpriteEffects.None;
                Move();
            }
            if (input.CheckKey(InputMonitor.LEFT) || input.CheckButton(InputMonitor.LEFT) || input.CheckLeftJoystickOnXAxis(InputMonitor.LEFT))
            {
                facing = SpriteEffects.FlipHorizontally;
                Move();
            }
            if (input.CheckPressAndReleaseKey(InputMonitor.SHOOT) || input.CheckPressAndReleaseButton(InputMonitor.SHOOT))
            {
                Shoot(gameTime);
            }
            if (input.CheckPressAndReleaseKey(InputMonitor.JUMP) || input.CheckPressAndReleaseButton(InputMonitor.JUMP))
            {
                Jump();
            }

            // -- Handle Physics -- //
            velocity = playerPhysics.ApplyPhysics(velocity);

            // -- Update Position -- //
            position += velocity;

            // --Reset the Velocity -- //
            playerPhysics.ResetVelocity();

            // -- Check For Max Ascent of Jump -- //
            if (currentState.Contains("Jumping"))
            {
                if (((-0.5 <= velocity.Y) && (velocity.Y <= 0.08)) && (!currentState.Contains("Descent")))
                {
                        ChangeState("JumpingDescent");
                }
            }

            // -- Check for Final State Changes -- //
            if (!currentState.Equals("Dead") && !currentState.Equals("Hit"))
            {
                if (isOnGround)
                {
                    if (!animationPlayer.Animation.animationName.Equals(currentState))
                    {
                        ChangeState(animationPlayer.Animation.animationName);
                    }
                    else if (!currentState.Contains("Shooting") && (velocity.X == 0) && (velocity.Y >= 0))
                    {
                        ChangeState("Idle");
                    }
                }
                else
                {
                    if (!animationPlayer.Animation.animationName.Equals(currentState))
                    {
                        ContinueAnimationNewState(animationPlayer.Animation.animationName);
                    }
                    else if (!currentState.Contains("Jumping"))
                    {
                        if ((velocity.Y >= 0) && !currentState.Contains("Falling"))
                        {
                            ChangeState("Falling");
                        }
                    }
                }
            }
            else
            {
                if(currentState.Equals("Dead"))
                {
                    //Death Operations go here
                }
                else if(currentState.Equals("Hit"))
                {
                    if (!animationPlayer.Animation.animationName.Equals(currentState))
                    {
                        ChangeState(animationPlayer.Animation.animationName);
                    }
                }
            }

            /// -- Animation Player Update Frames -- ///
            animationPlayer.Update(gameTime);

            base.Update(gameTime);
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
        /// Creates a bullet object which travels in a straight line.
        /// </summary>
        private void GenerateBullet()
        {
            short direction = 1;
            Vector2 position = this.Position + new Vector2(40f, 23f);

            if (this.Facing == SpriteEffects.FlipHorizontally)
            {
                direction = -1;
                position = this.Position + new Vector2(10f, 20f);
            }

            FlintNormalProjectile proj = new FlintNormalProjectile(this.ParentScreen, this.SpriteBatch, position, this, direction);
        }

        #region IDamageable Members
        /// <summary>
        /// The player's maximum health.
        /// </summary>
        private int maxHealth;

        /// <summary>
        /// The player's maximum health.
        /// </summary>
        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        /// <summary>
        /// The player's current health.
        /// </summary>
        private int currentHealth;

        /// <summary>
        /// The player's current health.
        /// </summary>
        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        /// <summary>
        /// The player takes full damage from enemies
        /// </summary>
        public float MitigationFactor
        {
            get { return 1; }
        }

        /// <summary>
        /// The player only wants to take damage from enemies
        /// </summary>
        public DamageCategory TakesDamageFrom
        {
            get { return DamageCategory.Enemy; }
        }


        /// <summary>
        /// If the proper damage type is done subtract from the current health
        /// </summary>
        /// <param name="damageItem">The other world object that the player collided with</param>
        public void TakeDamage(IDamaging damageItem)
        {
            if (this.TakesDamageFrom != damageItem.DoesDamageTo)
            {
                currentHealth -= (int)Math.Ceiling((MitigationFactor * damageItem.AmountOfDamage));
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

        public Rectangle Rectangle
        {
            get 
            {
                int x = (int)(this.Position.X);
                int y = (int)(this.Position.Y);
                return new Rectangle(x, y, this.AnimationPlayer.Animation.FrameWidth, this.AnimationPlayer.Animation.FrameHeight);
            }
        }

        public void OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
            IDamaging damage = characterCollidedWith as IDamaging;

            ChangeState("Hit");
            if (facing == SpriteEffects.FlipHorizontally)
            {
                position += (-1) * hitPushBack;
            }
            else
            {
                position += hitPushBack;
            }

            if (damage != null)
            {
                this.TakeDamage(damage);
            }
        }

        #endregion
    }
}
