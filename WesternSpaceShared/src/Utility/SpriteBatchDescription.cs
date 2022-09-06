using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.Utility
{
    /// <summary>
    /// Holds a description of sprite batch and its needed settings
    /// </summary>
    public class SpriteBatchDescription
    {
        /// <summary>
        /// The name of the sprite batch. Components use this to find the right sprite batch
        /// </summary>
        private String spriteBatchName;

        /// <summary>
        /// The name of the sprite batch. Components use this to find the right sprite batch
        /// </summary>
        public String SpriteBatchName
        {
            get { return spriteBatchName; }
            set { spriteBatchName = value; }
        }

        /// <summary>
        /// The blend mode for the associated sprite batch
        /// </summary>
        private BlendState desiredBlendMode;

        /// <summary>
        /// The blend mode for the associated sprite batch
        /// </summary>
        public BlendState DesiredBlendMode
        {
            get { return desiredBlendMode; }
            set { desiredBlendMode = value; }
        }

        /// <summary>
        /// The sort mode for the associated sprite batch
        /// </summary>
        private SpriteSortMode desiredSortMode;

        /// <summary>
        /// The sort mode for the associated sprite batch
        /// </summary>
        public SpriteSortMode DesiredSortMode
        {
            get { return desiredSortMode; }
            set { desiredSortMode = value; }
        }

        /// <summary>
        /// The video card save mode for the associated sprite batch
        /// </summary>
        private BlendState desiredStateMode;

        /// <summary>
        /// The video card save mode for the associated sprite batch
        /// </summary>
        public BlendState DesiredStateMode
        {
            get { return desiredStateMode; }
            set { desiredStateMode = value; }
        }

        /// <summary>
        /// Flag used to determine if the current matrix needs to be updated every update cycle
        /// </summary>
        private bool updateCameraMatrix;

        /// <summary>
        /// Flag used to determine if the current matrix needs to be updated every update cycle
        /// </summary>
        public bool UpdateCameraMatrix
        {
            get { return updateCameraMatrix; }
            set { updateCameraMatrix = value; }
        }

        /// <summary>
        /// The current view transformation matrix that is passed to SpriteBatch.Begin
        /// </summary>
        private Matrix currentMatrix;

        /// <summary>
        /// The current view transformation matrix that is passed to SpriteBatch.Begin
        /// </summary>
        public Matrix CurrentMatrix
        {
            get { return currentMatrix; }
            set { currentMatrix = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatchName">The string name used to access the sprite batch</param>
        /// <param name="desiredBlendMode">The blend mode to use for the sprite batch</param>
        /// <param name="desiredSortMode">The sort mode to use for the sprite batch</param>
        /// <param name="desiredSaveMode">The save mode to use for the sprite batch</param>
        /// <param name="updateCameraMatrix">The flag used to denote if the matrix needs to be updated every update cycle</param>
        public SpriteBatchDescription(string spriteBatchName, BlendState desiredBlendMode, 
            SpriteSortMode desiredSortMode, bool updateCameraMatrix)
        {
            this.spriteBatchName = spriteBatchName;
            this.desiredBlendMode = desiredBlendMode;
            this.desiredSortMode = desiredSortMode;
            //this.desiredStateMode = desiredSaveMode;
            this.updateCameraMatrix = updateCameraMatrix;
            this.currentMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Creates a custom hash code for dictionary look up
        /// </summary>
        /// <returns>A hash code suitable for this spritebatch</returns>
        public override int GetHashCode()
        {
            return spriteBatchName.GetHashCode() + desiredBlendMode.GetHashCode() + desiredSortMode.GetHashCode() 
                 + updateCameraMatrix.GetHashCode();
        }

        /// <summary>
        /// Checks to see if two descriptions are equal
        /// </summary>
        /// <param name="obj">The other object that this object is being compared to</param>
        /// <returns>True if both objects are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(SpriteBatchDescription))
            {
                SpriteBatchDescription otherSbd = (SpriteBatchDescription)obj;

                if (this.spriteBatchName == otherSbd.spriteBatchName &&
                    this.desiredBlendMode == otherSbd.desiredBlendMode &&
                    this.desiredSortMode == otherSbd.desiredSortMode &&
                    this.updateCameraMatrix == otherSbd.updateCameraMatrix)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
