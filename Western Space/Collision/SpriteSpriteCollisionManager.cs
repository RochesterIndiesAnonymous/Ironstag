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

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : DrawableGameComponent //GameComponent
    {
        protected int IDNumberCounter = 0;
        static Boolean debug = false;
        // Object Collision Grid 
        protected CollisionObjectBin[,] objectCollisionGrid;
        // Number Of Bins
        protected int gridWidth;
        protected int gridHeight;
        // Bin Dimensions      
        protected int binWidth;
        protected int binHeight;        
        // Registered Object List
        protected List<ISpriteCollideable> registeredObject;
        // Object Bins To Check (This is added to in Grid Bins when a bin contains multiple sprites)
        protected List<CollisionObjectBin> objectBinsToCheck;
        public List<CollisionObjectBin> ObjectBinsToCheck
        {
            get { return objectBinsToCheck; }
        }
        // Dictionary of points that an object occupies
        // int = object id number
        // List<Point> = the object points that it occupies
        protected Dictionary<int, List<Point>> objBinLookupTable;
        // Reference to a spritebatch
        protected SpriteBatch refSpriteBatch;
        // Reference to the camera class
        protected ICameraService camera;
        public SpriteSpriteCollisionManager(Game game, ISpriteBatchService spriteBatch, int binWidth, int binHeight)
            : base(game)
        {
            registeredObject = new List<ISpriteCollideable>();
            objectBinsToCheck = new List<CollisionObjectBin>();            
            objBinLookupTable = new Dictionary<int, List<Point>>();
            refSpriteBatch = spriteBatch.GetSpriteBatch("Camera Sensitive");
            this.binWidth = binWidth;
            this.binHeight = binHeight;
        }
        public override void Initialize()
        {
            camera = (ICameraService)ScreenManager.Instance.Services.GetService(typeof(ICameraService));           
            gridWidth = (int)camera.VisibleArea.Width / binWidth;
            gridHeight = (int)camera.VisibleArea.Height / binHeight;
            objectCollisionGrid = new CollisionObjectBin[gridWidth, gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    objectCollisionGrid[x, y] = new CollisionObjectBin(this, x, y);
                }
            }
            base.Initialize();
        }
        public Boolean findOutIfObjectIsInCameraSpace(ISpriteCollideable collideableObject)
        {
            if (findOutIfPointIsInCameraSpace(collideableObject.Rectangle.Left, collideableObject.Rectangle.Top) &&
                findOutIfPointIsInCameraSpace(collideableObject.Rectangle.Right, collideableObject.Rectangle.Bottom))            
                return true;            
            return false;
        }
        public Boolean findOutIfPointIsInCameraSpace(float x, float y)
        {
            if (x >= camera.VisibleArea.Left && x < camera.VisibleArea.Right &&
                y >= camera.VisibleArea.Top && y < camera.VisibleArea.Bottom)
                return true;
            return false;
        }
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // add Object to Bins
            registeredObject.Add(collideableObject);
            // assigns a id number to object
            collideableObject.IdNumber = this.IDNumberCounter;
            //Debug.Print("Object Added to Registered List: " + collideableObject.IdNumber);
            // increments the idnumberCounter 
            IDNumberCounter++;
        }
        public void removeObjectFromRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // 
            this.objBinLookupTable.Remove(collideableObject.IdNumber);
            OnRemoveObjectFromBin(collideableObject, this.getObjectCollisionBinCoord(collideableObject));            
            // Debug.Print("Object Removed from Registered List: " + collideableObject.IdNumber);
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
                if (binCoord.X >= 0 && binCoord.X < gridWidth && binCoord.Y >= 0 && binCoord.Y < gridHeight)
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
            return new Point((int)x / binWidth, (int)y / binHeight);                       
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
            // Make a copy of the bins to check so when a bin is removed the loop doesnt freak out 
            IEnumerable<CollisionObjectBin> objBinsToCheckCopy = objectBinsToCheck.ToList();
            // Scan all bins on the Object List to be Checked
            foreach (CollisionObjectBin gameObjBin in objBinsToCheckCopy)
            {                
                for (int i = 0; i < gameObjBin.ListOfCollideableObjects.Count - 1; i++)
                {
                    for (int j = 1; j < gameObjBin.ListOfCollideableObjects.Count; j++)
                    {
                        if (BoundingBox(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j)))
                        //if (PixelCollision(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j), gameTime))
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
                // Draw a Rectangle around the characters
                foreach (ISpriteCollideable collidableSprite in registeredObject)
                {
                    PrimitiveDrawer.Instance.DrawRect(refSpriteBatch, collidableSprite.Rectangle, Color.Blue);               
                }

                //Grids
                int positionX = 0;
                int positionY = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)                    
                    {
                        if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects == 0)
                        {
                            PrimitiveDrawer.Instance.DrawRect(refSpriteBatch,
                                new Rectangle(positionX + (int)this.camera.VisibleArea.X,
                                              positionY + (int)this.camera.VisibleArea.Y,
                                              this.binWidth, this.binHeight), Color.Red);
                        }
                        else if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects == 1)
                        {
                            PrimitiveDrawer.Instance.DrawRect(refSpriteBatch,
                                new Rectangle(positionX + (int)this.camera.VisibleArea.X,
                                              positionY + (int)this.camera.VisibleArea.Y, 
                                              this.binWidth, this.binHeight), Color.Purple);
                        }
                        else if (this.objectCollisionGrid[x, y].NumberOfCollideableObjects > 1)
                        {
                            PrimitiveDrawer.Instance.DrawSolidRect(refSpriteBatch,
                                new Rectangle(positionX + (int)this.camera.VisibleArea.X,
                                              positionY + (int)this.camera.VisibleArea.Y,
                                              this.binWidth, this.binHeight), Color.Green);
                        }
                        positionX += binWidth;
                    }
                    positionX = 0;
                    positionY += binHeight;
                }               
            }     
            base.Draw(gameTime);
        }
        Boolean BoundingBox(ISpriteCollideable collideableObjectA, ISpriteCollideable collideableObjectB)
        {
            return collideableObjectA.Rectangle.Intersects(collideableObjectB.Rectangle);
        }

        /*
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

         * */
    }
}
