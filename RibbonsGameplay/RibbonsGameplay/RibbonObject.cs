using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RibbonsGameplay
{
    public class RibbonObject : ComplexPhysicsObject
    {
        #region Fields
            // Invisible anchor objects needed for Farseer 3.5
            protected Body start = null;
            protected Body finish = null;

            // Dimension information
            protected Vector2 dimension;
            protected float linksize = 1.0f;
            protected float spacing = 0.0f;

            // The plank texture
            protected Texture2D texture;
        #endregion

        public RibbonObject(World world, Texture2D texture, Vector2 pos, float linksize, List<Vector2> path) :
            this(world, texture, pos, linksize, new Vector2(linksize * texture.Width, texture.Height), path) { }

        public RibbonObject(World world, Texture2D texture, Vector2 pos, float linksize, Vector2 size, List<Vector2> path) : 
            base(pos)
        {
            this.texture = texture;
            dimension = size;
            this.BodyType = BodyType.Static;

            for(int i=1; i < path.Count; i++)
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
                    link.ActivatePhysics(world, texture, 1/32f);

                    if(deltaX == 0){
                        link.Position = new Vector2(pos.X, pos.Y + (k * linksize));
                    }
                    else{
                        link.Position = new Vector2(pos.X + (k * linksize), pos.Y);
                    }
                    link.BodyType = BodyType.Static;
                    bodies.Add(link);
                }
            }

            //BoxObject link = new BoxObject();
            //link.ActivatePhysics(world, texture);
            //link.Density = 100000f;
            //bodies.Add(link);
            //link = new BoxObject();
            //link.ActivatePhysics(world, texture);
            //link.Position = new Vector2(100, 100);
            //link.BodyType = BodyType.Static;
            //link.Density = 100000f;
            //bodies.Add(link);


        }

        public bool ActivatePhysics(World world, float scale)
        {

            return true;
        }

        protected override bool CreateJoints(World world)
        {
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
