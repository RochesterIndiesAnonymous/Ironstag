using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using WesternSpace.TilingEngine;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Collision
{
    public class SpriteTileCollisionManager : GameComponent
    {
        // The Map that objects interact with
        protected World world;
        protected TileMap worldMap
        {
            get { return world.Map; }
        }
        protected List<ITileCollideable> collideableObjectList;
        public List<ITileCollideable> CollideableObjectList
        {
            get { return collideableObjectList; }
        }
        public SpriteTileCollisionManager(Game game, World world) 
            : base(game)
        {
            collideableObjectList = new List<ITileCollideable>();
            this.world = world;
        }
        public Point CalculateTileCoord(Vector2 position)
        {
            Point tileCoord = new Point();
            tileCoord.X = (int)position.X / worldMap.TileWidth;
            tileCoord.Y = (int)position.Y / worldMap.TileHeight;
            return tileCoord;
        }
        // Checks all registed entities for collision with tiles
        public override void Update(GameTime gameTime)
        {
            Point tileCoord;
            Tile tile;
            foreach (Character collideableObject in collideableObjectList)
            {
                // Test each hotspots against tile                  
                foreach (CollisionHotspot hotspot in collideableObject.Hotspots)
                {
                    if (hotspot.WorldPosition.X > 0 && hotspot.WorldPosition.Y > 0)
                    {
                        // run tile function to get tile with edge
                        tileCoord = CalculateTileCoord(hotspot.WorldPosition);
                        tile = worldMap.Tiles[tileCoord.X, tileCoord.Y];
                        if (tile != null)
                        {
                            Rectangle tileRect = new Rectangle(tileCoord.X * worldMap.TileWidth,
                                tileCoord.Y * worldMap.TileHeight, worldMap.TileWidth, worldMap.TileHeight);
                            hotspot.OnTileCollision(tile, tileRect);
                            collideableObject.OnTileColision(tile, hotspot, tileRect);
                        }
                    }
                }
            }
            base.Update(gameTime);
        }
    }
}
