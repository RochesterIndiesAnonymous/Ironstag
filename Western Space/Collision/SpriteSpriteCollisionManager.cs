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
                    objCollisionGrid[x, y] = new GameObjectBin(this);
                }
            }
            base.Initialize();
        }
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            registeredObject.Add(collideableObject);
            collideableObject.IdNumber = IDNumberCount;
            IDNumberCount++;
        }
        public void removeObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            OnRemoveObjectFromBin(collideableObject, this.NewRectToCoord(collideableObject.Rectangle));
            this.objBinLookupTable.Remove(collideableObject.IdNumber);
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
            Point binCoord = new Point((int)vector.X / binDimension.X, (int)vector.Y / binDimension.Y);
            return binCoord;
        }
        public List<Point> NewRectToCoord(Rectangle rect)
        {
            
            List<Point> list = new List<Point>();
            Point leftTop = xformScreenCoordToBinCoord(new Vector2(rect.Left, rect.Top));
            Point rightBottom = xformScreenCoordToBinCoord(new Vector2(rect.Right, rect.Bottom));
            if (!leftTop.Equals(rightBottom))
            {
                for (int y = leftTop.Y; y < rightBottom.Y; y++)
                {
                    for (int x = leftTop.X; x < rightBottom.X; x++)
                    {
                        list.Add(new Point(x, y));
                    }
                }
            }
            else
            {
                list.Add(leftTop);
            }
            return list;
        }
        public override void Update(GameTime gameTime)
        {
            List<Point> newCoords;
            List<Point> oldCoords;
            // Update Collision Bins
            foreach (ISpriteCollideable gameObj in registeredObject)
            {
                //newCoords = this.rectToBinCoord(gameObj.Rectangle);
                newCoords = this.NewRectToCoord(gameObj.Rectangle);
                if (objBinLookupTable.TryGetValue(gameObj.IdNumber, out oldCoords))
                {
                   this.OnUpdateObjectInBin(gameObj, oldCoords, newCoords);
                }
                else
                {
                    this.OnAddObjectToBin(gameObj, newCoords);
                }
            }
            // Check Collision Bins (Collision Bins)
            // Collection was modifed, need to recycle bullets
            foreach (GameObjectBin gameObjBin in objBinsToCheck)
            {                
                for (int i = 0; i < gameObjBin.ListOfObjects.Count - 1; i++)
                {
                    for (int j = 1; j < gameObjBin.ListOfObjects.Count; j++)
                    {
                        if(BoundingBoxA(gameObjBin.ListOfObjects.ElementAt(i), gameObjBin.ListOfObjects.ElementAt(j)))
                        {
                            gameObjBin.ListOfObjects.ElementAt(i).OnSpriteCollision(gameObjBin.ListOfObjects.ElementAt(j));
                            gameObjBin.ListOfObjects.ElementAt(j).OnSpriteCollision(gameObjBin.ListOfObjects.ElementAt(i));
                            Debug.Print("Collision");
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
