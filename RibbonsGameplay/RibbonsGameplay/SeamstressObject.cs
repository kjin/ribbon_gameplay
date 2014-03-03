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

        //includes initialize physics
            public SeamstressObject(World w) : base(w)
            {

            }

            public override void Draw(GameCanvas g)
            {

            }

            public void OnGround()
            {
                isGrounded = true;
            }

            public void OffGround()
            {
                isGrounded = false;
            }

        #endregion

    }
}
