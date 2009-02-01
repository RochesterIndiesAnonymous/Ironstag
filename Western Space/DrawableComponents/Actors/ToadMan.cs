using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

using WesternSpace.ServiceInterfaces;
using WesternSpace.AnimationFramework;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Actors
{
    class ToadMan
    {
        public ToadMan()
        {
        }
    }
}
     /*   //Set Facing
        SpriteEffects facing = SpriteEffects.None;

        // The constructor for the Toad Man 
        public ToadMan(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position, String xmlFile)
            : base(parentScreen, spriteBatch, position, xmlFile)
        {
            //Set the character's Name
            name = "Toad Man";

            //Create all of the Animations for Toad Man
            SetUpAnimation(xmlFile);

            //Create the Animation Player and give it the Idle Animation
            this.animationPlayer = new AnimationPlayer(spriteBatch, animationMap["Idle"], animationMap["Idle"]);

            //Set the current animation
            currentAnimation = animationPlayer.Animation;

            //Set the current state
            currentState = "Idle";

            //Set the Velocity
            velocity = new Vector2(0, 0);

            //Set the position
            this.Position = position;
        }
        // Sets up all of the Animations associated with the particular character
        // and adds them to the collection mapping states to animations.
        // param: xmlFile - The XML file name which stores the Character's Animation data.
        public override void SetUpAnimation(String xmlFile)
        {
            Animation idle = new Animation(xmlFile, "Idle");
            Animation walking = new Animation(xmlFile, "Walking");
            //Animation jumping = new Animation(xmlFile, "Jumping");
            Animation shooting = new Animation(xmlFile, "Shooting");

            this.animationMap.Add("Idle", idle);
            this.animationMap.Add("Walking", walking);
           // this.animationMap.Add("Jumping", jumping);
            this.animationMap.Add("Shooting", shooting);
        }
        public override void Update(GameTime gameTime)
        {
            //Input Logic / State Changing

            //Let the Animation Player Update the Frame
            animationPlayer.Update(gameTime);

            if (gameTime.TotalRealTime.Seconds >= 10)
            {
                ChangeState("Shooting");
            }
        }
        public override void Draw(GameTime gameTime)
        {
            //Let the Animation Player Draw
            animationPlayer.Draw(gameTime, this.SpriteBatch, this.Position, facing);
        }
        public void OnSpriteCollision()
        {
        }
    }
}
    */