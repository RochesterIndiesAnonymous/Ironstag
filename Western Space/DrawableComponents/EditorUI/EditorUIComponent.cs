using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.Input;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.EditorUI
{
    /// <summary>
    /// An object that can be interacted with via the mouse.
    /// </summary>
    public class EditorUIComponent : DrawableGameObject
    {
        // This element is used to represent the interactive area of 
        //  this UI component.
        private RectangleF bounds;

        private Microsoft.Xna.Framework.Graphics.Color color;

        public Microsoft.Xna.Framework.Graphics.Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        private IInputManagerService inputManager;

        private bool mouseWasInside;

        virtual public void OnMouseEnter()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.Red;
        }

        virtual public void OnMouseLeave()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
        }

        virtual public void OnMouseClick(int button)
        {
            if (button == 0) // Left click
            {
                this.Color = Microsoft.Xna.Framework.Graphics.Color.LightGreen;
            }
            else if (button == 1) // Middle click
            {
                this.Color = Microsoft.Xna.Framework.Graphics.Color.Yellow;
            }
            else if (button == 2) // Right click
            {
                this.Color = Microsoft.Xna.Framework.Graphics.Color.Orange;
            }
        }

        virtual public void OnMouseUnclick(int button)
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.Red;
        }

        virtual public void OnMouseScroll(int amount)
        {
        }

        virtual public bool MouseIsInside()
        {
            return bounds.Contains(new PointF(inputManager.BetterMouse.ScaledPosition.X, inputManager.BetterMouse.ScaledPosition.Y));
        }

        public EditorUIComponent(Game game, SpriteBatch spriteBatch, RectangleF bounds)
            :base(game, spriteBatch, new Vector2(bounds.X, bounds.Y))
        {
            this.mouseWasInside = false;
            this.bounds = bounds;
        }

        public override void Initialize()
        {
            inputManager = (IInputManagerService)Game.Services.GetService(typeof(IInputManagerService));
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            BetterMouse mouse = inputManager.BetterMouse;
            bool mouseIsInside = MouseIsInside();

            if (mouseIsInside)
            {
                if (!mouseWasInside)
                {
                    OnMouseEnter();
                }
                for (int i = 0; i < 3; ++i)
                {
                    if (mouse.ButtonsClicked[i])
                        OnMouseClick(i);
                    if (mouse.ButtonsUnclicked[i])
                        OnMouseUnclick(i);
                }
                int scrollAmount = mouse.ScrollAmount;
                if (scrollAmount != 0)
                {
                    OnMouseScroll(scrollAmount);
                }
            }
            else
            {
                if (mouseWasInside)
                {
                    OnMouseLeave();
                }
            }

            mouseWasInside = mouseIsInside;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height);
            PrimitiveDrawer.Instance.DrawRect(this.SpriteBatch, rect, this.color);
            base.Draw(gameTime);
        }
    }
}
