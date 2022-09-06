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
        public static readonly string SHOOTINGUP = "ShootingUp";
        public static readonly string SHOOTINGDOWN = "ShootingDown";
        public static readonly string RUNNING = "Running";
        public static readonly string JUMPINGASCENT = "JumpingAscent";
        public static readonly string JUMPINGDESCENT = "JumpingDescent";
        public static readonly string JUMPINGLAND = "JumpingLand";

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
            Animation shootingUp = new Animation(xmlFile, SHOOTINGUP);
            Animation shootingDown = new Animation(xmlFile, SHOOTINGDOWN);
            Animation jumpingAscent = new Animation(xmlFile, JUMPINGASCENT);
            Animation jumpingDescent = new Animation(xmlFile, JUMPINGDESCENT);
            Animation jumpingLand = new Animation(xmlFile, JUMPINGLAND);
            Animation dead = new Animation(xmlFile, DEAD);
            Animation hit = new Animation(xmlFile, HIT);


            this.animationMap.Add(IDLE, idle);
            this.animationMap.Add(LAUGHING, laughing);
            this.animationMap.Add(RUNNING, running);
            this.animationMap.Add(SHOOTING, shooting);
            this.animationMap.Add(SHOOTINGUP, shootingUp);
            this.animationMap.Add(SHOOTINGDOWN, shootingDown);
            this.animationMap.Add(JUMPINGASCENT, jumpingAscent);
            this.animationMap.Add(JUMPINGDESCENT, jumpingDescent);
            this.animationMap.Add(JUMPINGLAND, jumpingLand);
            this.animationMap.Add(DEAD, dead);
            this.animationMap.Add(HIT, hit);
        }
    }

}
