using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.AnimationFramework;

namespace WesternSpace.Utility
{
    class Boss : Role
    {
        /// Constants ///
        public static readonly string IDLE = "Idle";
        public static readonly string LAUGHING = "Laughing";
        public static readonly string DEAD = "Dead";
        public static readonly string HIT = "Hit";
        public static readonly string SHOOTING = "Shooting";
        public static readonly string RUNNING = "Running";

          /// <summary>
        /// Bandit Constructor
        /// </summary>
        /// <param name="xmlFile">XML filename which houses the animation data for a Bandit.</param>
        public Boss(string xmlFile, string name)
            :base(xmlFile, name)
        {
        }

        public override void SetUpAnimation(String xmlFile)
        {   
            Animation idle = new Animation(xmlFile, IDLE);
            Animation laughing = new Animation(xmlFile, LAUGHING);
            Animation running = new Animation(xmlFile, RUNNING);
            Animation shooting = new Animation(xmlFile, SHOOTING);
            Animation dead = new Animation(xmlFile, DEAD);
            //Animation hit = new Animation(xmlFile, HIT);


            this.animationMap.Add(IDLE, idle);
            this.animationMap.Add(LAUGHING, laughing);
            this.animationMap.Add(RUNNING, running);
            this.animationMap.Add(SHOOTING, shooting);
            this.animationMap.Add(DEAD, dead);
            //this.animationMap.Add(HIT, hit);
        }
    }

}
