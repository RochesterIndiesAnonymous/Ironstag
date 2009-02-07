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
using WesternSpace.Utility;
using WesternSpace.ServiceInterfaces;
/*
 * Note: I forgot to make sure these calculations the sprite position and stuff need to happen 
 * in screen space
 */

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : DrawableGameComponent //GameComponent
    {
        static int IDNumberCount = 0;
        static Boolean debug = false;
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
        SpriteBatch refSpriteBatch;
        ICameraService camera;
        public SpriteSpriteCollisionManager(Game game, ISpriteBatchService spriteBatch, Point binWH)
            : base(game)
        {
            registeredObject = new List<ISpriteCollideable>();
            objectBinsToCheck = new List<CollisionObjectBin>();            
            objBinLookupTable = new Dictionary<int, List<Point>>();
            refSpriteBatch = spriteBatch.GetSpriteBatch("Camera Sensitive");                   
            binDimension = binWH;
        }
        public override void Initialize()
        {
            camera = (ICameraService)ScreenManager.Instance.Services.GetService(typeof(ICameraService));
            numOfBins.X = (int)camera.VisibleArea.Width / binDimension.X;
            numOfBins.Y = (int)camera.VisibleArea.Height / binDimension.Y;
            objectCollisionGrid = new CollisionObjectBin[numOfBins.X, numOfBins.Y];
            for (int y = 0; y < numOfBins.Y; y++)
            {
                for (int x = 0; x < numOfBins.X; x++)
                {
                    objectCollisionGrid[x, y] = new CollisionObjectBin(this, x, y);
                }
            }
            base.Initialize();
        }
        public Boolean findOutIfObjectIsInCameraSpace(ISpriteCollideable collideableObject)
        {
            if (findOutIfPointIsInCameraSpace(new Vector2(
                collideableObject.Rectangle.Left,
                collideableObject.Rectangle.Top)) &&
            findOutIfPointIsInCameraSpace(new Vector2(
                collideableObject.Rectangle.Right,
                collideableObject.Rectangle.Bottom)))
            {
                return true;
            }
            return false;
        }
        public Boolean findOutIfPointIsInCameraSpace(Vector2 vector)
        {
            if (vector.X >= camera.VisibleArea.Left && vector.X < camera.VisibleArea.Right &&
                vector.Y >= camera.VisibleArea.Top  && vector.Y < camera.VisibleArea.Bottom)
                return true;
            return false;
        }
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // add Object to Bins
            registeredObject.Add(collideableObject);
            // assigns a id number to object
            collideableObject.IdNumber = IDNumberCount;
            //Debug.Print("Object Added to Registered List: " + collideableObject.IdNumber);
            // increments the idnumberCounter
            IDNumberCount++;
        }
        public void removeObjectFromRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // 
            this.objBinLookupTable.Remove(collideableObject.IdNumber);

            OnRemoveObjectFromBin(collideableObject, this.getObjectCollisionBinCoord(collideableObject));
            
            //Debug.Print("Object Removed from Registered List: " + collideableObject.IdNumber);
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
                // Remove Object to ObjectList of a Bin
                if(binCoord.X >= 0 && binCoord.X < numOfBins.X && binCoord.Y >= 0 && binCoord.Y < numOfBins.Y)
                    this.objectCollisionGrid[binCoord.X, binCoord.Y].OnObjectRemoved(collideableObject);
            }
            //// Update Object Look Up Table               
            this.objBinLookupTable[collideableObject.IdNumber] = listOfObjectBinCoord;
            
        }
        public Point xformScreenCoordToBinCoord(Vector2 vector)
        {
            // CameraSpace
            float x = (vector.X - this.camera.VisibleArea.X);
            float y = (vector.Y - this.camera.VisibleArea.Y);
            // GridSpace
            return new Point((int)x / binDimension.X, (int)y / binDimension.Y);                       
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
                for (int y = leftTop.Y; y <= rightBottom.Y; y++)
                {
                    for (int x = leftTop.X; x <= rightBottom.X; x++)
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
                    if (findOutIfObjectIsInCameraSpace(gameObj) == false)
                    {
                        OnRemoveObjectFromBin(gameObj, oldCoords);
                    }
                    else
                    {
                        // Update Bin Only if the coords have changed
                        if (!CoordsEqual(oldCoords, newCoords))
                        {
                            //Debug.Print(gameObj.IdNumber + " Occupies ");
                            //foreach (Point coord in newCoords)
                            //{
                            //    Debug.Print(">" + coord.ToString());
                            //}
                            OnRemoveObjectFromBin(gameObj, oldCoords);
                            OnAddObjectToBin(gameObj, newCoords);                            
                        }
                    }                    
                }
                else
                {
                    if (findOutIfObjectIsInCameraSpace(gameObj))
                    {
                        //Debug.Print("Camera Pos:\n>" + camera.VisibleArea.ToString() +
                        //    "\n>CameraLRTB: {" + 
                        //    camera.VisibleArea.Left + "," + camera.VisibleArea.Right + "," +
                        //    camera.VisibleArea.Top + "," + camera.VisibleArea.Bottom + "}" +
                        //    "Sprite Pos: \n>" + gameObj.Rectangle.ToString()
                        //);
                        this.OnAddObjectToBin(gameObj, newCoords);
                    }
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
                        //if (BoundingBox(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j)))
                        if (PixelCollision(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j), gameTime))
                        {
                            ISpriteCollideable collidedObj1 = gameObjBin.ListOfCollideableObjects.ElementAt(i);
                            ISpriteCollideable collidedObj2 = gameObjBin.ListOfCollideableObjects.ElementAt(j);
                            collidedObj1.OnSpriteCollision(collidedObj2);
                            collidedObj2.OnSpriteCollision(collidedObj1);                            
                        }
                    }
                }             
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (debug)
            {
                ////Characters
                foreach (ISpriteCollideable collidableSprite in registeredObject)
                {
                    PrimitiveDrawer.Instance.DrawRect(refSpriteBatch, collidableSprite.Rectangle, Color.Blue);
                    PrimitiveDrawer.Instance.DrawLine(refSpriteBatch,
                        new Vector2(collidableSprite.Rectangle.X, collidableSprite.Rectangle.Y),
                        new Vector2(collidableSprite.Rectangle.X + 2, collidableSprite.Rectangle.Y), Color.White);
                    PrimitiveDrawer.Instance.DrawLine(refSpriteBatch,
                        new Vector2(collidableSprite.Rectangle.Right, collidableSprite.Rectangle.Y),
                        new Vector2(collidableSprite.Rectangle.Right + 2, collidableSprite.Rectangle.Y), Color.White);
                    PrimitiveDrawer.Instance.DrawLine(refSpriteBatch,
                        new Vector2(collidableSprite.Rectangle.X, collidableSprite.Rectangle.Bottom),
                        new Vector2(collidableSprite.Rectangle.X + 2, collidableSprite.Rectangle.Bottom), Color.White);
                }

                //Grids
                for (int y = 0; y < this.numOfBins.Y; y++)
                {
                    for (int x = 0; x < this.numOfBins.X; x++)
                    {
                        if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects == 0)
                            PrimitiveDrawer.Instance.DrawRect(refSpriteBatch,
                                new Rectangle((x * this.binDimension.X) + (int)this.camera.VisibleArea.X,
                                    (y * binDimension.Y) + (int)this.camera.VisibleArea.Y, this.binDimension.X, this.binDimension.Y), Color.Red);
                        else if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects == 1)
                            PrimitiveDrawer.Instance.DrawRect(refSpriteBatch,
                                new Rectangle((x * this.binDimension.X) + (int)this.camera.VisibleArea.X,
                                    (y * binDimension.Y) + (int)this.camera.VisibleArea.Y, this.binDimension.X, this.binDimension.Y), Color.Purple);
                        else if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects > 1)
                            PrimitiveDrawer.Instance.DrawSolidRect(refSpriteBatch,
                                new Rectangle((x * this.binDimension.X) + (int)this.camera.VisibleArea.X,
                                    (y * binDimension.Y) + (int)this.camera.VisibleArea.Y, this.binDimension.X, this.binDimension.Y), Color.Green);
                    }
                }
            }     
            base.Draw(gameTime);
        }
        Boolean BoundingBox(ISpriteCollideable collideableObjectA, ISpriteCollideable collideableObjectB)
        {
            return collideableObjectA.Rectangle.Intersects(collideableObjectB.Rectangle);
        }
        void GetTextureData(ISpriteCollideable spriteCollideable, out Color [] colorData)
        {
            Color[] newColorData = new Color[spriteCollideable.Rectangle.Width * spriteCollideable.Rectangle.Height];         
            if (spriteCollideable.collideableFacing == SpriteEffects.FlipHorizontally)            
                newColorData = spriteCollideable.CurrentAnimation.GetHorizontalTextureDataFlip();
            else if (spriteCollideable.collideableFacing == SpriteEffects.FlipVertically)
            {
                //Debug.Print("Vertical Flip function not complete");
            }
            else
                newColorData = spriteCollideable.CurrentAnimation.textureColorData;            
            colorData = newColorData;
        }
        Boolean PixelCollision(ISpriteCollideable collideableObjectA,
                               ISpriteCollideable collideableObjectB,
                               GameTime gameTime)
        {
            Color[] objectTextureDataA;
            Color[] objectTextureDataB;
            GetTextureData(collideableObjectA, out objectTextureDataA);
            GetTextureData(collideableObjectB, out objectTextureDataB);
            if (IntersectPixels(collideableObjectA.Rectangle, objectTextureDataA,
                collideableObjectB.Rectangle, objectTextureDataB))
            {
                return true;
            }
            return false;
        }
        bool IntersectPixels(Rectangle rectA, Color[] dataA,
                             Rectangle rectB, Color[] dataB)
        {
            int top = Math.Max(rectA.Top, rectB.Top);
            int bottom = Math.Min(rectA.Bottom, rectB.Bottom);
            int left = Math.Max(rectA.Left, rectB.Left);
            int right = Math.Min(rectA.Right, rectB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dataA[(x - rectA.Left) + (y - rectA.Top) * rectA.Width];
                    Color colorB = dataB[(x - rectB.Left) + (y - rectB.Top) * rectB.Width];
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
