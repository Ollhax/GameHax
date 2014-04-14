using System;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// Simple three-dimensional integer vector.
	/// </summary>
	public struct Vector3I : IEquatable<Vector3I>
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
		/// Z position.
		/// </summary>
		public int Z;

		/// <summary>
		/// Returns a vector with all components set to zero.
		/// </summary>
		public static Vector3I Zero { get { return zero; } }

		/// <summary>
		/// Returns a vector with all components set to one.
		/// </summary>
		public static Vector3I One { get { return one; } }

		/// <summary>
		/// Returns a unit vector pointing to the left.
		/// </summary>
		public static Vector3I Left { get { return left; } }

		/// <summary>
		/// Returns a unit vector pointing to the right.
		/// </summary>
		public static Vector3I Right { get { return right; } }

		/// <summary>
		/// Returns a unit vector pointing up.
		/// </summary>
		public static Vector3I Up { get { return up; } }

		/// <summary>
		/// Returns a unit vector pointing down.
		/// </summary>
		public static Vector3I Down { get { return down; } }

		/// <summary>
		/// Returns a unit vector pointing forward.
		/// </summary>
		public static Vector3I Forward { get { return forward; } }

		/// <summary>
		/// Returns a unit vector pointing backward.
		/// </summary>
		public static Vector3I Backward { get { return backward; } }

		/// <summary>
		/// Returns a unit X vector.
		/// </summary>
		public static Vector3I UnitX { get { return unitX; } }

		/// <summary>
		/// Returns a unit Y vector.
		/// </summary>
		public static Vector3I UnitY { get { return unitY; } }

		/// <summary>
		/// Returns a unit Z vector.
		/// </summary>
		public static Vector3I UnitZ { get { return unitZ; } }
		
		private static Vector3I zero = new Vector3I(0, 0, 0);
		private static Vector3I one = new Vector3I(1, 1, 1);
		private static Vector3I left = new Vector3I(-1, 0, 0);
		private static Vector3I right = new Vector3I(1, 0, 0);
		private static Vector3I up = new Vector3I(0, 1, 0);
		private static Vector3I down = new Vector3I(0, -1, 0);
		private static Vector3I forward = new Vector3I(0, 0, -1);
		private static Vector3I backward = new Vector3I(0, 0, 1);
		private static Vector3I unitX = new Vector3I(1, 0, 0);
		private static Vector3I unitY = new Vector3I(0, 1, 0);
		private static Vector3I unitZ = new Vector3I(0, 0, 1);

		/// <summary>
		/// Returns true if all this vector's components are zero.
		/// </summary>
		public bool IsZero
		{
			get { return X == 0 && Y == 0 && Z == 0; }
		}

		/// <summary>
		/// Create a vector with all three components set to the same value.
		/// </summary>
		/// <param name="value">Value to set.</param>
		public Vector3I(int value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		/// <summary>
		/// Create a vector with the specified components.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector3I(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		
		/// <summary>
		/// Create a vector with the specified components.
		/// </summary>
		/// <param name="value">X and Y components.</param>
		/// <param name="z">Z component.</param>
		public Vector3I(Vector2I value, int z)
		{
			X = value.X;
			Y = value.Y;
			Z = z;
		}

		/// <summary>
		/// Create a copy of the specified vector.
		/// </summary>
		/// <param name="other">Vector to copy.</param>
		public Vector3I(Vector3I other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		/// <summary>
		/// Create a copy of the specified vector.
		/// </summary>
		/// <param name="other">Vector to copy.</param>
		public Vector3I(Vector3 other)
		{
			X = (int)other.X;
			Y = (int)other.Y;
			Z = (int)other.Z;
		}

		/// <summary>
		/// Convert an integer vector to a float vector.
		/// </summary>
		/// <param name="v">Input vector.</param>
		/// <returns>Output vector.</returns>
		public static explicit operator Vector3(Vector3I v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		/// <summary>
		/// Return the string representation of this vector.
		/// </summary>
		/// <returns>The string representation of this vector.</returns>
		public override string ToString()
		{
			return "{X=" + TypeConvert.ToString(X) + ", Y=" + TypeConvert.ToString(Y) + ", Z=" + TypeConvert.ToString(Z) + "}";
		}

		/// <summary>
		/// Add two vectors and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The added vector.</returns>
		public static Vector3I operator +(Vector3I value1, Vector3I value2)
		{
			return new Vector3I(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
		}

		/// <summary>
		/// Subtract the second vector from the first and return the resulting vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The subtracted vector.</returns>
		public static Vector3I operator -(Vector3I value1, Vector3I value2)
		{
			return new Vector3I(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
		}

		/// <summary>
		/// Multiply a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The first vector multiplied by the second.</returns>
		public static Vector3I operator *(Vector3I value1, Vector3I value2)
		{
			return new Vector3I(value1.X * value2.X, value1.Y * value2.Y, value1.Z * value2.Z);
		}
		
		/// <summary>
		/// Divide a vector by the components of another vector.
		/// </summary>
		/// <param name="value">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The first vector divided by the second.</returns>
		public static Vector3I operator /(Vector3I value, Vector3I value2)
		{
			return new Vector3I(value.X / value2.X, value.Y / value2.Y, value.Z / value2.Z);
		}

		/// <summary>
		/// Returns a vector scaled by the specified value.
		/// </summary>
		/// <param name="value">Source vector.</param>
		/// <param name="scale">Scale factor.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator *(Vector3I value, int scale)
		{
			return new Vector3I(value.X * scale, value.Y * scale, value.Z * scale);
		}

		/// <summary>
		/// Divide a vector by a the specified value.
		/// </summary>
		/// <param name="value">Source vector.</param>
		/// <param name="div">Divider value.</param>
		/// <returns>The divided vector.</returns>
		public static Vector3I operator /(Vector3I value, int div)
		{
			return new Vector3I(value.X / div, value.Y / div, value.Z / div);
		}

		/// <summary>
		/// Test if the two vectors are not equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are not equal.</returns>
		public static bool operator !=(Vector3I value1, Vector3I value2)
		{
			return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
		}

		/// <summary>
		/// Test if the two vectors are equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are equal.</returns>
		public static bool operator ==(Vector3I value1, Vector3I value2)
		{
			return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
		}

		/// <summary>
		/// Negate a vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <returns>The negated vector.</returns>
		public static Vector3I operator -(Vector3I value)
		{
			return new Vector3I(-value.X, -value.Y, -value.Z);
		}

		/// <summary>
		/// Test if this vector equals the specified vector.
		/// </summary>
		/// <param name="other">Vector to test for equality.</param>
		/// <returns>True if the vectors are equal.</returns>
		public bool Equals(Vector3I other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		/// <summary>
		/// Get the hash value of this vector.
		/// </summary>
		/// <returns>The hash code of this vector.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		/// <summary>
		/// Test if this vector equals the specified object.
		/// </summary>
		/// <param name="obj">Object to test for equality.</param>
		/// <returns>True if the specified object equals this vector.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Vector3I)
			{
				var other = (Vector3I)obj;
				return other.X == X && other.Y == Y && other.Z == Z;
			}
			return false;
		}
	}	
}
