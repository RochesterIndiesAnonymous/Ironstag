using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Services
{
    public class AnimationDataService : IAnimationDataService
    {
        private IDictionary<Texture2D, AnimationData> data;

        #region IAnimationDataService Members

        public IDictionary<Texture2D, AnimationData> AnimationData
        {
            get { return data; }
        }

        #endregion

        public AnimationDataService()
        {
            data = new Dictionary<Texture2D, AnimationData>();
        }

        #region IAnimationDataService Members


        public AnimationData GetAnimationData(Texture2D spriteSheet, string xmlName)
        {
            if (!data.ContainsKey(spriteSheet))
            {
                data[spriteSheet] = new AnimationData(spriteSheet, xmlName);
            }

            return data[spriteSheet];
        }

        #endregion
    }
}
