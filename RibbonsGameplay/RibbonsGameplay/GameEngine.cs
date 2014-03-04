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

                public const float GRAVITY = 9.8f;

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
                protected Texture2D spool_tex;
                protected Texture2D tallbox_tex;
                protected Texture2D saverock;
                protected Texture2D shortflatbox_tex;
                protected Texture2D tallflatbox_tex;

                //Textures
                Texture2D ribbon_segment;

                protected float scale = 32f;

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
                canvas.Scale = scale * Vector2.One;

                world = new World(new Vector2(0,GRAVITY));

                world.ContactManager.BeginContact += ContactStarted;
                world.ContactManager.EndContact += ContactEnded;

                canvas.LoadContent(content);

                ribbon_segment = canvas.GetTexture("ribbon_segment");
                List<Vector2> path = new List<Vector2>();
                path.Add(new Vector2(100, 100));
                path.Add(new Vector2(500, 100));
                ribbon = new RibbonObject(world, ribbon_segment, new Vector2(100,100), ribbon_segment.Width, path);
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

                testbox.ActivatePhysics(world, boxtext, scale);
                testbox.Position = new Vector2(2, 7);
                testbox.BodyType = BodyType.Static;
                objects.Add(testbox);

                BoxObject box1 = new BoxObject();
                box1.ActivatePhysics(world, tallflatbox_tex, scale);
                box1.Position = new Vector2(14, 8);
                box1.BodyType = BodyType.Static;
                objects.Add(box1);

                BoxObject saveRock = new BoxObject();
                saveRock.ActivatePhysics(world, saverock, scale);
                saveRock.Position = new Vector2(20, 20);
                saveRock.BodyType = BodyType.Static;
                saveRock.Body.IgnoreCollisionWith(seamstress.Body);
                objects.Add(saveRock);

                BoxObject savebox = new BoxObject();
                savebox.ActivatePhysics(world, shortflatbox_tex, scale);
                savebox.Position = new Vector2(20, 21.4f);
                savebox.BodyType = BodyType.Static;
                objects.Add(savebox);
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
                seamstress.ActivatePhysics(world, spritestanding, spritejump, spritefalling, spritewalking, scale);

                background = canvas.GetTexture("backgrounds/bluemt");

                ribbon.ActivatePhysics(world, scale);

                boxtext = canvas.GetTexture("64x64platform");
                spool_tex = canvas.GetTexture("64x64thimbs");
                tallbox_tex = canvas.GetTexture("64x128platform");
                saverock = canvas.GetTexture("saverock");
                shortflatbox_tex = canvas.GetTexture("128x32platform");
                tallflatbox_tex = canvas.GetTexture("128x64platform");

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

                if (inputController.Reset) seamstress.Position = new Vector2(2, 2);

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
                canvas.DrawSprite(background, Color.White, new Vector2(background.Width, background.Height) / scale / 2, Vector2.One / scale, 0);
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
