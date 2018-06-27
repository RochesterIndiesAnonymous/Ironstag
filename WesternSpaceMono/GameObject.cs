using Microsoft.Xna.Framework;

using WesternSpace.Screens;
using WesternSpace.Interfaces;

namespace WesternSpace
{
    /// <summary>
    /// Adds additional functionality to GameComponent necessary for our game
    /// </summary>
    public class GameObject : GameComponent, IScreenComponent
    {
        private Screen parentScreen;

        public GameObject(Screen parentScreen)
            : base(ScreenManager.Instance)
        {
            ParentScreen = parentScreen;
        }

        #region IScreenComponent Members

        public Screen ParentScreen
        {
            get
            {
                return parentScreen;
            }
            set
            {
                parentScreen = value;
            }
        }

        #endregion
    }
}