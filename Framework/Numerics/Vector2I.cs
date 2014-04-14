using System;
using System.ComponentModel;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// Simple two-dimensional integer vector.
	/// </summary>
	[TypeConverter(typeof(Vector2IConverter))]
	public struct Vector2I : IEquatable<Vector2I>
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
		/// Returns a vector with X and Y set to 1.
		/// </summary>
		public static Vector2I One
		{
			get { return new Vector2I(1, 1); }
		}
		
		/// <summary>
		/// Returns a vector with X and Y set to 0.
		/// </summary>
		public static Vector2I Zero
		{
			get { return new Vector2I(0, 0); }
		}

		/// <summary>
		/// Create a vector with both vectors set to the specified value.
		/// </summary>
		/// <param name="value">Specified value.</param>
		public Vector2I(int value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
		/// Create a vector with the specified X and Y coordinates.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public Vector2I(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Copy the specified vector.
		/// </summary>
		/// <param name="v">Vector to copy.</param>
		public Vector2I(Vector2I v)
		{
			X = v.X;
			Y = v.Y;
		}

		/// <summary>
		/// Copy the specified vector.
		/// </summary>
		/// <param name="v">Vector to copy.</param>
		public Vector2I(Vector2 v)
		{
			X = (int)v.X;
			Y = (int)v.Y;
		}

		/// <summary>
		/// Convert an integer vector to a float vector.
		/// </summary>
		/// <param name="v">Input vector.</param>
		/// <returns>Output float vector.</returns>
		public static explicit operator Vector2(Vector2I v)
		{
			return new Vector2((float)v.X, (float)v.Y);
		}

		/// <summary>
		/// Returns true if all this vector's components are zero.
		/// </summary>
		public bool IsZero
		{
			get { return X == 0 && Y == 0; }
		}
		
		/// <summary>
		/// Returns a string represention of this vector.
		/// </summary>
		/// <returns>A string represention of this vector.</returns>
		public override string ToString()
		{
			return "{X=" + TypeConvert.ToString(X) + ", Y=" + TypeConvert.ToString(Y) + "}";
		}

		/// <summary>
		/// Returns the two vectors added together.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>The added vector.</returns>
		public static Vector2I operator +(Vector2I v1, Vector2I v2)
		{
			return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
		}

		/// <summary>
		/// Returns the first vector subtracted from the second.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>The vector after subtracting the second vector.</returns>
		public static Vector2I operator -(Vector2I v1, Vector2I v2)
		{
			return new Vector2I(v1.X - v2.X, v1.Y - v2.Y);
		}

		/// <summary>
		/// Returns the first vector multiplied by the second vector.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>The multiplied vector.</returns>
		public static Vector2I operator *(Vector2I v1, Vector2I v2)
		{
			return new Vector2I(v1.X * v2.X, v1.Y * v2.Y);
		}

		/// <summary>
		/// Returns the first vector divided by the second vector.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>The divided vector.</returns>
		public static Vector2I operator /(Vector2I v1, Vector2I v2)
		{
			return new Vector2I(v1.X / v2.X, v1.Y / v2.Y);
		}

		/// <summary>
		/// Returns the vector multiplied by a value.
		/// </summary>
		/// <param name="v1">Specified vector.</param>
		/// <param name="scale">Scale value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator *(Vector2I v1, int scale)
		{
			return new Vector2I(v1.X * scale, v1.Y * scale);
		}

		/// <summary>
		/// Returns the vector divided by a value.
		/// </summary>
		/// <param name="v1">Specified vector.</param>
		/// <param name="div">Divider value.</param>
		/// <returns>The divided vector.</returns>
		public static Vector2I operator /(Vector2I v1, int div)
		{
			return new Vector2I(v1.X / div, v1.Y / div);
		}

		/// <summary>
		/// Test if the first vector is not equal to the second vector.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>True if the two vectors are not equal.</returns>
		public static bool operator !=(Vector2I v1, Vector2I v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y;
		}

		/// <summary>
		/// Test if the first vector is equal to the second vector.
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>True if the two vectors are equal.</returns>
		public static bool operator ==(Vector2I v1, Vector2I v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y;
		}

		/// <summary>
		/// Return the negated vector.
		/// </summary>
		/// <param name="v1">Vector to negate.</param>
		/// <returns>Negated vector.</returns>
		public static Vector2I operator -(Vector2I v1)
		{
			return new Vector2I(-v1.X, -v1.Y);
		}

		/// <summary>
		/// Test if this vector is equal to another vector.
		/// </summary>
		/// <param name="other">Vector to test for equality.</param>
		/// <returns>True if the two vectors are equal.</returns>
		public bool Equals(Vector2I other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		/// Get the hash value for this vector.
		/// </summary>
		/// <returns>Hash value for this vector.</returns>
		public override int GetHashCode()
		{
			return 37 * X.GetHashCode() + 397 * Y.GetHashCode();
		}

		/// <summary>
		/// Test if this vector is equal to the specified object.
		/// </summary>
		/// <param name="obj">Object to test against.</param>
		/// <returns>True if this vector is equal to the specified object.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Vector2I)
			{
				var other = (Vector2I)obj;
				return other.X == X && other.Y == Y;
			}
			return false;
		}
	}	
}
