using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Input;

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

        private IInputManagerService inputManager;

        private bool mouseWasInside;

        virtual public void OnMouseEnter()
        { 
        }

        virtual public void OnMouseLeave()
        {
        }

        virtual public void OnMouseClick(int button)
        {
        }

        virtual public void OnMouseUnclick(int button)
        {
        }

        virtual public void OnMouseScroll(int amount)
        {
        }

        virtual public bool MouseIsInside()
        {
            bool returnVal;
            returnVal = bounds.Contains(new PointF(inputManager.MouseState.X, inputManager.MouseState.Y));
            mouseWasInside = returnVal;
            return returnVal;
        }

        public EditorUIComponent(Game game, SpriteBatch spriteBatch, RectangleF bounds)
            :base(game, spriteBatch, new Vector2(bounds.X, bounds.Y))
        {
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
    }
}
