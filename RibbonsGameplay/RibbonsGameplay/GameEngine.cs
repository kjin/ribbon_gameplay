using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;
using RibbonsGameplay.GameCanvas;
#endregion

namespace RibbonsGameplay
{
    class GameEngine
    {
        /// <summary>
    /// The primary controller class for an XNA game.
    /// </summary>
        public class GameEngine : Microsoft.Xna.Framework.Game
        {

            #region Constants
            
            #endregion

            #region Fields

            // Used to draw the game onto the screen (VIEW CLASS)
            protected GameCanvas canvas;

            // Used to load the sounds and graphics (CONTROLLER CLASS)
            protected ContentManager content;

            // Read input from keyboard or game pad (CONTROLLER CLASSES)
            protected MainInputController inputController;

            #endregion

            #region Initialization
            /// <summary>
            ///  Constructor to create a new instance of our game.
            /// </summary>
            public GameEngine()
            {
                // Tell the program to load all files relative to the "Content" directory.
                content = new ContentManager(Services);
                content.RootDirectory = "Content";
                canvas = new GameCanvas(this);
            }

            /// <summary>
            /// Preform any initialization necessary before running the game.  This
            /// includes both (preliminary) model and view creation.
            /// </summary>
            protected override void Initialize()
            {
                canvas.Initialize(this);
                //inputController = new MainInputController();
                base.Initialize();
            }

            /// <summary>
            /// Load all graphic and audio content (this game has no audio).
            /// </summary>
            protected override void LoadContent()
            {
                // General view content
                canvas.LoadContent(content);

            }

            /// <summary>
            /// Unload all graphic and audio content.
            /// </summary>
            protected override void UnloadContent()
            {
                // Content managers make this step easy.
                content.Unload();
            }
            #endregion

            #region Game Loop
            /// <summary>
            /// Read user input, calculate physics, and update the models.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                inputController.ReadInput();

                base.Update(gameTime);
            }

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                canvas.Reset();

                base.Draw(gameTime);
            }
            #endregion

        }
    }
}
