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
    public class Projectile : DrawableGameObject, ISpriteCollideable, IDamaging, IDisposable
    {
        private Vector2 velocity;

        private object owner;
        private DamageCategory doesDamageTo;
        private int amountOfDamage;
        private short direction;
        private SpriteEffects animationDirection;

        private AnimationPlayer player;

        private SpriteSpriteCollisionManager collision;
        private ICameraService camera;

        public Projectile(Screen screen, SpriteBatch batch, Vector2 position, Animation animation, short direction,
            Vector2 velocity, object owner, DamageCategory doesDamageTo, int amountOfDamage)
            : base(screen, batch, position)
        {
            player = new AnimationPlayer(batch, animation, animation);

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
        }

        public override void Initialize()
        {
            //collision = (SpriteSpriteCollisionManager)this.Game.Services.GetService(typeof(SpriteSpriteCollisionManager));
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            //collision.RegisteredObjectList.Add(this);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // check to see if we are outside the camera.
            if (this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X < camera.VisibleArea.X)
            {
                // hopefully this will garbage collect it otherwise we have a huge memory leak.
                //this.collision.RegisteredObjectList.Remove(this);
                this.ParentScreen.Components.Remove(this);
                this.Dispose();
                return;
            }

            player.Update(gameTime);

            this.Position += (direction * this.velocity);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
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

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
