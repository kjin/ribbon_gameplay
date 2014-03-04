#region File Description
//-----------------------------------------------------------------------------
// SimplePhysicsObject.cs
//
// Base model class to support collisions.
//
// This class is a subclass of PhysicsObject that supports mutliple Bodies.
// This is the base class for objects that are tied together with joints.
//
// This class does not provide Shape information, and cannot be instantiated
// directly.
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
using System.Collections.ObjectModel;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace RibbonsGameplay
{

    /// <summary>
    /// Instance of a Physics object with just one body.
    /// </summary>
    /// <remarks>
    /// ComplexPhysicsObject instances are built of many bodies, and are assumed to be connected
    /// by joints (though this is not actually a requirement). This is the class to use for chain,
    /// ropes, levers, and so on. This class does not provide Shape information, and cannot be
    /// instantiated directly.
    ///
    /// ComplexPhysicsObject is a hierarchical class.  It groups children as PhysicsObjects,
    /// not bodies.  So you could have a ComplexPhysicsObject made up of other ComplexPhysicsObjects.
    /// </remarks>
    public abstract class ComplexPhysicsObject : PhysicsObject
    {

        #region Fields
        // We are made of other objects
        protected List<PhysicsObject> bodies;
        protected List<Joint> joints;
        #endregion

        #region Properties (READ-WRITE)
        /// <summary>
        /// BodyType for Farseer physics
        /// </summary>
        /// <remarks>
        /// If you want to lock a body in place (e.g. a platform) set this value to STATIC.
        /// KINEMATIC allows the object to move and collide, but ignores external forces
        /// (e.g. gravity). DYNAMIC makes this is a full-blown physics object.
        /// </remarks>
        public override BodyType BodyType
        {
            get { return bodyType; }
            set
            {
                bodyType = value; // Always update the buffer.
                foreach (PhysicsObject obj in bodies)
                {
                    obj.BodyType = value;
                }
            }
        }

        /// <summary>
        /// Current position for the FIRST body in this object.
        /// </summary>
        public override Vector2 Position
        {
            get { return (bodies.Count > 0 ? bodies[0].Position : position); }
            set
            {
                // Adjust all children by same amount.
                Vector2 offset = value - Position;
                foreach (PhysicsObject obj in bodies)
                {
                    Vector2 diff = obj.Position - position;
                    obj.Position += offset;
                }
                position = value; // Always update the buffer.
            }
        }

        /// <summary>
        /// Linear velocity for the FIRST body in this object.
        /// </summary>
        public override Vector2 LinearVelocity
        {
            get { return (bodies.Count > 0 ? bodies[0].LinearVelocity : linearVelocity); }
            set
            {
                linearVelocity = value; // Always update the buffer.
                foreach (PhysicsObject obj in bodies)
                {
                    obj.LinearVelocity = value;
                }
            }
        }

        /// <summary>
        /// Angle of rotation for the FIRST body in this object.
        /// </summary>
        public override float Rotation
        {
            get { return (bodies.Count > 0 ? bodies[0].Rotation : rotation); }
            set
            {
                // Rotate everyone through the origin.
                float offset = value - Rotation;
                float cos = (float)Math.Cos(offset);
                float sin = (float)Math.Sin(offset);
                foreach (PhysicsObject obj in bodies)
                {
                    Vector2 diff = obj.Position - position;
                    Vector2 pos = new Vector2(cos * diff.X - sin * diff.Y,
                                              sin * diff.X + cos * diff.Y);
                    obj.Position = obj.Position + pos;
                    obj.Rotation = obj.Rotation + offset;
                }
                rotation = value; // Always update the buffer.
            }
        }

        /// <summary>
        /// Uniform density for this object.
        /// </summary>
        /// <remarks>
        /// If you want to set different densities for individual components,
        /// you must access them directly.
        /// </remarks>
        public override float Density
        {
            get { return density; }
            set
            {
                Debug.Assert(value >= 0, "Density must be >= 0");
                density = value; // Always update the buffer.
                foreach (PhysicsObject obj in bodies)
                {
                    obj.Density = value;
                }
                if (density == 0)
                {
                    bodyType = BodyType.Static;
                }
            }
        }

        /// <summary>
        /// Uniform friction for this object.
        /// </summary>
        /// <remarks>
        /// If you want to set different frictions for individual components,
        /// you must access them directly.
        /// </remarks>
        public override float Friction
        {
            get { return friction; }
            set
            {
                friction = value; // Always update the buffer
                foreach (PhysicsObject obj in bodies)
                {
                    obj.Friction = value;
                }
            }
        }

        /// <summary>
        /// Uniform restitution for this object.
        /// </summary>
        /// <remarks>
        /// If you want to set different restitutions for individual components,
        /// you must access them directly.
        /// </remarks>
        public override float Restitution
        {
            get { return restitution; }
            set
            {
                restitution = value; // Always update the buffer
                foreach (PhysicsObject obj in bodies)
                {
                    obj.Restitution = value;
                }
            }
        }
        #endregion

        #region Properties (READ-ONLY)
        /// <summary>
        /// The FIRST Body in this PhysicsObject
        /// </summary>
        /// <remarks>
        /// All attributes are set according to the first body.  This makes it
        /// the "most important" body in this object.
        /// </remarks>
        public override Body Body
        {
            get { return (bodies.Count > 0 ? bodies[0].Body : null); }
        }

        /// <summary>
        /// The collection of component physics objects
        /// </summary>
        public ReadOnlyCollection<PhysicsObject> Bodies
        {
            get { return bodies.AsReadOnly(); }
        }

        /// <summary>
        /// The collection of joints for this object (may be empty)
        /// </summary>
        public ReadOnlyCollection<Joint> Joints
        {
            get { return joints.AsReadOnly(); }
        }
        #endregion

        #region Physics Initialization
        /// <summary>
        /// Create a new complex physics object at the origin.
        /// </summary>
        protected ComplexPhysicsObject()
            : this(Vector2.Zero) { }

        /// <summary>
        /// Create a new complex physics object
        /// </summary>
        /// <param name="pos">Location of FIRST object in world coordinates</param>
        protected ComplexPhysicsObject(Vector2 pos)
            : base(pos)
        {
            bodies = new List<PhysicsObject>();
            joints = new List<Joint>();
        }

        /// <summary>
        /// Creates the physics Bodies for this object, adding them to the world.
        /// </summary>
        /// <remarks>
        /// This method invokes ActivatePhysics for the individual PhysicsObjects
        /// in the list. It also calls the internal method CreateJoints() to
        /// link them all together. You should override that method, not this one,
        /// for specific physics objects.
        /// </remarks>
        /// <param name="world">Farseer world to store bodies</param>
        /// <returns><c>true</c> if object allocation succeeded</returns>
        public override bool ActivatePhysics(World world)
        {
            isActive = true;
            bool success = true;

            // Create all other bodies.
            foreach (PhysicsObject obj in bodies)
            {
                success = success && obj.ActivatePhysics(world);
            }
            success = success && CreateJoints(world);

            // Clean up if we failed
            if (!success)
            {
                DeactivatePhysics(world);
            }

            isActive = success;
            return isActive;
        }

        /// <summary>
        /// Destroys the physics Bodies of this object if applicable,
        /// removing them (and their joints) from the world.
        /// </summary>
        /// <param name="world">Farseer world that stores the bodies</param>
        public override void DeactivatePhysics(World world)
        {
            if (isActive)
            {
                // Should be good for most (simple) applications.
                foreach (Joint joint in joints)
                {
                    world.RemoveJoint(joint);
                }
                foreach (PhysicsObject obj in bodies)
                {
                    obj.DeactivatePhysics(world);
                }
                isActive = false;
            }
        }

        /// <summary>
        /// Creates the joints for this object. Executed as part of ActivatePhysics.
        /// </summary>
        /// <remarks>
        /// This is the primary method to override for custom physics objects
        /// </remarks>
        /// <param name="world">Farseer world to store joints</param>
        /// <returns><c>true</c> if joint allocation succeeded</returns>
        protected abstract bool CreateJoints(World world);
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
        public override void Update(float dt)
        {
            // Just pass the simulation to the individual objects.
            foreach (PhysicsObject obj in bodies)
            {
                obj.Update(dt);
            }
        }

        /// <summary>
        /// Draws the physics object.
        /// </summary>
        /// <remarks>
        /// This is sufficient for most complex objects.
        /// </remarks>
        /// <param name="canvas">Drawing context</param>
        public override void Draw(GameCanvas canvas)
        {
            // Just pass the drawing to the individual objects.
            foreach (PhysicsObject obj in bodies)
            {
                obj.Draw(canvas);
            }
        }
        #endregion

    }
}
