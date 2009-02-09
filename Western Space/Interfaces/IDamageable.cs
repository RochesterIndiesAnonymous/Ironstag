using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Utility;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// Provides information is to provide health, mitigation, and methods to take damage
    /// 
    /// All items in the world that take damage from an IDamaging character or object
    /// need to implement this interface.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// The maximum health that this character has
        /// </summary>
        float MaxHealth { get; }
        
        /// <summary>
        /// The current amount of health this character has left
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// The percentage of damage this character takes when it is hit.
        /// e.g. 0.85 to take 85% of the damage received.
        /// </summary>
        float MitigationFactor { get; }

        /// <summary>
        /// The type of damage this character takes damage from.
        /// If this is the player, set to Enemy. If this is an enemy
        /// set to Player
        /// </summary>
        DamageCategory TakesDamageFrom
        {
            get;
        }

        /// <summary>
        /// Notifies this character to take damage from the given IDamaging
        /// </summary>
        /// <param name="damageItem">The item that collided with this character and needs to take damage as a result</param>
        void TakeDamage(IDamaging damageItem);
    }
}
