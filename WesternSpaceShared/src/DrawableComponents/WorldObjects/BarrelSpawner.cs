using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Physics;

using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Misc;
using WesternSpace.Collision;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.WorldObjects
{
    /// <summary>
    /// When you collide with this, a storyboard plays.
    /// </summary>
    public class BarrelSpawner : WorldObject, ISpriteCollideable
    {
        public BarrelSpawner(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            halfWidth = 200;
            halfHeight = 12;
        }
        private static readonly int TIME_BETWEEN_SPAWNS = 1000;
        private int timeUntilSpawn = TIME_UNTIL_SPAWNING_STARTS;
        public override void Update(GameTime gameTime)
        {
            if (timeUntilSpawn < 0)
            {
                Random rand = new Random();
                // Spawn a new barrel, reset timer.
                timeUntilSpawn = TIME_BETWEEN_SPAWNS;
                ExplosiveBarrel barrel = new ExplosiveBarrel(World, World.SpriteBatch, Position);
                barrel.Velocity = new Vector2(rand.Next(-6, 6), rand.Next(0,3));
                World.AddWorldObject(barrel);
            }
            else if (playerCollided)
            {
                timeUntilSpawn -= gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        // Only drawn if the world is paused?
        public override void Draw(GameTime gameTime)
        {
            if (World.Paused)
            {
                PrimitiveDrawer.Instance.DrawRect(World.SpriteBatch, Rectangle, Color.Black);
            }
            base.Draw(gameTime);
        }

        #region ISpriteCollideable Members
        private int idNumber;

        public int IdNumber
        {
            get { return idNumber; }
            set { idNumber = value; }
        }

        private int halfWidth, halfHeight;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X - halfWidth, (int)Position.Y - halfHeight,
                                      halfWidth * 2, halfHeight * 2);
            }
        }

        public SpriteEffects collideableFacing
        {
            get
            {
                return SpriteEffects.None;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public static readonly int TIME_UNTIL_SPAWNING_STARTS = 1300;

        private bool playerCollided = false;

        // Activated on sprite collision.
        public void OnSpriteCollision(ISpriteCollideable objectCollidedWith)
        {
            if (objectCollidedWith == World.Player)
            {
                playerCollided = true;
            }
        }

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
