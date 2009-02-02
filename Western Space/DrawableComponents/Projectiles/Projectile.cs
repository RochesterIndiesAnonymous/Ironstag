using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Collision;
using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.AnimationFramework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.DrawableComponents.Projectiles
{
    /// <summary>
    /// Manages the the creation, updating, drawing, and destruction of a projectile
    /// </summary>
    public class Projectile : DrawableGameObject, ISpriteCollideable, IDamaging, IDisposable
    {
        /// <summary>
        /// The speed at which this projectile moves across the screen
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// The character that fired this projectile
        /// </summary>
        private object owner;
        
        /// <summary>
        /// the type of character that this projectile is attempting to hit
        /// </summary>
        private DamageCategory doesDamageTo;

        /// <summary>
        /// The amount of damage this projectile will do when it hits an enemy
        /// </summary>
        private int amountOfDamage;

        /// <summary>
        /// The direction constant to apply to the velocity vector
        /// </summary>
        private short direction;

        /// <summary>
        /// The direction to draw the projectile in
        /// </summary>
        private SpriteEffects animationDirection;

        /// <summary>
        /// The object that is used to draw the projectile to the screen
        /// </summary>
        private AnimationPlayer player;

        /// <summary>
        /// The collision manager that this projectile needs to register with
        /// </summary>
        private SpriteSpriteCollisionManager collision;

        /// <summary>
        /// The game screen this object belongs to
        /// This is used to get access to the camera and collision manager
        /// </summary>
        private GameScreen gameScreen;

        public Projectile(Screen screen, SpriteBatch batch, Vector2 position, Animation animation, short direction,
            Vector2 velocity, object owner, DamageCategory doesDamageTo, int amountOfDamage)
            : base(screen, batch, position)
        {
            player = new AnimationPlayer(batch, animation);

            this.direction = direction;
            this.velocity = velocity;
            this.owner = owner;
            this.doesDamageTo = doesDamageTo;
            this.amountOfDamage = amountOfDamage;

            animationDirection = SpriteEffects.None;

            if (direction == 1)
            {
                animationDirection = SpriteEffects.FlipHorizontally;
            }

            this.ParentScreen.Components.Add(this);

            gameScreen = (GameScreen)screen;
        }

        /// <summary>
        /// Registers this projectile with the collision manager
        /// </summary>
        public override void Initialize()
        {
            //collision = (SpriteSpriteCollisionManager)this.Game.Services.GetService(typeof(SpriteSpriteCollisionManager));

            //collision.RegisteredObjectList.Add(this);

            base.Initialize();
        }

        /// <summary>
        /// Checks to see if the projectile is off the screen, updates the animation state, and updates the position
        /// </summary>
        /// <param name="gameTime">Time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            // check to see if we are outside the camera.
            if (this.Position.X > gameScreen.World.Camera.VisibleArea.X + gameScreen.World.Camera.VisibleArea.Width || this.Position.X + this.player.Animation.FrameWidth < gameScreen.World.Camera.VisibleArea.X)
            {
                // This allows the bullet to be garbage collected. Verified that garbage collection happens on 2/1/2009
                //this.collision.RegisteredObjectList.Remove(this);
                this.ParentScreen.Components.Remove(this);
                this.Dispose();
                return;
            }

            // update the animation player
            player.Update(gameTime);

            // update the position of the projectile
            this.Position += (direction * this.velocity);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the projectile to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // draw the projectile
            player.Draw(gameTime, this.SpriteBatch, this.Position, animationDirection);

            base.Draw(gameTime);
        }

        #region IDamaging Members

        public object Owner
        {
            get { return owner; }
        }

        public DamageCategory DoesDamageTo
        {
            get { return doesDamageTo; }
        }

        public int AmountOfDamage
        {
            get { return amountOfDamage; }
        }

        #endregion

        #region ISpriteCollideable Members

        public void OnSpriteCollision(WesternSpace.DrawableComponents.Actors.Character characterCollidedWith)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Called to flag that this projectile is ready to be garbage collected.
        /// Might not be needed :/
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
