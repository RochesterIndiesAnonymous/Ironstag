using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Utility;
using WesternSpace.Screens;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.DrawableComponents.EditorUI
{
    // Let's you change the starting position of the player.
    public class PlayerMover : EditorUIComponent
    {
        private Player player;

        public Player Player
        {
            get { return player; }
        }

        public ICameraService Camera
        {
            get { return player.World.Camera; }
        }

        public PlayerMover(Screen parentScreen, SpriteBatch spriteBatch, Player player)
            : base(parentScreen, spriteBatch, 
            new RectangleF(0, 0, 0, 0))
        {
            this.player = player;
            base.Draggable[0] = true; // Draggable with the left mouse button!
        }

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF(player.ScreenPosition.X, player.ScreenPosition.Y, 
                                      player.Rectangle.Width, player.Rectangle.Height);
            }
            set 
            {
                player.ScreenPosition = new Vector2(value.X, value.Y);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Dragging is done in base.Update:
            base.Update(gameTime);

            //Vector2 offset = new Vector2(Bounds.X, Bounds.Y) - Camera.RealPosition;
            //player.ScreenPosition = new Vector2(Bounds.X, Bounds.Y);
        }

        #region MOUSE EVENT HANDLERS

        #endregion

    }
}
