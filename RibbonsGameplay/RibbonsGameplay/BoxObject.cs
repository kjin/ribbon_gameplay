using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace RibbonsGameplay
{
    class BoxObject : Object
    {

        #region Fields
            // The geometric data.
            protected Body body;
            protected Fixture fixture;
        #endregion

        public virtual void Draw(GameCanvas g)
        {
        }
    }
}
