using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using FarseerPhysics.Collision.Shapes;

namespace RibbonsGameplay
{
    class PinObject : BoxObject
    {
        public PinObject(World world, Vector2 position)
        {
            // This will be used to create fasters between the ribbon and the background
            Body roller = BodyFactory.CreateCircle(world, .1f, 1, this);
            Body roller2 = BodyFactory.CreateCircle(world, .25f, 1, this);
            roller.BodyType = BodyType.Static;
            roller2.BodyType = BodyType.Dynamic;
            roller.Position = position;
            roller2.Position = position;
            Joint J = JointFactory.CreateRevoluteJoint(world, roller, roller2, new Vector2());
        }
    }
}
