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
    public class RibbonForceController : Controller
    {

        #region Fields

            private RibbonObject ribbon;

        #endregion

        #region Methods

            public RibbonForceController(RibbonObject r)
                : base(ControllerType.AbstractForceController)
            {
                ribbon = r;
            }


            public override void Update(float dt)
            {
                //throw new NotImplementedException();
            }

        #endregion

    }
}
