//using System;
//using System.Collections.Generic;
//using System.Text;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

//namespace WesternSpace
//{
//    class PlayerObject : GameObject
//    {
//        //Constants
//        const int FLYING_SPEED = 160;
//        const int MOVE_UP = -1;
//        const int MOVE_DOWN = 1;
//        const int MOVE_LEFT = -1;
//        const int MOVE_RIGHT = 1;

//        //State
//        enum State
//        {
//            Flying
//        }

//        //Class Variables
//        State currentState = State.Flying;
//        Vector2 direction = Vector2.Zero;
//        Vector2 speed = Vector2.Zero;
//        KeyboardState oldKeyboardState;

//        public PlayerObject(Game1 game, String contentName)
//            : base(game, contentName)
//        { 
//        }

//        public void Update(GameTime gameTime)
//        {
//            KeyboardState currentKeyboardState = Keyboard.GetState();

//            UpdateMovement(currentKeyboardState);

//            oldKeyboardState = currentKeyboardState;

//            base.Update(gameTime, speed, direction);
//        }

//        private void UpdateMovement(KeyboardState currentKeyboardState)
//        {
//            if (currentState == State.Flying)
//            {
//                speed = Vector2.Zero;
//                direction = Vector2.Zero;

//                if (currentKeyboardState.IsKeyDown(Keys.Left) == true)
//                {
//                    speed.X = FLYING_SPEED;
//                    direction.X = MOVE_LEFT;
//                }
//                else if (currentKeyboardState.IsKeyDown(Keys.Right) == true)
//                {
//                    speed.X = FLYING_SPEED;
//                    direction.X = MOVE_RIGHT;
//                }
//                if (currentKeyboardState.IsKeyDown(Keys.Up) == true)
//                {
//                    speed.Y = FLYING_SPEED;
//                    direction.Y = MOVE_UP;
//                }
//                else if (currentKeyboardState.IsKeyDown(Keys.Down) == true)
//                {
//                    speed.Y = FLYING_SPEED;
//                    direction.Y = MOVE_DOWN;
//                }
//            }
//        }
//    }
//}
