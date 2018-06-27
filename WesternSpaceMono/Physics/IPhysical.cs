using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace WesternSpace.Physics
{
    public interface IPhysical
    {
        PhysicsHandler PhysicsHandler
        {
            get;
        }

        Vector2 Position
        {
            get;
            set;
        }

        Vector2 Velocity
        {
            get;
            set;
        }

        Vector2 NetForce
        {
            get;
            set;
        }

        float Mass
        {
            get;
            set;
        }
    }
}
