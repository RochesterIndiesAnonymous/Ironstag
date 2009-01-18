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
        IDictionary<Texture2D, AnimationData> AnimationData
        {
            get;
        }

        AnimationData GetAnimationData(Texture2D textureName, string xmlName);
    }
}
