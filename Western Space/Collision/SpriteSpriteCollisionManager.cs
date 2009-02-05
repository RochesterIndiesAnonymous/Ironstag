using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;
using System.Diagnostics;
using WesternSpace.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.Screens;
/*
 * Note: I forgot to make sure these calculations the sprite position and stuff need to happen 
 * in screen space
 */

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : GameComponent
    {
        static int IDNumberCount = 0;
        // Object Collision Grid 
        protected CollisionObjectBin[,] objectCollisionGrid;
        // Bin Dimensions
        protected Point binDimension;
        // Number Of Bins
        protected Point numOfBins;//k
        // Registered Object List
        protected List<ISpriteCollideable> registeredObject;
        // Object Bins To Check (This is added to in Grid Bins when a bin contains multiple sprites)
        protected List<CollisionObjectBin> objectBinsToCheck;
        public List<CollisionObjectBin> ObjectBinsToCheck
        {
            get { return objectBinsToCheck; }
        }

        protected Dictionary<int, List<Point>> objBinLookupTable;
        //protected GameScreen gameScreen;
        public SpriteSpriteCollisionManager(Game game, Screen parentScreen, Point binWH)
            : base(game)
        {
            //gameScreen = (GameScreen)parentScreen;
            //gameScreen.World.Camera.
            registeredObject = new List<ISpriteCollideable>();
            objectBinsToCheck = new List<CollisionObjectBin>();            
            objBinLookupTable = new Dictionary<int, List<Point>>();
            
            binDimension = binWH;
            numOfBins.X = game.GraphicsDevice.Viewport.Width / binWH.X;
            numOfBins.Y = game.GraphicsDevice.Viewport.Height / binWH.Y;
            objectCollisionGrid = new CollisionObjectBin[numOfBins.X, numOfBins.Y];
        }
        public override void Initialize()
        {
            for (int y = 0; y < numOfBins.Y; y++)
            {
                for (int x = 0; x < numOfBins.X; x++)
                {
                    objectCollisionGrid[x, y] = new CollisionObjectBin(this, x, y);
                }
            }
            base.Initialize();
        }
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // add Object to Bins
            registeredObject.Add(collideableObject);
            // assigns a id number to object
            collideableObject.IdNumber = IDNumberCount;
            Debug.Print("Object Added to Registered List: " + collideableObject.IdNumber);
            // increments the idnumberCounter
            IDNumberCount++;
        }
        public void removeObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // 
            this.objBinLookupTable.Remove(collideableObject.IdNumber);

            OnRemoveObjectFromBin(collideableObject, this.getObjectCollisionBinCoord(collideableObject));
            
            Debug.Print("Object Removed from Registered List: " + collideableObject.IdNumber);
            // Removes the registered Object
            registeredObject.Remove(collideableObject);

        }
        protected void OnAddObjectToBin(ISpriteCollideable collideableObject, List<Point> listOfObjectBinCoord)
        {            
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to Object Collision Grid
                this.objectCollisionGrid[binCoord.X, binCoord.Y].OnObjectAdded(collideableObject);                
            }
            //// Update Look Up Table               
            this.objBinLookupTable[collideableObject.IdNumber] = listOfObjectBinCoord;
        }
        protected void OnRemoveObjectFromBin(ISpriteCollideable collideableObject, List<Point> listOfObjectBinCoord)
        {
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to ObjectList of a Bin
                this.objectCollisionGrid[binCoord.X, binCoord.Y].OnObjectRemoved(collideableObject);
            }
            //// Update Object Look Up Table               
            this.objBinLookupTable[collideableObject.IdNumber] = listOfObjectBinCoord;
            
        }
        public Point xformScreenCoordToBinCoord(Vector2 vector)
        {
            // ScreenSpace
            //float x = (vector.X - gameScreen.World.Camera.Position.X);
            //float y = (vector.Y - gameScreen.World.Camera.Position.Y);
            // GridSpace
            return new Point((int)vector.X / binDimension.X, (int)vector.Y / binDimension.Y);
            //return new Point((int)x / binDimension.X, (int)y / binDimension.Y);
        }
        /*
         * Changed NewRectToCoord = getObjectCollisionBinCoord
         * getObjectCollisionBinCoord - Takes the upperLeft and the lowerRight corner points
         * and interpolates between their x and y values to determin all of the
         * grid points they occupy         
         */
        public List<Point> getObjectCollisionBinCoord(ISpriteCollideable collideableObject)
        {
            List<Point> listOfBinCoord = new List<Point>();
            Point leftTop = xformScreenCoordToBinCoord(
                new Vector2(collideableObject.Rectangle.Left, collideableObject.Rectangle.Top));
            Point rightBottom = xformScreenCoordToBinCoord(
                new Vector2(collideableObject.Rectangle.Right, collideableObject.Rectangle.Bottom));
            if (!leftTop.Equals(rightBottom))
            {
                for (int y = leftTop.Y; y < rightBottom.Y; y++)
                {
                    for (int x = leftTop.X; x < rightBottom.X; x++)
                    {
                        listOfBinCoord.Add(new Point(x, y));
                    }
                }
            }
            else
            {
                listOfBinCoord.Add(leftTop);
            }
            return listOfBinCoord;
        }
        public Boolean CoordsEqual(List<Point> pointA, List<Point> pointB)
        {
            if (pointA.Count != pointB.Count)
                return false;
            for (int i = 0; i < pointA.Count; i++)
            {
                if (!pointA[i].Equals(pointB[i]))
                    return false;
            }
            return true;
        }
        public override void Update(GameTime gameTime)
        {
            List<Point> newCoords;
            List<Point> oldCoords;
            // Update Collision Bins
            foreach (ISpriteCollideable gameObj in registeredObject)
            {                
                newCoords = this.getObjectCollisionBinCoord(gameObj);
                // Lookup table contains the last coordnates of each game object
                if (objBinLookupTable.TryGetValue(gameObj.IdNumber, out oldCoords))
                {
                    // Update Bin Only if the coords have changed
                    if (!CoordsEqual(oldCoords, newCoords))
                    {
                        Debug.Print(gameObj.IdNumber + " Occupies ");
                        foreach (Point coord in newCoords)
                        {
                            Debug.Print(">" + coord.ToString());
                        }
                        OnRemoveObjectFromBin(gameObj, oldCoords);
                        OnAddObjectToBin(gameObj, newCoords);
                    }
                    //Debug.Print("Update Object In Bin ID: " + gameObj.IdNumber + " New Coord: "
                    //    + newCoords[0] + " Old Coord: " + oldCoords[0]);                            
                }
                else
                {
                    this.OnAddObjectToBin(gameObj, newCoords);
                }
            }
            // make a copy of the bins to check
            IEnumerable<CollisionObjectBin> objBinsToCheckCopy = objectBinsToCheck.ToList();
            // Scan all bins on the Object List to be Checked
            foreach (CollisionObjectBin gameObjBin in objBinsToCheckCopy)
            {                
                for (int i = 0; i < gameObjBin.ListOfCollideableObjects.Count - 1; i++)
                {
                    for (int j = 1; j < gameObjBin.ListOfCollideableObjects.Count; j++)
                    {
                        if (BoundingBoxA(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j)))
                        {
                            gameObjBin.ListOfCollideableObjects.ElementAt(i).OnSpriteCollision(gameObjBin.ListOfCollideableObjects.ElementAt(j));
                            gameObjBin.ListOfCollideableObjects.ElementAt(j).OnSpriteCollision(gameObjBin.ListOfCollideableObjects.ElementAt(i));                            
                        }
                    }
                }             
            }
            base.Update(gameTime);
        }
        Boolean BoundingBoxA(ISpriteCollideable collideableObjectA, ISpriteCollideable collideableObjectB)
        {
            return collideableObjectA.Rectangle.Intersects(collideableObjectB.Rectangle);
        }
    }
}
