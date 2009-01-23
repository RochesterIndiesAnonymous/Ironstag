using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Utility
{
    public class ResolutionSettings
    {
        private int renderTargetWidth;

        public int RenderTargetWidth
        {
            get { return renderTargetWidth; }
        }

        private int renderTargetHeight;

        public int RenderTargetHeight
        {
            get { return renderTargetHeight; }
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

        public ResolutionSettings(int renderTargetWidth, int renderTargetHeight, int backBufferWidth, int backBufferHeight)
        {
            this.renderTargetWidth = renderTargetWidth;
            this.renderTargetHeight = renderTargetHeight;
            this.backBufferWidth = backBufferWidth;
            this.backBufferHeight = backBufferHeight;
        }
    }
}
