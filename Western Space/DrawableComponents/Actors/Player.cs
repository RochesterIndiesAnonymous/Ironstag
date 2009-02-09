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
using System.Diagnostics;
using WesternSpace.DrawableComponents.WorldObjects;
using WesternSpace.DrawableComponents.Misc;

namespace WesternSpace.DrawableComponents.Actors
{
    public class Player : Character, IDamageable, ISpriteCollideable
    {
        /// Constants ///
        public static readonly string XMLFILENAME = Character.XMLPATH + "\\" + typeof(Player).Name;
        private static readonly string COWBOY = "Cowboy";
        private static readonly string SPACE_COWBOY = "SpaceCowboy";

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

        /// The velocity to move the player back with when dead.
        /// </summary>
        private Vector2 deathPushBack;

        /// <summary>
        /// The amount of time before the player can shoot again.
        /// </summary>
        private int shotCoolDown;

        /// <summary>
        /// Keeps track of the time, in milliseconds, until the next shot can be fired.
        /// </summary>
        private int shotDelay;

        /// Flint's hat object for use in the dying animation.
        /// </summary>
        private FlintHat hat;

        private bool invincible = false;

        private int invincibilityTimer = 0;

        private const int INVINCIBILITY_TIME_SPAN = 1000;

        private TimeSpan gameOverTimer = TimeSpan.Zero;

        private bool gameOverDisplayed = false;

        /// <summary>
        /// Constructor for Flint Ironstag.
        /// </summary>
        /// <param name="parentScreen">The screen which this object is a part of.</param>
        /// <param name="spriteBatch">The sprite batch which handles drawing this object.</param>
        /// <param name="position">The initial position of this character.</param>
        public Player(World world, SpriteBatch spriteBatch,  Vector2 position)
            : base(world, spriteBatch, position)
        {
            Mass = 1;
            //Set the character's Name
            name = "Flint Ironstag";

            //Load the player information from the XML file
            LoadPlayerXmlFile();

            //Load the Player's Roles
            SetUpRoles();

            //Set current health
            currentHealth = maxHealth;

            //Set the character's transformation guage
            currentGauge = maxGauge;

            //Create the Animation Player and give it the Idle Animation
            this.animationPlayer = new AnimationPlayer(spriteBatch, currentRole.AnimationMap["Idle"]);

            //Set the current animation
            currentAnimation = animationPlayer.Animation;

            //Set the current state
            currentState = "Idle";

            //Set the Velocity
            Velocity = new Vector2(0, 0);

            //Set the position
            this.Position = position;

            //Set the facing
            facing = SpriteEffects.None;

            //Initializes the player's hotspots.
            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
          /*hotspots.Add(new CollisionHotspot(this, new Vector2(32, 3), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(20, 24), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(20, 42), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(36, 24), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(36, 42), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(30, 60), HOTSPOT_TYPE.bottom));
        */
           // hotspots.Add(new CollisionHotspot(this, new Vector2(3, -27), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, -27), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-11, 20), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-11, -20), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(11, 20), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(11, -20), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-5, 32), HOTSPOT_TYPE.bottom));
            hotspots.Add(new CollisionHotspot(this, new Vector2(5, 32), HOTSPOT_TYPE.bottom));
         
            Hotspots = hotspots;

            //Set up the ShotCoolDown Timer
            UpdateShotCoolDown();

            //Initialize the last fired shot time
            shotDelay = 0;

            //Sets the hitback Vector
            hitPushBack = new Vector2(-2f, 0f);

            //Sets the death push back vector
            deathPushBack = new Vector2(-2f, 0f);

            //Temp: Loads the gunshot sound.
            gunShot = this.Game.Content.Load<SoundEffect>("System\\Sounds\\flintShot");

            //Temp: Sets the input monitor up.
            input = InputMonitor.Instance;

            //Set the Bounding Box Height and Width
            // Lou note: We should define this in XML.
            this.boundingBoxHeight = 61;
            this.boundingBoxWidth = 33;
            this.boundingBoxOffset = new Vector2();
            this.boundingBoxOffset.X = boundingBoxWidth/2;
            this.boundingBoxOffset.Y = boundingBoxHeight/2;

            //Set Invincibility
            invincible = false;
        }

