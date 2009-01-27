using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : GameComponent
    {

        // ObjectToCheck 
        protected GameObjectBin[,] objCollisionGrid;
        protected Point binGridDimension;
        protected List<GameObject> objListToCheck;
        protected List<GameObjectBin> objBinsToCheck;
        protected Dictionary<int, Object> objBinLookupTable;
        public SpriteSpriteCollisionManager(Game game)
            : base(game)
        {
            objCollisionGrid = new GameObjectBin[10, 10];
            objListToCheck = new List<GameObject>();
            objBinsToCheck = new List<GameObjectBin>();
            objBinLookupTable = new Dictionary<int,object>();
        }
        public override void Initialize()
        {
            for (int y = 0; y < binGridDimension.Y; y++)
            {
                for (int x = 0; x < binGridDimension.X; x++)
                {
                    objCollisionGrid[x, y] = new GameObjectBin();
                }
            }
            base.Initialize();
        }
        public void AddGameObject(GameObject gameObject)
        {
            objListToCheck.Add(gameObject);
        }
        public override void Update(GameTime gameTime)
        {
            // Update Collision Bins
            foreach (GameObject gameObj in objListToCheck)
            {

            }
            // Check Collision Bins
            foreach (Object gameObjBin in objBinsToCheck)
            {

            }
            base.Update(gameTime);
        }
    }
}
