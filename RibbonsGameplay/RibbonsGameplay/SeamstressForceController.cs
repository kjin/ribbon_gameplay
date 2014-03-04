#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace RibbonsGameplay
{
    public class SeamstressForceController : Controller
    {

        #region Fields
            private SeamstressObject seamstress;
        #endregion

        #region Properties (READ-WRITE)
            /// <summary>
            /// The currently active avatar
            /// </summary>
            /// <remarks>
            /// The controller can only affect one avatar at a time.
            /// </remarks>
            public SeamstressObject Seamstress
            {
                get { return seamstress; }
                set { seamstress = value; }
            }
        #endregion

        #region Methods
            /// <summary>
            /// Create a new controller for the given avatar
            /// </summary>
            /// <param name="s">The avatar</param>
            public SeamstressForceController(SeamstressObject s)
                : base(ControllerType.AbstractForceController)
            {
                seamstress = s;
            }

            /// <summary>
            /// Apply appropriate forces while collisions are processed
            /// </summary>
            /// <param name="dt">Timing values from parent loop</param>
            public override void Update(float dt)
            {
                if (!seamstress.Body.Enabled)
                {
                    return;
                }

                Vector2 moveForce = new Vector2(seamstress.Movement, 0.0f);
                Vector2 velocity = seamstress.LinearVelocity;

                // Don't want to be moving. Damp out player motion
                if (moveForce.X == 0f)
                {
                    if (seamstress.IsGrounded)
                    {
                        Vector2 dampForce = new Vector2(-seamstress.GroundDamping * velocity.X, 0);
                        seamstress.Body.ApplyForce(dampForce, seamstress.Position);
                    }
                    else
                    {
                        Vector2 dampForce = new Vector2(-seamstress.AerialDamping * velocity.X, 0);
                        seamstress.Body.ApplyForce(dampForce, seamstress.Position);
                    }
                }

                // Velocity too high, clamp it
                if (Math.Abs(velocity.X) >= seamstress.MaxSpeed)
                {
                    velocity.X = Math.Sign(velocity.X) * seamstress.MaxSpeed;
                    seamstress.LinearVelocity = velocity;
                }
                else
                {
                    seamstress.Body.ApplyForce(moveForce, seamstress.Position);
                }
                
                // Jump!
                if (seamstress.IsJumping)
                {
                    Vector2 impulse = new Vector2(0,SeamstressObject.SEAMSTRESS_JUMPFORCE);
                    seamstress.Body.ApplyLinearImpulse(impulse, seamstress.Position);
                }
            }

        #endregion

    }
}
