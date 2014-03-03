using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace RibbonsGameplay
{
    public class BoxObject : Object
    {

        #region Fields

            // The geometric data.
            protected Body body;
            protected Fixture fixture;

        #endregion

        #region Methods

            public BoxObject(World w)
            {

            }

            public virtual void Draw(GameCanvas g)
            {

            }

        #endregion
    }
}
