using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Services
{
    public class ScreenResolutionService
    {
        private Rectangle scaleRectangle;

        public Rectangle ScaleRectangle
        {
            get { return scaleRectangle; }
            set { scaleRectangle = value; }
        }

        private int startTextureWidth;

        public int StartTextureWidth
        {
            get { return startTextureWidth; }
            set { startTextureWidth = value; }
        }

        private int startTextureHeight;

        public int StartTextureHeight
        {
            get { return startTextureHeight; }
            set { startTextureHeight = value; }
        }

        public ScreenResolutionService(GraphicsDeviceManager graphics, int startingTextureWidth, int startingTextureHeight)
        {
            scaleRectangle = CalculateResolution(graphics, startingTextureWidth, startingTextureHeight);

            this.startTextureWidth = startingTextureWidth;
            this.startTextureHeight = startingTextureHeight;
        }

        private Rectangle CalculateResolution(GraphicsDeviceManager graphics, int startingTextureWidth, int startingTextureHeight)
        {
            int currentCalculatedWidth = startingTextureWidth;
            int currentCalculatedHeight = startingTextureHeight;

            while (currentCalculatedWidth + startingTextureWidth <= graphics.PreferredBackBufferWidth && currentCalculatedHeight + startingTextureHeight <= graphics.PreferredBackBufferHeight)
            {
                currentCalculatedWidth += startingTextureWidth;
                currentCalculatedHeight += startingTextureHeight;
            }

            int x = (graphics.GraphicsDevice.Viewport.Width - currentCalculatedWidth) / 2;
            int y = (graphics.GraphicsDevice.Viewport.Height - currentCalculatedHeight) / 2;

            return new Rectangle(x, y, currentCalculatedWidth, currentCalculatedHeight);
        }
    }
}
