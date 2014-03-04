using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

namespace RibbonsGameplay
{
    public class RibbonObject : ComplexPhysicsObject
    {
        #region Fields
            // Invisible anchor objects needed for Farseer 3.5
            protected Vector2 start;
            protected Vector2 finish;

            // Dimension information
            protected Vector2 dimension;
            protected float linksize = 1.0f;
            protected float spacing = 0.0f;

            // The plank texture
            protected Texture2D texture;
            protected List<Vector2> path;
        #endregion

        public RibbonObject(World world, Texture2D texture, Vector2 pos, float linksize, List<Vector2> path) :
            this(world, texture, pos, linksize, new Vector2(linksize * texture.Width, texture.Height), path) { }

        public RibbonObject(World world, Texture2D texture, Vector2 pos, float linksize, Vector2 size, List<Vector2> path) : 
            base(pos)
        {
            System.Diagnostics.Debug.WriteLine("test");
            this.texture = texture;
            this.path = path;
            dimension = size;
            this.BodyType = BodyType.Static;

            float scale = 1 / 32f;
            linksize *= scale;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 point1 = path[i];
                Vector2 point2 = path[i - 1];

                double deltaX = point1.X - point2.X;
                double deltaY = point1.Y - point2.Y;

                double distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                float numLinks = (float)distance / linksize;

                BoxObject link;


                for (int k = 0; k < numLinks; k++)
                {
                    
                    link = new BoxObject();
                    link.ActivatePhysics(world, texture, scale);
                    System.Diagnostics.Debug.WriteLine(link);

                    Vector2 startPos = path[i - 1];
                    if (deltaX == 0)
                    {
                        if (deltaY > 0){
                            link.Position = new Vector2(startPos.X - 4 * scale, startPos.Y + (k * linksize) + 2 * scale);
                            link.Rotation = 0.5F * (float)Math.PI;
                        }
                        else{
                            link.Position = new Vector2(startPos.X - 2 * scale, startPos.Y - (k * linksize) - 2 * scale);
                            link.Rotation = 0.5F * (float)Math.PI;
                        }
                        
                    }
                    else
                    {
                        if (deltaX > 0){
                            link.Position = new Vector2(startPos.X + (k * linksize), startPos.Y);
                        }
                        else{
                            link.Position = new Vector2(startPos.X - (k * linksize) - 6 * scale, startPos.Y);
                        }
                    }
                    link.BodyType = BodyType.Dynamic;
                    bodies.Add(link);
                }
            }
        }

        protected override bool CreateJoints(World world)
        {
            System.Diagnostics.Debug.WriteLine("hello");
            start = path[0];
            finish = path[path.Count - 1];

            Vector2 anchor1 = new Vector2();
            Vector2 anchor2 = new Vector2(-linksize / 2, 0);
            Joint joint;

            // Link the ribbon together
            anchor1.X = linksize / 2;
            for (int ii = 0; ii < bodies.Count - 1; ii++)
            {

                // Link the interior links
                joint = JointFactory.CreateWeldJoint(world, bodies[ii].Body, bodies[ii + 1].Body, -anchor2, anchor2);
                bodies[ii].Body.GravityScale = 0F;
                joints.Add(joint);
            }
            return true;
        }

        public override void Draw(GameCanvas g)
        {
            foreach (BoxObject link in bodies)
            {
                link.Draw(g);
            }
        }
    }
}
