using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace WesternSpace.EventSystem
{
    /**
     * The Trigger class essentially represents a "conditional".
     * When the condition is met, any Events added to this Trigger
     * will be executed. Only comparisons between integers is 
     * represented by the Trigger class right now.
     */
    class Trigger
    {
        private List<Event> eventList;
        private Object triggeringObject;
        private FieldInfo triggeringField;
        private int value;
        private int comparison;

        /**
         * Construct trigger with the object that will trigger the...trigger. Trigger.
         * 
         * triggeringObject - The object that the trigger is checking a condition on.
         * fieldName -        Name of the field on the object to check the condition on.
         * comparison -       The comparison to use in the conditional.
         * value -            The value to compare the field against.
         */
        public Trigger(Object triggeringObject, String fieldName, String comparison, String value)
        {
            this.triggeringObject = triggeringObject;
            eventList = new List<Event>();

            // Get the field using reflection.
            triggeringField = triggeringObject.GetType().GetField(fieldName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Store an int to represent the comparison.
            if (comparison.Equals(">"))
            {
                this.comparison = 1;
            }
            else if (comparison.Equals("<"))
            {
                this.comparison = -1;
            }
            else if (comparison.Equals("=="))
            {
                this.comparison = 0;
            }

            // Parse the value to an int.
            Int32.TryParse(value, out this.value);
        }

        public void addEvent(Event e)
        {
            eventList.Add(e);
        }

        /**
         * Alerts the Trigger to check its conditional. If it is true,
         * the associated Events are executed.
         */
        public void notifyTrigger()
        {
            // Use reflection to get the field value.
            int fieldValue = (int)triggeringField.GetValue(triggeringObject);

            bool triggerEvents = false;

            // Execute the comparison.
            if (comparison <= -1)
            {
                triggerEvents = (fieldValue < value);
            }
            else if (comparison == 0)
            {
                triggerEvents = (fieldValue == value);
            }
            else if(comparison >= 1 )
            {
                triggerEvents = (fieldValue > value);
            }

            // Invoke the associated Events if the condition is true.
            if (triggerEvents)
            {
                invokeEvents();
            }
        }

        private void invokeEvents()
        {
            foreach (Event e in eventList)
            {
                e.execute();
            }
        }
    }
}
