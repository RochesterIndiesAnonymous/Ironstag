using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Actors;
using System.Diagnostics;
using System.Drawing;

namespace WesternSpace.Collision
{
    /********************************************************************
     * GameObjectBin - Is a single Bin where collisions take place
    ********************************************************************/
    public class CollisionObjectBin
    {       
        // Reference to Collision Manager
        protected SpriteSpriteCollisionManager refCollisionManager;        
        protected Boolean hasMultipleObjects;
        public Boolean HasMultipleObjects
        {
            get { return hasMultipleObjects; }
        }
        // List of Collideable Objects
        protected List<ISpriteCollideable> listOfCollideableObjects;
        public List<ISpriteCollideable> ListOfCollideableObjects
        {
            get { return listOfCollideableObjects; }
        }
        public int NumberOfCollideableObjects
        {
            get { return listOfCollideableObjects.Count; }
        }
        protected Point gridCoord;
        public CollisionObjectBin(SpriteSpriteCollisionManager collisionManager, int gridCoordX, int gridCoordY)
        {
            this.gridCoord = new Point(gridCoordX, gridCoordY);
            this.listOfCollideableObjects = new List<ISpriteCollideable>();
            this.refCollisionManager = collisionManager;
            this.hasMultipleObjects = false;
        }
        public void OnObjectAdded(ISpriteCollideable gameObject)
        {
            this.listOfCollideableObjects.Add(gameObject);
            if (!hasMultipleObjects && this.listOfCollideableObjects.Count > 1)
            {
                // add this bin to a list of bins to check for collision
                refCollisionManager.ObjectBinsToCheck.Add(this);
                hasMultipleObjects = true;
                Debug.Print("Multiple Object in Bin: " + gridCoord.ToString() + " are true");
            }
        }
        public void OnObjectRemoved(ISpriteCollideable gameObject)
        {
            this.listOfCollideableObjects.Remove(gameObject);
            if (hasMultipleObjects && this.listOfCollideableObjects.Count < 2)
            {
                // remove this bin from the list of bins to check for collision
                refCollisionManager.ObjectBinsToCheck.Remove(this);
                hasMultipleObjects = false;
                Debug.Print("Multiple Object in Bin: " + gridCoord.ToString() + " are false");
            }
        }
    }
}
