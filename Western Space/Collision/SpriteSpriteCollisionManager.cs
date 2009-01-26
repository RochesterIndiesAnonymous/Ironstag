using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Collision
{
    public class SpriteSpriteCollisionManager : GameComponent
    {
        protected List<Object> objBinsToCheck;
        protected Dictionary<int, Object> objBinLookupTable;
        public SpriteSpriteCollisionManager(Game game)
            : base(game)
        {
            objBinsToCheck = new List<object>();
            objBinLookupTable = new Dictionary<int,object>();

        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
