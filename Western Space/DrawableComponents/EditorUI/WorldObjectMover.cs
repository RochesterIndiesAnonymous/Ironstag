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
    public class WorldObjectMover : EditorUIComponent
    {
        /// <summary>
        /// The "handle" that lets you move the worldObject around. It's 
        /// top-left corner is defined as the centerpoint of the worldObject that we're moving
        /// in the constructor.
        /// </summary>
        public static RectangleF MIN_BOUNDS = new RectangleF(0,0, 10, 10);

        private WorldObject worldObject;

        public WorldObject WorldObject
        {
          get { return worldObject; }
          set { worldObject = value; }
        }

        public ICameraService Camera
        {
            get { return worldObject.World.Camera; }
        }

        public WorldObjectMover(EditorScreen parentScreen, SpriteBatch spriteBatch, WorldObject worldObject)
            : base(parentScreen, spriteBatch, 
            new RectangleF(worldObject.ScreenPosition.X, worldObject.ScreenPosition.Y, MIN_BOUNDS.Width, MIN_BOUNDS.Height))
        {
            this.worldObject = worldObject;
            base.Draggable[0] = true; // Draggable with the left mouse button!
        }

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF(WorldObject.ScreenPosition.X, WorldObject.ScreenPosition.Y, base.Bounds.Width, base.Bounds.Height);
            }
            set 
            {
                worldObject.ScreenPosition = new Vector2(value.X, value.Y);
            }
        }

        #region MOUSE EVENT HANDLERS
        // Dragging implemented in base.
        protected override void OnMouseClickAndUnclick(int button)
        {
            // Remove this worldObject from the world and our editorScreen.
            base.OnMouseUnclick(button);
        }
        #endregion

    }
}
