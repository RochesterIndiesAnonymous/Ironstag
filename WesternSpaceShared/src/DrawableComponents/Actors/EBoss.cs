using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Collision;
using WesternSpace.Interfaces;
using System.Xml.Linq;
using WesternSpace.Utility;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Projectiles;
using WesternSpace.DrawableComponents.Actors.EBossStates;
using WesternSpace.DrawableComponents.WorldObjects;

namespace WesternSpace.DrawableComponents.Actors
{
    public class EBoss : Character, IDamageable, ISpriteCollideable
    {
        /// Constants ///
        public static readonly string XMLFILENAME = Character.XMLPATH + "\\" + typeof(EBoss).Name;
        private static readonly string NAME = "Boss";
        private const int INVINCIBILITY_TIME_SPAN = 1000;

        /// <summary>
        /// Camera used to see if the enemy is visible.
        /// </summary>
        private ICameraService camera;

        /// <summary>
        /// The velocity to move the boss back with when dead.
        /// </summary>
        private Vector2 deathPushBack;

        private bool didBulletCollide;

        public bool DidBulletCollide
        {
            get { return didBulletCollide; }
            set { didBulletCollide = value; }
        }

        /// <summary>
        /// The current state to use for executing the AI.
        /// </summary>
        private EBossState currentAIState;

        private EBossState laughingAIState;

        private EBossShootState shootAIState;

        private EBossJumpState jumpAIState;

        private EbossMoveState moveAIState;

        private EBossHitState hitAIState;

        private SpaceBoss spaceBoss;

        private bool invincible = false;

        private int invincibilityTimer = 0;

