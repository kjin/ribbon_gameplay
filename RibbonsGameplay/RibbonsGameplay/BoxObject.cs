﻿using System;
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
            /// BodyType for Farseer physics
            /// </summary>
            /// <remarks>
            /// If you want to lock a body in place (e.g. a platform) set this value to STATIC.
            /// KINEMATIC allows the object to move and collide, but ignores external forces 
            /// (e.g. gravity). DYNAMIC makes this is a full-blown physics object.
            /// </remarks>
            public BodyType BodyType
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
            public Vector2 Position
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
            public Vector2 LinearVelocity
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
            public float Rotation
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
            public Texture2D Texture
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
            public Body Body
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
                g.DrawSprite(texture, Color.White, position, scale, rotation);
            }

        #endregion
    }
}
