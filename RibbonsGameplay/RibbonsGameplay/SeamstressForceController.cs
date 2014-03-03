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


            public override void Update(float dt)
            {
                //throw new NotImplementedException();
            }

        #endregion

    }
}
