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
        private IDictionary<string, AnimationData> data;

        #region IAnimationDataService Members

        public IDictionary<string, AnimationData> AnimationData
        {
            get { return data; }
        }

        #endregion

        public AnimationDataService()
        {
            data = new Dictionary<string, AnimationData>();
        }

        #region IAnimationDataService Members


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
