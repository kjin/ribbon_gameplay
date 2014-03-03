using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RibbonsGameplay
{
    /// <summary>
    /// Data structure for a color gradient stopper.
    /// </summary>
    public struct GradientStopper
    {
        /// <summary>
        /// The color of this stopper.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The location of this stopper.
        /// </summary>
        public float Percent;

        /// <summary>
        /// Constructs a new gradient stopper.
        /// </summary>
        /// <param name="percent">The location of this stopper.</param>
        /// <param name="color">The color of this stopper.</param>
        public GradientStopper(float percent, Color color)
        {
            Color = color;
            Percent = percent;
        }

        /// <summary>
        /// Returns this stopper represented as a string.
        /// </summary>
        /// <returns>This stopper represented as a string.</returns>
        public override string ToString()
        {
            return "{" + Color + " " + Percent + "}";
        }
    }

    /// <summary>
    /// Represents a smooth color gradient defined by a set of "stoppers" which
    /// specify the exact color of the gradient at a certain location.
    /// </summary>
    public class Gradient
    {
        LinkedList<GradientStopper> colors;

        /// <summary>
        /// Constructs a very basic gradient with a start and end color.
        /// </summary>
        /// <param name="start">The start color.</param>
        /// <param name="end">The end color.</param>
        public Gradient(Color start, Color end)
        {
            colors = new LinkedList<GradientStopper>();
            colors.AddLast(new GradientStopper(0, start));
            colors.AddLast(new GradientStopper(1, end));
        }

        /// <summary>
        /// Constructs a gradient with custom stopper locations.
        /// </summary>
        /// <param name="colors">An non-empty array of stoppers. Each stopper must have a location between 0 and 1.</param>
        public Gradient(GradientStopper[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
                if (colors[i].Percent < 0 || colors[i].Percent > 1)
                    throw new ArgumentOutOfRangeException();
            this.colors = new LinkedList<GradientStopper>(colors);
        }

        /// <summary>
        /// Constructs a gradient with evenly spaced colors.
        /// </summary>
        /// <param name="colors">A list of colors. There must be at least color.</param>
        public Gradient(params Color[] colors)
        {
            if (colors.Length == 0)
                throw new ArgumentOutOfRangeException();
            this.colors = new LinkedList<GradientStopper>();
            if (colors.Length > 1)
                for (int i = 0; i < colors.Length; i++)
                    this.colors.AddLast(new GradientStopper((float)i / (colors.Length - 1), colors[i]));
            else
            {
                this.colors.AddLast(new GradientStopper(0, colors[0]));
                this.colors.AddLast(new GradientStopper(1, colors[0]));
            }
        }

        /// <summary>
        /// Constructs a gradient with custom stopper locations.
        /// </summary>
        /// <param name="colors">A non-zero array of colors.</param>
        /// <param name="percents">A non-zero array of stopper locations.</param>
        public Gradient(Color[] colors, float[] percents)
        {
            if (colors.Length != percents.Length)
                throw new ArgumentOutOfRangeException();
            this.colors = new LinkedList<GradientStopper>();
            if (colors.Length > 1)
                for (int i = 0; i < colors.Length; i++)
                    this.colors.AddLast(new GradientStopper(percents[i], colors[i]));
            else
            {
                this.colors.AddLast(new GradientStopper(0, colors[0]));
                this.colors.AddLast(new GradientStopper(1, colors[0]));
            }
        }

        /// <summary>
        /// Adds a color to the gradient.
        /// </summary>
        /// <param name="color">The color to add.</param>
        /// <param name="percent">The stopper location of this new color. If this number is outside of the range (0, 1), the method returns without doing anything.</param>
        public void AddColor(Color color, float percent)
        {
            if (percent <= 0 || percent >= 1) return;
            LinkedListNode<GradientStopper> currentNode = colors.First;
            while (currentNode.Next != null && currentNode.Next.Value.Percent < percent)
                currentNode = currentNode.Next;
            colors.AddAfter(currentNode, new GradientStopper(percent, color));
        }

        /// <summary>
        /// Gets a linearly interpolated color from the gradient.
        /// </summary>
        /// <param name="percent">The location from which to sample a color. This location will be clamped to range [0, 1].</param>
        /// <returns>The linearly interpolated color.</returns>
        public Color GetColor(float percent)
        {
            if (percent <= 0) return colors.First.Value.Color;
            if (percent >= 1) return colors.Last.Value.Color;
            LinkedListNode<GradientStopper> currentNode = colors.First;
            while (currentNode.Next != null && currentNode.Next.Value.Percent < percent)
                currentNode = currentNode.Next;
            return Color.Lerp(currentNode.Value.Color, currentNode.Next.Value.Color, (percent - currentNode.Value.Percent) / (currentNode.Next.Value.Percent - currentNode.Value.Percent));
        }
    }
}
