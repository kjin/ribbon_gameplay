using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;

using Microsoft.Xna.Framework.Graphics;

namespace RibbonsGameplay
{
    public class CoolRibbonObject : Object
    {

        public const float SPEED = 10.0f;

        List<Vector2> points;

        Body body;
        Fixture fixture;

        public BoxObject box;
        List<Hinge> hinges;

        float[] stops;

        public bool left = false;
        public bool right = false;



        public CoolRibbonObject(World world, Texture2D tex)
        {
            points = new List<Vector2>();

            points.Add(new Vector2(4, 5));
            points.Add(new Vector2(4, 22));
            points.Add(new Vector2(30, 22));
            points.Add(new Vector2(30, 5));

            stops = new float[4];

            for(int i = 0; i < points.Count-1; i++)
            {
                stops[i] = Vector2.Distance(points[i],points[i+1]);
            }

            body = BodyFactory.CreateBody(world);
            body.BodyType = BodyType.Static;
            body.Position = new Vector2(0,0);
            body.UserData = this;

            hinges = new List<Hinge>();

            AddBox(world, tex);

            ChainShape chain = new ChainShape(new Vertices(points));

            fixture = body.CreateFixture(chain);
            
        }

        private void AddBox(World world, Texture2D tex)
        {

            Body attach1 = BodyFactory.CreateBody(world);
            attach1.BodyType = BodyType.Kinematic;
            attach1.Position = new Vector2(4, 7);


            Body attach2 = BodyFactory.CreateBody(world);
            attach2.BodyType = BodyType.Kinematic;
            attach2.Position = new Vector2(4, 9);

            box = new BoxObject();
            box.Position = new Vector2(5, 8);
            box.BodyType = BodyType.Dynamic;
            box.ActivatePhysics(world, tex, 32f);
            box.Body.UserData = box;
            box.Body.IgnoreCollisionWith(body);
            RevoluteJoint j1 = JointFactory.CreateRevoluteJoint(world, box.Body, attach1, new Vector2(-1, -1));
            RevoluteJoint j2 = JointFactory.CreateRevoluteJoint(world, box.Body, attach2, new Vector2(-1, 1));

            hinges.Add(new Hinge(attach1,3));
            hinges.Add(new Hinge(attach2,5));

        }

        public void Draw(GameCanvas g)
        {

            for (int i = 0; i < points.Count - 1; i++)
            {
                g.DrawLine(points[i], points[i + 1], Color.Red);
            }

            box.Draw(g);

        }

        public void MoveLeft()
        {
            foreach (Hinge h in hinges)
            {

                float temp = h.position;
                int i = 0;

                while (i < stops.Length && temp > stops[i])
                {
                    i++;
                    temp -= stops[i - 1];
                }

                Console.WriteLine(i);

                Vector2 v;
                if (i >= stops.Length)
                {
                    v = h.body.Position - points[i - 1];
                    v.Normalize();
                    v *= -SPEED;
                }

                else
                {
                    v = points[i + 1] - h.body.Position;
                    v.Normalize();
                    v *= -SPEED;
                }
                h.body.LinearVelocity = v;
                h.position -= SPEED / 60;
            }

            left = false;
        }

        public void MoveRight()
        {
            foreach (Hinge h in hinges)
            {

                float temp = h.position;
                int i = 0;

                while (i < stops.Length && temp > stops[i])
                {
                    i++;
                    temp -= stops[i - 1];
                }

                Console.WriteLine(i);

                Vector2 v;
                if (i >= stops.Length)
                {
                    v = h.body.Position - points[i - 1];
                    v.Normalize();
                    v *= SPEED;
                }

                else
                {
                    v = points[i + 1] - h.body.Position;
                    v.Normalize();
                    v *= SPEED;
                }

                h.body.LinearVelocity = v;
                h.position += SPEED / 60;
            }

            right = false;

        }

        public void Stop()
        {
            foreach (Hinge h in hinges)
            {
                h.body.LinearVelocity = new Vector2(0, 0);
            }

        }

    }
}
