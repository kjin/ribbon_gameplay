#region File Description
// SeamstressObject.cs
//
// Player avatar for Ribbons: the Ribboning
//
// Largely based on Walker White's Physics lab
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RibbonsGameplay
{
    public class SeamstressObject : BoxObject
    {

        #region Constants
            // Sensor Constants
            public const String SENSOR_NAME = "SeamstressGroundSensor";
            private const float SENSOR_HEIGHT = 0.05f;
            private const float SENSOR_WIDTH_COEF = 0.7f;

            // Physics constants
            private const float DEFAULT_DENSITY = 1.0f;

            // Movement constants
            private const float SEAMSTRESS_FORCE = 20.0f;
            private const float SEAMSTRESS_DAMPING = 10.0f;
            private const float SEAMSTRESS_MAXSPEED = 6.0f;

            // Cooldown constants
            private const int JUMP_COOLDOWN = 30;

            // textures
            protected Texture2D standing;
            protected Texture2D jumping;
        #endregion

        #region Properties

            /// <summary>
            /// Name of the ground sensor (used for Farseer)
            /// </summary>
            public String SensorName
            {
                get { return SENSOR_NAME; }
            }

            /// <summary>
            /// Left/right movement of this character.  Result of input times seamstress force.
            /// </summary>
            public float Movement
            {
                get { return movement; }
                set
                {
                    movement = value;
                    // Change facing if appropriate
                    if (movement < 0)
                    {
                        facingRight = false;
                    }
                    else if (movement > 0)
                    {
                        facingRight = true;
                    }
                }
            }

            /// <summary>
            /// Whether the seamstress is actively jumping.
            /// </summary>
            public bool IsJumping
            {
                get { return isJumping && jumpCooldown <= 0 && isGrounded; }
                set { isJumping = value; }
            }

            /// <summary>
            /// Whether or not this dude is touching the ground
            /// </summary>
            public bool IsGrounded
            {
                get { return isGrounded; }
                set { isGrounded = value; }
            }

            /// <summary>
            /// How much force to apply to get the dude moving
            /// </summary>
            public float Force
            {
                get { return SEAMSTRESS_FORCE; }
            }

            /// <summary>
            /// How hard the brakes are applied to get a SEAMSTRESS to stop moving
            /// </summary>
            public float Damping
            {
                get { return SEAMSTRESS_DAMPING; }
            }

            /// <summary>
            /// Upper limit on SEAMSTRESS left-right movement.  Does NOT apply to vertical movement.
            /// </summary>
            public float MaxSpeed
            {
                get { return SEAMSTRESS_MAXSPEED; }
            }

            /// <summary>
            /// Shape of the ground detecting sensor.
            /// </summary>
            public Fixture SensorFixture
            {
                get { return sensorFixture; }
            }

            /// <summary>
            /// Whether this character is facing right
            /// </summary>
            public bool FacingRight
            {
                get { return facingRight; }
            }
        #endregion

        #region Fields
            // Cooldown for seamstress abilities
            private float movement;
            private bool facingRight;

            private int jumpCooldown;
            private bool isJumping;

            // Ground sensor to represent feet
            private Fixture sensorFixture;
            private bool isGrounded;

        #endregion

        #region Methods

        /// <summary>
        /// Create a new seamstress at the origin. Activate Physics.
        /// </summary>
        /// <param name="texture">Avatar texture</param>
        public SeamstressObject(World w, Texture2D standing, Texture2D jumping) : 
            this(w, standing, jumping, Vector2.Zero, new Vector2((float)standing.Width, (float)standing.Height)) { }
    
        /// <summary>
        /// Create a new seamstress object. Activate Physics.
        /// </summary>
        /// <param name="texture">Avatar texture</param>
        /// <param name="pos">Location in world coordinates</param>
        public SeamstressObject(World w, Texture2D standing, Texture2D jumping, Vector2 pos) :
            this(w, standing, jumping, pos, new Vector2((float)standing.Width, (float)standing.Height)) { }

        /// <summary>
        /// Create a new seamstress object. Activate Physics.
        /// </summary>
        /// <param name="texture">Avatar texture</param>
        /// <param name="pos">Location in world coordinates</param>
        /// <param name="dimension">Dimensions in world coordinates</param>
        public SeamstressObject(World w, Texture2D standing, Texture2D jumping, Vector2 pos, Vector2 dimension)
            : base(w, standing)
            {
                this.standing = standing;
                this.jumping = jumping;
            
                // Physics attributes
                bodyType = BodyType.Dynamic;
                density = DEFAULT_DENSITY;

                // Gameplay attributes
                isGrounded = false;
                isJumping = false;

                jumpCooldown = 0;    
            
                // Activate Physics
                body.FixedRotation = true;

                // Ground Sensor
                // -------------
                // We only allow the seamstress to jump when she's on the ground. 
                // Double jumping is not allowed.
                //
                // To determine whether or not the seamstress is on the ground, 
                // we create a thin sensor under her feet, which reports 
                // collisions with the world but has no collision response.
                Vector2 sensorCenter = new Vector2(0, dimension.Y / 2);
                sensorFixture = FixtureFactory.AttachRectangle(dimension.X, SENSOR_HEIGHT, DEFAULT_DENSITY, sensorCenter, body, SensorName);
                sensorFixture.IsSensor = true;
            }

            public override void Draw(GameCanvas g)
            {

            }

        //remove soon
            public void OnGround()
            {
                isGrounded = true;
            }
        //remove soon
            public void OffGround()
            {
                isGrounded = false;
            }

        #endregion

    }
}
