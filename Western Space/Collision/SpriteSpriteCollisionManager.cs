using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;
using System.Diagnostics;
using WesternSpace.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : GameComponent
    {
        static int IDNumberCount = 0;
        // Object Collision Grid 
        protected GameObjectBin[,] objCollisionGrid;
        // Bin Dimensions
        protected Point binDimension;
        // Number Of Bins
        protected Point numOfBins;
        // Registered Object List
        protected List<ISpriteCollideable> registeredObject;
        // Object Bins To Check
        protected List<GameObjectBin> objBinsToCheck;
        public List<GameObjectBin> ObjBinsToCheck
        {
            get { return objBinsToCheck; }
        }
        protected Dictionary<int, List<Point>> objBinLookupTable;
        public SpriteSpriteCollisionManager(Game game, Point binWH)
            : base(game)
        {
            registeredObject = new List<ISpriteCollideable>();
            objBinsToCheck = new List<GameObjectBin>();

            objBinLookupTable = new Dictionary<int, List<Point>>();
            
            binDimension = binWH;
            numOfBins.X = game.GraphicsDevice.Viewport.Width / binWH.X;
            numOfBins.Y = game.GraphicsDevice.Viewport.Height / binWH.Y;
            objCollisionGrid = new GameObjectBin[numOfBins.X, numOfBins.Y];
        }
        public override void Initialize()
        {
            for (int y = 0; y < numOfBins.Y; y++)
            {
                for (int x = 0; x < numOfBins.X; x++)
                {
                    objCollisionGrid[x, y] = new GameObjectBin(this, x, y);
                }
            }
            base.Initialize();
        }
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            
            registeredObject.Add(collideableObject);
            collideableObject.IdNumber = IDNumberCount;
            Debug.Print("Object Added to Registered List: " + collideableObject.IdNumber);
            IDNumberCount++;
        }
        public void removeObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {

            OnRemoveObjectFromBin(collideableObject, this.getObjectCollisionBinCoord(collideableObject));
            this.objBinLookupTable.Remove(collideableObject.IdNumber);
            Debug.Print("Object Removed from Registered List: " + collideableObject.IdNumber);
            registeredObject.Remove(collideableObject);

        }       
        protected void OnAddObjectToBin(ISpriteCollideable gameObject, List<Point> listOfObjectBinCoord)
        {            
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to Object Collision Grid
                this.objCollisionGrid[binCoord.X, binCoord.Y].OnObjectAdded(gameObject);
                // Update Look Up Table               
                this.objBinLookupTable[gameObject.IdNumber] = listOfObjectBinCoord;
            }
        }
        protected void OnUpdateObjectInBin(ISpriteCollideable gameObject, List<Point> oldObjCoord, List<Point> newObjCoord)
        {
            OnRemoveObjectFromBin(gameObject, oldObjCoord);
            OnAddObjectToBin(gameObject, newObjCoord);
        }
        protected void OnRemoveObjectFromBin(ISpriteCollideable gameObject, List<Point> listOfObjectBinCoord)
        {
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to ObjectList of a Bin
                this.objCollisionGrid[binCoord.X, binCoord.Y].OnObjectRemoved(gameObject);
                // Update Object Look Up Table               
                this.objBinLookupTable[gameObject.IdNumber] = listOfObjectBinCoord;
            }
        }
        public Point xformScreenCoordToBinCoord(Vector2 vector)
        {
            return new Point((int)vector.X / binDimension.X, (int)vector.Y / binDimension.Y);
        }
        /*
         * NewRectToCoord - Takes the upperLeft and the lowerRight corner points
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
        public override void Update(GameTime gameTime)
        {
            List<Point> newCoords;
            List<Point> oldCoords;
            // Update Collision Bins
            foreach (ISpriteCollideable gameObj in registeredObject)
            {                
                newCoords = this.getObjectCollisionBinCoord(gameObj);
                if (objBinLookupTable.TryGetValue(gameObj.IdNumber, out oldCoords))
                {
                   // Debug.Print("Update Object In Bin ID: " + gameObj.IdNumber + " New Coord: "
                    //    + newCoords[0] + " Old Coord: " + oldCoords[0]);
                   this.OnUpdateObjectInBin(gameObj, oldCoords, newCoords);
                }
                else
                {
                    this.OnAddObjectToBin(gameObj, newCoords);
                }
            }
            // Check Collision Bins (Collision Bins)
            // Collection was modifed, need to recycle bullets

            // make a copy of the bins to check
            IEnumerable<GameObjectBin> objBinsToCheckCopy = objBinsToCheck.ToList();

            foreach (GameObjectBin gameObjBin in objBinsToCheckCopy)
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
        Boolean BoundingBoxA(ISpriteCollideable entityA, ISpriteCollideable entityB)
        {          
            return entityA.Rectangle.Intersects(entityB.Rectangle);
        }
    }
}
