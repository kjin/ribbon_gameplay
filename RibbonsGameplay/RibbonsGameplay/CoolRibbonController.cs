using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RibbonsGameplay
{
    public class CoolRibbonController : Controller
    {

        public CoolRibbonObject ribbon;


        public CoolRibbonController(CoolRibbonObject r) : base(ControllerType.AbstractForceController)
        {
            ribbon = r;
        }

        public override void Update(float dt)
        {
            if (ribbon.left)
            {
                ribbon.MoveLeft();
            }
            else if (ribbon.right)
            {
                ribbon.MoveRight();
            }
            else
            {
                ribbon.Stop();
            }
        }

    }
}
