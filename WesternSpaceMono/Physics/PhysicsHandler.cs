using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Physics
{
    /// <summary>
    /// This class handles the movement of objects through velocity and the acceleration due to gravity.
    /// </summary>
    public class PhysicsHandler
    {
        private World world;

        public World World
        {
            get { return world; }
        }

        /// <summary>
        /// The maximum value a velocity can reach.
        /// </summary>
        public float maxSpeed;

        public float MaxSpeed
        {
            get { return maxSpeed; }
        }

        /// <summary>
        /// Constructor for the Physics Handler.
        /// Takes no arguements, and so it uses the default min
        /// and max velocity.
        /// </summary>
        public PhysicsHandler(World world)
        {
            this.world = world;
            if (world.Map.TileWidth >= world.Map.TileHeight)
                this.maxSpeed = world.Map.TileWidth / 4;
            else
                this.maxSpeed = world.Map.TileHeight / 4;
        }

        /// <summary>
        /// Applies the physics vectors to a DrawableGameObject's velocity vector.
        /// </summary>
        /// <param name="velocity">The velocity vector to manipulate.</param>
        /// <returns>The updated velocity vector.</returns>
        public void ApplyPhysics(IPhysical physicalObject)
        {
            physicalObject.Velocity += (physicalObject.NetForce / physicalObject.Mass);

            // Prevent the physicalObject's speed from being greater than
            // the maxSpeed.
            if (physicalObject.Velocity.Length() > MaxSpeed)
            {
                physicalObject.Velocity /= physicalObject.Velocity.Length();
                physicalObject.Velocity *= MaxSpeed;
            }

            physicalObject.NetForce = Vector2.Zero;

            physicalObject.Position += physicalObject.Velocity;
           // Console.WriteLine("Handler Added VEL: " + physicalObject.Velocity);
        }
    }
}