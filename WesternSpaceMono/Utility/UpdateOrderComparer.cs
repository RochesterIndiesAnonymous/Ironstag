using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Utility
{
    /// <summary>
    /// Reimplementation of Microsoft's UpdateOrderComparer. Bastards made it internal
    /// </summary>
    public class UpdateOrderComparer : IComparer<IUpdateable>
    {
        /// <summary>
        /// The default comparer to use for figuring out the update order
        /// </summary>
        public static readonly UpdateOrderComparer Default = new UpdateOrderComparer();

        #region IComparer<IUpdateable> Members

        /// <summary>
        /// Determines whther an updateable item should go before or after another update item
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item that is being used for comparison</param>
        /// <returns>
        /// 0 - items are equal
        /// -1 - x belongs before y
        /// 1 - x belongs after y
        /// </returns>
        public int Compare(IUpdateable x, IUpdateable y)
        {
            if ((x == null) && (y == null))
            {
                return 0;
            }
            if (x != null)
            {
                if (y == null)
                {
                    return -1;
                }
                if (x.Equals(y))
                {
                    return 0;
                }
                if (x.UpdateOrder < y.UpdateOrder)
                {
                    return -1;
                }
            }
            return 1;
        }

        #endregion
    }
}
