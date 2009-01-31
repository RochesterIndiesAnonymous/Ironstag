using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Drawing;

using WesternSpace.Interfaces;
using WesternSpace.Screens;

namespace WesternSpace.Services
{
    /// <summary>
    /// The implementation of the camera service
    /// </summary>
    class CameraService : GameComponent, ICameraService
    {
        /// <summary>
        /// The position of the camera within the world
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The visible area of the world in the form of a rectangle
        /// </summary>
        private RectangleF visibleArea;

        private Matrix viewMatrix;

        private IScreenResolutionService resolutionService;

        private IInputManagerService inputManager;

        private const int SCROLL_SPEED = 14;


        #region ICamera Members

        /// <summary>
        /// The non-rounded position of our camera in the world.
        /// </summary>
        public Vector2 RealPosition
        {
            get { return position; }
        }

        private Vector2 roundedPosition;

        /// <summary>
        /// The position of the camera within the world. Rounded to the nearest int.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return roundedPosition;
            }
            set
            {
                position = value;
                visibleArea.X = position.X;
                visibleArea.Y = position.Y;
                roundedPosition = new Vector2((float)Math.Floor(position.X + 0.5f), (float)Math.Floor(position.Y + 0.5f));
            }
        }

        /// <summary>
        /// The screen position of the camera.
        /// </summary>
        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        /// <summary>
        /// The visible area of the world in the form of a rectangle
        /// </summary>
        public RectangleF VisibleArea
        {
            get
            {
                return visibleArea;
            }
        }

        /// <summary>
        /// The current view transformation matrix for sprite batches
        /// </summary>
        public Matrix CurrentViewMatrix
        {
            get { return viewMatrix; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this camera is associated with</param>
        public CameraService(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes the internal state of the camera
        /// </summary>
        public override void Initialize()
        {
            position = Vector2.Zero;
            viewMatrix = Matrix.Identity;

            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));
            resolutionService = (IScreenResolutionService)this.Game.Services.GetService(typeof(IScreenResolutionService));
            if(this.visibleArea == null)
                this.visibleArea = new RectangleF();

            base.Initialize();
        }

        /// <summary>
        /// Updates the camera position based on input
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            CreateViewTransformationMatrix();
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the visible area rectangle of the camera
        /// </summary>
        public void UpdateVisibleArea(ScreenResolutionService resolutionService)
        {
            visibleArea = new RectangleF(position.X, position.Y, resolutionService.StartTextureWidth, resolutionService.StartTextureHeight);
        }

        /// <summary>
        /// Updates the view transformation matrix
        /// </summary>
        public void CreateViewTransformationMatrix()
        {
            Vector3 matrixRotationOrigin = new Vector3(Position, 0);
            Vector3 matrixScreenPosition = new Vector3(ScreenPosition, 0.0f);

            viewMatrix = Matrix.CreateTranslation(-matrixRotationOrigin) * Matrix.CreateTranslation(matrixScreenPosition);
        }
    }
}
