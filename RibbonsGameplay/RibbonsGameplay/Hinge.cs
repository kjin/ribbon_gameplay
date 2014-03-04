using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;

namespace RibbonsGameplay
{
    public class Hinge
    {

        public Hinge(Body b, float p)
        {
            body = b;
            position = p;
        }

        public Body body;

        public float position;
    }
}
