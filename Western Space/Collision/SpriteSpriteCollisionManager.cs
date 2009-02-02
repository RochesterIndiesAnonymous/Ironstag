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
    public class SpriteSpriteCollisionManager : GameComponent, IDebugOutput
    {

        // Object Collision Grid 
        protected GameObjectBin[,] objCollisionGrid;
        // Bin Dimensions
        protected Point binDimension;
        // Number Of Bins
        protected Point numOfBins;
        // Registered Object List
        protected List<ISpriteCollideable> registeredObject;
        public List<ISpriteCollideable> RegisteredObjectList
        {
            get { return registeredObject; }
        }
        // Object Bins To Cheack
        protected List<GameObjectBin> objBinsToCheck;
        public List<GameObjectBin> ObjBinsToCheck
        {
            get { return objBinsToCheck; }
        }
        protected Dictionary<int, List<Point>> objBinLookupTable;
        //
        protected String DebugOutput;
        //
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
        // Bin Opperations
        protected void OnAddObjectToBin(Character gameObject, List<Point> listOfObjectBinCoord)
        {
            foreach (Point binCoord in listOfObjectBinCoord)
            {
                // Add Object to Object Collision Grid
                this.objCollisionGrid[binCoord.X, binCoord.Y].OnObjectAdded(gameObject);
                // Update Look Up Table               
                this.objBinLookupTable[gameObject.IdNumber] = listOfObjectBinCoord;
            }
        }
        protected void OnUpdateObjectInBin(Character gameObject, List<Point> oldObjCoord, List<Point> newObjCoord)
        {
            OnRemoveObjectFromBin(gameObject, oldObjCoord);
            OnAddObjectToBin(gameObject, newObjCoord);
        }
        protected void OnRemoveObjectFromBin(Character gameObject, List<Point> listOfObjectBinCoord)
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
        // Rectangle To Bin Coords - takes rectangle returns bin coordinates
        public List<Point> rectToBinCoord(Rectangle rect)
        {
            Point[] pArray = new Point[4];
            List<Point> list = new List<Point>();
            pArray[0] = xformScreenCoordToBinCoord(new Vector2(rect.Left, rect.Top));
            pArray[1] = xformScreenCoordToBinCoord(new Vector2(rect.Left, rect.Bottom));
            pArray[2] = xformScreenCoordToBinCoord(new Vector2(rect.Right, rect.Top));
            pArray[3] = xformScreenCoordToBinCoord(new Vector2(rect.Right, rect.Bottom));
            foreach (Point binCoord in pArray)
            {
                if ((binCoord.X >= 0 && binCoord.X < numOfBins.X) && binCoord.Y < numOfBins.Y)
                {
                    if (!list.Exists(element => element == binCoord))
                        list.Add(binCoord);
                }
            }
            return list;
        }
       
        public override void Update(GameTime gameTime)
        {
            List<Point> newCoords;
            List<Point> oldCoords;
            // Update Collision Bins
            DebugOutput = "Grid Size: " + this.numOfBins.ToString();
            
            foreach (Character gameObj in registeredObject)
            {
                newCoords = this.rectToBinCoord(gameObj.Rectangle);
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
            foreach (GameObjectBin gameObjBin in objBinsToCheck)
            {                
                ICollection<List<Character>> p = gameObjBin.LookUpTableOfObjects.Values;
                // Start at the first value end at the last - 1
                for (int i = 0; i < p.Count - 1; i++)
                {
                    // Start at the second value and ends at the last
                    for (int j = 1; j < p.Count; j++)
                    {
                        BoundingBox(p.ElementAt(i), p.ElementAt(j), gameTime);
                    }
                }                        
               // Debug.Print("Checking Bins");
                //BoundingBox(gameObjBin.LookUpTableOfObjects["Flint Ironstag"],
                  //  gameObjBin.LookUpTableOfObjects["Toad Man"], gameTime);
            }
            base.Update(gameTime);
        }
        Boolean BoundingBox(List<Character> entityListA, List<Character> entityListB, GameTime gameTime)
        {
            foreach (Character entityA in entityListA)
            {
                Rectangle rectA = entityA.Rectangle;
                foreach (Character entityB in entityListB)
                {
                    Rectangle rectB = entityB.Rectangle;
                    if (rectA.Intersects(rectB))
                    {
                        DebugOutput += "\nCollision At: ";
                        entityA.OnSpriteCollision(entityB);
                        entityB.OnSpriteCollision(entityA);
                    }
                }
            }
            return false;
        }
        Boolean PixelCollision(List<Character> entityListA, List<Character> entityListB, GameTime gameTime)
        {
            foreach (Character entityA in entityListA)
            {               
                //Rectangle rectA = new Rectangle(entityA.CurrentAnimation.FrameWidth * entityA.AnimationPlayer.CurrentFrame.FrameIndex,
                //        entityA.CurrentAnimation.FrameHeight * entityA.AnimationPlayer.CurrentFrame.FrameIndex,
                //        entityA.CurrentAnimation.FrameWidth, entityA.CurrentAnimation.FrameHeight);
                //Color[] entityATextureData = new Color[entityA.CurrentAnimation.FrameWidth * entityA.CurrentAnimation.FrameHeight];
                //entityA.CurrentAnimation.SpriteSheet.GetData(0, rectA, entityATextureData,
                //        0, entityA.CurrentAnimation.SpriteSheet.Width * entityA.CurrentAnimation.SpriteSheet.Height);
                ////Color[] entityBTextureData;
                //foreach (Character entityB in entityListB)
                //{
                //    entityBTextureData = new Color[entityB.Sprite.Width * entityB.Sprite.Height];
                //    entityB.Sprite.GetData(entityBTextureData);
                //    if (IntersectPixels(entityA.Rectangle, entityATextureData, entityB.Rectangle, entityBTextureData))
                //    {
                //        entityA.SpriteSpriteCollision();
                //        entityB.SpriteSpriteCollision();
                //        //CollisionEvent(this, new CollisionEventArgs(entityA, entityB, gameTime));
                //    }
                //}
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
        #region IDebugOutput Members

        public string Output
        {
            get { return DebugOutput; }
        }

        #endregion
    }
}
