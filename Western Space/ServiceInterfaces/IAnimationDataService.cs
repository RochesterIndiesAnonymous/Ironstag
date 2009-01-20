using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// Provides an interface to the Animation Data Service
    /// </summary>
    public interface IAnimationDataService
    {
        /// <summary>
        /// The collection of animation data already created. This is used for cacheing to avoid 
        /// multiple AnimationData objects with the same information
        /// </summary>
        IDictionary<string, AnimationData> AnimationData
        {
            get;
        }

        /// <summary>
        /// Gets the animation data object based on the xml file name given
        /// </summary>
        /// <param name="xmlName">The asset name for the xml file to get</param>
        /// <returns>An AnimationData object that has all the information for animating a specific sprite</returns>
        AnimationData GetAnimationData(string xmlName);
    }
}
