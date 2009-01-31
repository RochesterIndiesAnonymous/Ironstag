using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Utility
{
    public class ResolutionSettings
    {

        /// <summary>
        /// True if the current game is in full screen. Used to determine which resolution to use for rendering
        /// </summary>
        private bool isFullScreen;

        /// <summary>
        /// True if the current game is in full screen. Used to determine which resolution to use for rendering
        /// </summary>
        public bool IsFullScreen
        {
            get { return isFullScreen; }
        }

        private int renderTargetWidth;

        public int RenderTargetWidth
        {
            get { return renderTargetWidth; }
            set { renderTargetWidth = value; }
        }

        private int renderTargetHeight;

        public int RenderTargetHeight
        {
            get { return renderTargetHeight; }
            set { renderTargetHeight = value; }
        }

        private int backBufferWidth;

        public int BackBufferWidth
        {
            get { return backBufferWidth; }
        }

        private int backBufferHeight;

        public int BackBufferHeight
        {
            get { return backBufferHeight; }
        }

        public ResolutionSettings(int renderTargetWidth, int renderTargetHeight, int backBufferWidth, int backBufferHeight, bool isFullScreen)
        {
            this.isFullScreen = isFullScreen;
            this.renderTargetWidth = renderTargetWidth;
            this.renderTargetHeight = renderTargetHeight;
            this.backBufferWidth = backBufferWidth;
            this.backBufferHeight = backBufferHeight;
        }
    }
}
