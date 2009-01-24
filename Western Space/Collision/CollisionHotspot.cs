using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Collision
{
    // Collision Hotspot tune sprite collisions
    public class CollisionHotspot
    {
        public enum HOTSPOT_TYPE
        {
            left,
            right,
            top,
            bottom
        };
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }
        protected Vector2 offset;
        public Vector2 Offset
        {
            get { return offset; }
        }
        protected HOTSPOT_TYPE hotspotType;
        public CollisionHotspot(Vector2 hostPosition, Vector2 offset, HOTSPOT_TYPE type)
        {
            this.position = hostPosition;
            this.offset = offset;
            this.hotspotType = type;
        }
    }
}
