using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Utility
{
    public class RectangleF2
    {
        private float x;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        private float y;

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        private float width;

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        private float height;

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Left
        {
            get { return x; }
        }

        public float Top
        {
            get { return y; }
        }

        public float Right
        {
            get { return x+width; }
        }

        public float Bottom
        {
            get { return y+height; }
        }


        public RectangleF2()
        {
            this.x = this.y = this.width = this.height = 0.0f;
        }

        public RectangleF2(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

    }
}