        public EBoss(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            this.Mass = 1;
            this.Name = EBoss.NAME;

            //Load the boss' information from the XML file
            LoadBossXmlFile();

            //Load the boss' Roles
            SetUpRoles();

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

            didBulletCollide = false;

            //Set the facing
            facing = SpriteEffects.FlipHorizontally;

            //Set the Death push back
            deathPushBack = new Vector2(-1.0f, 0);

            //Set Movement Speed
            this.groundVelocity = new Vector2(0.5f, 0);

            // set up our FAKE jump velocity
            this.jumpVelocity = new Vector2(0f, 13f);

            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(13, -27), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, -6), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, 12), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(17, -6), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(17, 12), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(11, 30), HOTSPOT_TYPE.bottom));

            Hotspots = hotspots;

            //Setup the Bounding Box
            boundingBoxHeight = 59;
            boundingBoxWidth = 34;
            this.boundingBoxOffset = new Vector2();
            this.boundingBoxOffset.X = boundingBoxWidth / 2;
            this.boundingBoxOffset.Y = boundingBoxHeight / 2;

            laughingAIState = new EBossLaughingState(this);
            shootAIState = new EBossShootState(this, this.ParentScreen);
            jumpAIState = new EBossJumpState(this.ParentScreen, this);
            moveAIState = new EbossMoveState(this);
            hitAIState = new EBossHitState(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
        }

        /// <summary>
        /// Called every Update
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        public override void Update(GameTime gameTime)
        {
            // --- Check For Max Ascent of Jump -- //
            if (currentState.Contains("Jumping") && !currentState.Contains("Land"))
            {
                if ((-0.5 <= Velocity.Y) || (Velocity.Y <= 0.8))
                {
                    ChangeState("JumpingDescent");
                }
            }

            NetForce += gravity / Mass;

            // -- Check for Final State Changes -- //
            if ((Velocity.X <= 0) && isOnGround && !currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {

                if (animationPlayer.Animation.animationName.Equals("Idle") && !currentState.Equals("Idle"))
                {
                    ChangeState("Idle");
                    jumpAIState.IsLogicComplete = true;
                }
            }

            ApplyGroundFriction();

            // -- Handle Physics -- //
            if (!this.TileCollisionDetectionEnabled)
            {
                this.Velocity += (NetForce / Mass);
                NetForce = Vector2.Zero;

                Position += this.Velocity;
            }
            else
            {
                PhysicsHandler.ApplyPhysics(this);
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

            if (isOnGround && currentState.Contains("Descent"))
            {
                ChangeState("JumpingLand");
            }

            if (shootAIState.HasTimerStarted || 
                !(this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X + this.AnimationPlayer.Animation.FrameWidth < camera.VisibleArea.X))
            {
                // if this is the first time he has become visible start the shoot timer
                if (!shootAIState.HasTimerStarted)
                {
                    shootAIState.StartTimer();
                    jumpAIState.StartTimers();
                }

                // -- AI -- //
                // if the current boss is not dead, update the AI
                if (!currentState.Contains("Dead"))
                {
                    BossAI(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, UpperLeft, facing);
        }

        /// <summary>
        /// Logic which determines what the boss does.
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        private void BossAI(GameTime gameTime)
        {
            if (currentAIState == null || currentAIState.IsLogicComplete)
            {
                bool aiStateDecided = false;

                if (!currentState.Contains("Dead") && !currentState.Contains("Hit"))
                {
                    if (!world.Player.CurrentState.Contains("Dead"))
                    {
                        if (shootAIState.IsReadyToShoot && this.currentState.Contains("Idle"))
                        {
                            SetAIState(shootAIState);
                            aiStateDecided = true;
                        }

                        if (!shootAIState.IsReadyToShoot && !jumpAIState.ShouldBossJump() && !aiStateDecided && this.currentState.Contains("Idle"))
                        {
                            SetAIState(moveAIState);
                            aiStateDecided = true;
                        }

                        if (!aiStateDecided && this.currentState.Contains("Idle") && jumpAIState.ShouldBossJump())
                        {
                            SetAIState(jumpAIState);
                            aiStateDecided = true;
                        }
                    }
                    else if (world.Player.CurrentState.Contains("Dead"))
                    {
                        // laugh indefinately if the player is dead
                        SetAIState(laughingAIState);
                    }

                        currentAIState.Update();
                }
                else
                {
                    //Perform Dead and Hit operations here
                    if (currentState.Contains("Dead"))
                    {
                        if (isOnGround && currentState.Equals("DeadAir"))
                        {
                            ChangeState("DeadAirGround");
                            this.Velocity = Vector2.Zero;
                        }
                    }
                    else if (currentState.Equals("Hit"))
                    {
                        if (currentHealth <= 0)
                        {
                            if (isOnGround)
                            {
                                ChangeState("Dead");
                            }

                            if (facing == SpriteEffects.FlipHorizontally)
                            {
                                NetForce += (-1) * deathPushBack;
                            }
                            else
                            {
                                NetForce += deathPushBack;
                            }

                            //Trigger Top Hat to Fall
                            if (spaceBoss == null)
                            {
                                spaceBoss = new SpaceBoss(this.ParentScreen, this.World, this.SpriteBatch, position, this);
                            }
                            spaceBoss.ChangeState("Fall");
                        }
                        else if (!animationPlayer.Animation.animationName.Equals(currentState) && currentHealth > 0)
                        {
                            ChangeState(animationPlayer.Animation.animationName);
                            SetAIState(moveAIState);
                        }

                        currentAIState.Update();
                    }
                }
            }
        }

        

        public override void ApplyGroundFriction()
        {
            if (isOnGround)
            {
                velocity.X = 0.9f * velocity.X;
            }
        }

        /// <summary>
        /// Loads a Character's information from a specified XML file.
        /// </summary>
        private void LoadBossXmlFile()
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(XMLFILENAME);

            this.maxHealth = Int32.Parse(fileContents.Root.Element("Health").Attribute("MaxHealth").Value);
            this.currentHealth = maxHealth;
        }

        /// <summary>
        /// Sets up the Character's individual Roles.
        /// </summary>
        /// <param name="xmlFile">The xml file containing the role information.</param>
        public override void SetUpRoles()
        {
            Boss bossRoles = new Boss(XMLFILENAME, NAME);

            this.roleMap.Add(NAME, bossRoles);

            this.currentRole = bossRoles;
        }

        private void SetAIState(EBossState state)
        {
            if (currentAIState != state)
            {
                currentAIState = state;
            }
        }

        #region IDamageable Members

        private float maxHealth;

        /// <summary>
        /// The maximum health the boss has
        /// </summary>
        public float MaxHealth
        {
            get { return maxHealth; }
        }

        private float currentHealth;

        /// <summary>
        /// The current health the boss has
        /// </summary>
        public float CurrentHealth
        {
            get { return currentHealth; }
        }

        /// <summary>
        /// The boss takes full damage from the player
        /// </summary>
        public float MitigationFactor
        {
            get { return 1.0f; }
        }

        /// <summary>
        /// The boss takes damage from the player
        /// </summary>
        public WesternSpace.Utility.DamageCategory TakesDamageFrom
        {
            get { return DamageCategory.Player; }
        }

        /// <summary>
        /// Take damage if the damage is from the player
        /// </summary>
        /// <param name="damageItem">The other world item that this boss collided with</param>
        public void TakeDamage(IDamaging damageItem)
        {
            if ((this.TakesDamageFrom != damageItem.DoesDamageTo) && !currentState.Equals("Dead") && !invincible)
            {
                //Can be used for when bullets hit the body of the boss
                this.World.playSound("nogoodHit");

                SetAIState(hitAIState);
                ChangeState("Hit");
                currentAIState.Update();
                currentHealth -= (int)Math.Ceiling((MitigationFactor * damageItem.AmountOfDamage));

                if (currentHealth > 0)
                {
                    invincible = true;
                }

                if (facing == SpriteEffects.FlipHorizontally)
                {
                    NetForce += (-1) * deathPushBack;
                }
                else
                {
                    NetForce += deathPushBack;
                }

                Projectile p = damageItem as Projectile;

                if (p != null)
                {
                    this.World.RemoveWorldObject(p);
                    Dispose();
                }

                if (currentHealth <= 0)
                {
                    shootAIState.StopTimer();
                    jumpAIState.StopTimers();
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

        public Microsoft.Xna.Framework.Graphics.SpriteEffects collideableFacing
        {
            get
            {
                return facing;
            }
            set
            {
                facing = value;
            }
        }

        private bool removeFromCollisionRegistration;

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

        public void OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
            IDamaging damage = characterCollidedWith as IDamaging;

            if (damage != null)
            {
                this.TakeDamage(damage);
            }
        }

        #endregion
    }
}
