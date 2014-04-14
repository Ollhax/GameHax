using System;
using System.ComponentModel;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// Simple two-dimensional float vector.
	/// </summary>
	[Serializable, TypeConverter(typeof(Vector2Converter))]
	public struct Vector2 : IEquatable<Vector2>
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
		/// Returns a vector with X and Y set to 1.
		/// </summary>
		public static Vector2 One
		{
			get { return new Vector2(1.0f, 1.0f); }
		}
		
		/// <summary>
		/// Returns a vector with X and Y set to 0.
		/// </summary>
		public static Vector2 Zero
		{
			get { return new Vector2(0.0f, 0.0f); }
		}
		
		/// <summary>
		/// Create a vector with both vectors set to the specified value.
		/// </summary>
		/// <param name="value">Specified value.</param>
		public Vector2(float value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
		/// Create a vector with the specified X and Y coordinates.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Copy the specified vector.
		/// </summary>
		/// <param name="v">Vector to copy.</param>
		public Vector2(Vector2 v)
		{
			X = v.X;
			Y = v.Y;
		}

		/// <summary>
		/// Copy the specified vector.
		/// </summary>
		/// <param name="v">Vector to copy.</param>
		public Vector2(Vector2I v)
		{
			X = v.X;
			Y = v.Y;
		}

		/// <summary>
		/// Convert a float vector to an integer vector.
		/// </summary>
		/// <param name="v">Input vector.</param>
		/// <returns>Output integer vector.</returns>
		public static explicit operator Vector2I(Vector2 v)
		{
			return new Vector2I((int)v.X, (int)v.Y);
		}
		
		/// <summary>
		/// Returns true if all this vector's components are zero.
		/// </summary>
		public bool IsZero
		{
			get { return X == 0 && Y == 0; }
		}
		
		/// <summary>
		/// Clamp a vector between the min and max values.
		/// </summary>
		/// <param name="value1">Vector to clamp.</param>
		/// <param name="min">Min vector.</param>
		/// <param name="max">Max vector.</param>
		/// <returns>The clamped vector.</returns>
		public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
		{
			Vector2 result;
			Clamp(ref value1, ref min, ref max, out result);
			return result;
		}

		/// <summary>
		/// Clamp a vector between the min and max values.
		/// </summary>
		/// <param name="value1">Vector to clamp.</param>
		/// <param name="min">Min vector.</param>
		/// <param name="max">Max vector.</param>
		/// <param name="result">The clamped vector.</param>
		public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
		{
			result.X = MathTools.Clamp(value1.X, min.X, max.X);
			result.Y = MathTools.Clamp(value1.Y, min.Y, max.Y);
		}

		/// <summary>
		/// Calculate the distance between two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>Distance between the two vectors.</returns>
		public static float Distance(Vector2 value1, Vector2 value2)
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
		/// <param name="result">Distance between the two vectors.</param>
		public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
		{
			Vector2 tmp;
			Subtract(ref value1, ref value2, out tmp);
			result = tmp.Length();
		}

		/// <summary>
		/// Calculate the squared distance between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>Squared distance between the two vectors</returns>
		public static float DistanceSquared(Vector2 value1, Vector2 value2)
		{
			float result;
			DistanceSquared(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Calculate the squared distance between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">Squared distance between the two vectors.</param>
		public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
		{
			Vector2 tmp;
			Subtract(ref value1, ref value2, out tmp);
			result = tmp.LengthSquared();
		}
		
		/// <summary>
		/// Perform the dot product between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The dot product between the first vector and the second.</returns>
		public static float Dot(Vector2 value1, Vector2 value2)
		{
			float result;
			Dot(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Perform the dot product between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="output">The dot product between the first vector and the second.</param>
		public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
		{
			result = value1.X*value2.X + value1.Y*value2.Y;
		}

		/// <summary>
		/// The hash code value for this vector.
		/// </summary>
		/// <returns>The hash code value for this vector.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}
		
		/// <summary>
		/// Returns the length of this vector.
		/// </summary>
		/// <returns>The length of this vector.</returns>
		public float Length()
		{
			return (float) Math.Sqrt((X*X) + (Y*Y));
		}

		/// <summary>
		/// Returns the squared length of this vector.
		/// </summary>
		/// <returns>The squared length of this vector.</returns>
		public float LengthSquared()
		{
			return (X*X) + (Y*Y);
		}

		/// <summary>
		/// Linearly interpolates between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <returns>A linearly interpolated vector.</returns>
		public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
		{
			Vector2 result;
			Lerp(ref value1, ref value2, amount, out result);
			return result;
		}

		/// <summary>
		/// Linearly interpolates between the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <param name="result">A linearly interpolated vector.</param>
		public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
		{
			result.X = MathTools.Lerp(value1.X, value2.X, amount);
			result.Y = MathTools.Lerp(value1.Y, value2.Y, amount);
		}

		/// <summary>
		/// Returns the per-component maximum value of the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A vector containing the highest values per component.</returns>
		public static Vector2 Max(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Max(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Returns the per-component maximum value of the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">A vector containing the highest values per component.</param>
		public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = (value1.X > value2.X) ? value1.X : value2.X;
			result.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
		}
		
		/// <summary>
		/// Returns the per-component minimum value of the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A vector containing the lowest values per component.</returns>
		public static Vector2 Min(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Min(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Returns the per-component minimum value of the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">A vector containing the lowest values per component.</param>
		public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = (value1.X < value2.X) ? value1.X : value2.X;
			result.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
		}

		/// <summary>
		/// Add the two vectors together and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>Resulting vector.</returns>
		public static Vector2 Add(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Vector2.Add(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Add the two vectors together and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">Resulting vector.</param>
		public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
		}

		/// <summary>
		/// Subtract the second vector from the first and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>Resulting vector.</returns>
		public static Vector2 Subtract(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Subtract(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Subtract the second vector from the first and return the result.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">Resulting vector.</param>
		public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result = value1 - value2;
		}

		/// <summary>
		/// Divide a vector by the specified value.
		/// </summary>
		/// <param name="value1">Input vector.</param>
		/// <param name="divider">Value to divide the components by.</param>
		/// <returns>Vector with components divided.</returns>
		public static Vector2 Divide(Vector2 value1, float divider)
		{
			Vector2 result;
			Vector2.Divide(ref value1, divider, out result);
			return result;
		}

		/// <summary>
		/// Divide a vector by the specified value.
		/// </summary>
		/// <param name="value1">Input vector.</param>
		/// <param name="divider">Value to divide the components by.</param>
		/// <param name="result">Vector with components divided.</param>
		public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
		{
			divider = 1f/divider;
			result.X = value1.X*divider;
			result.Y = value1.Y*divider;
		}

		/// <summary>
		/// Divide a vector by the specified value.
		/// </summary>
		/// <param name="value1">Input vector.</param>
		/// <param name="value2">Dividing vector.</param>
		/// <returns>A vector where each component of the first vector was divided by that of the second.</returns>
		public static Vector2 Divide(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Divide(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Divide a vector by the specified value.
		/// </summary>
		/// <param name="value1">Input vector.</param>
		/// <param name="value2">Dividing vector.</param>
		/// <param name="result">A vector where each component of the first vector was divided by that of the second.</param>
		public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X/value2.X;
			result.Y = value1.Y/value2.Y;
		}

		/// <summary>
		/// Multiplies the specified vector by a scale factor.
		/// </summary>
		/// <param name="value1">Specified vector.</param>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <returns>A scaled vector.</returns>
		public static Vector2 Multiply(Vector2 value1, float scaleFactor)
		{
			Vector2 result;
			Multiply(ref value1, scaleFactor, out result);
			return result;
		}

		/// <summary>
		/// Multiplies the specified vector by a scale factor.
		/// </summary>
		/// <param name="value1">Specified vector.</param>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <param name="result">A scaled vector.</param>
		public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
		{
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
		}

		/// <summary>
		/// Multiplies the specified vector by the components of another vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A multiplied vector.</returns>
		public static Vector2 Multiply(Vector2 value1, Vector2 value2)
		{
			Vector2 result;
			Multiply(ref value1, ref value2, out result);
			return result;
		}

		/// <summary>
		/// Multiplies the specified vector by the components of another vector.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <param name="result">A multiplied vector.</param>
		public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X*value2.X;
			result.Y = value1.Y*value2.Y;
		}

		/// <summary>
		/// Return the negated vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <returns>A negated vector.</returns>
		public static Vector2 Negate(Vector2 value)
		{
			Vector2 result;
			Negate(ref value, out result);
			return result;
		}

		/// <summary>
		/// Return the negated vector.
		/// </summary>
		/// <param name="value">Vector to negate.</param>
		/// <param name="result">A negated vector.</param>
		public static void Negate(ref Vector2 value, out Vector2 result)
		{
			result = -value;
		}

		/// <summary>
		/// Normalize this vector, i.e. divide each component by the vector's length.
		/// The resulting vector will have length = 1.
		/// </summary>
		public void Normalize()
		{
			float divider = 1f/Length();
			X *= divider;
			Y *= divider;
		}
		
		/// <summary>
		/// Normalize the specified vector,  i.e. divide each component by the vector's length.
		/// The resulting vector will have length = 1.
		/// </summary>
		/// <param name="value">Vector to normalize.</param>
		/// <returns>A normalized vector.</returns>
		public static Vector2 Normalize(Vector2 value)
		{
			float divider = 1f/value.Length();
			return new Vector2(value.X*divider, value.Y*divider);
		}

		/// <summary>
		/// Normalize the specified vector,  i.e. divide each component by the vector's length.
		/// The resulting vector will have length = 1.
		/// </summary>
		/// <param name="value">Vector to normalize.</param>
		/// <param name="result">A normalized vector.</param>
		public static void Normalize(ref Vector2 value, out Vector2 result)
		{
			float divider = 1f/value.Length();
			result.X = value.X*divider;
			result.Y = value.Y*divider;
		}

		/// <summary>
		/// Returns a normalized vector if source is not zero length.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector2 SafeNormalized()
		{
			if (!IsZero)
			{
				float divider = 1f / Length();
				return new Vector2(X * divider, Y * divider);
			}

			return Zero;
		}

		/// <summary>
		/// Returns a normalized vector if source is not zero length.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector2 SafeNormalized(float length)
		{
			if (length > 0)
			{
				float divider = 1f / Length();
				return new Vector2(X * divider, Y * divider);
			}

			return Zero;
		}
		
		/// <summary>
		/// Reflect a vector around the specified normal.
		/// </summary>
		/// <param name="vector">Vector to reflect.</param>
		/// <param name="normal">Normal vector.</param>
		/// <returns>A reflected vector.</returns>
		public static Vector2 Reflect(Vector2 vector, Vector2 normal)
		{
			Vector2 result;
			Reflect(ref vector, ref normal, out result);
			return result;
		}

		/// <summary>
		/// Reflect a vector around the specified normal.
		/// </summary>
		/// <param name="vector">Vector to reflect.</param>
		/// <param name="normal">Normal vector.</param>
		/// <param name="result">A reflected vector.</param>
		public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
		{
			float sub = 2 * Dot(vector, normal);
			result.X = vector.X - (sub*normal.X);
			result.Y = vector.Y - (sub*normal.Y);
		}

		/// <summary>
		/// Return the string representation of this vector.
		/// </summary>
		/// <returns>The string representation of this vector.</returns>
		public override string ToString()
		{
			return "{X=" + TypeConvert.ToString(X) + ", Y=" + TypeConvert.ToString(Y) + "}";
		}

		/// <summary>
		/// Transform this vector by the specified matrix.
		/// </summary>
		/// <param name="position">Vector to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <returns>The resulting vector.</returns>
		public static Vector2 Transform(Vector2 position, Matrix matrix)
		{
			Vector2 result;
			Transform(ref position, ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Transform this vector by the specified matrix.
		/// </summary>
		/// <param name="position">Vector to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="result">The resulting vector.</param>
		public static void Transform(ref Vector2 position, ref Matrix matrix, out Vector2 result)
		{
			result.X = ((position.X*matrix.M11) + (position.Y*matrix.M21)) + matrix.M41;
			result.Y = ((position.X*matrix.M12) + (position.Y*matrix.M22)) + matrix.M42;
		}
		
		/// <summary>
		/// Transform a list of vectors by the specified matrix.
		/// </summary>
		/// <param name="sourceArray">Vectors to transform.</param>
		/// <param name="sourceIndex">Start index of source array.</param>
		/// <param name="matrix">Transforming matrix</param>
		/// <param name="destinationArray">Output array.</param>
		/// <param name="destinationIndex">Output start index.</param>
		/// <param name="length">Number of vectors to transform.</param>
		public static void Transform(Vector2[] sourceArray, int sourceIndex, ref Matrix matrix, Vector2[] destinationArray,
		                             int destinationIndex, int length)
		{
			length += sourceIndex;
			for (int i = sourceIndex; i < length; i++, destinationIndex++)
				Transform(ref sourceArray[i], ref matrix, out destinationArray[destinationIndex]);
		}
		
		/// <summary>
		/// Transform a list of vectors by the specified matrix.
		/// </summary>
		/// <param name="sourceArray">Vectors to transform.</param>
		/// <param name="matrix">Transforming matrix</param>
		/// <param name="destinationArray">Output array.</param>
		public static void Transform(Vector2[] sourceArray, ref Matrix matrix, Vector2[] destinationArray)
		{
			for (int i = 0; i < sourceArray.Length; i++)
				Transform(ref sourceArray[i], ref matrix, out destinationArray[i]);
		}
		
		/// <summary>
		/// Transform the normal by the specified matrix.
		/// </summary>
		/// <param name="normal">Normal to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <returns>Transformed normal.</returns>
		public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
		{
			Vector2 result;
			TransformNormal(ref normal, ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Transform the normal by the specified matrix.
		/// </summary>
		/// <param name="normal">Normal to transform.</param>
		/// <param name="matrix">Transform matrix.</param>
		/// <param name="result">Transformed normal.</param>
		public static void TransformNormal(ref Vector2 normal, ref Matrix matrix, out Vector2 result)
		{
			result.X = ((normal.X*matrix.M11) + (normal.Y*matrix.M21));
			result.Y = ((normal.X*matrix.M12) + (normal.Y*matrix.M22));
		}

		/// <summary>
		/// Transform a list of normals.
		/// </summary>
		/// <param name="sourceArray">Normals to transform.</param>
		/// <param name="sourceIndex">Start index of the normal list.</param>
		/// <param name="matrix">Tranform matrix.</param>
		/// <param name="destinationArray">Output array.</param>
		/// <param name="destinationIndex">Output array starting index.</param>
		/// <param name="length">Number of normals to transform.</param>
		public static void TransformNormal(
			Vector2[] sourceArray,
			int sourceIndex,
			ref Matrix matrix,
			Vector2[] destinationArray,
			int destinationIndex,
			int length)
		{
			length += sourceIndex;
			for (int i = sourceIndex; i < length; i++, destinationIndex++)
				TransformNormal(ref sourceArray[i], ref matrix, out destinationArray[destinationIndex]);
		}

		/// <summary>
		/// Transform a list of normals.
		/// </summary>
		/// <param name="sourceArray">Normals to transform.</param>
		/// <param name="matrix">Tranform matrix.</param>
		/// <param name="destinationArray">Output array.</param>
		public static void TransformNormal(
			Vector2[] sourceArray,
			ref Matrix matrix,
			Vector2[] destinationArray)
		{
			for (int i = 0; i < sourceArray.Length; i++)
				TransformNormal(ref sourceArray[i], ref matrix, out destinationArray[i]);
		}

		/// <summary>
		/// Test if this vector equals the specified object.
		/// </summary>
		/// <param name="obj">Object to test for equality.</param>
		/// <returns>True if the specified object equals this vector.</returns>
		public override bool Equals(Object obj)
		{
			return (obj is Vector2) ? Equals((Vector2) obj) : false;
		}

		/// <summary>
		/// Test if this vector equals the specified vector.
		/// </summary>
		/// <param name="other">Vector to test for equality.</param>
		/// <returns>True if the vectors are equal.</returns>
		public bool Equals(Vector2 other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		/// Add the two vectors together.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The two vectors added together.</returns>
		public static Vector2 operator +(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
		}

		/// <summary>
		/// Divide a vector by the specified amount.
		/// </summary>
		/// <param name="value1">Vector to divide.</param>
		/// <param name="divider">Divisor.</param>
		/// <returns>A vector divided by the specified amount.</returns>
		public static Vector2 operator /(Vector2 value1, float divider)
		{
			var factor = 1.0f / divider;
			return new Vector2(value1.X * factor, value1.Y * factor);
		}

		/// <summary>
		/// Divide the components of the first vector by the second vector's components.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>The divided vector.</returns>
		public static Vector2 operator /(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X/value2.X, value1.Y/value2.Y);
		}

		/// <summary>
		/// Test if the two vectors are equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are equal.</returns>
		public static bool operator ==(Vector2 value1, Vector2 value2)
		{
			return value1.X.Equals(value2.X) && value1.Y.Equals(value2.Y);
		}

		/// <summary>
		/// Test if the two vectors are not equal.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>True if the two vectors are not equal.</returns>
		public static bool operator !=(Vector2 value1, Vector2 value2)
		{
			return !value1.X.Equals(value2.X) || !value1.Y.Equals(value2.Y);
		}

		/// <summary>
		/// Multiply a vector by a scaling factor.
		/// </summary>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <param name="value">Vector.</param>
		/// <returns>A multiplied vector.</returns>
		public static Vector2 operator *(float scaleFactor, Vector2 value)
		{
			return new Vector2(value.X*scaleFactor, value.Y*scaleFactor);
		}

		/// <summary>
		/// Multiply a vector by a scaling factor.
		/// </summary>
		/// <param name="value">Vector.</param>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <returns>A multiplied vector.</returns>
		public static Vector2 operator *(Vector2 value, float scaleFactor)
		{
			return new Vector2(value.X*scaleFactor, value.Y*scaleFactor);
		}

		/// <summary>
		/// Multiply the components of the two vectors.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A multiplied vector.</returns>
		public static Vector2 operator *(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X*value2.X, value1.Y*value2.Y);
		}

		/// <summary>
		/// Subtract the values of the second vector from the first one.
		/// </summary>
		/// <param name="value1">First vector.</param>
		/// <param name="value2">Second vector.</param>
		/// <returns>A subtracted vector.</returns>
		public static Vector2 operator -(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		/// <summary>
		/// Negate the values of a vector.
		/// </summary>
		/// <param name="value">Specified vector.</param>
		/// <returns>A vector with negated component values.</returns>
		public static Vector2 operator -(Vector2 value)
		{
			return new Vector2(-value.X, -value.Y);
		}

		///// <summary>
		///// Returns a new Vector2 with by casting each of the vector's components to integers.
		///// </summary>
		///// <remarks>
		///// This is useful for rendering 2D sprites. It's almost always best to round the values
		///// to integers for rendering to avoid filtering artifacts, but generally you want to 
		///// use floating point values for the position for easier dynamics code.
		///// </remarks>
		///// <param name="vector">The source vector.</param>
		///// <returns>A new Vector2 with the value of {(int)x, (int)y}.</returns>
		//public static Vector2 ToFloorVector(this Vector2 vector)
		//{
		//    return new Vector2((int)vector.X, (int)vector.Y);
		//}

		///// <summary>
		///// Returns a new Vector2 that has rounded its components.
		///// </summary>
		///// <param name="vector">The source vector.</param>
		///// <returns>A new Vector2 with the value of {Math.Round(x), Math.Round(y)}.</returns>
		//public static Vector2 ToRoundVector(this Vector2 vector)
		//{
		//    return new Vector2((float)Math.Round(vector.X), (float)Math.Round(vector.Y));
		//}
		
		/// <summary>
		/// Returns a Vector2 that is perpendicular to the vector.
		/// </summary>
		/// <returns>A Vector2 that is perpendicular to the vector</returns>
		public Vector2 GetPerpendicular()
		{
			return new Vector2(-Y, X);
		}
		
		/// <summary>
		/// If the specified vector is zero, return instead a vector with the specified defaults.
		/// </summary>
		/// <param name="defaultVector">The value to default to if the input vector is zero.</param>
		/// <returns>A vector that is either the input vector or replacement vector.</returns>
		public Vector2 DefaultTo(Vector2 defaultVector)
		{
			if (IsZero)
			{
				return defaultVector;
			}

			return this;
		}

		/// <summary>
		/// If the specified vector is zero, return instead a vector with the specified defaults.
		/// </summary>
		/// <param name="defaultX">The X value to default to if the input vector is zero.</param>
		/// <param name="defaultY">The Y value to default to if the input vector is zero.</param>
		/// <returns>A vector that is either the input vector or replacement vector.</returns>
		public Vector2 DefaultTo(float defaultX = 1.0f, float defaultY = 0.0f)
		{
			if (IsZero)
			{
				return new Vector2(defaultX, defaultY);
			}

			return this;
		}

		/// <summary>
		/// Returns a normalized vector.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector2 Normalized()
		{
			float divider = 1f / Length();
			return new Vector2(X * divider, Y * divider);
		}

		/// <summary>
		/// Normalizes vector with the specified length.
		/// </summary>
		/// <returns>A normalized version of the source vector.</returns>
		public Vector2 Normalized(float length)
		{
			float divider = 1f / length;
			return new Vector2(X * divider, Y * divider);
		}

		///// <summary>
		///// Check this vector is valid.
		///// </summary>
		///// <returns>True if both components are valid. False if one of them is NaN or infinite.</returns>
		//public static bool IsValid(this Vector2 vector)
		//{
		//    return (
		//        !float.IsNaN(vector.X) &&
		//        !float.IsNaN(vector.Y) &&
		//        !float.IsInfinity(vector.X) &&
		//        !float.IsInfinity(vector.Y));
		//}

		/// <summary>
		/// Returns a vector that is rotated around the origin by the specified angle.
		/// </summary>
		/// <param name="angle">Rotation angle (radians).</param>
		/// <returns>The rotated vector.</returns>
		public Vector2 Rotated(float angle)
		{
			Matrix transform = Matrix.CreateRotationZ(angle);
			return Transform(this, transform);
		}

		/// <summary>
		/// Returns a vector that is rotated around a specified axis by the specified angle.
		/// </summary>
		/// <param name="axis">Axis to rotate around.</param>
		/// <param name="angle">Rotation angle (radians).</param>
		/// <returns>The rotated vector.</returns>
		public Vector2 Rotated(Vector2 axis, float angle)
		{
			Matrix transform = Matrix.CreateRotationZ(angle);
			return Transform((this - axis), transform) + axis;
		}

		///// <summary>
		///// Returns a vector that is mirrored around the specified line.
		///// </summary>
		///// <param name="vector">The source vector.</param>
		///// <param name="point">Any point on the line to reflect around.</param>
		///// <param name="direction">Normalized direction of line.</param>
		///// <returns>The reflected vector.</returns>
		//public static Vector2 Mirror(this Vector2 vector, Vector2 point, Vector2 direction)
		//{
		//    // Get closest point on line
		//    Vector2 diff = vector - point;
		//    Vector2 closestPoint = Vector2.Dot(diff, direction) * direction + point;
		//    diff = closestPoint - vector;
		//    return closestPoint + diff;
		//}

		/// <summary>
		/// Project this vector unto another vector.
		/// </summary>
		/// <param name="target">Target vector. Will be normalized.</param>
		/// <returns>Projected vector.</returns>
		public Vector2 Project(Vector2 target)
		{
			if (!target.IsZero)
			{
				target.Normalize();

				float v = Vector2.Dot(this, target);
				target = target * v;

				return target;
			}

			return Zero;
		}

		/// <summary>
		/// Project this vector unto a normal.
		/// </summary>
		/// <param name="target">Target normal. Must be normalized.</param>
		/// <returns>Projected vector.</returns>
		public Vector2 ProjectNormal(Vector2 normal)
		{
			if (!normal.IsZero)
			{
				float v = Dot(this, normal);
				normal = normal * v;

				return normal;
			}

			return Zero;
		}

		/// <summary>
		/// Returns the angle this vector is pointing at.
		/// </summary>
		/// <returns>The angle, measured in radians.</returns>
		public float Angle()
		{
			return (float)Math.Atan2(Y, X);
		}
	}
}