using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using WesternSpace.Interfaces;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.WorldObjects
{
    /// <summary>
    /// A barrel... that EXPLODES!
    /// </summary>
    public class ExplosiveBarrel 
    {
        #region IDamageable Members

        public int MaxHealth
        {
            get { throw new NotImplementedException(); }
        }

        private int currentHealth = 1; // This is one highly volatile barrel.

        public int CurrentHealth
        {
            get { return currentHealth; }
        }

        public float MitigationFactor
        {
            get { return 1; }
        }

        public DamageCategory TakesDamageFrom
        {
            get { throw new NotImplementedException(); }
        }

        public void TakeDamage(IDamaging damageItem)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
