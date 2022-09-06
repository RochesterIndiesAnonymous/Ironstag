using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;

namespace WesternSpace.Utility
{
    class Bird : Role
    {
        /// Constants ///
        public static readonly string DEAD = "Dead";
        //public static readonly string CLIMBING = "Climbing";
        //public static readonly string DIVING = "Diving";
        //public static readonly string ATTACKING = "Attacking";
        public static readonly string FLYING = "Flying";

        /// <summary>
        /// Bird Constructor
        /// </summary>
        /// <param name="xmlFile">XML filename which houses the animation data for a Bird.</param>
        public Bird(string xmlFile, string name)
            : base(xmlFile, name)
        {
        }

        public override void SetUpAnimation(String xmlFile)
        {
            Animation dead = new Animation(xmlFile, DEAD);
            //Animation climbing = new Animation(xmlFile, CLIMBING);
            //Animation diving = new Animation(xmlFile, DIVING);
            //Animation attacking = new Animation(xmlFile, ATTACKING);
            Animation flying = new Animation(xmlFile, FLYING);
          
            this.animationMap.Add(DEAD, dead);
            //this.animationMap.Add(CLIMBING, climbing);
            //this.animationMap.Add(DIVING, diving);
            //this.animationMap.Add(ATTACKING, attacking);
            this.animationMap.Add(FLYING, flying);
        }
    }

}
