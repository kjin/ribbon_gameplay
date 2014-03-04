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
            /// <summary>
            /// Density of this body
            /// </summary>
            /// <remarks>
            /// Changes density requires changing the Fixture.  Therefore, modifying
            /// this attribute flags the object as dirty.  Dirty objects have their
            /// fixtures destroyed and rebuilt in the Update method.
            /// </remarks>
            public virtual float Density
            {
                get { return density; }
                set
                {
                    Debug.Assert(value >= 0, "Density must be >= 0");
                    isDirty = true;
                    density = value;
                    if (density == 0)
                    {
                        BodyType = BodyType.Static;
                    }
                }
            }

            /// <summary>
            /// Whether our object has been flagged for garbage collection
            /// </summary>
            public virtual bool Remove
            {
                get { return toRemove; }
                set { toRemove = value; }
            }

            /// <summary>
            /// X-coordinate for this physics body
            /// </summary>
            /// <remarks>
            /// This value is buffered if it is set before body creation
            /// </remarks>
            public virtual float X
            {
                get { return position.X; }
                set { Position = new Vector2(value, position.Y); }
            }

            /// <summary>
            /// Y-coordinate for this physics body
            /// </summary>
            /// <remarks>
            /// This value is buffered if it is set before body creation
            /// </remarks>
            public virtual float Y
            {
                get { return position.Y; }
                set { Position = new Vector2(position.X, value); }
            }

            /// <summary>
            /// X-coordinate for this physics body's velocity
            /// </summary>
            /// <remarks>
            /// This value is buffered if it is set before body creation
            /// </remarks>
            public virtual float VX
            {
                get { return linearVelocity.X; }
                set { LinearVelocity = new Vector2(value, linearVelocity.Y); }
            }

            /// <summary>
            /// Y-coordinate for this physics body's velocity
            /// </summary>
            /// <remarks>
            /// This value is buffered if it is set before body creation
            /// </remarks>
            public virtual float VY
            {
                get { return linearVelocity.Y; }
                set { LinearVelocity = new Vector2(linearVelocity.X, value); }
            }
        
            /// <summary>
            /// Dimensions of this box
            /// </summary>
            /// <remarks>
            /// We track the difference between the box shape and the texture size
            /// for drawing purposes.  All changes are buffered until Update is called.
            /// </remarks>
            public virtual Vector2 Dimension
            {
                get { return dimension; }
                set
                {
                    Debug.Assert(value.X > 0 && value.Y > 0, "Dimension attributes must be > 0");
                    isDirty = true;
                    dimension = value;
                    scale.X = dimension.X / texture.Width;
                    scale.Y = dimension.Y / texture.Height;
                }
            }

            /// <summary>
            /// Box width
            /// </summary>
            /// <remarks>
            /// We track the difference between the box shape and the texture size
            /// for drawing purposes.  All changes are buffered until Update is called.
            /// </remarks>
            public virtual float Width
            {
                get { return dimension.X; }
                set
                {
                    Debug.Assert(value > 0, "Width must be > 0");
                    isDirty = true;
                    dimension.X = value;
                    scale.X = dimension.X / texture.Width;
                }
            }

            /// <summary>
            /// Box height
            /// </summary>
            /// <remarks>
            /// We track the difference between the box shape and the texture size
            /// for drawing purposes.  All changes are buffered until Update is called.
            /// </remarks>
            public virtual float Height
            {
                get { return dimension.Y; }
                set
                {
                    Debug.Assert(value > 0, "Height must be > 0");
                    isDirty = true;
                    dimension.X = value;
                    scale.Y = dimension.Y / texture.Height;
                }
            }

            /// <summary>
            /// Friction of this body
            /// </summary>
            /// <remarks>
            /// Unlike density, we can pass this value through to the fixture.
            /// </remarks>
            public virtual float Friction
            {
                get { return friction; }
                set
                {
                    friction = value; // Always update the buffer
                    if (fixture != null)
                    {
                        fixture.Friction = value;
                    }
                }
            }

            /// <summary>
            /// Restitution of this body
            /// </summary>
            /// <remarks>
            /// Unlike density, we can pass this value through to the fixture.
            /// </remarks>
            public virtual float Restitution
            {
                get { return restitution; }
                set
                {
                    restitution = value; // Always update the buffer
                    if (fixture != null)
                    {
                        fixture.Restitution = value;
                    }
                }
            }    
        
            /// <summary>
            /// BodyType for Farseer physics
            /// </summary>
            /// <remarks>
            /// If you want to lock a body in place (e.g. a platform) set this value to STATIC.
            /// KINEMATIC allows the object to move and collide, but ignores external forces 
            /// (e.g. gravity). DYNAMIC makes this is a full-blown physics object.
            /// </remarks>
            public virtual BodyType BodyType
            {
                get { return (body != null ? body.BodyType : bodyType); }
                set
                {
                    bodyType = value; // Always update the buffer.
                    if (body != null)
                    {
                        body.BodyType = value;
                    }
                }
            }

            /// <summary>
            /// Current position for this physics body
            /// </summary>
            public virtual Vector2 Position
            {
                get { return (body != null ? body.Position : position); }
                set
                {
                    position = value; // Always update the buffer.
                    if (body != null)
                    {
                        body.Position = value;
                    }
                }
            }

            /// <summary>
            /// Linear velocity for this physics body
            /// </summary>
            public virtual Vector2 LinearVelocity
            {
                get { return (body != null ? body.LinearVelocity : linearVelocity); }
                set
                {
                    linearVelocity = value; // Always update the buffer.
                    if (body != null)
                    {
                        body.LinearVelocity = value;
                    }
                }
            }

            /// <summary>
            /// Angle of rotation for this body (about the center).
            /// </summary>
            public virtual float Rotation
            {
                get { return (body != null ? body.Rotation : rotation); }
                set
                {
                    rotation = value; // Always update the buffer.
                    if (body != null)
                    {
                        body.Rotation = value;
                    }
                }
            }

            /// <summary>
            /// Object texture for drawing purposes.
            /// </summary>
            public virtual Texture2D Texture
            {
                get { return texture; }
                set { texture = value; }
            }
            
            /// <summary>
            /// The Farseer body for this object.
            /// </summary>
            /// <remarks>
            /// Use this body to add joints and apply forces.
            /// </remarks>
            public virtual Body Body
            {
                get { return body; }
            }
        #endregion

        #region Methods
            /// <summary>
            /// Create a new box at the origin. Don't forget to add the texture and activate physics.
            /// </summary>
            public BoxObject()
            {
                this.position = new Vector2(6.0f, 2.0f);
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
            public virtual bool ActivatePhysics(World world, Texture2D texture, float scale)
            {
                // Initialize
                this.texture = texture;
                this.dimension = 1f / scale * new Vector2((float)texture.Width, (float)texture.Height);
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
            /// Updates the object's physics state (NOT GAME LOGIC).
            /// </summary>
            /// <remarks>
            /// This method is called AFTER the collision resolution
            /// state.  Therefore, it should not be used to process
            /// actions or any other gameplay information.  Its primary
            /// purpose is to adjust changes to the fixture, which
            /// have to take place after collision.
            /// </remarks>
            public virtual void Update(float dt)
            {
                // Recreate the fixture object if dimensions changed.
                if (isDirty)
                {
                    CreateShape();
                }
            }

            /// <summary>
            /// Draws the physics object.
            /// </summary>
            /// <param name="canvas">Drawing context</param>
            public virtual void Draw(GameCanvas g)
            {
                g.DrawSprite(texture, Color.White, Position, scale, rotation);
            }

        #endregion
    }
}
