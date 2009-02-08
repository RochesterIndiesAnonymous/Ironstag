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
    public class WorldObjectPlacer : EditorUIComponent
    {
        private World world;

        public World World
        {
            get { return world; }
        }

        public ICameraService Camera
        {
            get { return World.Camera; }
        }

        public WorldObjectPlacer(EditorScreen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, World world)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.index = 2;
            this.world = world;
        }

        private int index;

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClick(int button)
        {
            switch (button)
            { 
                case 0: // Left click, place a new WorldObject.
                    object[] woParams = new object[]{World, SpriteBatch, Mouse.WorldPosition};
                    WorldObject wo =
                            (WorldObject)EditorScreen.WorldObjectCtorInfos[index].Invoke(woParams);
                    //wo.Initialize();
                    world.AddWorldObject(wo);
                    break;
            }
            base.OnMouseClick(button);
        }

        #endregion

    }
}