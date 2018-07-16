using System;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.Utility;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// Interface definition for the sprite batch service
    /// </summary>
    public interface ISpriteBatchService
    {
        /// <summary>
        /// Specifies all the batches to begin their draw operation
        /// </summary>
        void Begin();

        /// <summary>
        /// Specifices all the batches to enter their draw operation
        /// </summary>
        void End();

        /// <summary>
        /// Gets a SpriteBatch based on the name that it is given
        /// </summary>
        /// <param name="batchName">The name of the batch that the component needs</param>
        /// <returns>The sprite batch that should be used to draw the component</returns>
        SpriteBatch GetSpriteBatch(string batchName);
    }
}
