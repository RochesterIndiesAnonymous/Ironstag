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

namespace WesternSpace.DrawableComponents.EditorUI
{
    // Let's you change the starting position of the player.
    public class SaveButton : EditorUIComponent
    {
        private World world;

        private ITextureService textureService;

        private Texture2D icon;

        public SaveButton(EditorScreen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, World world)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.world = world;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));
        }

        public override void Initialize()
        {
            icon = ParentScreen.Game.Content.Load<Texture2D>("Textures\\floppy");
            this.Bounds = new RectangleF(Bounds.X, Bounds.Y, icon.Width, icon.Height);
            base.Initialize();
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseUnclick(int button)
        {
            if (button == 0) // Only handle left clicks, ignore others.
            {
                TileMap orig = world.Map;

                // Save TileMap here.
                TileMap copy = new TileMap(orig.Width, orig.Height,
                                           orig.TileWidth, orig.TileHeight,
                                           orig.LayerCount, orig.SubLayerCount);


                Tile tile;
                for (int i = 0; i < orig.Width; ++i)
                {
                    for (int j = 0; j < orig.Height; ++j)
                    {
                        tile = orig[i,j];
                        if (tile != null)
                            copy[i, j] = tile;
                    }
                }
                copy.Minimize();
                world.ShiftWorldObjects(-EditorScreen.Offset);
                
                XDocument tileDoc = new XDocument(copy.ToXElement());
                tileDoc.Save("..\\..\\..\\Content\\"+orig.FileName+".xml");
                XDocument worldDoc = new XDocument(world.ToXElement());
                worldDoc.Save("..\\..\\..\\Content\\WorldXML\\TestWorld.xml");
                world.ShiftWorldObjects(EditorScreen.Offset);
            }
            base.OnMouseUnclick(button);
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            PrimitiveDrawer.Instance.DrawSolidRect(SpriteBatch, 
                   new Microsoft.Xna.Framework.Rectangle(0, 0, 39, 480),
                   Microsoft.Xna.Framework.Graphics.Color.Black);
            SpriteBatch.Draw(icon, new Vector2((int)Bounds.X, (int)Bounds.Y), Color);
            base.Draw(gameTime);
        }
    }
}
