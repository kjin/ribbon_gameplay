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
    public class BoxObject : Object
    {

        #region Fields

            // The geometric data.
            protected Body body;
            protected Fixture fixture;
            protected Vector2 dimension;
            protected Vector2 scale;
            protected Texture2D texture;

            private World world;

            // Control the type of Farseer physics
            protected BodyType bodyType = BodyType.Dynamic;

            // Track physics status for garbage collection
            protected bool toRemove;
            protected bool isActive;
            protected bool isDirty;

            // Buffered physics and geometric data.
            protected Vector2 position;
            protected Vector2 linearVelocity;
            protected float rotation = 0.0f;
            protected float density = 0.0f;
            protected float friction = 0.0f;
            protected float restitution = 0.0f;

        #endregion

        #region Methods

            /// <summary>
            /// Create a new box at the origin.
            /// </summary>
            /// <param name="world">World object</param>
            /// <param name="texture">Object texture</param>
            public BoxObject(World w, Texture2D texture) :
                this(w, texture, Vector2.Zero, new Vector2((float)texture.Width, (float)texture.Height)) { }

            /// <summary>
            /// Create a new box object
            /// </summary>
            /// <param name="world">World object</param>
            /// <param name="texture">Object texture</param>
            /// <param name="pos">Location in world coordinates</param>
            public BoxObject(World w, Texture2D texture, Vector2 pos) :
                this(w, texture, pos, new Vector2((float)texture.Width, (float)texture.Height)) { }

            /// <summary>
            /// Create a new box object
            /// </summary>
            /// <param name="world">World object</param>
            /// <param name="texture">Object texture</param>
            /// <param name="pos">Location in world coordinates</param>
            /// <param name="dimension">Dimensions in world coordinates</param>
            public BoxObject(World w, Texture2D texture, Vector2 pos, Vector2 dimension)
            {
                // Initialize
                this.dimension = dimension;
                scale = new Vector2(dimension.X / texture.Width, dimension.Y / texture.Height);
                this.world = w;

                // Activate Physics
                // Make a body, if possible
                body = BodyFactory.CreateBody(world, this);

                // Only initialize if a body was created.
                if (body != null)
                {
                    body.BodyType = bodyType;
                    body.Position = position;
                    body.LinearVelocity = linearVelocity;
                    body.Rotation = rotation;
                    CreateShape();
                }
            }

            /// <summary>
            /// Create a new box shape.
            /// </summary>
            /// <remarks>
            /// Note the usage of Factories to make fixtures easily.  Looking at this, you
            /// should be able to generalize this idea to an Ellipse or Triangle object. 
            /// Look at the class PolygonTools in Farseer to see shapes we omitted.
            /// </remarks>
            protected void CreateShape()
            {
                if (fixture != null)
                {
                    body.DestroyFixture(fixture);
                }
                Vertices rectangleVertices = PolygonTools.CreateRectangle(dimension.X / 2, dimension.Y / 2);
                PolygonShape rectangleShape = new PolygonShape(rectangleVertices, density);
                fixture = body.CreateFixture(rectangleShape, this);
                fixture.Friction = friction;
                fixture.Restitution = restitution;
                isDirty = false;
            }

            /// <summary>
            /// Draws the physics object.
            /// </summary>
            /// <param name="canvas">Drawing context</param>
            public virtual void Draw(GameCanvas g)
            {
                g.DrawSprite(texture, Color.White, position, scale, rotation);
            }

        #endregion
    }
}
