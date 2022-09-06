using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WesternSpace.EventSystem
{
    /**
     * An Event class is a generic container for invoking
     * some "action" on an object. Only methods that don't
     * have any arguments are supported right now.
     */
    class Event
    {
        private Object host;
        private MethodInfo method;
        private Object[] parameters;

        /**
         * Construct an Event object that executes a method without
         * any arguments.
         */
        public Event(Object host, String methodName)
        {
            this.host = host;
            this.method = host.GetType().GetMethod(methodName);

            parameters = new Object[0];
        }

        /**
         * Execute the action.
         */
        public void execute()
        {
            Object returnVal = method.Invoke(host, parameters);
            //Console.WriteLine(returnVal);
        }
    }
}