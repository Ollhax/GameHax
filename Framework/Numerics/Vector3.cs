using System;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// Simple three-dimensional float vector.
	/// </summary>
	[Serializable]
	public struct Vector3 : IEquatable<Vector3>
	{
		/// <summary>
		/// X position.
		/// </summary>
		public float X;

		/// <summary>
		/// Y position.
		/// </summary>
		public float Y;

		/// <summary>
		/// Z position.
		/// </summary>
		public float Z;

		/// <summary>
		/// Returns a vector with all components set to zero.
		/// </summary>
		public static Vector3 Zero { get { return zero; } }

		/// <summary>
		/// Returns a vector with all components set to one.
		/// </summary>
		public static Vector3 One { get { return one; } }

		/// <summary>
		/// Returns a unit vector pointing to the left.
		/// </summary>
		public static Vector3 Left { get { return left; } }

		/// <summary>
		/// Returns a unit vector pointing to the right.
		/// </summary>
		public static Vector3 Right { get { return right; } }

		/// <summary>
		/// Returns a unit vector pointing up.
		/// </summary>
		public static Vector3 Up { get { return up; } }

		/// <summary>
		/// Returns a unit vector pointing down.
		/// </summary>
		public static Vector3 Down { get { return down; } }

		/// <summary>
		/// Returns a unit vector pointing forward.
		/// </summary>
		public static Vector3 Forward { get { return forward; } }

		/// <summary>
		/// Returns a unit vector pointing backward.
		/// </summary>
		public static Vector3 Backward { get { return backward; } }

		/// <summary>
		/// Returns a unit X vector.
		/// </summary>
		public static Vector3 UnitX { get { return unitX; } }

		/// <summary>
		/// Returns a unit Y vector.
		/// </summary>
		public static Vector3 UnitY { get { return unitY; } }
		
		/// <summary>
		/// Returns a unit Z vector.
		/// </summary>
		public static Vector3 UnitZ { get { return unitZ; } }

		private static Vector3 zero = new Vector3(0f, 0f, 0f);
		private static Vector3 one = new Vector3(1f, 1f, 1f);
		private static Vector3 left = new Vector3(-1f, 0f, 0f);
		private static Vector3 right = new Vector3(1f, 0f, 0f);
		private static Vector3 up = new Vector3(0f, 1f, 0f);
		private static Vector3 down = new Vector3(0f, -1f, 0f);
		private static Vector3 forward = new Vector3(0f, 0f, -1f);
		private static Vector3 backward = new Vector3(0f, 0f, 1f);
		private static Vector3 unitX = new Vector3(1f, 0f, 0f);
		private static Vector3 unitY = new Vector3(0f, 1f, 0f);
		private static Vector3 unitZ = new Vector3(0f, 0f, 1f);

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
		public Vector3(float value)
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
		public Vector3(float x, float y, float z)
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
		public Vector3(Vector2 value, float z)
		{
			X = value.X;
			Y = value.Y;
			Z = z;
		}

		/// <summary>
		/// Create a copy of the specified vector.
		/// </summary>
		/// <param name="other">Vector to copy.</param>
		public Vector3(Vector3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		/// <summary>
		/// Create a copy of the specified vector.
		/// </summary>
		/// <param name="other">Vector to copy.</param>
		public Vector3(Vector3I other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		/// <summary>
		/// Convert a float vector to an integer vector.
		/// </summary>
		/// <param name="v">Input vector.</param>
		/// <returns>Output integer vector.</returns>
		public static explicit operator Vector3I(Vector3 v)
		{
			return new Vector3I((int)v.X, (int)v.Y, (int)v.Z);
		}
		
		/// <summary>
		/// Add two vectors together and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The two vectors added together.</returns>
		public static Vector3 Add(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Add(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Add two vectors together and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The two vectors added together.</param>
		public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
		}

		/// <summary>
		/// Subtract the second vector from the first and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The subtracted vector.</returns>
		public static Vector3 Subtract(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Subtract(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Subtract the second vector from the first and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The subtracted vector.</param>
		public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
		}

		/// <summary>
		/// Multiply a vector by the specified factor.
		/// </summary>
		/// <param name="value1">Specified vector.</param>
		/// <param name="scaleFactor">Scale factor.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3 Multiply(Vector3 value1, float scaleFactor)
		{
			Vector3 result;
			Multiply(ref value1, scaleFactor, out result);
			return result;
		}

		/// <summary>
		/// Multiply a vector by the specified factor.
		/// </summary>
		/// <param name="value1">Specified vector.</param>
		/// <param name="scaleFactor">Scale factor.</param>
		/// <param name="result">The scaled vector.</param>
		public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
		{
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
			result.Z = value1.Z * scaleFactor;
		}

		/// <summary>
		/// Multiply the components of two vector and return the resulting vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The resulting vector.</returns>
		public static Vector3 Multiply(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Multiply(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Multiply the components of two vector and return the resulting vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The resulting vector.</param>
		public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
		}

		/// <summary>
		/// Divide a vector byt the specified value.
		/// </summary>
		/// <param name="value1">Vector to divide.</param>
		/// <param name="value2">Division value.</param>
		/// <returns>The divided vector.</returns>
		public static Vector3 Divide(Vector3 value1, float value2)
		{
			Vector3 result;
			Divide(ref value1, value2, out result);
			return result;
		}

		/// <summary>
		/// Divide a vector byt the specified value.
		/// </summary>
		/// <param name="value1">Vector to divide.</param>
		/// <param name="value2">Division value.</param>
		/// <param name="result">The divided vector.</param>
		public static void Divide(ref Vector3 value1, float value2, out Vector3 result)
		{
			float divFactor = 1f / value2;
			result.X = value1.X * divFactor;
			result.Y = value1.Y * divFactor;
			result.Z = value1.Z * divFactor;
		}

		/// <summary>
		/// Divide the components of the first vector by the ones of the second and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The resulting vector.</returns>
		public static Vector3 Divide(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Divide(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Divide the components of the first vector by the ones of the second and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The resulting vector.</param>
		public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
		}

		/// <summary>
		/// Perform the dot product between the two specified vectors.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>The dot product of the first vector by the second.</returns>
		public static float Dot(Vector3 vector1, Vector3 vector2)
		{
			float result;
			Dot(ref vector1, ref vector2, out result);
			return result;
		}

		/// <summary>
		/// Perform the dot product between the two specified vectors.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <param name="result">The dot product of the first vector by the second.</param>
		public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
		}

		/// <summary>
		/// Perform the cross product between the first and the second vector.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>The cross product of the first vector by the second.</returns>
		public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
		{
			Vector3 result;
			Cross(ref vector1, ref vector2, out result);
			return result;
		}

		/// <summary>
		/// Perform the cross product between the first and the second vector.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <param name="result">The cross product of the first vector by the second.</param>
		public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
		{
			result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
			result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
			result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
		}

		/// <summary>
		/// Normalize the length of a vector.
		/// </summary>
		/// <param name="value">Vector to normalize.</param>
		/// <returns>The normalized vector.</returns>
		public static Vector3 Normalize(Vector3 value)
		{
			Vector3 result;
			Normalize(ref value, out result);
			return result;
		}

		/// <summary>
		/// Normalize the length of a vector.
		/// </summary>
		/// <param name="value">Vector to normalize.</param>
		/// <param name="result">The normalized vector.</param>
		public static void Normalize(ref Vector3 value, out Vector3 result)
		{
			float divFactor = 1f / value.Length();
			result.X = value.X * divFactor;
			result.Y = value.Y * divFactor;
			result.Z = value.Z * divFactor;
		}

		/// <summary>
		/// Calculate the distance between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		public static float Distance(Vector3 value1, Vector3 value2)
		{
			float result;
			Distance(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Calculate the distance between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The distance between two vectors.</param>
		public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
		{
			Vector3 resultVector;
			Subtract(ref value1, ref value2, out resultVector);
			result = resultVector.Length();
		}

		/// <summary>
		/// Calculate the squared distance between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The squared distance between two vectors.</returns>
		public static float DistanceSquared(Vector3 value1, Vector3 value2)
		{
			float result;
			DistanceSquared(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Calculate the squared distance between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">The squared distance between two vectors.</param>
		public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
		{
			Vector3 resultVector;
			Subtract(ref value1, ref value2, out resultVector);
			result = resultVector.LengthSquared();
		}

		/// <summary>
		/// Returns a vector that is clamped between two other vector's values.
		/// </summary>
		/// <param name="value1">Vector to clamp.</param>
		/// <param name="min">Min values.</param>
		/// <param name="max">Max values.</param>
		/// <returns>The clamped vector.</returns>
		public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
		{
			Vector3 result;
			Clamp(ref value1, ref min, ref max, out result);
			return result;
		}

		/// <summary>
		/// Returns a vector that is clamped between two other vector's values.
		/// </summary>
		/// <param name="value1">Vector to clamp.</param>
		/// <param name="min">Min values.</param>
		/// <param name="max">Max values.</param>
		/// <param name="result">The clamped vector.</param>
		public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
		{
			result.X = MathTools.Clamp(value1.X, min.X, max.X);
			result.Y = MathTools.Clamp(value1.Y, min.Y, max.Y);
			result.Z = MathTools.Clamp(value1.Z, min.Z, max.Z);
		}

		/// <summary>
		/// Linearly interpolate between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <returns>The linearly interpolated vector.</returns>
		public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
		{
			Vector3 result;
			Lerp(ref value1, ref value2, amount, out result);
			return result;
		}

		/// <summary>
		/// Linearly interpolate between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <param name="result">The linearly interpolated vector.</param>
		public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
		{
			result.X = MathTools.Lerp(value1.X, value2.X, amount);
			result.Y = MathTools.Lerp(value1.Y, value2.Y, amount);
			result.Z = MathTools.Lerp(value1.Z, value2.Z, amount);
		}

		/// <summary>
		/// Returns a vector containing the maximum of the two vector's component values.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A vector containing the maximum component values.</returns>
		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Max(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Returns a vector containing the maximum of the two vector's component values.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">A vector containing the maximum component values.</param>
		public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X > value2.X ? value1.X : value2.X;
			result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
			result.Z = value1.Z > value2.Z ? value1.Z : value2.Z;
		}

		/// <summary>
		/// Returns a vector containing the minimum of the two vector's component values.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A vector containing the minimum component values.</returns>
		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			Vector3 result;
			Min(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Returns a vector containing the minimum of the two vector's component values.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">A vector containing the minimum component values.</param>
		public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X < value2.X ? value1.X : value2.X;
			result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
			result.Z = value1.Z < value2.Z ? value1.Z : value2.Z;
		}

		/// <summary>
		/// Return the negated vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <returns>Negated vector.</returns>
		public static Vector3 Negate(Vector3 value)
		{
			Vector3 result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Return the negated vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <param name="result">Negated vector.</param>
		public static void Negate(ref Vector3 value, out Vector3 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
		}

		/// <summary>
		/// Reflects a vector across a normal and returns the result.
		/// </summary>
		/// <param name="vector">Vector to reflect.</param>
		/// <param name="normal">Normal value.</param>
		/// <returns>The reflected vector.</returns>
		public static Vector3 Reflect(Vector3 vector, Vector3 normal)
		{
			Vector3 result;
			Reflect(ref vector, ref normal, out result);
			return result;
		}

		/// <summary>
		/// Reflects a vector across a normal and returns the result.
		/// </summary>
		/// <param name="vector">Vector to reflect.</param>
		/// <param name="normal">Normal value.</param>
		/// <param name="result">The reflected vector.</param>
		public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
		{
			float scaleFactor = Dot(vector, normal) * 2;
			Vector3 tmp;
			Multiply(ref normal, scaleFactor, out tmp);
			Subtract(ref vector, ref tmp, out result);
		}
		
		/// <summary>
		/// Transforms a vector by the specified matrix.
		/// </summary>
		/// <param name="position">Vector to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <returns>A transformed vector.</returns>
		public static Vector3 Transform(Vector3 position, Matrix matrix)
		{
			Vector3 result;
			Transform(ref position, ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms a vector by the specified matrix.
		/// </summary>
		/// <param name="position">Vector to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="result">A transformed vector.</param>
		public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result)
		{
			result.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41;
			result.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42;
			result.Z = (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43;
		}

		/// <summary>
		/// Transforms a list of vectors by the specified matrix.
		/// </summary>
		/// <param name="sourceArray">List of vectors.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="destinationArray">Output list.</param>
		public static void Transform(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
		{
			for (int i = 0; i < sourceArray.Length; i++)
			{
				Transform(ref sourceArray[i], ref matrix, out destinationArray[i]);
			}
		}

		/// <summary>
		/// Transforms a list of vectors by the specified matrix.
		/// </summary>
		/// <param name="sourceArray">List of vectors.</param>
		/// <param name="sourceIndex">Start index of source list.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="destinationArray">Output list.</param>
		/// <param name="destinationIndex">Output list starting index.</param>
		/// <param name="length">Number of vectors to transform.</param>
		public static void Transform(
			Vector3[] sourceArray,
			int sourceIndex,
			ref Matrix matrix,
			Vector3[] destinationArray,
			int destinationIndex,
			int length)
		{
			length += sourceIndex;
			for (int i = sourceIndex; i < length; i++, destinationIndex++)
			{
				Transform(ref sourceArray[i], ref matrix, out destinationArray[destinationIndex]);
			}
		}

		/// <summary>
		/// Transforms a normal by a matrix.
		/// </summary>
		/// <param name="normal">Normal to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <returns>The transformed normal.</returns>
		public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
		{
			Vector3 result;
			TransformNormal(ref normal, ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms a normal by a matrix.
		/// </summary>
		/// <param name="normal">Normal to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="result">The transformed normal.</param>
		public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result)
		{
			result.X = (normal.X * matrix.M11) + (normal.Y * matrix.M21) + (normal.Z * matrix.M31);
			result.Y = (normal.X * matrix.M12) + (normal.Y * matrix.M22) + (normal.Z * matrix.M32);
			result.Z = (normal.X * matrix.M13) + (normal.Y * matrix.M23) + (normal.Z * matrix.M33);
		}

		/// <summary>
		/// Transforms a list of normals by a matrix.
		/// </summary>
		/// <param name="sourceArray">List of normals.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="destinationArray">Output list.</param>
		public static void TransformNormal(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
		{
			for (int i = 0; i < sourceArray.Length; i++)
			{
				TransformNormal(ref sourceArray[i], ref matrix, out destinationArray[i]);
			}
		}

		/// <summary>
		/// Transforms a list of normals by the specified matrix.
		/// </summary>
		/// <param name="sourceArray">List of normals.</param>
		/// <param name="sourceIndex">Start index of source list.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="destinationArray">Output list.</param>
		/// <param name="destinationIndex">Output list starting index.</param>
		/// <param name="length">Number of normals to transform.</param>
		public static void TransformNormal(
			Vector3[] sourceArray,
			int sourceIndex,
			ref Matrix matrix,
			Vector3[] destinationArray,
			int destinationIndex,
			int length)
		{
			length += sourceIndex;
			for (int i = sourceIndex; i < length; i++, destinationIndex++)
			{
				TransformNormal(ref sourceArray[i], ref matrix, out destinationArray[destinationIndex]);
			}
		}
		
		/// <summary>
		/// Add two vectors and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The added vector.</returns>
		public static Vector3 operator +(Vector3 value1, Vector3 value2)
		{
			Vector3 result = new Vector3();
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
			return result;
		}

		/// <summary>
		/// Divide a vector by a the specified value.
		/// </summary>
		/// <param name="value1">Source vector.</param>
		/// <param name="divider">Divider value.</param>
		/// <returns>The divided vector.</returns>
		public static Vector3 operator /(Vector3 value1, float divider)
		{
			Vector3 result = new Vector3();
			float divFactor = 1f / divider;
			result.X = value1.X * divFactor;
			result.Y = value1.Y * divFactor;
			result.Z = value1.Z * divFactor;
			return result;
		}

		/// <summary>
		/// Divide a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The first vector divided by the second.</returns>
		public static Vector3 operator /(Vector3 value1, Vector3 value2)
		{
			Vector3 result = new Vector3();
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			return result;
		}

		/// <summary>
		/// Test if the two vectors are equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are equal.</returns>
		public static bool operator ==(Vector3 value1, Vector3 value2)
		{
			return value1.Equals(value2);
		}

		/// <summary>
		/// Test if the two vectors are not equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are not equal.</returns>
		public static bool operator !=(Vector3 value1, Vector3 value2)
		{
			return !value1.Equals(value2);
		}

		/// <summary>
		/// Returns a vector scaled by the specified value.
		/// </summary>
		/// <param name="value">Source vector.</param>
		/// <param name="scaleFactor">Scale factor.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3 operator *(Vector3 value, float scaleFactor)
		{
			Vector3 result = new Vector3();
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			result.Z = value.Z * scaleFactor;
			return result;
		}

		/// <summary>
		/// Returns a vector scaled by the specified value.
		/// </summary>
		/// <param name="scaleFactor">Scale factor.</param>
		/// <param name="value">Source vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3 operator *(float scaleFactor, Vector3 value)
		{
			Vector3 result = new Vector3();
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			result.Z = value.Z * scaleFactor;
			return result;
		}
		
		/// <summary>
		/// Multiply a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The first vector multiplied by the second.</returns>
		public static Vector3 operator *(Vector3 value1, Vector3 value2)
		{
			Vector3 result = new Vector3();
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
			return result;
		}

		/// <summary>
		/// Subtract the second vector from the first and return the resulting vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The subtracted vector.</returns>
		public static Vector3 operator -(Vector3 value1, Vector3 value2)
		{
			Vector3 result = new Vector3();
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
			return result;
		}

		/// <summary>
		/// Negate a vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <returns>The negated vector.</returns>
		public static Vector3 operator -(Vector3 value)
		{
			Vector3 result = new Vector3();
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			return result;
		}

		/// <summary>
		/// Subtract a value from a vector.
		/// </summary>
		/// <param name="value">Vector to modify.</param>
		/// <param name="difValue">Value to subtract.</param>
		/// <returns>The reduced vector.</returns>
		public static Vector3 operator -(Vector3 value, float difValue)
		{
			Vector3 result = new Vector3();
			result.X = value.X - difValue;
			result.Y = value.Y - difValue;
			result.Z = value.Z - difValue;
			return result;
		}
		
		/// <summary>
		/// Calculate the length of this vector.
		/// </summary>
		/// <returns>Length of this vector.</returns>
		public float Length()
		{
			return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
		}

		/// <summary>
		/// Calculate the squared length of this vector.
		/// </summary>
		/// <returns>Squared length of this vector.</returns>
		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		/// <summary>
		/// Normalize this vector.
		/// </summary>
		public void Normalize()
		{
			float divFactor = 1f / Length();
			X = X * divFactor;
			Y = Y * divFactor;
			Z = Z * divFactor;
		}

		/// <summary>
		/// Get the hash value of this vector.
		/// </summary>
		/// <returns>The hash code of this vector.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
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
		/// Test if this vector equals the specified object.
		/// </summary>
		/// <param name="obj">Object to test for equality.</param>
		/// <returns>True if the specified object equals this vector.</returns>
		public override bool Equals(Object obj)
		{
			return (obj is Vector3) ? Equals((Vector3)obj) : false;
		}

		/// <summary>
		/// Test if this vector equals the specified vector.
		/// </summary>
		/// <param name="other">Vector to test for equality.</param>
		/// <returns>True if the vectors are equal.</returns>
		public bool Equals(Vector3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		/// <summary>
		/// Returns a normalized vector if source is not zero length.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector3 SafeNormalized()
		{
			if (IsZero)
			{
				Vector3 result;
				Normalize(ref this, out result);
				return result;
			}

			return Zero;
		}

		/// <summary>
		/// Returns a normalized vector if source is not zero length.
		/// </summary>
		/// <param name="length">The length of this vector.</param>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector3 SafeNormalized(float length)
		{
			if (length > 0)
			{
				return Normalized(length);
			}

			return Zero;
		}

		/// <summary>
		/// Returns a normalized vector.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector3 Normalized()
		{
			Vector3 result;
			Normalize(ref this, out result);
			return result;
		}

		/// <summary>
		/// Normalizes vector with the specified length.
		/// </summary>
		/// <param name="length">Length of this vector.</param>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector3 Normalized(float length)
		{
			return new Vector3(X / length, Y / length, Z / length);
		}
	}
}