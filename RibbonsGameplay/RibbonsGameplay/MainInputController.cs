﻿#region File Description
//-----------------------------------------------------------------------------
// MainInputController.cs
//
// Device-independent input manager.
//
// As this application shows, input controllers can be game specific.
// Sometimes you want to use a mouse, and sometimes you want to use
// a keyboard.
//
// This input controller is used by the GameEngine to handle the world
// state.  It allows the player to move between worlds and exit the
// game.  It does not process game-specific input for any world.
//
// Author: Walker M. White
// Based on original PhysicsDemo Lab by Don Holden, 2007
// MonoGame version, 2/14/2014
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
#endregion

/// <summary>
/// The namespace, or package, of this application.
/// </summary>
namespace RibbonsGameplay {

    /// <summary>
    /// Class for reading player input. Supports both a keyboard and X-Box controller.
    /// </summary>
    public class MainInputController {

    #region Fields
        // Fields to manage game state
        // Pairs are necessary to ignore when a button is held down.
        // We just want the first press.
        protected bool resetPressed;
        protected bool resetPrevious;
        protected bool nextPressed;
        protected bool nextPrevious;
        protected bool prevPressed;
        protected bool prevPrevious;
        protected bool exitPressed;
        protected bool exitPrevious;
    #endregion

    #region Properties (READ-ONLY)
        /// <summary>
        /// Whether the reset button was pressed.
        /// </summary>
        public bool Reset {
            get { return resetPressed && !resetPrevious; }
        }

        /// <summary>
        /// Whether the player pressed to move to the next world.
        /// </summary>
        public bool Next {
            get { return nextPressed && !nextPrevious; }
        }

        /// <summary>
        /// Whether the player pressed to move to the previous world.
        /// </summary>
        public bool Previous {
            get { return prevPressed && !prevPrevious; }
        }

        /// <summary>
        /// Whether the exit button was pressed.
        /// </summary>
        public bool Exit {
            get { return exitPressed && !exitPrevious; }
        }
    #endregion

    #region Methods
        /// <summary>
        /// Creates a new input controller.
        /// </summary>
        public MainInputController() { }

        /// <summary>
        /// Reads the input for the player and converts the result into game logic.
        /// </summary>
        public void ReadInput() {
            // Copy state from last animation frame
            // Helps us ignore buttons that are held down
            resetPrevious = resetPressed;
            nextPrevious = nextPressed;
            prevPrevious = prevPressed;
            exitPrevious = exitPressed;

            // Check to see if a GamePad is connected
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            if (gamePad.IsConnected) {
                ReadGamepadInput();
            } else {
                ReadKeyboardInput();
            }
        }

        /// <summary>
        /// Reads input from an X-Box controller connected to this computer.
        /// </summary>
        protected void ReadGamepadInput() {
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            // Game State
            resetPressed = gamePad.Buttons.Y == ButtonState.Pressed;
            nextPressed = gamePad.Buttons.RightShoulder == ButtonState.Pressed;
            prevPressed = gamePad.Buttons.LeftShoulder == ButtonState.Pressed;
            exitPressed = gamePad.Buttons.Back == ButtonState.Pressed;
        }

        /// <summary>
        /// Reads input from the keyboard.
        /// </summary>
        private void ReadKeyboardInput() {
            KeyboardState keyboard = Keyboard.GetState();

            resetPressed = keyboard.IsKeyDown(Keys.R);
            nextPressed = keyboard.IsKeyDown(Keys.N);
            prevPressed = keyboard.IsKeyDown(Keys.P);
            exitPressed = keyboard.IsKeyDown(Keys.Escape);
        }
    #endregion

    }
}