using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : GameComponent
    {

        // ObjectToCheck 
        protected GameObjectBin[,] objCollisionGrid;
        protected Point binDimension;
        protected Point numOfBins;
        protected List<ISpriteCollideable> objListToCheck;
        protected List<GameObjectBin> objBinsToCheck;
        protected Dictionary<Character, List<Point>> objBinLookupTable;
        public SpriteSpriteCollisionManager(Game game, Point binWH)
            : base(game)
        {
            
            objListToCheck = new List<ISpriteCollideable>();
            objBinsToCheck = new List<GameObjectBin>();
            objBinLookupTable = new Dictionary<Character, List<Point>>();
            binDimension = binWH;
            int binRows = game.GraphicsDevice.Viewport.Width / binWH.X;
            int binCols = game.GraphicsDevice.Viewport.Height / binWH.Y;
            objCollisionGrid = new GameObjectBin[binRows, binCols];
        }
        public override void Initialize()
        {
            for (int y = 0; y < numOfBins.Y; y++)
            {
                for (int x = 0; x < numOfBins.X; x++)
                {
                    objCollisionGrid[x, y] = new GameObjectBin();
                }
            }
           
            base.Initialize();
        }
        public void AddGameObject(ISpriteCollideable gameObject)
        {
            objListToCheck.Add(gameObject);
        }
        public void RemoveGameObject(ISpriteCollideable gameObject)
        {
            objListToCheck.Remove(gameObject);
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
            foreach (Character gameObj in objListToCheck)
            {
                newCoords = this.rectToBinCoord(gameObj.Rectangle);
                //if (objBinLookupTable.TryGetValue(gameObj, oldCoords))
                //{
                    
                //}
                //else
                //{
                //    this.AddGameObject(gameObj);
                //}
            }
            // Check Collision Bins
            foreach (Object gameObjBin in objBinsToCheck)
            {

            }
            base.Update(gameTime);
        }
    }
}
