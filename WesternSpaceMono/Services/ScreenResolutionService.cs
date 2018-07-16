using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Services
{
    public class ScreenResolutionService : IScreenResolutionService
    {
        /// <summary>
        /// The intermediate render target we use to provide scaling of the game screen resolution
        /// </summary>
        private RenderTarget2D renderTarget;

        public RenderTarget2D RenderTarget
        {
            get { return renderTarget; }
        }

        private Rectangle scaleRectangle;

        public Rectangle ScaleRectangle
        {
            get { return scaleRectangle; }
        }

        GraphicsDeviceManager graphics;

        /// <summary>
        /// A pointer to the current resolution settings the game is using.
        /// </summary>
        private static ResolutionSettings currentResolutionSettings;

        public ResolutionSettings CurrentResolutionSettings
        {
            get { return currentResolutionSettings; }
            set 
            {
                currentResolutionSettings = value;

                graphics.PreferredBackBufferWidth = value.BackBufferWidth;
                graphics.PreferredBackBufferHeight = value.BackBufferHeight;

                Viewport vp = graphics.GraphicsDevice.Viewport;
                vp.Width = value.BackBufferWidth;
                vp.Height = value.BackBufferHeight;
                graphics.GraphicsDevice.Viewport = vp;

                scaleRectangle = CalculateResolution();

                graphics.IsFullScreen = CurrentResolutionSettings.IsFullScreen;
                graphics.ApplyChanges();

                renderTarget = new RenderTarget2D(graphics.GraphicsDevice,CurrentResolutionSettings.RenderTargetWidth, CurrentResolutionSettings.RenderTargetHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);

                CameraService cameraService = (CameraService)ScreenManager.Instance.Services.GetService(typeof(ICameraService));
                if (cameraService != null)
                {
                    cameraService.UpdateVisibleArea(this);
                    cameraService.CreateViewTransformationMatrix();
                }
            }
        }

        public int StartTextureWidth
        {
            get { return CurrentResolutionSettings.RenderTargetWidth; }
        }

        public int StartTextureHeight
        {
            get { return CurrentResolutionSettings.RenderTargetHeight; }
        }

        public int ScaleFactor
        {
            get { return (scaleRectangle.Width / StartTextureWidth); }
        }

        public ScreenResolutionService(GraphicsDeviceManager graphics, ResolutionSettings resolutionSettings)
        {
            this.graphics = graphics;
            CurrentResolutionSettings = resolutionSettings;
        }

        private Rectangle CalculateResolution()
        {
            int currentCalculatedWidth = StartTextureWidth;
            int currentCalculatedHeight = StartTextureHeight;

            while (currentCalculatedWidth + StartTextureWidth <= graphics.PreferredBackBufferWidth && currentCalculatedHeight + StartTextureHeight <= graphics.PreferredBackBufferHeight)
            {
                currentCalculatedWidth += StartTextureWidth;
                currentCalculatedHeight += StartTextureHeight;
            }

            int x = (graphics.GraphicsDevice.Viewport.Width - currentCalculatedWidth) / 2;
            int y = (graphics.GraphicsDevice.Viewport.Height - currentCalculatedHeight) / 2;

            return new Rectangle(x, y, currentCalculatedWidth, currentCalculatedHeight);
        }
    }
}