        /// <summary>
        /// Sets up the Character's individual Roles.
        /// </summary>
        /// <param name="xmlFile">The xml file containing the role information.</param>
        public override void SetUpRoles()
        {
            Cowboy cowboy = new Cowboy(XMLFILENAME, COWBOY);
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
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {
                if (!currentState.Contains("Jumping") && !currentState.Contains("Falling"))
                {
                    ApplyJump();
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

            if (!currentState.Contains("Dead") && !currentState.Equals("Hit") && !currentState.Equals("Shooting"))
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
                        ApplyGroundMove(direction);

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
                    ApplyAirMove(direction);
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

                if (!currentState.Contains("Dead") && !currentState.Equals("Hit") && !currentState.Contains("Shooting"))
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
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
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
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
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
            if ( input.IsPressed(InputMonitor.RIGHT) || input.CheckLeftJoystickOnXAxis(InputMonitor.RIGHT) )
            {
                if(!currentState.Contains("Dead") && !currentState.Equals("Hit"))
                {
                    facing = SpriteEffects.None;
                    Move();
                }
            }
            if (input.IsPressed(InputMonitor.LEFT) || input.CheckLeftJoystickOnXAxis(InputMonitor.LEFT))
            {
                if(!currentState.Contains("Dead") && !currentState.Equals("Hit"))
                {
                    facing = SpriteEffects.FlipHorizontally;
                    Move();
                }
            }
            if (input.WasJustPressed(InputMonitor.SHOOT))
            {
                Shoot(gameTime);
            }
            if (input.WasJustPressed(InputMonitor.JUMP))
            {
                Jump();
            }

            // -- Apply the Effects of Various Forces -- //
            NetForce += gravity / Mass;

            if ((input.WasJustReleased(InputMonitor.RIGHT) || input.WasJustReleased(InputMonitor.LEFT)) && !isOnGround)
            {
                ApplyAirFriction();
            }
            else if ((!input.IsPressed(InputMonitor.RIGHT) && !input.IsPressed(InputMonitor.LEFT) && !input.CheckLeftJoystickOnXAxis(InputMonitor.RIGHT) && !input.CheckLeftJoystickOnXAxis(InputMonitor.LEFT)) && isOnGround)
            {
                ApplyGroundFriction();
            }

            // -- Handle Physics -- //
            PhysicsHandler.ApplyPhysics(this);

            // -- Check For Max Ascent of Jump -- //
            if (currentState.Contains("Jumping"))
            {
                if (((-0.5 <= Velocity.Y) && (Velocity.Y <= 0.08)) && (!currentState.Contains("Descent")))
                {
                        ChangeState("JumpingDescent");
                }
            }

            // -- Check for Final State Changes -- //
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {
                if (isOnGround)
                {
                    if (!animationPlayer.Animation.animationName.Equals(currentState))
                    {
                        ChangeState(animationPlayer.Animation.animationName);
                    }
                    else if (!currentState.Contains("Shooting") && (Velocity.X == 0) && (Velocity.Y >= 0))
                    {
                        ChangeState("Idle");
                    }
                    else if (!currentState.Contains("Shooting") && (Velocity.X != 0))
                    {
                        ChangeState("Running");
                    }
                    else if (currentState.Contains("Shooting") && (Velocity.X != 0))
                    {
                        ChangeState("RunningShooting");
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
                        if ((Velocity.Y >= 0) && !currentState.Contains("Falling"))
                        {
                            ChangeState("Falling");
                        }
                    }
                }
            }
            else
            {
                //Perform Dead and Hit operations here
                if(currentState.Contains("Dead"))
                {
                    if(isOnGround && currentState.Equals("DeadAir"))
                    {
                        ChangeState("DeadAirGround");
                        this.Velocity = Vector2.Zero;
                    }
                }
                else if(currentState.Equals("Hit"))
                {
                    if (currentHealth <= 0)
                    {
                        if (isOnGround)
                        {
                            ChangeState("Dead");
                        }
                        else
                        {
                            ChangeState("DeadAir");
                        }

                        if (facing == SpriteEffects.FlipHorizontally)
                        {
                            NetForce += (-1) * deathPushBack;
                        }
                        else
                        {
                            NetForce += deathPushBack;
                        }

                        //Trigger Flint's Hat to Fall
                        if (hat == null)
                        {
                            hat = new FlintHat(this.ParentScreen, this.World, this.SpriteBatch, position, this);
                        }
                        hat.ChangeState("Fall");
                    }
                    else if (!animationPlayer.Animation.animationName.Equals(currentState))
                    {
                        ChangeState(animationPlayer.Animation.animationName);
                    }
                }
            }

            if (hat != null && hat.AnimationPlayer.isDonePlaying() && gameOverTimer == TimeSpan.Zero && !gameOverDisplayed)
            {
                gameOverTimer = gameTime.TotalRealTime;
            }

            TimeSpan timerCheck = gameTime.TotalRealTime - gameOverTimer;

            if(gameOverTimer != TimeSpan.Zero && !gameOverDisplayed && timerCheck.Seconds >= 2)
            {
                // trigger game over screen
                ISpriteBatchService batchService = (ISpriteBatchService)this.Game.Services.GetService(typeof(ISpriteBatchService));
                GameOverContents goc = new GameOverContents(this.ParentScreen, batchService.GetSpriteBatch(GameOverContents.SpriteBatchName), new Vector2(0, 0));
                this.ParentScreen.Components.Add(goc);
                gameOverDisplayed = true;
            }

            // -- Check Invincibility Timer -- //
            if (invincible)
            {
                invincibilityTimer += (gameTime.ElapsedGameTime.Milliseconds);
                this.Visible = !this.Visible;

                if (invincibilityTimer >= INVINCIBILITY_TIME_SPAN)
                {
                    invincible = false;
                    invincibilityTimer = 0;
                    this.Visible = true;
                }
            }

            // -- Animation Player Update Frames -- //
            animationPlayer.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Loads a Character's information from a specified XML file.
        /// </summary>
        /// <param name="fileName">The name of the xml file housing the character's information.</param>
        private void LoadPlayerXmlFile()
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(XMLFILENAME);

            this.maxHealth = Int32.Parse(fileContents.Root.Element("Health").Attribute("MaxHealth").Value);
            this.maxGauge = Int32.Parse(fileContents.Root.Element("Transformation").Attribute("MaxGauge").Value);
        }

        /// <summary>
        /// Creates a bullet object which travels in a straight line.
        /// </summary>
        private void GenerateBullet()
        {
            short direction = 1;
            Vector2 position = this.Position + new Vector2(10, -11f);

            if (this.Facing == SpriteEffects.FlipHorizontally)
            {
                direction = -1;
                position = this.Position - new Vector2(25f, 13f);
            }

            FlintNormalProjectile proj = new FlintNormalProjectile(this.world, this.SpriteBatch, position, this, direction);
        }

        #region IDamageable Members
        /// <summary>
        /// The player's maximum health.
        /// </summary>
        private float maxHealth;

        /// <summary>
        /// The player's maximum health.
        /// </summary>
        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        /// <summary>
        /// The player's current health.
        /// </summary>
        private float currentHealth;

        /// <summary>
        /// The player's current health.
        /// </summary>
        public float CurrentHealth
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

                if ((this.TakesDamageFrom != damageItem.DoesDamageTo) && !invincible)
                {
                    ChangeState("Hit");
                    invincible = true;
                    if (facing == SpriteEffects.FlipHorizontally)
                    {
                        NetForce += (-1) * hitPushBack;
                    }
                    else
                    {
                        NetForce += hitPushBack;
                    }
                    currentHealth -= (int)Math.Ceiling((MitigationFactor * damageItem.AmountOfDamage));

                    Projectile p = damageItem as Projectile;

                    if (p != null && !currentState.Contains("Dead"))
                    {
                        this.World.RemoveWorldObject(p);
                        Dispose();
                    }
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

        public void OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
#if !XBOX && DEBUG
            //Debug.Print("Player Hit By: " + characterCollidedWith.IdNumber);
#endif
            IDamaging damage = characterCollidedWith as IDamaging;

            if (!currentState.Contains("Hit") && !currentState.Contains("Dead"))
            {
                if (damage != null)
                {
                    this.TakeDamage(damage);
                }
            }
        }

        public SpriteEffects collideableFacing
        {
            get
            {
                return facing;
            }
            set
            {
                
            }
        }

        #endregion
        #region ISpriteCollideable Members

        Boolean removeFromCollisionRegistration;
        public bool removeFromRegistrationList
        {
            get
            {
                return removeFromCollisionRegistration;
            }
            set
            {
                removeFromCollisionRegistration = value;
            }
        }

        #endregion
    }
}
