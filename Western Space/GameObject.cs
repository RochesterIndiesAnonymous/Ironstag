using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class GameObject
{
    public Vector2 position = new Vector2(0, 0);    //Current Position of Sprite
    private Texture2D spriteTexture;                //Texture of Sprite

    /*
     * Default Constructor
     */
	public GameObject(Texture2D loadedTexture)
	{
        position = Vector2.Zero;
        spriteTexture = loadedTexture;
	}

    //Draw the sprite to the screen
    public void Draw(SpriteBatch batch)
    {
        batch.Draw(spriteTexture, position, Color.White);
    }


}
