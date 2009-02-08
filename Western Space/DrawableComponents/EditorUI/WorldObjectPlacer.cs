using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.Reflection;

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

        private WorldObject[] worldObjects;

        private int index;

        private bool drawObjectCursor;

        public bool DrawObjectCursor
        {
            get { return drawObjectCursor; }
            set { drawObjectCursor = value; }
        }

        public WorldObjectPlacer(EditorScreen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, World world)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.index = 1;
            this.world = world;
            this.drawObjectCursor = true;

            object[] defaultParams = new object[] { World, SpriteBatch, new Vector2(bounds.X - 30, bounds.Y + 20) };

            int i = 0;
            worldObjects = new WorldObject[World.WorldObjectCtorInfos.Length];
            foreach (ConstructorInfo ci in World.WorldObjectCtorInfos)
            {
                worldObjects[i] = (WorldObject)World.WorldObjectCtorInfos[i].Invoke(defaultParams);
                ++i;
            }
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClick(int button)
        {
            switch (button)
            { 
                case 0: // Left click, place a new WorldObject.
                    object[] woParams = new object[]{World, World.SpriteBatch, Mouse.WorldPosition};
                    WorldObject wo =
                            (WorldObject)World.WorldObjectCtorInfos[index].Invoke(woParams);
                    //wo.Initialize();
                    world.AddWorldObject(wo);

                    WorldObjectMover wom = new WorldObjectMover(EditorScreen, SpriteBatch, wo);
                    EditorScreen.WorldObjectMovers.Add(wom);
                    ParentScreen.Components.Add(wom);
                    break;
            }
            base.OnMouseClick(button);
        }

        protected override void OnMouseScroll(int amount)
        {
            index = (int)MathHelper.Clamp((index + (amount > 0 ? 1 : -1)),
                                                                 0,
                                                                 worldObjects.Length - 1);
            base.OnMouseScroll(amount);
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            if (drawObjectCursor)
            {
                worldObjects[index].Position = Mouse.Position;
                worldObjects[index].Draw(gameTime);
            }
            PrimitiveDrawer.Instance.DrawLine(SpriteBatch, new Vector2(Mouse.Position.X-6, Mouse.Position.Y),
                                                           new Vector2(Mouse.Position.X+5, Mouse.Position.Y), Color);
            PrimitiveDrawer.Instance.DrawLine(SpriteBatch, new Vector2(Mouse.Position.X, Mouse.Position.Y-5),
                                                           new Vector2(Mouse.Position.X, Mouse.Position.Y+6), Color);
            base.Draw(gameTime);
        }
    }
}