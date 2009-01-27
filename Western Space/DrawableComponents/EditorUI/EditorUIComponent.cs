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
        // Time the mouse is pressed inside this component in milliseconds
        private int[] heldTime;

        // Time the mouse has been inside this component in milliseconds
        // Negative values describe the amount of time *outside* (see OutsideTime)
        private int insideTime;

        public int InsideTime
        {
            get { return insideTime; }
        }

        public int OutsideTime
        {
            get { return -insideTime; }
        }

        public int[] HeldTime
        {
            get { return heldTime; }
        }

        // This element is used to represent the interactive area of 
        //  this UI component.
        protected RectangleF bounds;

        virtual public RectangleF Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        private Microsoft.Xna.Framework.Graphics.Color color;

        public Microsoft.Xna.Framework.Graphics.Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private BetterMouse mouse;

        public BetterMouse Mouse
        {
            get { return mouse; }
            set { mouse = value; }
        }

        private IInputManagerService inputManager;

        private bool mouseWasInside;

        // Mouse entering/leaving:

        virtual public void OnMouseEnter()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.Red;
        }

        virtual protected void WhileMouseInside()
        {
        }

        virtual protected void OnMouseLeave()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
        }

        virtual protected void WhileMouseOutside()
        { 
        }

        // Mouse clicking/unclicking inside:
        virtual protected void OnMouseClick(int button)
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

        virtual protected void WhileMouseHeld(int button)
        {
        }

        virtual protected void OnMouseUnclick(int button)
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.Red;
        }

        virtual protected void OnMouseScroll(int amount)
        {
        }

        // Mouse clicking/unclicking outside:
        virtual protected void OnMouseClickOutside(int button)
        { 
        }

        virtual protected void WhileMouseHeldOutside(int button)
        {
        }

        virtual protected void OnMouseUnClickOutside(int button)
        {
        }

        virtual protected void OnMouseScrollOutside(int amount)
        {
        }


        virtual protected bool MouseIsInside()
        {
            return bounds.Contains(new PointF(inputManager.BetterMouse.ScaledPosition.X, inputManager.BetterMouse.ScaledPosition.Y));
        }

        public EditorUIComponent(Game game, SpriteBatch spriteBatch, RectangleF bounds)
            :base(game, spriteBatch, new Vector2(bounds.X, bounds.Y))
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            this.mouseWasInside = false;
            this.bounds = bounds;
            this.heldTime = new int[3];
            this.insideTime = 0;
        }

        public override void Initialize()
        {
            this.inputManager = (IInputManagerService)Game.Services.GetService(typeof(IInputManagerService));
            this.mouse = inputManager.BetterMouse;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            bool mouseIsInside = MouseIsInside();

            if (mouseIsInside)
            {
                if (!mouseWasInside)
                {
                    OnMouseEnter();
                    insideTime = 0;
                }
                insideTime = insideTime + gameTime.ElapsedGameTime.Milliseconds;
                WhileMouseInside();
                for (int i = 0; i < 3; ++i)
                {
                    if (mouse.ButtonsClicked[i])
                        OnMouseClick(i);
                    if (mouse.ButtonsHeld[i] > 0)
                    {
                        heldTime[i] += gameTime.ElapsedGameTime.Milliseconds;
                        WhileMouseHeld(i);
                    }
                    else
                    {
                        heldTime[i] = 0;
                    }
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
                    insideTime = 0;
                }

                insideTime = insideTime - gameTime.ElapsedGameTime.Milliseconds;

                WhileMouseOutside();

                for (int i = 0; i < 3; ++i)
                {
                    if (mouse.ButtonsClicked[i])
                        OnMouseClickOutside(i);
                    if (mouse.ButtonsHeld[i] > 0)
                    {
                        heldTime[i] += gameTime.ElapsedGameTime.Milliseconds;
                        WhileMouseHeldOutside(i);
                    }
                    else
                    {
                        heldTime[i] = 0;
                    }
                    if (mouse.ButtonsUnclicked[i])
                        OnMouseUnClickOutside(i);
                }
                int scrollAmount = mouse.ScrollAmount;
                if (scrollAmount != 0)
                {
                    OnMouseScrollOutside(scrollAmount);
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
