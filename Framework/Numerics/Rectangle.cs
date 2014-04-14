using System;
using System.ComponentModel;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// An integer rectangle.
	/// </summary>
	[TypeConverter(typeof(RectangleConverter))]
	public struct Rectangle : IEquatable<Rectangle>
	{
		/// <summary>
		/// X position.
		/// </summary>
		public int X;

		/// <summary>
		/// Y position.
		/// </summary>
		public int Y;

		/// <summary>
		/// Height of the rectangle.
		/// </summary>
		public int Height;

		/// <summary>
		/// Width of the rectangle.
		/// </summary>
		public int Width;
		
		/// <summary>
		/// Return a rectangle centered on origin with zero size.
		/// </summary>
		public static Rectangle Zero { get { return new Rectangle(0, 0, 0, 0); } }

		/// <summary>
		/// Return a rectangle centered on origin with zero size.
		/// </summary>
		public static Rectangle Empty { get { return new Rectangle(0, 0, 0, 0); } }

		/// <summary>
		/// Construct a rectangle of the specified dimensions.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public Rectangle(int x, int y, int width, int height)
		{
			Height = height;
			Width = width;
			X = x;
			Y = y;
		}

		/// <summary>
		/// Creates a copy of the specified rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to copy.</param>
		public Rectangle(RectangleF rectangle)
		{
			X = (int)rectangle.X;
			Y = (int)rectangle.Y;
			Width = (int)rectangle.Width;
			Height = (int)rectangle.Height;
		}

		/// <summary>
		/// Creates a copy of the specified rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to copy.</param>
		public Rectangle(Rectangle rectangle)
		{
			X = rectangle.X;
			Y = rectangle.Y;
			Width = rectangle.Width;
			Height = rectangle.Height;
		}

		/// <summary>
		/// Construct the rectangle that spans between two vectors.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>A rectangle that spans between the two input vectors.</returns>
		public static Rectangle ConstructSpanning(Vector2I vector1, Vector2I vector2)
		{
			var r = new Rectangle();
			r.X = Math.Min(vector1.X, vector2.X);
			r.Y = Math.Min(vector1.Y, vector2.Y);
			r.Width = Math.Abs(Math.Max(vector1.X, vector2.X) - r.X);
			r.Height = Math.Abs(Math.Max(vector1.Y, vector2.Y) - r.Y);

			return r;
		}
		
		/// <summary>
		/// Convert a rectangle to the target type.
		/// </summary>
		/// <param name="r">Source rectangle.</param>
		/// <returns>The converted rectangle.</returns>
		public static explicit operator RectangleF(Rectangle r)
		{
			return new RectangleF(r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>
		/// Convert the rectangle to string format.
		/// </summary>
		/// <returns>String representation for this rectangle.</returns>
		public override string ToString()
		{
			return "{X=" + TypeConvert.ToString(X) +
				", Y=" + TypeConvert.ToString(Y) +
				", W=" + TypeConvert.ToString(Width) +
				", H=" + TypeConvert.ToString(Height) + "}";
		}
		
		/// <summary>
		/// Get or set the position of this rectangle.
		/// </summary>
		public Vector2I Position
		{
			get
			{
				return new Vector2I(X, Y);
			}

			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the top left position of this rectangle.
		/// </summary>
		public Vector2I TopLeft
		{
			get { return new Vector2I(X, Y); }
			set
			{
				Width += X - value.X;
				Height += Y - value.Y;
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the top right position of this rectangle.
		/// </summary>
		public Vector2I TopRight
		{
			get { return new Vector2I(X + Width, Y); }
			set
			{
				Width = value.X - X;
				Height += Y - value.Y;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the bottom left position of this rectangle.
		/// </summary>
		public Vector2I BottomLeft
		{
			get { return new Vector2I(X, Y + Height); }
			set
			{
				Height = value.Y - Y;
				Width += X - value.X;
				X = value.X;
			}
		}

		/// <summary>
		/// Get or set the bottom right position of this rectangle.
		/// </summary>
		public Vector2I BottomRight
		{
			get { return new Vector2I(X + Width, Y + Height); }
			set
			{
				Width = value.X - X;
				Height = value.Y - Y;
			}
		}

		/// <summary>
		/// Get or set the center position of this rectangle.
		/// </summary>
		public Vector2I Center
		{
			get { return new Vector2I(X + Width / 2, Y + Height / 2); }
			set
			{
				X = value.X - Width / 2;
				Y = value.Y - Height / 2;
			}
		}

		/// <summary>
		/// Get the center left position of this rectangle.
		/// </summary>
		public Vector2I CenterLeft
		{
			get { return new Vector2I(X, Y + Height / 2); }
		}

		/// <summary>
		/// Get the center right position of this rectangle.
		/// </summary>
		public Vector2I CenterRight
		{
			get { return new Vector2I(X + Width, Y + Height / 2); }
		}

		/// <summary>
		/// Get the center top position of this rectangle.
		/// </summary>
		public Vector2I CenterTop
		{
			get { return new Vector2I(X + Width / 2, Y); }
		}
		
		/// <summary>
		/// Get the center bottom position of this rectangle.
		/// </summary>
		public Vector2I CenterBottom
		{
			get { return new Vector2I(X + Width / 2, Y + Height); }
		}

		/// <summary>
		/// Get or set the left side of this rectangle.
		/// </summary>
		public int Left
		{
			get { return X; }
			set
			{
				Width += X - value;
				X = value;
			}
		}

		/// <summary>
		/// Get or set the right side of this rectangle.
		/// </summary>
		public int Right
		{
			get { return X + Width; }
			set
			{
				Width = value - X;
			}
		}

		/// <summary>
		/// Get or set the top of this rectangle.
		/// </summary>
		public int Top
		{
			get { return Y; }
			set
			{
				Height += Y - value;
				Y = value;
			}
		}

		/// <summary>
		/// Get or set the bottom of this rectangle.
		/// </summary>
		public int Bottom
		{
			get { return Y + Height; }
			set
			{
				Height = value - Y;
			}
		}

		/// <summary>
		/// Test if this rectangle is empty.
		/// </summary>
		/// <returns>True if the rectangle is empty.</returns>
		public bool IsEmpty { get { return X == 0 && Y == 0 && Width == 0 && Height == 0; } }

		/// <summary>
		/// Get or set the size of this rectangle.
		/// </summary>
		public Vector2I Size
		{
			get
			{
				return new Vector2I(Width, Height);
			}

			set
			{
				Width = value.X;
				Height = value.Y;
			}
		}

		/// <summary>
		/// Move this rectangle by the specified offset.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public void Offset(int x, int y)
		{
			X += x;
			Y += y;
		}

		/// <summary>
		/// Move this rectangle by the specified offset.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		public void Offset(Vector2I offset)
		{
			X += offset.X;
			Y += offset.Y;
		}

		/// <summary>
		/// Return a copy of this rectangle that is offsetted 
		/// by the specified amount.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		/// <returns>An offsetted copy.</returns>
		public Rectangle Offseted(int x, int y)
		{
			var copy = new Rectangle(this);
			copy.X += x;
			copy.Y += y;
			return copy;
		}

		/// <summary>
		/// Return a copy of this rectangle that is offsetted 
		/// by the specified amount.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		/// <returns>An offsetted copy.</returns>
		public Rectangle Offseted(Vector2I offset)
		{
			var copy = new Rectangle(this);
			copy.X += offset.X;
			copy.Y += offset.Y;
			return copy;
		}

		/// <summary>
		/// Test if the input point is within this rectangle.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>True if this rectangle contains the specified point.</returns>
		public bool Contains(int x, int y)
		{
			bool result;
			Contains(ref x, ref y, out result);
			return result;
		}

		/// <summary>
		/// Test if the input point is within this rectangle.
		/// </summary>
		/// <param name="value">Point to test.</param>
		/// <returns>True if this rectangle contains the specified point.</returns>
		public bool Contains(Vector2I value)
		{
			bool result;
			Contains(ref value.X, ref value.Y, out result);
			return result;
		}

		/// <summary>
		/// Test if the input point is within this rectangle.
		/// </summary>
		/// <param name="value">Point to test.</param>
		/// <param name="result">Result value.</param>
		public void Contains(ref Vector2I value, out bool result)
		{
			Contains(ref value.X, ref value.Y, out result);
		}

		/// <summary>
		/// Test if the input point is within this rectangle.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="result">Result value.</param>
		private void Contains(ref int x, ref int y, out bool result)
		{
			result = x > X && x < X + Width && y > Y && y < Y + Height;
		}

		/// <summary>
		/// Test if the specified rectangle is fully within this rectangle.
		/// </summary>
		/// <param name="value">Rectangle to test.</param>
		/// <returns>True if the specified rectangle is fully within this rectangle.</returns>
		public bool Contains(Rectangle value)
		{
			bool result;
			Contains(ref value, out result);
			return result;
		}

		/// <summary>
		/// Test if the specified rectangle is fully within this rectangle.
		/// </summary>
		/// <param name="value">Rectangle to test.</param>
		/// <param name="result">Outputs true if the specified rectangle is fully within this rectangle.</param>
		public void Contains(ref Rectangle value, out bool result)
		{
			result = value.X >= X && value.X + value.Width <= X + Width && value.Y >= Y
			         && value.Y + Height <= Y + Height;
		}

		/// <summary>
		/// Get the hash code for this rectangle.
		/// </summary>
		/// <returns>A hash code for this rectangle.</returns>
		public override int GetHashCode()
		{
			int hash = 37;
			hash = (hash * 397) ^ X.GetHashCode();
			hash = (hash * 397) ^ Y.GetHashCode();
			hash = (hash * 397) ^ Width.GetHashCode();
			hash = (hash * 397) ^ Height.GetHashCode();
			return hash;
		}

		/// <summary>
		/// Inflate this rectangle by the specified amount.
		/// </summary>
		/// <param name="horizontalAmount">Horizontal expansion.</param>
		/// <param name="verticalAmount">Vertical expansion.</param>
		public void Inflate(int horizontalAmount, int verticalAmount)
		{
			X -= horizontalAmount;
			Y -= verticalAmount;
			Width += horizontalAmount * 2;
			Height += verticalAmount * 2;
		}

		/// <summary>
		/// Test if we intersect the specified shape.
		/// </summary>
		/// <param name="value">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(Rectangle value)
		{
			return ((((value.X < (X + Width)) && (X < (value.X + value.Width))) && (value.Y < (Y + Height)))
			        && (Y < (value.Y + value.Height)));
		}

		/// <summary>
		/// Test if we intersect the specified shape.
		/// </summary>
		/// <param name="value">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public void Intersects(ref Rectangle value, out bool result)
		{
			result = (((value.X < (X + Width)) && (X < (value.X + value.Width))) && (value.Y < (Y + Height)))
			         && (Y < (value.Y + value.Height));
		}
		
		/// <summary>
		/// Get the intersection area between the two shapes.
		/// </summary>
		/// <param name="value1">First shape.</param>
		/// <param name="value2">Second shape.</param>
		/// <returns>The area of intersection.</returns>
		public static Rectangle Intersect(Rectangle value1, Rectangle value2)
		{
			Rectangle result;
			Intersect(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Get the intersection area between the two shapes.
		/// </summary>
		/// <param name="value1">First shape.</param>
		/// <param name="value2">Second shape.</param>
		/// <param name="result">The area of intersection.</param>
		public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
		{
			result = new Rectangle();
			int x, y, w, h;
			if (value1.X > value2.X)
			{
				if (value1.X < value2.X + value2.Width)
				{
					x = value1.X;
				}
				else
				{
					return;
				}
			}
			else
			{
				if (value2.X < value1.X + value1.Width)
				{
					x = value2.X;
				}
				else
				{
					return;
				}
			}

			if (value1.Y > value2.Y)
			{
				if (value1.Y < value2.Y + value2.Height)
				{
					y = value1.Y;
				}
				else
				{
					return;
				}
			}
			else
			{
				if (value2.Y < value1.Y + value1.Height)
				{
					y = value2.Y;
				}
				else
				{
					return;
				}
			}

			if (value1.X + value1.Width < value2.X + value2.Width)
			{
				if (value1.X + value1.Width > value2.X)
				{
					w = value1.Width;
				}
				else
				{
					return;
				}
			}
			else
			{
				if (value2.X + value2.Width > value1.X)
				{
					w = value2.Width;
				}
				else
				{
					return;
				}
			}

			if (value1.Y + value1.Height < value2.Y + value2.Height)
			{
				if (value1.Y + value1.Height > value2.Y)
				{
					h = value1.Height;
				}
				else
				{
					return;
				}
			}
			else
			{
				if (value2.Y + value2.Height > value1.Y)
				{
					h = value2.Height;
				}
				else
				{
					return;
				}
			}

			result = new Rectangle(x, y, w - x, h - y);
		}

		/// <summary>
		/// Return the union of the two shapes.
		/// </summary>
		/// <param name="value1">First shape.</param>
		/// <param name="value2">Second shape.</param>
		/// <returns>The united rectangle.</returns>
		public static Rectangle Union(Rectangle value1, Rectangle value2)
		{
			Rectangle result;
			Union(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Return the union of the two shapes.
		/// </summary>
		/// <param name="value1">First shape.</param>
		/// <param name="value2">Second shape.</param>
		/// <param name="result">The united rectangle.</param>
		public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
		{
			int x = value1.X < value2.X ? value1.X : value2.X;
			int y = value1.Y < value2.Y ? value1.Y : value2.Y;

			int w = value1.X + value1.Width > value2.X + value2.Width ? value1.X + value1.Width : value2.X + value2.Width;
			int h = value1.Y + value1.Height > value2.Y + value2.Height ? value1.Y + value1.Height : value2.Y + value2.Height;

			result = new Rectangle(x, y, w - x, h - y);
		}
		
		/// <summary>
		/// Test if this rectangle equals the specified value.
		/// </summary>
		/// <param name="obj">Object to test against.</param>
		/// <returns>True if the two values are equal.</returns>
		public override bool Equals(Object obj)
		{
			return (obj is Rectangle) ? Equals((Rectangle)obj) : false;
		}

		/// <summary>
		/// Test if this rectangle equals the specified one.
		/// </summary>
		/// <param name="other">Rectangle to test against.</param>
		/// <returns>True if the rectangles are equal.</returns>
		public bool Equals(Rectangle other)
		{
			return Height == other.Height && Width == other.Width && X == other.X && Y == other.Y;
		}
		
		/// <summary>
		/// Test if two rectangles are equal.
		/// </summary>
		/// <param name="a">First rectangle.</param>
		/// <param name="b">Second rectangle.</param>
		/// <returns>True if the two rectangles are equal.</returns>
		public static bool operator ==(Rectangle a, Rectangle b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Test if two rectangles are not equal.
		/// </summary>
		/// <param name="a">First rectangle.</param>
		/// <param name="b">Second rectangle.</param>
		/// <returns>True if the two rectangles are not equal.</returns>
		public static bool operator !=(Rectangle a, Rectangle b)
		{
			return !a.Equals(b);
		}
	}
}