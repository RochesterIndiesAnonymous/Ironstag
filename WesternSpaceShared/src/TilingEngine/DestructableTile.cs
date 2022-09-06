using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using WesternSpace.Interfaces;
using WesternSpace.Collision;
using WesternSpace.Utility;

namespace WesternSpace.TilingEngine
{
    class DestructableTile : Tile, IDamageable, ISpriteCollideable
    {
        #region IDamageable Members

        private float maxHealth;

        public float MaxHealth
        {
            get { return maxHealth; }
        }

        private float currentHealth;

        public float CurrentHealth
        {
            get { return currentHealth; }
        }

        /// <summary>
        /// I still don't know what this is.
        /// </summary>
        public float MitigationFactor
        {
            get { return 1.0f; }
        }

        /// <summary>
        /// What are the conventions for this? Where do these get used?
        /// </summary>
        public DamageCategory TakesDamageFrom
        {
            get { return DamageCategory.All; }
        }


        public void TakeDamage(IDamaging damageItem)
        {
            currentHealth -= damageItem.AmountOfDamage;

            if (currentHealth <= 0)
            {
                Destruct();
            }
        }

        #endregion

        #region ISpriteCollideable Members

        private int idNumber;

        public int IdNumber
        {
            get
            {
                return idNumber;
            }
            set
            {
                idNumber = value;
            }
        }

        public Rectangle Rectangle
        {
            // Should I instead *store* the rectangle, under the (fairly sane) assumption that
            // the width and height of a tileMap's tiles shouldn't change? I'm not for now. This
            // may be kinda slow, so when things start to crawl, I'll probably change this and enforce
            // that tilemap's width/height cannot change.
            get { return new Rectangle(Map.TileWidth*x, Map.TileHeight*y, Map.TileWidth, Map.TileHeight); }
        }

        /// <summary>
        /// Not implemented. What's this for, anyway?
        /// </summary>
        public Microsoft.Xna.Framework.Graphics.SpriteEffects collideableFacing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Boolean removeFromCollisionRegistration;
        public bool removeFromRegistrationList
        {
            get
            {
                return removeFromCollisionRegistration;
            }
            set
            {
                removeFromCollisionRegistration = value;
            }
        }

        public void OnSpriteCollision(ISpriteCollideable objectCollidedWith)
        {
            IDamaging damagingObject = objectCollidedWith as IDamaging;
            if (damagingObject != null)
            {
                TakeDamage(damagingObject);
            }
        }

        #endregion

        /// <summary>
        /// The map this destructable tile belongs to.
        /// </summary>
        /// 
        public TileMap Map
        {
            get { return World.Map; }
        }

        private World world;

        public World World
        {
            get { return world; }
        }

        /// <summary>
        /// The x and y coordinates of this tile in the map.
        /// </summary>
        private int x, y;

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }

        private List<IDestructionEffect> destructionEffects;

        internal List<IDestructionEffect> DestructionEffects
        {
            get { return destructionEffects; }
        }

        /// <summary>
        /// Create a new DestructableTile that gets it's attributes (textures, edges) from 
        /// another tile.
        /// </summary>
        /// <param name="tile">The tile to copy subTextures and edges from.</param>
        /// <param name="maxHealth">The initial "health" of this tile.</param>
        public DestructableTile(Tile tile, World world, int x, int y, float maxHealth)
            :base(tile)
        {
            this.world = world;
            this.x = x;
            this.y = y;
            this.maxHealth = maxHealth;
            this.destructionEffects = new List<IDestructionEffect>();
        }

        public DestructableTile(Tile tile, World world, int x, int y, float maxHealth, List<IDestructionEffect> destructionEffects)
            : base(tile)
        {
            this.world = world;
            this.x = x;
            this.y = y;
            this.maxHealth = maxHealth;
            this.destructionEffects = destructionEffects;
        }

        /// <summary>
        /// No DestructableTile is considered empty regardless of it's
        /// edges or textures.
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return false;
        }

        private bool destructed = false;

        /// <summary>
        /// Called when this DestructableTile is destroyed.
        /// </summary>
        private void Destruct()
        {
            if (!destructed)
            {

                foreach (IDestructionEffect effect in DestructionEffects)
                {
                    effect.OnDestruct(this);
                }

                base.textures[Map.LayerCount - 1, Map.SubLayerCount - 1] = null;
                base.SetSolid(false);
                Map.SetTile(this, x, y);
                this.removeFromCollisionRegistration = true;
                destructed = true;
            }
        }
    }
}
