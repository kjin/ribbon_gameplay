#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace RibbonsGameplay
{
        /// <summary>
    /// The primary controller class for an XNA game.
    /// </summary>
        public class GameEngine : Microsoft.Xna.Framework.Game
        {

            #region Constants

                public const float GRAVITY = 1000f;

            #endregion

            #region Fields

                // Used to draw the game onto the screen (VIEW CLASS)
                protected GameCanvas canvas;

                // Used to load the sounds and graphics (CONTROLLER CLASS)
                protected ContentManager content;

                // Read input from keyboard or game pad (CONTROLLER CLASSES)
                protected MainInputController inputController;

                // List of objects
                protected List<Object> objects;

                // To process the sensor callback
                protected HashSet<Fixture> sensorFixtures = new HashSet<Fixture>();

                // The seamstress and ribbon
                protected SeamstressObject seamstress;
                protected RibbonObject ribbon;

                // Controllers for the seamstress and ribbon
                protected SeamstressForceController seamstressController;
                protected RibbonForceController ribbonController;

                // Physics simulator
                protected World world;

                // Test texture
                Texture2D background;
                protected Texture2D boxtext;

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
            /// Perform any initialization necessary before running the game.  This
            /// includes both (preliminary) model and view creation.
            /// </summary>
            protected override void Initialize()
            {
                canvas.Initialize(this);

                world = new World(new Vector2(0,GRAVITY));

                world.ContactManager.BeginContact += ContactStarted;
                world.ContactManager.EndContact += ContactEnded;

                ribbon = new RibbonObject();
                seamstress = new SeamstressObject();

                seamstressController = new SeamstressForceController(seamstress);
                world.AddController(seamstressController);

                ribbonController = new RibbonForceController(ribbon);
                world.AddController(ribbonController);

                objects = new List<Object>();
                objects.Add(ribbon);
                objects.Add(seamstress);

                inputController = new MainInputController(seamstress, ribbon);               

                base.Initialize();
            }

            private void MakeLevel()
            {
                BoxObject testbox = new BoxObject();

                testbox.ActivatePhysics(world, boxtext);
                testbox.Position = new Vector2(50, 300);
                testbox.BodyType = BodyType.Static;
                objects.Add(testbox);
            }

            /// <summary>
            /// Load all graphic and audio content (this game has no audio).
            /// </summary>
            protected override void LoadContent()
            {
                // General view content
                canvas.LoadContent(content);

                Texture2D spritejump = canvas.GetTexture("spritejump");
                Texture2D spritestanding = canvas.GetTexture("standing");
                Texture2D spritefalling = canvas.GetTexture("spritefall");
                Texture2D spritewalking = canvas.GetTexture("walkfstrip");
                seamstress.ActivatePhysics(world, spritestanding, spritejump, spritefalling, spritewalking);

                background = canvas.GetTexture("backgrounds/bluemt");

                ribbon.ActivatePhysics(world);

                boxtext = canvas.GetTexture("64x64platform");

                MakeLevel();
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

            private bool ContactStarted(Contact contact)
            {
                Body body1 = contact.FixtureA.Body;
                Body body2 = contact.FixtureB.Body;
                var ud1 = contact.FixtureA.UserData;
                var ud2 = contact.FixtureB.UserData;

                // See if we have landed on the ground.
                if ((seamstress.SensorName.Equals(ud2) && seamstress != body1.UserData) ||
                   (seamstress.SensorName.Equals(ud1) && seamstress != body2.UserData))
                {
                    seamstress.IsGrounded = true;
                    sensorFixtures.Add(seamstress == body1.UserData ? contact.FixtureB : contact.FixtureA);
                }

                return true; // keep the contact
            }


            private void ContactEnded(Contact contact)
            {
                Body body1 = contact.FixtureA.Body;
                Body body2 = contact.FixtureB.Body;
                var ud1 = contact.FixtureA.UserData;
                var ud2 = contact.FixtureB.UserData;

                // See if we are off the ground.
                if ((seamstress.SensorName.Equals(ud2) && seamstress != body1.UserData) ||
                   (seamstress.SensorName.Equals(ud1) && seamstress != body2.UserData))
                {
                    sensorFixtures.Remove(seamstress == body1.UserData ? contact.FixtureB : contact.FixtureA);
                    if (sensorFixtures.Count == 0)
                    {
                        seamstress.IsGrounded = false;
                    }
                }
            }


            /// <summary>
            /// Read user input, calculate physics, and update the models.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Update(GameTime gameTime)
            {
                inputController.ReadInput();

                float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

                world.Step(dt);
                seamstress.Update(dt);

                base.Update(gameTime);
            }

            /// <summary>
            /// This is called when the game should draw itself.
            /// </summary>
            /// <param name="gameTime">Provides a snapshot of timing values.</param>
            protected override void Draw(GameTime gameTime)
            {
                canvas.Reset();

                canvas.BeginSpritePass(BlendState.AlphaBlend);
                canvas.DrawSprite(background, Color.White, new Vector2(background.Width / 2, background.Height / 2));
                foreach (Object o in objects)
                {
                    o.Draw(canvas);
                }
                canvas.EndSpritePass();
                
                base.Draw(gameTime);
            }
            #endregion

        }
    }
