using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Collision
{  
    public enum HOTSPOT_TYPE
        {
            left,
            right,
            top,
            bottom
        };
    // Collision Hotspot tune sprite collisions
    public class CollisionHotspot
    {
      
        DrawableGameObject refGameObject;
        protected Vector2 offsetPosition;
        protected Vector2 worldPosition;
        public Vector2 WorldPosition
        {
            get 
            {
                worldPosition = refGameObject.Position + offsetPosition;
                return worldPosition;
            }
        }
        public Vector2 HostPosition
        {
            get { return refGameObject.Position; }
        }
        public Vector2 Offset
        {
            get { return offsetPosition; }
        }
        protected HOTSPOT_TYPE hotspotType;
        public HOTSPOT_TYPE HotSpotType
        {
            get { return hotspotType; }
        }
        public CollisionHotspot(DrawableGameObject hostObject, Vector2 offset, HOTSPOT_TYPE type)
        {
            refGameObject = hostObject;
            this.offsetPosition = offset;
            this.hotspotType = type;
        }
    }
}
