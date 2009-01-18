using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.ServiceInterfaces
{
    public interface IAnimationDataService
    {
        IDictionary<string, AnimationData> AnimationData
        {
            get;
        }

        AnimationData GetAnimationData(string xmlName);
    }
}
