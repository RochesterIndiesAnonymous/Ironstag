using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Collision
{
    public class GameObjectBin
    {
        protected SpriteSpriteCollisionManager refCollisionManager;
        protected List<ISpriteCollideable> listOfObjects;
        public List<ISpriteCollideable> ListOfObjects
        {
            get { return listOfObjects; }
        }
        protected int numberOfGameObjects;
        protected Boolean hasMultipleObjects;
        public GameObjectBin(SpriteSpriteCollisionManager collisionManager)
        {
            this.refCollisionManager = collisionManager;
            this.listOfObjects = new List<ISpriteCollideable>();
            this.hasMultipleObjects = false;
        }
        public void OnObjectAdded(ISpriteCollideable gameObject)
        {            
            this.listOfObjects.Add(gameObject);
            numberOfGameObjects++;
            if (!hasMultipleObjects && numberOfGameObjects > 1)
            {
                refCollisionManager.ObjBinsToCheck.Add(this);
                hasMultipleObjects = true;
            }
        }
        public void OnObjectRemoved(ISpriteCollideable gameObject)
        {
            this.listOfObjects.Remove(gameObject);
            numberOfGameObjects--;
            if (hasMultipleObjects)
            {
                refCollisionManager.ObjBinsToCheck.Remove(this);
                hasMultipleObjects = false;
            }
        }
    }
}
