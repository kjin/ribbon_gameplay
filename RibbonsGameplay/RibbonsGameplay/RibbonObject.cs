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

        public RibbonObject(Texture2D texture, Vector2 pos, float linksize) :
            this(texture, pos, linksize, new Vector2(linksize * texture.Width, texture.Height)) { }

        public RibbonObject(Texture2D texture, Vector2 pos, float linksize, Vector2 size) : 
            base(pos)
        {
            this.texture = texture;
            dimension = size;

            BoxObject plank = new BoxObject(texture, anchor, boxSize);
            plank.Density = 100000f;
            bodies.Add(plank);
        }

        public void Draw(GameCanvas g)
        {

        }
    }
}
