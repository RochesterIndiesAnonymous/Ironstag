using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Collision
{
    public class GameObjectBin
    {
        protected List<GameObject> listOfGameObjects;
        protected Dictionary<Object, GameObject> lookupTableOfObjects;
        protected Boolean hasMultipleObjects;
        public GameObjectBin()
        {
            listOfGameObjects = new List<GameObject>();
            lookupTableOfObjects = new Dictionary<Object, GameObject>();
            hasMultipleObjects = false;
        }
        public void OnObjectAdded(GameObject gameObject)
        {
            if (!hasMultipleObjects && listOfGameObjects.Count > 1)
            {
            }
        }
        public void OnObjectRemoved(GameObject gameObject)
        {
            if (hasMultipleObjects)
            {
            }
        }
    }
}
