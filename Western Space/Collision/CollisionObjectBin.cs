using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.DrawableComponents.Actors;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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
        // GridSpace Coordinates usefull for debugging
        protected int xCoordInGrid;
        protected int yCoordInGrid;
        public CollisionObjectBin(SpriteSpriteCollisionManager collisionManager, int gridCoordX, int gridCoordY)
        {       
            this.listOfCollideableObjects = new List<ISpriteCollideable>();
            this.refCollisionManager = collisionManager;
            this.hasMultipleObjects = false;
            this.xCoordInGrid = gridCoordX;
            this.yCoordInGrid = gridCoordY;                                    
        }
        // This function is called when a object needs to be added to bin
        public void OnObjectAdded(ISpriteCollideable gameObject)
        {
            // Add object to the object list
            this.listOfCollideableObjects.Add(gameObject);
            // If this bin does not have multiple objects and the the number of
            // collideable objects are greater than 1. the bin adds itself the
            // managerslist of object bins to check
            if (!hasMultipleObjects && this.listOfCollideableObjects.Count > 1)
            {
                // add this bin to a list of bins to check for collision
                refCollisionManager.ObjectBinsToCheck.Add(this);
                hasMultipleObjects = true;
                //Debug.Print("Multiple Object in Bin: " + gridCoord.ToString() + " are true");
            }
        }
        // This function is called when a object needs to be removed from a bin
        public void OnObjectRemoved(ISpriteCollideable gameObject)
        {
            // Remove Object from the object list
            this.listOfCollideableObjects.Remove(gameObject);
            // If this bin has multiple objects and the the number of
            // collideable objects are less than 2. the bin removes itself from 
            // the managerslist of object bins to check
            if (hasMultipleObjects && this.listOfCollideableObjects.Count < 2)
            {
                // remove this bin from the list of bins to check for collision
                refCollisionManager.ObjectBinsToCheck.Remove(this);
                hasMultipleObjects = false;
                //Debug.Print("Multiple Object in Bin: " + gridCoord.ToString() + " are false");
            }
        }
    }
}
