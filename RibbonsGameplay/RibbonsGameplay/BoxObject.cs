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

        #region Properties
            public virtual Vector2 Position
            {
                get { return position; }
                set { position = value; }
            }
        #endregion


        #region Methods
            /// <summary>
            /// Create a new box at the origin. Don't forget to add the texture and activate physics.
            /// </summary>
            public BoxObject()
            {
                this.position = new Vector2(50.0f, 50.0f);
            }
            
            /// <summary>
            /// Creates the physics Body for this object, adding it to the world.
            /// </summary>
            /// <remarks>
            /// This method depends on the internal method CreateShape() for
            /// the specific body allocation. You should override that method,
            /// not this one, for specific physics objects.
            /// </remarks>
            /// <param name="world">Farseer world that stores body</param>
            /// <param name="texture">Texture for physics object sizing</param>
            /// <returns><c>true</c> if object allocation succeeded</returns>
            public virtual bool ActivatePhysics(World world, Texture2D texture)
            {
                // Initialize
                this.texture = texture;
                this.dimension = new Vector2((float)texture.Width, (float)texture.Height);
                this.scale = new Vector2(dimension.X / texture.Width, dimension.Y / texture.Height);
                
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
                    isActive = true;
                }
                return isActive;
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
