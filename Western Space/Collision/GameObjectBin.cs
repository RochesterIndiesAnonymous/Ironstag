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
        protected Dictionary<String, List<Character> > lookupTableOfObjects;
        public Dictionary<String, List<Character>> LookUpTableOfObjects
        {
            get { return lookupTableOfObjects; }
        }
    
        protected int numberOfGameObjects;
        protected Boolean hasMultipleObjects;
        public GameObjectBin(SpriteSpriteCollisionManager collisionManager)
        {
            this.refCollisionManager = collisionManager;
            this.lookupTableOfObjects = new Dictionary<String, List<Character> >();
            this.hasMultipleObjects = false;
        }
        public void OnObjectAdded(Character gameObject)
        {
            List<Character> pCharacterList;
            if (lookupTableOfObjects.TryGetValue(gameObject.Name, out pCharacterList))
            {
                pCharacterList.Add(gameObject);
            }
            else
            {
                lookupTableOfObjects.Add(gameObject.Name,  new List<Character>());
                lookupTableOfObjects[gameObject.Name].Add(gameObject);
            }
            numberOfGameObjects++;
            if (!hasMultipleObjects && numberOfGameObjects > 1)
            {
                refCollisionManager.ObjBinsToCheck.Add(this);
                hasMultipleObjects = true;
            }
        }
        public void OnObjectRemoved(Character gameObject)
        {
            List<Character> pCharacterList;
            if (lookupTableOfObjects.TryGetValue(gameObject.Name, out pCharacterList))
            {
                pCharacterList.Remove(gameObject);
            }
            else
            {
                lookupTableOfObjects[gameObject.Name].Remove(gameObject);
            }        
            numberOfGameObjects--;
            if (hasMultipleObjects)
            {
                refCollisionManager.ObjBinsToCheck.Remove(this);
                hasMultipleObjects = false;
            }
        }
    }
}
