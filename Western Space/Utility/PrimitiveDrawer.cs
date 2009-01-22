using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;

// CREDITS: 
// Big thanks to "verTex":
// http://v3ction.blogspot.com/2008/04/drawline-function-in-xna-using.html

namespace WesternSpace.Utility
{
    class PrimitiveDrawer
    {
        Texture2D pixel;
        GraphicsDevice graphicsDevice;

        private static PrimitiveDrawer instance;

        public static PrimitiveDrawer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PrimitiveDrawer();
                }

                return instance;
            }
        }

        public PrimitiveDrawer()
        {
            graphicsDevice = ((IGraphicsDeviceManagerService)ScreenManager.Instance.Services.GetService(typeof(IGraphicsDeviceManagerService))).GraphicsDevice.GraphicsDevice;
            CreatePixelTexture();

            graphicsDevice.DeviceReset += new EventHandler(myDevice_DeviceReset);
        }

        //Each time the device is reset, we need to recreate the Texture - otherwise it crashes in windowed mode
        void myDevice_DeviceReset(object sender, EventArgs e)
        {
            CreatePixelTexture();
        }

        //Creates a white 1*1 Texture that is used for the lines by scaling and rotating it
        public void CreatePixelTexture()
        {
            int TargetWidth = 1;
            int TargetHeight = 1;

            RenderTarget2D LevelRenderTarget = new RenderTarget2D(graphicsDevice, TargetWidth, TargetHeight, 1,
                graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.MultiSampleType,
                graphicsDevice.PresentationParameters.MultiSampleQuality, RenderTargetUsage.PreserveContents);

            DepthStencilBuffer stencilBuffer = new DepthStencilBuffer(graphicsDevice, TargetWidth, TargetHeight,
                graphicsDevice.DepthStencilBuffer.Format, graphicsDevice.PresentationParameters.MultiSampleType,
                graphicsDevice.PresentationParameters.MultiSampleQuality);

            graphicsDevice.SetRenderTarget(0, LevelRenderTarget);

            // Cache the current depth buffer
            DepthStencilBuffer old = graphicsDevice.DepthStencilBuffer;
            // Set our custom depth buffer
            graphicsDevice.DepthStencilBuffer = stencilBuffer;

            graphicsDevice.Clear(Color.White);

            graphicsDevice.SetRenderTarget(0, null);

            // Reset the depth buffer
            graphicsDevice.DepthStencilBuffer = old;
            pixel = LevelRenderTarget.GetTexture();
        }

        //Calculates the distances and the angle and than draws a line
        public void DrawLine(SpriteBatch sprite, Vector2 start, Vector2 end, Color color)
        {
            int distance = (int)Vector2.Distance(start, end);

            Vector2 connection = end - start;
            Vector2 baseVector = new Vector2(1, 0);

            float alpha = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            if (pixel != null)
                sprite.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, distance, 1),
                    null, color, alpha, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        //Draws a rect with the help of DrawLine
        public void DrawRect(SpriteBatch sprite, Rectangle rect, Color color)
        {
            // | left
            DrawLine(sprite, new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), color);
            // - top
            DrawLine(sprite, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), color);
            // - bottom
            DrawLine(sprite, new Vector2(rect.X, rect.Y + rect.Height),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);
            // | right
            DrawLine(sprite, new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color);

        }
    }
}
