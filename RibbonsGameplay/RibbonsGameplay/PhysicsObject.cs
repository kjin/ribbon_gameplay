#region File Description
//-----------------------------------------------------------------------------
// PhysicsObject.cs
//
// Base model class to support collisions.
//
// This class, and all of the others in the Physics.Objects namespace,
// are provided to give you easy access to Farseer.  Farseer's separation
// of bodies and shapes (e.g. fixtures) can be a bit daunting. This class
// combines them together to simplify matters.
//
// This, and its superclasses, are fairly robust.  You may want to use
// them in your game.  They make Farseer a lot easier.
//
// Author: Walker M. White
// Based on original PhysicsDemo Lab by Don Holden, 2007
// MonoGame version, 2/14/2014
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

/// <summary>
/// Subnamespace for Farseer-aware models.
/// </summary>
namespace RibbonsGameplay
{

    /// <summary>
    /// Instances represents a body and/or a group of bodies.
    /// </summary>
    /// <remarks>
    /// There should be NO game controlling logic code in a physics objects, 
    /// that should reside in the Controllers.
    /// 
    /// This abstract class has no Body or Shape information and should never 
    /// be instantiated directly. Instead, you should instantiate either
    /// SimplePhysicsObject or ComplexPhysicsObject.  This class only exists
    /// to unify common functionality.
    /// </remarks>
    public abstract class PhysicsObject
    {

        #region Fields
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

        // The appropriate drawing pass for this object
        protected DrawState drawState;
        #endregion

        #region Properties (READ-WRITE)
        /// <summary>
        /// Whether our object has been flagged for garbage collection
        /// </summary>
        public bool Remove
        {
            get { return toRemove; }
            set { toRemove = value; }
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
            get { return bodyType; }
            set { bodyType = value; }
        }

        /// <summary>
        /// Current position for this physics body
        /// </summary>
        /// <remarks>
        /// This value is buffered if it is set before body creation
        /// </remarks>
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// X-coordinate for this physics body
        /// </summary>
        /// <remarks>
        /// This value is buffered if it is set before body creation
        /// </remarks>
        public float X
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
        public float Y
        {
            get { return position.Y; }
            set { Position = new Vector2(position.X, value); }
        }

        /// <summary>
        /// Linear velocity for this physics body
        /// </summary>
        /// <remarks>
        /// This value is buffered if it is set before body creation
        /// </remarks>
        public virtual Vector2 LinearVelocity
        {
            get { return linearVelocity; }
            set { linearVelocity = value; }
        }

        /// <summary>
        /// X-coordinate for this physics body's velocity
        /// </summary>
        /// <remarks>
        /// This value is buffered if it is set before body creation
        /// </remarks>
        public float VX
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
        public float VY
        {
            get { return linearVelocity.Y; }
            set { LinearVelocity = new Vector2(linearVelocity.X, value); }
        }

        /// <summary>
        /// Angle of rotation for this body (about the center).
        /// </summary>
        /// <remarks>
        /// This value is buffered if it is set before body creation
        /// </remarks>
        public virtual float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

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
        /// Friction of this body
        /// </summary>
        /// <remarks>
        /// Changes friction requires changing the Fixture.  Therefore, modifying
        /// this attribute flags the object as dirty.  Dirty objects have their
        /// fixtures destroyed and rebuilt in the Update method.
        /// </remarks>
        public virtual float Friction
        {
            get { return friction; }
            set
            {
                isDirty = true;
                friction = value;
            }
        }

        /// <summary>
        /// Restitution of this body
        /// </summary>
        /// <remarks>
        /// Changes restitution requires changing the Fixture.  Therefore, modifying
        /// this attribute flags the object as dirty.  Dirty objects have their
        /// fixtures destroyed and rebuilt in the Update method.
        /// </remarks>
        public virtual float Restitution
        {
            get { return restitution; }
            set
            {
                isDirty = true;
                restitution = value;
            }
        }
        #endregion

        #region Properties (READ-ONLY)
        /// <summary>
        /// Whether our object is actively part of a physics world.
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <summary>
        /// Whether the object requires an update Fixture.
        /// </summary>
        /// <remarks>
        /// Attributes tied to a fixture must be reset after collision.
        /// Objects are reset in the Update method.
        /// </remarks>
        public bool IsDirty
        {
            get { return isDirty; }
        }

        /// <summary>
        /// Whether this object uses sprite or polygonal drawing.
        /// </summary>
        public DrawState DrawState
        {
            get { return drawState; }
        }

        /// <summary>
        /// The Farseer body for this object.
        /// </summary>
        /// <remarks>
        /// Use this body to add joints and apply forces.
        /// </remarks>
        public virtual Body Body
        {
            get { return null; }
        }
        #endregion

        #region Physics Initialization

        /// <summary>
        /// Create a new physics object at the origin.
        /// </summary>
        protected PhysicsObject() : this(Vector2.Zero) { }

        /// <summary>
        /// Create a new physics object
        /// </summary>
        /// <param name="pos">Location of object in world coordinates</param>
        protected PhysicsObject(Vector2 pos)
        {
            // Object has yet to be deactivated
            toRemove = false;
            isActive = false;
            isDirty = false;

            // Objects are physics objects unless otherwise noted
            bodyType = BodyType.Dynamic;
            position = pos;
            rotation = 0.0f;
            density = 0.0f;
            friction = 0.0f;
            restitution = 0.0f;

            // Objects are sprites by default
            drawState = DrawState.SpritePass;
        }

        /// <summary>
        /// Creates the physics Body(s) for this object, adding them to the world.
        /// </summary>
        /// <remarks>
        /// Implementations of this method should NOT retain a reference to World.  
        /// That is a tight coupling that we should avoid.
        /// </remarks>   
        /// <param name="world">Farseer world to store body</param>
        /// <returns><c>true</c> if object allocation succeeded</returns>
        public abstract bool ActivatePhysics(World world);

        /// <summary>
        /// Destroys the physics Body(s) of this object if applicable,
        /// removing them from the world.
        /// </summary>
        /// <param name="world">Farseer world that stores body</param>
        public abstract void DeactivatePhysics(World world);
        #endregion

        #region Game Loop (Update and Draw)
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
        /// <param name="dt">Timing values from parent loop</param>
        public virtual void Update(float dt) { }

        /// <summary>
        /// Draws the physics object.
        /// </summary>
        /// <param name="canvas">Drawing context</param>
        public abstract void Draw(GameCanvas canvas);
        #endregion

    }
}
