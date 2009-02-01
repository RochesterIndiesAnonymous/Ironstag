using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.TilingEngine;

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
        Character refGameObject;
        protected Vector2 offsetPosition;
        protected Vector2 worldPosition;
        protected Boolean isOnGround;
        public Boolean IsOnGround
        {
            get { return isOnGround; }
        }
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
        public void OnTileCollision(Tile tile, Rectangle tileRectangle)
        {
            if (tile.TopEdge && this.HotSpotType == HOTSPOT_TYPE.bottom)
            {
                // Puts the sprite above the tile;      
                this.refGameObject.Position = new Vector2(this.HostPosition.X,
                    this.HostPosition.Y - (this.WorldPosition.Y - tileRectangle.Top));
                this.isOnGround = true;
            }
            if (tile.BottomEdge && this.HotSpotType == HOTSPOT_TYPE.top)
            {
                this.refGameObject.Position = new Vector2(this.HostPosition.X,
                    this.HostPosition.Y + (tileRectangle.Bottom - this.WorldPosition.Y));
                this.isOnGround = false;
            }
            if (tile.LeftEdge && this.HotSpotType == HOTSPOT_TYPE.right)
            {
                this.refGameObject.Position = new Vector2(this.HostPosition.X - (this.WorldPosition.X - tileRectangle.Left),
                    this.HostPosition.Y);
            }
            if (tile.RightEdge && this.HotSpotType == HOTSPOT_TYPE.left)
            {
                this.refGameObject.Position = new Vector2(this.HostPosition.X + (tileRectangle.Right - this.WorldPosition.X),
                 this.HostPosition.Y);
            }

        }
        public CollisionHotspot(Character hostObject, Vector2 offset, HOTSPOT_TYPE type)
        {
            refGameObject = hostObject;
            this.offsetPosition = offset;
            this.hotspotType = type;
        }
    }
}
