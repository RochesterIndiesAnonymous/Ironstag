using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Utility;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// This interface is used to provide information about how much
    /// damage is to be done to the player
    /// 
    /// All items in the world that do damage to the player
    /// need to implement this class.
    /// </summary>
    public interface IDamaging
    {
        /// <summary>
        /// The owner of of the damage.
        /// For projectiles this would be a reference to the enemy that fired the projectile
        /// For enemies this would just be a refence to itself
        /// </summary>
        object Owner
        {
            get;
        }

        /// <summary>
        /// What this item or character is seeking to damage. If this is an enemy character or
        /// enemy projectile, set to Player. If this is the Player or player projectile set to Enemy
        /// </summary>
        DamageCategory DoesDamageTo
        {
            get;
        }

        /// <summary>
        /// The amount of damage this character/projectile does to the player
        /// </summary>
        float AmountOfDamage
        {
            get;
        }
    }
}
