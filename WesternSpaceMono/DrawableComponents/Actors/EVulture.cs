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
    class EVulture : Character, IDamageable, ISpriteCollideable, IDamaging
    {
        /// Constants ///
        public static readonly string XMLFILENAME = Character.XMLPATH + "\\" + typeof(EVulture).Name;
        private static readonly string VULTURE = "Vulture";
        private readonly Vector2 airMovement = new Vector2(-2f, 0f);
        private readonly Vector2 gravity = new Vector2(0f, 0.4f);

        /// <summary>
        /// Sound Effect will be moved later on.
        /// </summary>
        //SoundEffect gunShot;

        /// <summary>
        /// Camera used to see if the enemy is visible.
        /// </summary>
        private ICameraService camera;

        bool flewThisFrame;
        int flyCount = 0;

        bool secondFlyFrame;
        /// <summary>
        /// Constructor for Vulture.
        /// </summary>
        /// <param name="world">The world this EVulture belongs to</param>
        /// <param name="spriteBatch">The sprite batch which handles drawing this object.</param>
        /// <param name="position">The initial position of this character.</param>
        /// <param name="xmlFile">The XML file which houses the information for this character.</param>
        public EVulture(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            Mass = 1;
            //Set the character's Name
            name = "Vulture";

            //Load the player information from the XML file
            LoadVultureXmlFile();

            //Load the Player's Roles
            SetUpRoles();

            //Set current health
            currentHealth = maxHealth;

            //Create the Animation Player and give it the Idle Animation
            this.animationPlayer = new AnimationPlayer(spriteBatch, currentRole.AnimationMap["Flying"]);

            //Set the current animation
            currentAnimation = animationPlayer.Animation;

            //Set the current state
            currentState = "Flying";

            //Set the Velocity
            Velocity = new Vector2(0, 0);
            NetForce = Vector2.Zero;

            //Set the position
            this.Position = position;

            //Set the facing
            facing = SpriteEffects.None;
            flewThisFrame = false;
            secondFlyFrame = false;

            List<CollisionHotspot> hotspots = new List<CollisionHotspot>();
            hotspots.Add(new CollisionHotspot(this, new Vector2(0, -4), HOTSPOT_TYPE.top));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-13, -3), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(-13, 3), HOTSPOT_TYPE.left));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, 14), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(1, 8), HOTSPOT_TYPE.right));
            hotspots.Add(new CollisionHotspot(this, new Vector2(2, 14), HOTSPOT_TYPE.bottom));

            Hotspots = hotspots;


            //Temp: Loads the gunshot sound.
            //gunShot = this.Game.Content.Load<SoundEffect>("System\\Sounds\\flintShot");

            //Setup the Bounding Box
            boundingBoxHeight = 17;
            boundingBoxWidth = 29;
            this.boundingBoxOffset = new Vector2();
            this.boundingBoxOffset.X = boundingBoxWidth / 2;
            this.boundingBoxOffset.Y = boundingBoxHeight / 2;
        }

        /// <summary>
        /// Initializes the objects assets at startup.
        /// </summary>
        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            base.Initialize();
        }

        /// <summary>
        /// Sets up the Character's individual Roles.
        /// </summary>
        /// <param name="xmlFile">The xml file containing the role information.</param>
        public override void SetUpRoles()
        {
            Bird vulture = new Bird(XMLFILENAME, VULTURE);

            this.roleMap.Add(VULTURE, vulture);

            this.currentRole = vulture;
        }

        /// <summary>
        /// Called when the Vulture should dive. If the Vulture is being hit, it will not dive
        /// </summary>
        /*public void Dive()
        {
            if (!currentState.Contains("Dead") && !currentState.Equals("Hit"))
            {
                
                  
                
            }
        }*/

        /// <summary>
        /// Called when the player presses a movement button.
        /// </summary>
       /* public void Move()
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
        */
      
        /// <summary>
        /// Called every Update
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        public override void Update(GameTime gameTime)
        {
                // -- Check for Final State Changes -- //
                if (currentState.Contains("Dead") && this.animationPlayer.isDonePlaying())
                {
                    this.World.RemoveWorldObject(this);
                }
                /*if ((Velocity.X == 0) && isOnGround && !currentState.Contains("Dead") && !currentState.Equals("Hit"))
                {

                    if (animationPlayer.Animation.animationName.Equals("Idle") && !currentState.Equals("Idle"))
                    {
                        ChangeState("Idle");
                    }
                    else if (!currentState.Contains("Shooting"))
                    {
                        ChangeState("Idle");
                    }
                }*/
                vultureAI(gameTime);
                // -- Handle Physics -- //
                PhysicsHandler.ApplyPhysics(this);

                // -- Animation Player Update Frames -- //
                animationPlayer.Update(gameTime);
                base.Update(gameTime);

              //  if (!(this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X + this.AnimationPlayer.Animation.FrameWidth < camera.VisibleArea.X))
              //  {
                    // -- AI -- //

               // }

        }

        public override void Draw(GameTime gameTime)
        {
            //if (!(this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X + this.AnimationPlayer.Animation.FrameWidth < camera.VisibleArea.X))
            //{
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, UpperLeft, facing);
            //}
        }

        /// <summary>
        /// Loads a Character's information from a specified XML file.
        /// </summary>
        /// <param name="fileName">The name of the xml file housing the character's information.</param>
        private void LoadVultureXmlFile()
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(XMLFILENAME);

            this.maxHealth = Int32.Parse(fileContents.Root.Element("Health").Attribute("MaxHealth").Value);
        }

        /// <summary>
        /// Logic which determines what a Vulture does.
        /// </summary>
        /// <param name="gameTime">The time the game has been running.</param>
        private void vultureAI(GameTime gameTime)
        {
            if (!currentState.Contains("Dead"))
            {
                // When vulture is directly over the player, play sound
                if ((World.Player.Position.X + 10 >= this.position.X) && (World.Player.Position.X - 10 <= this.position.X - 10))
                {
                    this.World.playSound("birdCaw");
                }

                // If the vulture flys too far away from the player it will turn around and fly the other direction
                if ((World.Player.Position.X + 100 < this.position.X) && (this.position.X - this.world.Player.Position.X <= 300))
                {
                    facing = SpriteEffects.None;
                    velocity.X = airMovement.X;
                }
               else if ((World.Player.Position.X - 100 > this.position.X) && (this.position.X - World.Player.Position.X >= -300))
                {
                    facing = SpriteEffects.FlipHorizontally;
                    velocity.X = (-1)*airMovement.X;
                }
                if ((this.animationPlayer.CurrentFrame.FrameIndex % 2)==0 && (!flewThisFrame))
                {
                    if (flyCount >= 1)
                    {
                        flewThisFrame = true;
                        flyCount = 0;
                    }
                    else
                    {
                        flyCount++;
                    }
                    velocity.Y = (-1)*gravity.Y;
                }
                else if((this.animationPlayer.CurrentFrame.FrameIndex % 2)!=0 && (flewThisFrame)){
                    
                    if (flyCount >= 1)
                    {
                        flewThisFrame = false;
                        flyCount = 0;
                    }
                    else
                    {
                        flyCount++;
                    }

                    velocity.Y = gravity.Y;
                }
            }


        }

        /// <summary>
        /// Called on a Sprite Collision?
        /// </summary>
        public void OnSpriteCollision()
        {
        }

        #region IDamageable Members

        private float maxHealth;

        /// <summary>
        /// The maximum health this bandit has
        /// </summary>
        public float MaxHealth
        {
            get { return maxHealth; }
        }

        private float currentHealth;

        /// <summary>
        /// The current health this bandit has
        /// </summary>
        public float CurrentHealth
        {
            get { return currentHealth; }
        }

        /// <summary>
        /// The bandit takes full damage from the player
        /// </summary>
        public float MitigationFactor
        {
            get { return 1; }
        }

        /// <summary>
        /// The bandit takes damage from the player
        /// </summary>
        public DamageCategory TakesDamageFrom
        {
            get { return DamageCategory.Player; }
        }

        /// <summary>
        /// Take damage if the damage is from the player
        /// </summary>
        /// <param name="damageItem">The other world item that this bandit collided with</param>
        public void TakeDamage(IDamaging damageItem)
        {
            if (this.TakesDamageFrom != damageItem.DoesDamageTo)
            {

                if (damageItem is Projectile && !currentState.Equals("Dead"))
                {
                    this.World.RemoveWorldObject(damageItem as Projectile);
                    Dispose();
                }
                currentHealth -= (int)Math.Ceiling((MitigationFactor * damageItem.AmountOfDamage));
                if (currentHealth <= 0)
                {
                    ChangeState("Dead");
                    this.amountOfDamage = 0;
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
            IDamaging damage = characterCollidedWith as IDamaging;

            if (damage != null)
            {
                this.TakeDamage(damage);
            }
        }

        #endregion

        #region ISpriteCollideable Members


        public SpriteEffects collideableFacing
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


        #region IDamaging Members

        object IDamaging.Owner
        {
            get { return this; }
        }

        WesternSpace.Utility.DamageCategory IDamaging.DoesDamageTo
        {
            get { return DamageCategory.Player; }
        }

        float amountOfDamage = 10;
        float IDamaging.AmountOfDamage
        {
            get { return amountOfDamage; }
            set { amountOfDamage = value; }
        }

        #endregion

    }
}





