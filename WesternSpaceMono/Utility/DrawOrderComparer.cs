using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Utility
{
    /// <summary>
    /// Reimplementation of Microsoft's DrawOrderComparer since the bastards hid it from us
    /// </summary>
    public class DrawOrderComparer : IComparer<IDrawable>
    {
        /// <summary>
        /// The default comparer to use for comparing IDrawables
        /// </summary>
        public static readonly DrawOrderComparer Default = new DrawOrderComparer();

        /// <summary>
        /// Determines whther an drawable item should go before or after another drawable item
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item that is being used for comparison</param>
        /// <returns>
        /// 0 - items are equal
        /// -1 - x belongs before y
        /// 1 - x belongs after y
        /// </returns>
        public int Compare(IDrawable x, IDrawable y)
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
                if (x.DrawOrder < y.DrawOrder)
                {
                    return -1;
                }
            }
            return 1;
        }
    }
}
