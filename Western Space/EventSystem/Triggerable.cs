using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.EventSystem
{
    /**
     * Necessary interface for any class that can trigger
     * an event
     */
    interface Triggerable
    {
        /**
         * Used to store Triggers for later notification
         */
        void addTrigger(Trigger t);

        /**
         * Used to alert the added triggers that something has changed.
         */
        void notifyTriggers();
    }
}
