using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Interfaces;
using WesternSpace.Screens;
using System.Xml.Linq;

namespace WesternSpace
{
    /// <summary>
    /// A world object is simply any object that can reside in a world.
    /// </summary>
    public class WorldObject : DrawableGameObject, IXElementOutput
    {
        private World world;

        public World World
        {
            get { return world; }
        }

        /*
        /// <summary>
        /// The position of this item in the world.
        /// This typically represents the "top left corner" of the object, but may be
        /// subject to change...
        /// </summary>
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }*/

        /// <summary>
        /// The position of this character on the current screen, based on
        ///  the world's camera's position.
        /// </summary>
        public Vector2 ScreenPosition
        {
            get { return Position - World.Camera.Position; }
            set { Position = World.Camera.Position + value; }
        }

        public WorldObject(World world, SpriteBatch spriteBatch, Vector2 position)
            :base(world.ParentScreen, spriteBatch, position)
        {
            this.world = world;
            this.position = position;
        }

        #region IXElementOutput Members

        public XElement ToXElement()
        {
            return new XElement("o", new XAttribute("n", this.GetType().Name), new XAttribute("x", Position.X), new XAttribute("y", Position.Y));
        }

        #endregion
    }
}
