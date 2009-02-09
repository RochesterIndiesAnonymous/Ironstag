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
    // SpriteSpriteCollisionManager - is a grid based collision management system
    // You pretty much add a ISpriteCollideable interface to an object and 
    // register it with this manager and this takes care of the collision detection
    // between the objects. It currently supports Bounding Box Algorithm
    public class SpriteSpriteCollisionManager : DrawableGameComponent //GameComponent
    {        
        static Boolean debug = false;
        // IDNumberCounter gets incremented every time a new object is added
        protected int IDNumberCounter = 0;
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
        // List of Registered Objects to be removed
        protected List<ISpriteCollideable> registeredObjectRemoveList;
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
            this.registeredObject = new List<ISpriteCollideable>();
            this.objectBinsToCheck = new List<CollisionObjectBin>();
            this.objBinLookupTable = new Dictionary<int, List<Point>>();
            this.refSpriteBatch = spriteBatch.GetSpriteBatch("Camera Sensitive");
            this.binWidth = binWidth;
            this.binHeight = binHeight;
            this.gridWidth = 0;
            this.gridHeight = 0;
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
        // Adds an object to the registered object list 
        // (Adds a object to a bin on the next update because the 
        // camera may not have been created yet)
        public void addObjectToRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // Registers a Collideable Object           
            registeredObject.Add(collideableObject);
            // assigns a id number to object
            collideableObject.IdNumber = this.IDNumberCounter;           
            // increment the idnumberCounter
            IDNumberCounter++;            
            //Debug.Print("Object Added to Registered List: " + collideableObject.IdNumber);
        }
        // Removes an object from the registered object list
        // the object is removed immediately
        public void removeObjectFromRegisteredObjectList(ISpriteCollideable collideableObject)
        {
            // Removes the object from the registered object list
            registeredObject.Remove(collideableObject);
            // Remove Object from the object bin lookup table 
            objBinLookupTable.Remove(collideableObject.IdNumber);
            

            // Removes object directly from the object bin
            OnRemoveObjectFromBin(collideableObject, this.getObjectCollisionBinCoord(collideableObject));                        
         
            // Debug.Print("Object Removed from Registered List: " + collideableObject.IdNumber);
        }
        // Finds out if the object is in camera space
        protected Boolean findOutIfObjectIsInCameraSpace(ISpriteCollideable collideableObject)
        {
            Rectangle objectRectangle = collideableObject.Rectangle;
            if ((objectRectangle.Left >= camera.VisibleArea.Left) &&
                (objectRectangle.Right < camera.VisibleArea.Right) &&
                (objectRectangle.Top >= camera.VisibleArea.Top) &&
                (objectRectangle.Bottom < camera.VisibleArea.Bottom))
                return true;
            return false;
        }
        // Finds out if point is in camera space
        protected Boolean findOutIfPointIsInCameraSpace(float x, float y)
        {
            if (x >= camera.VisibleArea.Left && x < camera.VisibleArea.Right &&
                y >= camera.VisibleArea.Top && y < camera.VisibleArea.Bottom)
                return true;
            return false;
        }
        // Adds object to the coresponding object bins
        protected void OnAddObjectToBin(ISpriteCollideable collideableObject, List<Point> listOfObjectBinCoord)
        {
            // loop though the coordinates add the object to the
            // coresponding grid space
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to Object Collision Grid
                if (binCoord.X >= 0 && binCoord.X < gridWidth && binCoord.Y >= 0 && binCoord.Y < gridHeight)
                    this.objectCollisionGrid[binCoord.X, binCoord.Y].OnObjectAdded(collideableObject);
            }
            // Update Look Up Table               
            this.objBinLookupTable[collideableObject.IdNumber] = listOfObjectBinCoord;
        }
        // Removes object from the corresponding object bins
        protected void OnRemoveObjectFromBin(ISpriteCollideable collideableObject, List<Point> listOfObjectBinCoord)
        {
            // loop though the coordinates remove the object to the
            // coresponding grid space
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Check if the coordinate is within the grid space
                // Remove Object from the ObjectList of a Bin
                if (binCoord.X >= 0 && binCoord.X < gridWidth && binCoord.Y >= 0 && binCoord.Y < gridHeight)
                    this.objectCollisionGrid[binCoord.X, binCoord.Y].OnObjectRemoved(collideableObject);
            }
            // Update Object Look Up Table               
            this.objBinLookupTable[collideableObject.IdNumber] = listOfObjectBinCoord;            
        }
        // Transform World Coordinates to Bin Coordinates
        protected Point WorldCoordToBinCoord(Vector2 vector)
        {
            // CameraSpace
            float x = (vector.X - this.camera.VisibleArea.X);
            float y = (vector.Y - this.camera.VisibleArea.Y);
            // GridSpace
            return new Point((int)x / binWidth, (int)y / binHeight);                       
        }     
        // Gets the Objects World Coordinates/Rectangle and transforms them into Bin Coordinates
        // Works by taking the topleft and the bottom right and interpolates to get all the
        // coordinates in between.
        protected List<Point> getObjectCollisionBinCoord(ISpriteCollideable collideableObject)
        {
            List<Point> listOfBinCoord = new List<Point>();
            Point leftTop = WorldCoordToBinCoord(
                new Vector2(collideableObject.Rectangle.Left, collideableObject.Rectangle.Top));
            Point rightBottom = WorldCoordToBinCoord(
                new Vector2(collideableObject.Rectangle.Right, collideableObject.Rectangle.Bottom));
            // If the left top and right bottom are not equal
            // scan through the points
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
            // if the left top and bottom right are equal add just the left top
            else
            {                
                listOfBinCoord.Add(leftTop);
            }
            return listOfBinCoord;
        }
        // Checks if 2 Lists of Coordinates are equal
        protected Boolean CoordsEqual(List<Point> pointA, List<Point> pointB)
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
            // This is the list of register game objects
            foreach (ISpriteCollideable gameObj in registeredObject)
            {
                // We get the new position of the game object
                newCoords = this.getObjectCollisionBinCoord(gameObj);
                // Lookup table contains the last coordnates of each game object
                if (objBinLookupTable.TryGetValue(gameObj.IdNumber, out oldCoords))
                {
                    // We find out if the object is in camera space
                    // if it is not we remove it from the object bin 
                    if (findOutIfObjectIsInCameraSpace(gameObj) == false)
                    {
                        OnRemoveObjectFromBin(gameObj, oldCoords);
                    }
                    // if it is not we add it to the object bins
                    else
                    {
                        // We only update the bins that the coordnates have changed
                        if (!CoordsEqual(oldCoords, newCoords))
                        {
                            //Debug.Print(gameObj.IdNumber + " Occupies ");
                            //foreach (Point coord in newCoords)
                            //{
                            //    Debug.Print(">" + coord.ToString());
                            //}
                            // Remove object from the bin
                            OnRemoveObjectFromBin(gameObj, oldCoords);
                            // Add object to the bin
                            OnAddObjectToBin(gameObj, newCoords);
                        }
                    }
                }
                // if the object is not in the lookup table we add it to 
                // the lookup table (this is for newly registered objects)
                // we do this here instead of the registeredfunction because this 
                // function is run after the camera is created
                else
                {
                    // first we find out if the object is in the
                    // cameraspace
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
                // We take two indexers Start i at 0 and end at n-1
                // and the second we start at j at 1 and at n
                // We do this to ensure that the none of the objects are checked against 
                // it self reducing the number of caluclations
                // A, B, C, D
                // This way (A,B),(A,C),(A,D),(B,C),(B,D),(C,D) = 6 calculations vs 16 
                // with a index that starts at zero
                for (int i = 0; i < gameObjBin.ListOfCollideableObjects.Count - 1; i++)
                {
                    for (int j = 1; j < gameObjBin.ListOfCollideableObjects.Count; j++)
                    {
                        if (BoundingBox(gameObjBin.ListOfCollideableObjects.ElementAt(i), gameObjBin.ListOfCollideableObjects.ElementAt(j)))                        
                        {
                            // Copys the collideable object if one of them gets deleted before the other
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
