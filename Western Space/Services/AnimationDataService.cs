using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Services
{
    /// <summary>
    /// The implementation of the Animation Data Service
    /// </summary>
    public class AnimationDataService : IAnimationDataService
    {
        /// <summary>
        /// The collection of animation data already created. This is used for cacheing to avoid 
        /// multiple AnimationData objects with the same information
        /// </summary>
        private IDictionary<string, AnimationData> data;

        #region IAnimationDataService Members

        /// <summary>
        /// The collection of animation data already created. This is used for cacheing to avoid 
        /// multiple AnimationData objects with the same information
        /// </summary>
        public IDictionary<string, AnimationData> AnimationData
        {
            get { return data; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationDataService()
        {
            data = new Dictionary<string, AnimationData>();
        }

        #region IAnimationDataService Members

        /// <summary>
        /// Gets the animation data object based on the xml file name given
        /// </summary>
        /// <param name="xmlName">The asset name for the xml file to get</param>
        /// <returns>An AnimationData object that has all the information for animating a specific sprite</returns>
        public AnimationData GetAnimationData(string xmlName)
        {
            if (!data.ContainsKey(xmlName))
            {
                data[xmlName] = new AnimationData(xmlName);
            }

            return data[xmlName];
        }

        #endregion
    }
}
