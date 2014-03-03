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

        #region Methods

            public SeamstressForceController(SeamstressObject s)
                : base(ControllerType.AbstractForceController)
            {
                seamstress = s;
            }


            public override void Update(float dt)
            {
                throw new NotImplementedException();
            }

        #endregion

    }
}
