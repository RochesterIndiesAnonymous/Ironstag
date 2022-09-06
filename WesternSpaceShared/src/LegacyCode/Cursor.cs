using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WesternSpace
{
    class Cursor
    {
        private Vector2 position;
        private Texture2D spriteTexture;
        private MouseState mouseState;

        /*
         * Default Constructor
         */
        public Cursor(Texture2D spriteTexture)
        {
            this.position = new Vector2(0,0);
            this.spriteTexture = spriteTexture;
        }

        /*
         * Called every update frame
         */
        public void Update()
        {
            mouseState = Mouse.GetState();

            position.X = mouseState.X;
            position.Y = mouseState.Y;
        }

        /*
         * Draws each frame
         */
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(spriteTexture, position, Color.White);
        }

        /*
         * Set's the cursor's image to the new texture
         */
        public Texture2D Sprite
        {
            set { spriteTexture = value; }
        }
      
        /*public void setSprite(Texture2D newSprite)
        {
            spriteTexture = newSprite;
        }
         */

        /*
         * Returns the cursor's current position
         */
        public Vector2 getPosition()
        {
            return position;
        }
    }
}
