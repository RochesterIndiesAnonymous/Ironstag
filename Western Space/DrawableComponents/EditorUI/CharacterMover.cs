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
    public class CharacterMover : EditorUIComponent
    {
        private Character character;

        public Character Character
        {
            get { return character; }
        }

        public ICameraService Camera
        {
            get { return character.World.Camera; }
        }

        public CharacterMover(EditorScreen parentScreen, SpriteBatch spriteBatch, Character character)
            : base(parentScreen, spriteBatch, 
            new RectangleF(0, 0, 0, 0))
        {
            this.character = character;
            base.Draggable[0] = true; // Draggable with the left mouse button!
        }

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF(character.ScreenPosition.X, character.ScreenPosition.Y, 
                                      character.Rectangle.Width, character.Rectangle.Height);
            }
            set 
            {
                character.ScreenPosition = new Vector2(value.X, value.Y);
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
