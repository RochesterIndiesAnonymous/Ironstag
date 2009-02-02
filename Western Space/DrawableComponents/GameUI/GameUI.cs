using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Screens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.Interfaces;

namespace WesternSpace.DrawableComponents.GameUI
{
    public class GameUI : DrawableGameObject
    {
        /// <summary>
        /// The type of SpriteBatch object this component needs
        /// </summary>
        public static readonly string SpriteBatchName = "Static";

        /// <summary>
        /// Reference to the health bar graphic stored in memory
        /// </summary>
        Texture2D healthBarGraphic;

        /// <summary>
        /// Reference to the health strip graphic that is scaled to draw life
        /// </summary>
        Texture2D healthStripGraphic;

        /// <summary>
        /// The position of the health bar realtive to this components position
        /// </summary>
        private Vector2 healthBarPosition = new Vector2(10, 10);

        /// <summary>
        /// Offset information used to determine where in the health bar graphic to draw the health remaining
        /// </summary>
        private readonly Vector2 healthBarStripOffset = new Vector2(1, 56);

        /// <summary>
        /// Fixed for the graphic made for health
        /// 1 pixel to the right
        /// 8 pixels from the top
        /// 14 pixels in width
        /// 48 pixels in height
        /// </summary>
        private Rectangle healthBarRectangle = new Rectangle(1, 8, 14, 48);

        /// <summary>
        /// A reference to the player to calculate the health
        /// </summary>
        IDamageable player;

        /// <summary>
        /// The current percentage of health that the player has
        /// </summary>
        float healthBarPercentage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentScreen">The parent screen that the game UI is part of</param>
        /// <param name="spriteBatch">The SpriteBatch object that the game UI is to use to draw itself</param>
        /// <param name="position">The position of the Game UI in absolute coordinates</param>
        public GameUI(Screen parentScreen, SpriteBatch spriteBatch, Vector2 position, IDamageable player)
            : base(parentScreen, spriteBatch, position)
        {
            this.player = player;
        }

        /// <summary>
        /// Gets the needed textures to draw the UI
        /// </summary>
        public override void Initialize()
        {
            ITextureService textureService = (ITextureService)this.Game.Services.GetService(typeof(ITextureService));
            healthBarGraphic = textureService.GetTexture("UIGraphics\\HealthBar");

            healthStripGraphic = textureService.GetTexture("UIGraphics\\LifeStrip");

            healthBarPercentage = 0.8f;

            this.healthBarPosition += this.Position;

            this.healthBarRectangle.Offset((int)healthBarPosition.X, (int)healthBarPosition.Y);

            base.Initialize();
        }

        /// <summary>
        /// Calculates the percentage of player health
        /// </summary>
        /// <param name="gameTime">Timing statistics relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            healthBarPercentage = ((float)player.CurrentHealth / (float)player.MaxHealth);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the health bar and the amount of life inside it.
        /// </summary>
        /// <param name="gameTime">Timing statistics relative to the game</param>
        public override void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Draw(healthBarGraphic, healthBarPosition, Color.White);

            Rectangle drawRect = healthBarRectangle;

            drawRect.Height = (int)(drawRect.Height * healthBarPercentage);
            drawRect.Y = drawRect.Y + (healthBarRectangle.Height - drawRect.Height);

            this.SpriteBatch.Draw(healthStripGraphic, drawRect, Color.White); 

            base.Draw(gameTime);
        }
    }
}
