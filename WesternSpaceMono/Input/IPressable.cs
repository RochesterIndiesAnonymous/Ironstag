using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WesternSpace.Input
{
    /// <summary>
    /// A generalized "button", if you will.
    /// It keeps track of one of whether or not it was *just*
    /// pressed (and released) and how long it's been pressed (or released)
    /// </summary>
    public interface IPressable
    {
        /// <summary>
        /// Returns true if the pressable is currently pressed.
        /// </summary>
        bool IsPressed
        {
            get;
        }

        /// <summary>
        /// Returns true if the pressable was not pressed the previous update, but
        /// is for the most recent update.
        /// </summary>
        bool WasJustPressed
        {
            get;
        }

        /// <summary>
        /// Returns true if the pressable is currently released.
        /// </summary>
        bool IsReleased
        {
            get;
        }

        /// <summary>
        /// Returns true if the pressable was pressed the previous update, but 
        /// is not for the most recent update.
        /// </summary>
        bool WasJustReleased
        {
            get;
        }

        /// <summary>
        /// The amount of time this trigger has been pressed for.
        /// Zero if not pressed.
        /// </summary>
        int PressedTime
        {
            get;
        }

        /// <summary>
        /// The amount of time this trigger has been Released for.
        /// Zero if pressed.
        /// </summary>
        int ReleasedTime
        {
            get;
        }

        /// <summary>
        /// A method to be called once every update cycle in the game.
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
    }
}
