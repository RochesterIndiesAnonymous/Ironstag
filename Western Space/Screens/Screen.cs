using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Screens
{
    /// <summary>
    /// A screen of components that it is responsible for drawing
    /// </summary>
    public class Screen : DrawableGameComponent
    {
        /// <summary>
        /// The ordered list of updating components
        /// </summary>
        private List<IUpdateable> updatingComponents;

        /// <summary>
        /// The ordered list of drawing components
        /// </summary>
        private List<IDrawable> drawingComponents;

        private string name;

        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Notes whether the screen has been initialized. This is used to determine if we should initialize a component or not.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Notes whether the screen has been initialized. This is used to determine if we should initialize a component or not.
        /// </summary>
        protected bool IsInitialized
        {
            get { return isInitialized; }
            set { isInitialized = value; }
        }

        /// <summary>
        /// The list of components this screen is responsible for drawing.
        /// </summary>
        private GameComponentCollection components;

        /// <summary>
        /// The list of components this screen is responsible for drawing.
        /// </summary>
        public GameComponentCollection Components
        {
            get { return components; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Screen(Game game, string name)
            : base(game)
        {
            components = new GameComponentCollection();
            components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(ComponentAdded);
            components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(ComponentRemoved);

            isInitialized = false;
            this.name = name;

            updatingComponents = new List<IUpdateable>();
            drawingComponents = new List<IDrawable>();
        }

        /// <summary>
        /// Initializes all components in the screen
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (IGameComponent gc in components)
            {
                gc.Initialize();
            }

            isInitialized = true;
        }

        /// <summary>
        /// Updates all the updating components in this screen
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Create a temporary list so we don't crash if
            // a component is added to the collection while
            // updating
            List<IUpdateable> updating = new List<IUpdateable>();

            // Populate the temporary list
            foreach (IUpdateable updateable in updatingComponents)
            {
                updating.Add(updateable);
            }

            // Update all components that have been initialized
            foreach (IUpdateable updateable in updating)
            {
                if (updateable.Enabled)
                {
                    updateable.Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Draws all drawable components to the screen
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            List<IDrawable> drawing = new List<IDrawable>();

            foreach (IDrawable drawable in drawingComponents)
            {
                drawing.Add(drawable);
            }

            foreach (IDrawable drawable in drawing)
            {
                if (drawable.Visible)
                {
                    drawable.Draw(gameTime);
                }
            }
        }

        /// <summary>
        /// Adds the component in the appropriate order
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Information pertaining to the state of the event</param>
        private void ComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            if (this.isInitialized)
            {
                e.GameComponent.Initialize();
            }

            // e.GameComponent is an IGameComponent, but we need the events in the IDrawable
            IDrawable drawableComponent = e.GameComponent as IDrawable;

            if (drawableComponent != null)
            {
                // subscribe to the order changing events
                drawableComponent.DrawOrderChanged += new EventHandler(this.ComponentDrawOrderChanged);

                // find where this drawing component belongs
                int index = this.drawingComponents.BinarySearch(drawableComponent, Utility.DrawOrderComparer.Default);
                if (index < 0)
                {
                    index = ~index;
                    while (index < this.drawingComponents.Count && this.drawingComponents[index].DrawOrder == drawableComponent.DrawOrder)
                    {
                        index++;
                    }
                    // insert it at the calculated index
                    this.drawingComponents.Insert(index, drawableComponent);
                }
            }
            
            // check to see if it is just a component
            IUpdateable component = e.GameComponent as IUpdateable;

            if (component != null)
            {
                // subscribe to the update order change event
                component.UpdateOrderChanged += new EventHandler(ComponentUpdateOrderChanged);

                // find where this drawing component belongs
                int index = this.updatingComponents.BinarySearch(component, Utility.UpdateOrderComparer.Default);
                if (index < 0)
                {
                    index = ~index;
                    // calculate the index
                    while (index < this.updatingComponents.Count && this.updatingComponents[index].UpdateOrder == component.UpdateOrder)
                    {
                        index++;
                    }

                    // insert at the calculated index
                    this.updatingComponents.Insert(index, component);
                }
            }
        }

        /// <summary>
        /// Removes the component in the appropriate order
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Information pertaining to the state of the event</param>
        private void ComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            this.components.Remove(e.GameComponent);

            // check to see if its drawable
            IDrawable drawableComponent = e.GameComponent as IDrawable;

            if (drawableComponent != null)
            {
                // remove from our drawing components and unhook the event
                this.drawingComponents.Remove(drawableComponent);
                drawableComponent.DrawOrderChanged -= new EventHandler(this.ComponentDrawOrderChanged);
            }

            // check to see if it is just a component
            IUpdateable component = e.GameComponent as IUpdateable;

            if (component != null)
            {
                // remove from the updating components and unhook the event
                this.updatingComponents.Remove(component);
                component.UpdateOrderChanged -= new EventHandler(this.ComponentUpdateOrderChanged);
            }
        }

        /// <summary>
        /// Called when a game component changes its update order
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Information pertaining to the state of the event</param>
        private void ComponentUpdateOrderChanged(object sender, EventArgs e)
        {
            // check to see if it is just a component
            IUpdateable component = sender as IUpdateable;

            if (component != null)
            {
                this.updatingComponents.Remove(component);

                // find where this drawing component belongs
                int index = this.updatingComponents.BinarySearch(component, Utility.UpdateOrderComparer.Default);
                if (index < 0)
                {
                    index = ~index;
                    // calculate the index
                    while (index < this.updatingComponents.Count && this.updatingComponents[index].UpdateOrder == component.UpdateOrder)
                    {
                        index++;
                    }

                    // insert at the calculated index
                    this.updatingComponents.Insert(index, component);
                }
            }
        }

        /// <summary>
        /// Called when a game component changes its draw order
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Information pertaining to the state of the event</param>
        private void ComponentDrawOrderChanged(object sender, EventArgs e)
        {
            // e.GameComponent is an IGameComponent, but we need the events in the IDrawable
            IDrawable drawableComponent = sender as IDrawable;

            if (drawableComponent != null)
            {
                this.drawingComponents.Remove(drawableComponent);

                int index = this.drawingComponents.BinarySearch(drawableComponent, Utility.DrawOrderComparer.Default);
                if (index < 0)
                {
                    index = ~index;
                    while (index < this.drawingComponents.Count && this.drawingComponents[index].DrawOrder == drawableComponent.DrawOrder)
                    {
                        index++;
                    }
                    // insert it at the calculated index
                    this.drawingComponents.Insert(index, drawableComponent);
                }
            }
        }

    }
}
