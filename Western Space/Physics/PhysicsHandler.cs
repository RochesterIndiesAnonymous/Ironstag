﻿using System;
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
    class PhysicsHandler
    {
        const float MAX_X_VELOCITY = 10f;
        const float MAX_Y_VELOCITY = 5f;
        const float MIN_Y_VELOCITY = -5f;
        const float MIN_X_VELOCITY = -10f;

        /// <summary>
        /// Vector representing the Acceleration due to gravity.
        /// </summary>
        readonly Vector2 gravity = new Vector2(0f, 0.2f);

        /// <summary>
        /// Vector representing the Velocity of moving on the ground.
        /// </summary>
        readonly Vector2 groundVelocity = new Vector2(3f, 0f);

        /// <summary>
        /// Vector representing the Velocity of moving in the air.
        ///</summary>
        readonly Vector2 airVelocity = new Vector2(2f, 0f);

        readonly Vector2 jumpVelocity = new Vector2(0f, 10f);

        /// <summary>
        /// Vector representing velocity which is added to the player's
        /// position.
        /// </summary>
        public Vector2 modifyVelocity;

        public Vector2 ModifyVelocity
        {
            get { return modifyVelocity; }
            set { modifyVelocity = value; }
        }

        /// <summary>
        /// Constructor for the Physics Handler.
        /// </summary>
        public PhysicsHandler()
        {
            modifyVelocity = new Vector2(0f, 0f);
        }

        /// <summary>
        /// Applies the physics vectors to a DrawableGameObject's velocity vector.
        /// </summary>
        /// <param name="velocity">The velocity vector to manipulate.</param>
        /// <returns>The updated velocity vector.</returns>
        public Vector2 ApplyPhysics(Vector2 velocity)
        {
            //Apply the force of Gravity
            velocity.Y = MathHelper.Clamp(velocity.Y + gravity.Y, MIN_Y_VELOCITY, MAX_Y_VELOCITY);

            //Apply modified Velocity
            velocity.X = MathHelper.Clamp(modifyVelocity.X, MIN_X_VELOCITY, MAX_X_VELOCITY);
            velocity.Y = MathHelper.Clamp(velocity.Y + modifyVelocity.Y, MIN_Y_VELOCITY, MAX_Y_VELOCITY);

            ResetVelocity();

            return velocity;

        }

        /// <summary>
        /// Applies the set ground velocity to a velocity vector.
        /// </summary>
        /// <param name="direction">The direction the object is facing. Right = 1, Left = -1</param>
        public void ApplyGroundMove(int direction)
        {
            modifyVelocity.X = (direction * groundVelocity.X);
        }

        /// <summary>
        /// Applies the set air velocity to a velocity vector.
        /// </summary>
        /// <param name="direction">The direction the object is facing. Right = 1, Left = -1</param>
        public void ApplyAirMove(int direction)
        {
            modifyVelocity.X = (direction * airVelocity.X);
        }

        /// <summary>
        /// Applies the set jump velocity to a velocity vector.
        /// </summary>
        public void ApplyJump()
        {
            modifyVelocity.Y -= jumpVelocity.Y;
        }

        /// <summary>
        /// Rests the velocity vector to 0.
        /// </summary>
        public void ResetVelocity()
        {
            modifyVelocity = Vector2.Zero;
        }
    }
}