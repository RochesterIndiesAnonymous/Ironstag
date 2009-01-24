using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Collision
{
    public class SpriteTileCollisionManager
    {
        protected List<ICollideable> collideableObjectList;
        public SpriteTileCollisionManager()
        {
            collideableObjectList = new List<ICollideable>();
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
        public void update()
        {
            foreach (ICollideable icObject in collideableObjectList)
            {
                // Test hotspots against tile
            }
        }
    }
}
