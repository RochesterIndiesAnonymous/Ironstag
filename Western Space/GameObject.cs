using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace WesternSpace
{

    public class GameObject
    {
        //Class Variables
        Game1 game;
        private Texture2D spriteTexture;                            //Texture of the Game Object
        public Vector2 position = new Vector2(0, 0);                //Position of the Game Object
        public Rectangle size;                                      //Size of the Sprite (including Scale)
        private float scale = 1.0f;                                 //Scale of the Game Object
        private SpriteEffects spriteEffect = SpriteEffects.None;    //Determines the facing of the sprite
        private String contentName;

        public GameObject(Game1 game, String contentName)
        {
            this.game = game;
            this.contentName = contentName;
            spriteTexture = game.textures[contentName];
            size = new Rectangle(0, 0, (int)(spriteTexture.Width * scale),
                                 (int)(spriteTexture.Height * scale));
        }

        //Getters and Setters
        public Texture2D Sprite
        {
            get { return spriteTexture; }
            set { spriteTexture = value; }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                //Recalculate the size of the Sprite
                scale = value;
                size = new Rectangle(0, 0, (int)(spriteTexture.Width * scale),
                                     (int)(spriteTexture.Height * scale));
            }
        }

        public SpriteEffects Effect
        {
            get { return spriteEffect; }
            set { spriteEffect = value; }
        }

        public Vector2 objPosition
        {
            get { return position; }
            set { position = value; }
        }

        /*
         * Update the Sprite's position
         */
        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /*
         * Draw the sprite to the screen
         */
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(spriteTexture, position, new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height),
                       Color.White, 0.0f, Vector2.Zero, Scale, spriteEffect, 0);
        }


    }
}