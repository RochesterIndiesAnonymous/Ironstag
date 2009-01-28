﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Interfaces;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace
{
    // AKA Sprite-sheet.
    public class SubTexture
    {
        private SubTextureSheet sheet;

        public SubTextureSheet Sheet
        {
            get { return sheet; }
        }

        public Texture2D Texture
        {
            get { return sheet.Texture; }
        }

        public Rectangle Rectangle
        {
            get { return sheet.Rectangles[index]; }
        }

        private int index;

        public int Index
        {
            get { return index; }
        }

        public SubTexture(SubTextureSheet sheet, int index)
        {
            this.sheet = sheet;
            this.index = index;
        }

        public static bool operator ==(SubTexture a, SubTexture b)
        {
            if ((object)a == null || (object)b == null)
                return (object)b == (object)a;
            else
                return  a.index == b.index && a.sheet.Name == b.sheet.Name;
        }

        public static bool operator !=(SubTexture a, SubTexture b)
        {
            return !(a == b);
        }
    }
}