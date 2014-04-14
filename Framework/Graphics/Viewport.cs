using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Representation of the renderer's viewport.
	/// </summary>
	public struct Viewport : IEquatable<Viewport>
	{
		/// <summary>
		/// Create a new viewport.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public Viewport(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// X position of the viewport.
		/// </summary>
		public int X;

		/// <summary>
		/// Y position of the viewport.
		/// </summary>
		public int Y;

		/// <summary>
		/// Width of the viewport.
		/// </summary>
		public int Width;

		/// <summary>
		/// Height of the viewport.
		/// </summary>
		public int Height;

		/// <summary>
		/// Determine if this viewport equals another viewport.
		/// </summary>
		/// <param name="other">Other viewport to test against.</param>
		/// <returns>True if the two viewports are equal.</returns>
		public bool Equals(Viewport other)
		{
			return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
		}

		/// <summary>
		/// Area of the viewport.
		/// </summary>
		public Rectangle Area
		{
			get { return new Rectangle(X, Y, Width, Height); }
			set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; }
		}

		/// <summary>
		/// Get the aspect ratio of the viewport.
		/// </summary>
		public float AspectRatio { get { if (Height > 0) return (float)Width / Height; return 0; } }
	}
}
