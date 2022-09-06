using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.TilingEngine;
using WesternSpace.Collision;

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
        ITileCollidable refGameObject;
        protected Vector2 offsetPosition;

        protected Boolean didCollide;

        /// <summary>
        /// Whether or not this hotspot collided since it was last updated. This
        ///  MUST be set to false 
        /// </summary>
        public Boolean DidCollide
        {
            get { return didCollide; }
            set { didCollide = value; }
        }

        public World World
        {
            get { return refGameObject.World; }
        }
        
        /// <summary>
        /// The current tile this hotspot resides in.
        /// </summary>
        public Tile Tile
        {
            get 
            {
                return World.Map[(int)WorldPosition.X / World.Map.TileWidth,
                                 (int)WorldPosition.Y / World.Map.TileHeight];
            }
        }

        public Rectangle TileRectangle
        {
            get
            {
                TileMap map = World.Map;
                int tileX = (int)WorldPosition.X / World.Map.TileWidth;
                int tileY = (int)WorldPosition.Y / World.Map.TileHeight;
                return new Rectangle(tileX * map.TileWidth,
                                     tileY * map.TileHeight,
                                     map.TileWidth,
                                     map.TileHeight);
            }
        }

        public Vector2 WorldPosition
        {
            get 
            {
                return refGameObject.Position + offsetPosition;
            }
            set
            {
                refGameObject.Position = value - offsetPosition;
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

        public void Collide()
        {
            Rectangle tileRectangle = TileRectangle;
            Tile tile = Tile;

            didCollide = false;
            if (tile != null)
            {
                switch (HotSpotType)
                {
                    case (HOTSPOT_TYPE.bottom):
                        if (tile.TopEdge && WorldPosition.Y - tileRectangle.Top < (tileRectangle.Height / 2))
                        {
                            WorldPosition = new Vector2(WorldPosition.X, tileRectangle.Top);
                            didCollide = true;
                        }
                        break;
                    case (HOTSPOT_TYPE.top):
                        if (tile.BottomEdge && tileRectangle.Bottom - WorldPosition.Y < (tileRectangle.Height / 2))
                        {
                            WorldPosition = new Vector2(WorldPosition.X, tileRectangle.Bottom);
                            didCollide = true;
                        }
                        break;
                    case (HOTSPOT_TYPE.left):
                        if (tile.RightEdge && tileRectangle.Right - WorldPosition.X < (tileRectangle.Width / 2))
                        {
                            WorldPosition = new Vector2(tileRectangle.Right, WorldPosition.Y);
                            didCollide = true;
                        }
                        break;
                    case (HOTSPOT_TYPE.right):
                        if (tile.LeftEdge && WorldPosition.X - tileRectangle.Left < (tileRectangle.Width / 2))
                        {
                            WorldPosition = new Vector2(tileRectangle.Left, WorldPosition.Y);
                            didCollide = true;
                        }
                        break;
                }
            }
        }

        public CollisionHotspot(ITileCollidable hostObject, Vector2 offset, HOTSPOT_TYPE type)
        {
            refGameObject = hostObject;
            this.offsetPosition = offset;
            this.hotspotType = type;
        }
    }
}
