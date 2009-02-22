﻿using System;
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

namespace WesternSpace.DrawableComponents.Actors
{
    public class Boss : Character, IDamageable, ISpriteCollideable
    {
        /// Constants ///
        public static readonly string XMLFILENAME = Character.XMLPATH + "\\" + typeof(EBandit).Name;
        private static readonly string NAME = "Boss";

        /// <summary>
        /// Camera used to see if the enemy is visible.
        /// </summary>
        private ICameraService camera;

        /// <summary>
        /// Sound Effect will be moved later on.
        /// </summary>
        SoundEffect gunShot;

        /// <summary>
        /// The velocity to move the boss back with when dead.
        /// </summary>
        private Vector2 deathPushBack;

        public Boss(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            this.Mass = 1;
            this.Name = Boss.NAME;

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

            //Set the facing
            facing = SpriteEffects.FlipHorizontally;

            //Set the Death push back
            deathPushBack = new Vector2(-1.0f, 0);

            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(13, -27), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, -6), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, 12), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(17, -6), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(17, 12), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(11, 30), HOTSPOT_TYPE.bottom));

            Hotspots = hotspots;

            //Temp: Loads the gunshot sound.
            gunShot = this.Game.Content.Load<SoundEffect>("System\\Sounds\\flintShot");

            //Setup the Bounding Box
            boundingBoxHeight = 59;
            boundingBoxWidth = 34;
            this.boundingBoxOffset = new Vector2();
            this.boundingBoxOffset.X = boundingBoxWidth / 2;
            this.boundingBoxOffset.Y = boundingBoxHeight / 2;
        }

        public override void Initialize()
        {
            base.Initialize();

            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
        }

        /// <summary>
        /// Called from the AI. If the boss is already
        /// in a jumping state then no action is to occurr.
        /// </summary>
        public void Jump()
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
        /// Called from the AI to move the boss
        /// </summary>
        public void Move()
        {
            int direction = 1;

            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {
                //Calculate Facing
                if (facing.Equals(SpriteEffects.None))
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }

                if (isOnGround)
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
                else
                {
                    ApplyAirMove(direction);
                }
            }
        }

        /// <summary>
        /// Causes the boss to generate a projectile and change its state accordingly.
        /// </summary>
        public void Shoot()
        {
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
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
                    GenerateBullet();
                }
            }
        }

        /// <summary>
        /// Called every Update
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        public override void Update(GameTime gameTime)
        {
            // --- Check For Max Ascent of Jump -- //
            if (currentState.Contains("Jumping"))
            {
                if ((-0.5 <= Velocity.Y) || (Velocity.Y <= 0.8))
                {
                    ChangeState("JumpingDescent");
                }
            }

            NetForce += gravity / Mass;

            // -- Check for Final State Changes -- //
            if ((Velocity.X == 0) && isOnGround && !currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {

                if (animationPlayer.Animation.animationName.Equals("Idle") && !currentState.Equals("Idle"))
                {
                    ChangeState("Idle");
                }
                else if (!currentState.Contains("Shooting"))
                {
                    ChangeState("Idle");
                }
            }

            ApplyGroundFriction();

            // -- Handle Physics -- //
            PhysicsHandler.ApplyPhysics(this);

            // -- Animation Player Update Frames -- //
            animationPlayer.Update(gameTime);
            base.Update(gameTime);

            if (!(this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X + this.AnimationPlayer.Animation.FrameWidth < camera.VisibleArea.X))
            {
                // -- AI -- //
                BossAI(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, UpperLeft, facing);
        }

        /// <summary>
        /// Logic which determines what a Bandit does.
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        private void BossAI(GameTime gameTime)
        {
            if (!currentState.Contains("Dead") && !world.Player.CurrentState.Contains("Dead"))
            {
                float shootTimer = 0f, shootTimeSpan = 3.0f;

                shootTimer += (float)(gameTime.TotalRealTime.TotalSeconds % 3.1);

                if (World.Player.Position.X < this.position.X)
                {
                    facing = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    facing = SpriteEffects.None;
                }

                //Shoot Logic
                if (shootTimer >= shootTimeSpan)
                {
                    Shoot();
                    shootTimer = 0f;
                }
            }

        }

        /// <summary>
        /// Creates a bullet object which travels in a straight line.
        /// </summary>
        public void GenerateBullet()
        {
            short direction = 1;
            Vector2 position = this.Position + new Vector2(23f, -15f);

            if (this.Facing == SpriteEffects.FlipHorizontally)
            {
                direction = -1;
                position = this.Position + new Vector2(-23f, -15);
            }

            BanditNormalProjectile proj = new BanditNormalProjectile(this.World, this.SpriteBatch, position, this, direction);

        }

        public override void ApplyGroundFriction()
        {
            if (isOnGround)
            {
                velocity.X = 0.9f * velocity.X;
            }
        }


        /// <summary>
        /// Called on a Sprite Collision?
        /// </summary>
        public void OnSpriteCollision()
        {
        }

        /// <summary>
        /// Loads a Character's information from a specified XML file.
        /// </summary>
        private void LoadBossXmlFile()
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(XMLFILENAME);

            this.maxHealth = Int32.Parse(fileContents.Root.Element("Health").Attribute("MaxHealth").Value);
        }

        /// <summary>
        /// Sets up the Character's individual Roles.
        /// </summary>
        /// <param name="xmlFile">The xml file containing the role information.</param>
        public override void SetUpRoles()
        {
            BossUtility bossRoles = new BossUtility(XMLFILENAME, NAME);

            this.roleMap.Add(NAME, bossRoles);

            this.currentRole = bossRoles;
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
            if ((this.TakesDamageFrom != damageItem.DoesDamageTo) && !currentState.Equals("Dead"))
            {
                currentHealth -= (int)Math.Ceiling((MitigationFactor * damageItem.AmountOfDamage));

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
                    ChangeState("Dead");
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
