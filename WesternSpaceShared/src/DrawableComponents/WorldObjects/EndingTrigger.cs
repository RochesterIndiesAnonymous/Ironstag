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
    /// A barrel... that EXPLODES!
    /// </summary>
    public class EndingTrigger : WorldObject, ISpriteCollideable
    {
        private Texture2D texture;

        public EndingTrigger(World world, SpriteBatch spriteBatch, Vector2 position)
            : base(world, spriteBatch, position)
        {
            halfWidth = World.Map.TileWidth / 2;
            halfHeight = World.Map.TileHeight / 2;
            playerColliding = false;
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

        private bool playerColliding;

        public void OnSpriteCollision(ISpriteCollideable objectCollidedWith)
        {
            if (objectCollidedWith == World.Player)
            {
                if (!playerColliding)
                {
                    playerColliding = true;
                    StoryboardScreen sbs = new StoryboardScreen("Ending", "Game", @"StoryboardXML\EndingStoryboard");
                    ScreenManager.Instance.ScreenList.Add(sbs);
                    ScreenTransition st = new ScreenTransition("Game", "Ending", 0.2f, 0.2f, false, false);
                    ScreenManager.Instance.Transition(st);
                    World.Paused = true;
                    World.RemoveWorldObject(this);
                }
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
