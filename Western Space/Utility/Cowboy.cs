using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.AnimationFramework;

namespace WesternSpace.Utility
{
    /// <summary>
    /// Represents the role "Cowboy" that an actor can play.
    /// </summary>
    public class Cowboy : Role
    {
        // Constants //
        public static readonly string IDLE = "Idle";
        public static readonly string DEAD = "Dead";
        public static readonly string HIT = "Hit";
        public static readonly string SHOOTING = "Shooting";
        public static readonly string RUNNING = "Running";
        public static readonly string TRANSFORMING = "Transforming";
        public static readonly string JUMPINGASCENT = "JumpingAscent";
        public static readonly string JUMPINGDESCENT = "JumpingDescent";
        public static readonly string JUMPINGASCENTSHOOTING = "JumpingAscentShooting";
        public static readonly string JUMPINGDESCENTSHOOTING = "JumpingDescentShooting";
        public static readonly string RUNNINGSHOOTING = "RunningShooting";
        public static readonly string SHOOTINGUP = "ShootingUp";
        public static readonly string JUMPINGASCENTSHOOTINGUP = "JumpingAscentShootingUp";
        public static readonly string JUMPINGDESCENTSHOOTINGUP = "JumpingDescentShootingUp";
        public static readonly string RUNNINGSHOOTINGUP = "RunningShootingUp";
        public static readonly string FALLING = "Falling";
        public static readonly string FALLINGSHOOTING = "FallingShooting";
        public static readonly string FALLINGSHOOTINGUP = "FallingShootingUp";

        // Class Variables //
        private int shootCoolDown;

        public int ShootCoolDown
        {
            get { return shootCoolDown; }
        }

        /// <summary>
        /// Cowboy Constructor
        /// </summary>
        /// <param name="xmlFile">XML filename which houses the role data for a Cowboy.</param>
        public Cowboy(string xmlFile, string name)
            :base(xmlFile, name)
        {
            LoadCowboyXMLFile(this.roleXmlFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFile"></param>
        public override void SetUpAnimation(String xmlFile)
        {   
            Animation idle = new Animation(xmlFile, IDLE);
            Animation running = new Animation(xmlFile, RUNNING);
            Animation jumpingAscent = new Animation(xmlFile, JUMPINGASCENT);
            Animation jumpingDescent = new Animation(xmlFile, JUMPINGDESCENT);
            Animation shooting = new Animation(xmlFile, SHOOTING);
            Animation runningShooting = new Animation(xmlFile, RUNNINGSHOOTING);
            Animation jumpingAscentShooting = new Animation(xmlFile, JUMPINGASCENTSHOOTING);
            Animation jumpingDescentShooting = new Animation(xmlFile, JUMPINGDESCENTSHOOTING);
            Animation falling = new Animation(xmlFile, FALLING);
            Animation fallingShooting = new Animation(xmlFile, FALLINGSHOOTING);
            //Animation dead = new Animation(xmlFile, DEAD);
            //Animation hit = new Animation(xmlFile, HIT);
            //Animation transforming = new Animation(xmlFile, TRANSFORMING);
            //Animation jumpingAscentShootingUp = new Animation(xmlFile, JUMPINGASCENTSHOOTINGUP);
            //Animation jumpingDescentShootingUp = new Animation(xmlFile, JUMPINGASCENTSHOOTINGUP);
            //Animation fallingShootingUp = new Animatin(xmlFile, FALLINGSHOOTINGUP);
            //Animation runningShootingUp = new Animation(xmlFile, RUNNINGSHOOTINGUP);
            //Animation shootingUp = new Animation(xmlFile, SHOOTINGUP);


            this.animationMap.Add(IDLE, idle);
            this.animationMap.Add(RUNNING, running);
            this.animationMap.Add(JUMPINGASCENT, jumpingAscent);
            this.animationMap.Add(JUMPINGDESCENT, jumpingDescent);
            this.animationMap.Add(SHOOTING, shooting);
            this.animationMap.Add(RUNNINGSHOOTING, runningShooting);
            this.animationMap.Add(JUMPINGASCENTSHOOTING, jumpingAscentShooting);
            this.animationMap.Add(JUMPINGDESCENTSHOOTING, jumpingDescentShooting);
            this.animationMap.Add(FALLING, falling);
            this.animationMap.Add(FALLINGSHOOTING, fallingShooting);
            //this.animationMap.Add(DEAD, dead);
            //this.animationMap.Add(HIT, hit);
            //this.animationMap.Add(TRANSFORMING, transforming);
            //this.animationMap.Add(JUMPINGASCENTSHOOTINGUP, jumpingAscentShootingUp);
            //this.animationMap.Add(JUMPINGDESCENTSHOOTINGUP, jumpingDescentShootingUp);
            //this.animationMap.Add(RUNNINGSHOOTINGUP, runningShootingUp);
            //this.animationMap.Add(SHOOTINGUP, shootingUp);
        }

        /// <summary>
        /// Loads Role specific attributes from the supplied xml file.
        /// </summary>
        /// <param name="xmlFile">XML file name containing this role's attributes.</param>
        private void LoadCowboyXMLFile(string xmlFile)
        {
            Console.WriteLine("FILE: " + xmlFile);
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(xmlFile);
            Int32.TryParse(fileContents.Root.Element("Attributes").Attribute("ShootCoolDown").Value, out this.shootCoolDown);
        }
    }
}
