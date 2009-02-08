using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.Collision;
using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.AnimationFramework;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.DrawableComponents.Misc
{
    /// <summary>
    /// A small cactus that the player gets hurt walking into.
    /// </summary>
    public class SmallCactus : WorldObject, ISpriteCollideable, IDamaging
    {
        private Rectangle boundingRectangle;
        private AnimationPlayer animationPlayer;
        private Animation excitingCactusAnimation;

        /// <summary>
        /// Camera used to see if the enemy is visible.
        /// </summary>
        private ICameraService camera;

        public SmallCactus(World world, SpriteBatch spriteBatch, Vector2 position) : base(world,spriteBatch,position)
        {
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, 24, 24);

            excitingCactusAnimation = new Animation("ActorXML\\SmallCactus", "ExcitingCactusAnimation");
            this.animationPlayer = new AnimationPlayer(spriteBatch, excitingCactusAnimation);
        }

        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!(this.Position.X > camera.VisibleArea.X + camera.VisibleArea.Width || this.Position.X + this.animationPlayer.Animation.FrameWidth < camera.VisibleArea.X))
            {
                //Let the Animation Player Draw
                animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position, SpriteEffects.None);
            }
        }

        #region ISpriteCollideable Members

        private int idNumber;
        int ISpriteCollideable.IdNumber
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

        Rectangle ISpriteCollideable.Rectangle
        {
            get { return boundingRectangle; }
        }

        SpriteEffects ISpriteCollideable.collideableFacing
        {
            get
            {
                return SpriteEffects.None;
            }
            set
            {
                // A cactus shouldn't flip.
            }
        }

        Animation CurrentAnimation
        {
            get { return animationPlayer.Animation; }
        }

        void ISpriteCollideable.OnSpriteCollision(ISpriteCollideable characterCollidedWith)
        {
            // Nothing happens to the cactus on collisions.
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

        int IDamaging.AmountOfDamage
        {
            get { return 1; }
        }

        #endregion
    }
}
