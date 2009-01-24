using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using WesternSpace.TilingEngine;

namespace WesternSpace.Collision
{
    public class SpriteTileCollisionManager : GameComponent

    {
        protected TileMap worldMap;
        protected List<ICollideable> collideableObjectList;
        public SpriteTileCollisionManager(Game game, World world) 
            : base(game)
        {
            collideableObjectList = new List<ICollideable>();
            // The Map that objects interact with
            worldMap = world.Map;
        }
        // Adds entity to be checked for collision
        public void addObjectToList(ICollideable collidableObject)
        {
            this.collideableObjectList.Add(collidableObject);
        }
        // Removes entity from being checked for collision
        public void removeObjectFromList(ICollideable collidableObject)
        {
            this.collideableObjectList.Remove(collidableObject);
        }
        // Checks all registed entities for collision with tiles
        public override void Update(GameTime gameTime)
        {
            foreach (ICollideable icObject in collideableObjectList)
            {
                // Test each hotspots against tile
                foreach(CollisionHotspot hotspot in icObject.Hotspots)
                {
                    // run tile function to get tile with edge
                    // if tile has an edge then test for collision
                }
            }
            base.Update(gameTime);
        }
    }
}
