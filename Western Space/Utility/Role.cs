using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WesternSpace.AnimationFramework;

namespace WesternSpace.Utility

{
    /// <summary>
    /// Defines the role an actor plays. An actor can only be one role at a time but
    /// may changes its role at any time. An actor's role determine's which Animations
    /// are associated with 
    /// </summary>
    public abstract class Role
    {

        /// <summary>
        /// The name representing this role.
        /// </summary>
        public string name;

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// A collection mapping a character's animation objects
        /// to their available states.
        /// </summary>
        protected Dictionary<String, Animation> animationMap;

        public Dictionary<String, Animation> AnimationMap
        {
            get { return animationMap; }
        }

        public Role(String xmlFile, String name)
        {
            this.name = name;
            this.animationMap = new Dictionary<string, Animation>();
            LoadRoleXmlFile(xmlFile);
        }

        /// <summary>
        /// Sets up all of the Animations associated with the particular role
        /// and adds them to the collection mapping states to animations.
        /// </summary>
        /// <param name="xmlFile">The XML file name which stores the role's animation data.</param>
        public abstract void SetUpAnimation(String xmlFile);

        /// <summary>
        /// Loads a Character's role information from a specified XML file.
        /// </summary>
        /// <param name="fileName">The name of the xml file housing the character's information.</param>
        private void LoadRoleXmlFile(string fileName)
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);

            IEnumerable<XElement> allRoles = from roles in fileContents.Descendants("Role")
                                             select roles;

            foreach (XElement role in allRoles)
            {
                //Read the XML Attribute "SpriteSheet" from the XML file and save Texture.
                String roleName = role.Attribute("RoleKey").Value;

                if (this.name.Equals(roleName))
                {
                    String animationXml = role.Attribute("AnimationXML").Value;
                    SetUpAnimation(animationXml);
                    SetAnimationParents();
                }
            }
        }

        /// <summary>
        /// Sets each Animation's parent animation to the correct animation.
        /// </summary>
        public void SetAnimationParents()
        {
                foreach (KeyValuePair<string, Animation> animation in AnimationMap)
                {
                    if (!animation.Value.parentName.Equals("None"))
                    {
                        animation.Value.setParentAnimation(AnimationMap[animation.Value.parentName]);
                    }
                }
        }

        /*
        public abstract void SetUpAnimation(String xmlFile)
        {
            Animation idle = new Animation(xmlFile, "Idle");
            Animation walking = new Animation(xmlFile, "Walking");
            Animation jumpingAscent = new Animation(xmlFile, "JumpingAscent");
            Animation jumpingDescent = new Animation(xmlFile, "JumpingDescent");
            Animation shooting = new Animation(xmlFile, "Shooting");

            this.animationMap.Add("Idle", idle);
            this.animationMap.Add("Walking", walking);
            this.animationMap.Add("JumpingAscent", jumpingAscent);
            this.animationMap.Add("JumpingDescent", jumpingDescent);
            this.animationMap.Add("Shooting", shooting);
        }
         * */
    }
}
