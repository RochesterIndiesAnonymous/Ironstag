using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.Utility;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework;

namespace WesternSpace.Services
{
    /// <summary>
    /// Implementation of the sprite batch service
    /// </summary>
    public class SpriteBatchService : GameObject, ISpriteBatchService
    {
        /// <summary>
        /// The camera that is used to update the sprite batch begin calls
        /// </summary>
        private ICameraService camera;
        
        /// <summary>
        /// An association between all batches and their descriptions
        /// </summary>
        private IDictionary<SpriteBatch, SpriteBatchDescription> batches;

        /// <summary>
        /// An association between all descriptions and their batches
        /// </summary>
        private IDictionary<SpriteBatchDescription, SpriteBatch> reverseBatches;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game that this component is associated with</param>
        public SpriteBatchService(Game game)
            : base(game)
        {
            batches = new Dictionary<SpriteBatch, SpriteBatchDescription>();
            reverseBatches = new Dictionary<SpriteBatchDescription, SpriteBatch>();
        }

        /// <summary>
        /// Creates all the needed batches in the game
        /// </summary>
        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            SpriteBatchDescription cameraSensitive = new SpriteBatchDescription("Camera Sensitive", SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                SaveStateMode.None, true);

            SpriteBatch cameraSensitiveBatch = new SpriteBatch(this.Game.GraphicsDevice);
            batches[cameraSensitiveBatch] = cameraSensitive;
            reverseBatches[cameraSensitive] = cameraSensitiveBatch;

            SpriteBatchDescription staticBatchDescription = new SpriteBatchDescription("Static", SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
                SaveStateMode.None, false);

            SpriteBatch staticBatch = new SpriteBatch(this.Game.GraphicsDevice);
            batches[staticBatch] = staticBatchDescription;
            reverseBatches[staticBatchDescription] = staticBatch;

            base.Initialize();
        }

        /// <summary>
        /// Updates the batches view matrix as needed
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            foreach (SpriteBatch batch in batches.Keys)
            {
                if (batches[batch].UpdateCameraMatrix)
                {
                    batches[batch].CurrentMatrix = camera.CurrentViewMatrix;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Specifies all the batches to begin their draw operation
        /// </summary>
        public SpriteBatch GetSpriteBatch(string batchName)
        {
            SpriteBatchDescription batch = (from singleBatch in batches.Values
                                 where singleBatch.SpriteBatchName == batchName
                                 select singleBatch).FirstOrDefault();

            if (batch != null)
            {
                return reverseBatches[batch];
            }

            return null;
        }

        /// <summary>
        /// Specifices all the batches to enter their draw operation
        /// </summary>
        public void Begin()
        {
            foreach (SpriteBatch batch in batches.Keys)
            {
                SpriteBatchDescription desc = batches[batch];
                batch.Begin(desc.DesiredBlendMode, desc.DesiredSortMode, desc.DesiredStateMode, desc.CurrentMatrix);
            }
        }

        /// <summary>
        /// Gets a SpriteBatch based on the name that it is given
        /// </summary>
        /// <param name="batchName">The name of the batch that the component needs</param>
        /// <returns>The sprite batch that should be used to draw the component</returns>
        public void End()
        {
            foreach (SpriteBatch batch in batches.Keys)
            {
                batch.End();
            }
        }
    }
}
