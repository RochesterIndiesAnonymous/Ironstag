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
        protected TileMap worldMap;
        protected List<ITileCollideable> collideableObjectList;
        public SpriteTileCollisionManager(Game game, World world) 
            : base(game)
        {
            collideableObjectList = new List<ITileCollideable>();
            // The Map that objects interact with
            worldMap = world.Map;
        }
        // Adds entity to be checked for collision
        public void addObjectToList(ITileCollideable collidableObject)
        {
            this.collideableObjectList.Add(collidableObject);
        }
        // Removes entity from being checked for collision
        public void removeObjectFromList(ITileCollideable collidableObject)
        {
            this.collideableObjectList.Remove(collidableObject);
        }
        public Point CalculateTileCoord(Vector2 position)
        {
            Point tileCoord = new Point();
            tileCoord.X = (int)position.X / worldMap.tileWidth;
            tileCoord.Y = (int)position.Y / worldMap.tileHeight;
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
                   // run tile function to get tile with edge
                   tileCoord = CalculateTileCoord(hotspot.WorldPosition);
                   tile = worldMap.Tiles[tileCoord.X, tileCoord.Y];          
                   if (tile != null)
                   {
                       Rectangle tileRect = new Rectangle(tileCoord.X * worldMap.tileWidth, 
                           tileCoord.Y * worldMap.tileHeight, worldMap.tileWidth, worldMap.tileHeight);
                    collideableObject.OnTileColision(tile, hotspot, tileRect);
                   }    
                }
            }
            base.Update(gameTime);
        }
    }
}
