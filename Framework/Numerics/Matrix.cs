using System.Runtime.InteropServices;
using System;
using System.Globalization;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A 4x4 matrix representation.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix : IEquatable<Matrix>
	{
		/// <summary>
		/// Value at row 1, column 1.
		/// </summary>
		public float M11;
		
		/// <summary>
		/// Value at row 1, column 2.
		/// </summary>
		public float M12;

		/// <summary>
		/// Value at row 1, column 3.
		/// </summary>
		public float M13;

		/// <summary>
		/// Value at row 1, column 4.
		/// </summary>
		public float M14;

		/// <summary>
		/// Value at row 2, column 1.
		/// </summary>
		public float M21;

		/// <summary>
		/// Value at row 2, column 2.
		/// </summary>
		public float M22;

		/// <summary>
		/// Value at row 2, column 3.
		/// </summary>
		public float M23;

		/// <summary>
		/// Value at row 2, column 4.
		/// </summary>
		public float M24;

		/// <summary>
		/// Value at row 3, column 1.
		/// </summary>
		public float M31;

		/// <summary>
		/// Value at row 3, column 2.
		/// </summary>
		public float M32;

		/// <summary>
		/// Value at row 3, column 3.
		/// </summary>
		public float M33;

		/// <summary>
		/// Value at row 3, column 4.
		/// </summary>
		public float M34;

		/// <summary>
		/// Value at row 4, column 1.
		/// </summary>
		public float M41;

		/// <summary>
		/// Value at row 4, column 2.
		/// </summary>
		public float M42;

		/// <summary>
		/// Value at row 4, column 3.
		/// </summary>
		public float M43;

		/// <summary>
		/// Value at row 4, column 4.
		/// </summary>
		public float M44;
		
		/// <summary>
		/// Returns the identity matrix.
		/// </summary>
		public static Matrix Identity { get { return identity; } }
		
		private static Matrix identity = new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

		/// <summary>
		/// Get or set the up vector.
		/// </summary>
		public Vector3 Up
		{
			get
			{
				Vector3 vector;
				vector.X = M21;
				vector.Y = M22;
				vector.Z = M23;
				return vector;
			}
			set
			{
				M21 = value.X;
				M22 = value.Y;
				M23 = value.Z;
			}
		}

		/// <summary>
		/// Get or set the down vector.
		/// </summary>
		public Vector3 Down
		{
			get
			{
				Vector3 vector;
				vector.X = -M21;
				vector.Y = -M22;
				vector.Z = -M23;
				return vector;
			}
			set
			{
				M21 = -value.X;
				M22 = -value.Y;
				M23 = -value.Z;
			}
		}

		/// <summary>
		/// Get or set the right vector.
		/// </summary>
		public Vector3 Right
		{
			get
			{
				Vector3 vector;
				vector.X = M11;
				vector.Y = M12;
				vector.Z = M13;
				return vector;
			}
			set
			{
				M11 = value.X;
				M12 = value.Y;
				M13 = value.Z;
			}
		}

		/// <summary>
		/// Get or set the left vector.
		/// </summary>
		public Vector3 Left
		{
			get
			{
				Vector3 vector;
				vector.X = -M11;
				vector.Y = -M12;
				vector.Z = -M13;
				return vector;
			}
			set
			{
				M11 = -value.X;
				M12 = -value.Y;
				M13 = -value.Z;
			}
		}

		/// <summary>
		/// Get or set the forward vector.
		/// </summary>
		public Vector3 Forward
		{
			get
			{
				Vector3 vector;
				vector.X = -M31;
				vector.Y = -M32;
				vector.Z = -M33;
				return vector;
			}
			set
			{
				M31 = -value.X;
				M32 = -value.Y;
				M33 = -value.Z;
			}
		}

		/// <summary>
		/// Get or set the backward vector.
		/// </summary>
		public Vector3 Backward
		{
			get
			{
				Vector3 vector;
				vector.X = M31;
				vector.Y = M32;
				vector.Z = M33;
				return vector;
			}
			set
			{
				M31 = value.X;
				M32 = value.Y;
				M33 = value.Z;
			}
		}

		/// <summary>
		/// Get or set the translation.
		/// </summary>
		public Vector3 Translation
		{
			get
			{
				Vector3 vector;
				vector.X = M41;
				vector.Y = M42;
				vector.Z = M43;
				return vector;
			}
			set
			{
				M41 = value.X;
				M42 = value.Y;
				M43 = value.Z;
			}
		}
		
		/// <summary>
		/// Get or set the translation on the X Y plane.
		/// </summary>
		public Vector2 TranslationXY
		{
			get
			{
				Vector2 vector;
				vector.X = M41;
				vector.Y = M42;
				return vector;
			}
			set
			{
				M41 = value.X;
				M42 = value.Y;
			}
		}

		/// <summary>
		/// Get or set the scale.
		/// </summary>
		public Vector3 Scale
		{
			get
			{
				Vector3 vector;
				vector.X = M11;
				vector.Y = M22;
				vector.Z = M33;
				return vector;
			}
			set
			{
				M11 = value.X;
				M22 = value.Y;
				M33 = value.Z;
			}
		}

		/// <summary>
		/// Create a new matrix.
		/// </summary>
		/// <param name="m11">Value at row 1, column 1.</param>
		/// <param name="m12">Value at row 1, column 2.</param>
		/// <param name="m13">Value at row 1, column 3.</param>
		/// <param name="m14">Value at row 1, column 4.</param>
		/// <param name="m21">Value at row 2, column 1.</param>
		/// <param name="m22">Value at row 2, column 2.</param>
		/// <param name="m23">Value at row 2, column 3.</param>
		/// <param name="m24">Value at row 2, column 4.</param>
		/// <param name="m31">Value at row 3, column 1.</param>
		/// <param name="m32">Value at row 3, column 2.</param>
		/// <param name="m33">Value at row 3, column 3.</param>
		/// <param name="m34">Value at row 3, column 4.</param>
		/// <param name="m41">Value at row 4, column 1.</param>
		/// <param name="m42">Value at row 4, column 2.</param>
		/// <param name="m43">Value at row 4, column 3.</param>
		/// <param name="m44">Value at row 4, column 4.</param>
		public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		/// <summary>
		/// Create a spherical billboard.
		/// </summary>
		/// <param name="objectPosition">Position of object.</param>
		/// <param name="cameraPosition">Position of camera.</param>
		/// <param name="cameraUpVector">Camera up vector.</param>
		/// <param name="cameraForwardVector">Camera forward direction. Use in case object and camera positions are close.</param>
		/// <returns>The created billboard matrix.</returns>
		public static Matrix CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3? cameraForwardVector)
		{
			Matrix result;
			CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out result);
			return result;
		}

		/// <summary>
		/// Create a spherical billboard.
		/// </summary>
		/// <param name="objectPosition">Position of object.</param>
		/// <param name="cameraPosition">Position of camera.</param>
		/// <param name="cameraUpVector">Camera up vector.</param>
		/// <param name="cameraForwardVector">Camera forward direction. Use in case object and camera positions are close.</param>
		/// <param name="result">The created billboard matrix.</param>
		public static void CreateBillboard(
			ref Vector3 objectPosition,
			ref Vector3 cameraPosition,
			ref Vector3 cameraUpVector,
			Vector3? cameraForwardVector,
			out Matrix result)
		{
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;

			vector.X = objectPosition.X - cameraPosition.X;
			vector.Y = objectPosition.Y - cameraPosition.Y;
			vector.Z = objectPosition.Z - cameraPosition.Z;
			float num = vector.LengthSquared();

			if (num < 0.0001f)
			{
				vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
			}
			else
			{
				Vector3.Multiply(ref vector, (float)(1f / ((float)Math.Sqrt((double)num))), out vector);
			}

			Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
			vector3.Normalize();
			Vector3.Cross(ref vector, ref vector3, out vector2);
			result.M11 = vector3.X;
			result.M12 = vector3.Y;
			result.M13 = vector3.Z;
			result.M14 = 0f;
			result.M21 = vector2.X;
			result.M22 = vector2.Y;
			result.M23 = vector2.Z;
			result.M24 = 0f;
			result.M31 = vector.X;
			result.M32 = vector.Y;
			result.M33 = vector.Z;
			result.M34 = 0f;
			result.M41 = objectPosition.X;
			result.M42 = objectPosition.Y;
			result.M43 = objectPosition.Z;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a cylindrical billboard.
		/// </summary>
		/// <param name="objectPosition">Position of object.</param>
		/// <param name="cameraPosition">Position of camera.</param>
		/// <param name="rotateAxis">Axis to rotate around</param>
		/// <param name="cameraForwardVector">Camera forward direction. Use in case object and camera positions are close.</param>
		/// <param name="objectForwardVector">Object forward vector.</param>
		/// <returns>The created billboard matrix.</returns>
		public static Matrix CreateConstrainedBillboard(
			Vector3 objectPosition,
			Vector3 cameraPosition,
			Vector3 rotateAxis,
			Vector3? cameraForwardVector,
			Vector3? objectForwardVector)
		{
			Matrix result;
			CreateConstrainedBillboard(ref objectPosition, ref cameraPosition, ref rotateAxis, cameraForwardVector, objectForwardVector, out result);
			return result;
		}

		/// <summary>
		/// Create a cylindrical billboard.
		/// </summary>
		/// <param name="objectPosition">Position of object.</param>
		/// <param name="cameraPosition">Position of camera.</param>
		/// <param name="rotateAxis">Axis to rotate around</param>
		/// <param name="cameraForwardVector">Camera forward direction. Use in case object and camera positions are close.</param>
		/// <param name="objectForwardVector">Optional forward vector. Use in case object and camera positions are close.</param>
		/// <param name="result">The created billboard matrix.</param>
		public static void CreateConstrainedBillboard(
			ref Vector3 objectPosition,
			ref Vector3 cameraPosition,
			ref Vector3 rotateAxis,
			Vector3? cameraForwardVector,
			Vector3? objectForwardVector,
			out Matrix result)
		{
			float num;
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;

			vector2.X = objectPosition.X - cameraPosition.X;
			vector2.Y = objectPosition.Y - cameraPosition.Y;
			vector2.Z = objectPosition.Z - cameraPosition.Z;

			float num2 = vector2.LengthSquared();
			if (num2 < 0.0001f)
			{
				vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
			}
			else
			{
				Vector3.Multiply(ref vector2, (float)(1f / ((float)Math.Sqrt((double)num2))), out vector2);
			}

			Vector3 vector4 = rotateAxis;
			Vector3.Dot(ref rotateAxis, ref vector2, out num);

			if (Math.Abs(num) > 0.9982547f)
			{
				if (objectForwardVector.HasValue)
				{
					vector = objectForwardVector.Value;
					Vector3.Dot(ref rotateAxis, ref vector, out num);
					if (Math.Abs(num) > 0.9982547f)
					{
						num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y))
						      + (rotateAxis.Z * Vector3.Forward.Z);
						vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
					}
				}
				else
				{
					num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y))
					      + (rotateAxis.Z * Vector3.Forward.Z);
					vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
				}

				Vector3.Cross(ref rotateAxis, ref vector, out vector3);
				vector3.Normalize();
				Vector3.Cross(ref vector3, ref rotateAxis, out vector);
				vector.Normalize();
			}
			else
			{
				Vector3.Cross(ref rotateAxis, ref vector2, out vector3);
				vector3.Normalize();
				Vector3.Cross(ref vector3, ref vector4, out vector);
				vector.Normalize();
			}

			result.M11 = vector3.X;
			result.M12 = vector3.Y;
			result.M13 = vector3.Z;
			result.M14 = 0f;
			result.M21 = vector4.X;
			result.M22 = vector4.Y;
			result.M23 = vector4.Z;
			result.M24 = 0f;
			result.M31 = vector.X;
			result.M32 = vector.Y;
			result.M33 = vector.Z;
			result.M34 = 0f;
			result.M41 = objectPosition.X;
			result.M42 = objectPosition.Y;
			result.M43 = objectPosition.Z;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a translation matrix.
		/// </summary>
		/// <param name="position">Translation vector.</param>
		/// <returns>A translation matrix.</returns>
		public static Matrix CreateTranslation(Vector3 position)
		{
			Matrix matrix;
			matrix.M11 = 1f;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = 1f;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = position.X;
			matrix.M42 = position.Y;
			matrix.M43 = position.Z;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a translation matrix.
		/// </summary>
		/// <param name="position">Translation vector.</param>
		/// <param name="result">A translation matrix.</param>
		public static void CreateTranslation(ref Vector3 position, out Matrix result)
		{
			result.M11 = 1f;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 1f;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f;
			result.M34 = 0f;
			result.M41 = position.X;
			result.M42 = position.Y;
			result.M43 = position.Z;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a translation matrix.
		/// </summary>
		/// <param name="xTranslation">Translation x value.</param>
		/// <param name="yTranslation">Translation y value.</param>
		/// <param name="zTranslation">Translation z value.</param>
		/// <returns>A translation matrix.</returns>
		public static Matrix CreateTranslation(float xTranslation, float yTranslation, float zTranslation)
		{
			Matrix matrix;
			matrix.M11 = 1f;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = 1f;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = xTranslation;
			matrix.M42 = yTranslation;
			matrix.M43 = zTranslation;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a translation matrix.
		/// </summary>
		/// <param name="xTranslation">Translation x value.</param>
		/// <param name="yTranslation">Translation y value.</param>
		/// <param name="zTranslation">Translation z value.</param>
		/// <param name="result">A translation matrix.</param>
		public static void CreateTranslation(float xTranslation, float yTranslation, float zTranslation, out Matrix result)
		{
			result.M11 = 1f;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 1f;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f;
			result.M34 = 0f;
			result.M41 = xTranslation;
			result.M42 = yTranslation;
			result.M43 = zTranslation;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="xScale">Scale x value.</param>
		/// <param name="yScale">Scale y value.</param>
		/// <param name="zScale">Scale z value.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(float xScale, float yScale, float zScale)
		{
			Matrix matrix;
			float num3 = xScale;
			float num2 = yScale;
			float num = zScale;
			matrix.M11 = num3;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = num2;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = num;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="xScale">Scale x value.</param>
		/// <param name="yScale">Scale y value.</param>
		/// <param name="zScale">Scale z value.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(float xScale, float yScale, float zScale, out Matrix result)
		{
			float num3 = xScale;
			float num2 = yScale;
			float num = zScale;
			result.M11 = num3;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = num2;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = num;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="scales">Scale vector.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(Vector3 scales)
		{
			Matrix matrix;
			float x = scales.X;
			float y = scales.Y;
			float z = scales.Z;
			matrix.M11 = x;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = y;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = z;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="scales">Scale vector.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(ref Vector3 scales, out Matrix result)
		{
			float x = scales.X;
			float y = scales.Y;
			float z = scales.Z;
			result.M11 = x;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = y;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = z;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="scale">Scale value.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix CreateScale(float scale)
		{
			Matrix matrix;
			float num = scale;
			matrix.M11 = num;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = num;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = num;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a scale matrix.
		/// </summary>
		/// <param name="scale">Scale value.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(float scale, out Matrix result)
		{
			float num = scale;
			result.M11 = num;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = num;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = num;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <returns>A rotation matrix.</returns>
		public static Matrix CreateRotationX(float radians)
		{
			Matrix matrix;
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			matrix.M11 = 1f;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = num2;
			matrix.M23 = num;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = -num;
			matrix.M33 = num2;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <param name="result">A rotation matrix.</param>
		public static void CreateRotationX(float radians, out Matrix result)
		{
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			result.M11 = 1f;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = num2;
			result.M23 = num;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = -num;
			result.M33 = num2;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <returns>A rotation matrix.</returns>
		public static Matrix CreateRotationY(float radians)
		{
			Matrix matrix;
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			matrix.M11 = num2;
			matrix.M12 = 0f;
			matrix.M13 = -num;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = 1f;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = num;
			matrix.M32 = 0f;
			matrix.M33 = num2;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <param name="result">A rotation matrix.</param>
		public static void CreateRotationY(float radians, out Matrix result)
		{
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			result.M11 = num2;
			result.M12 = 0f;
			result.M13 = -num;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 1f;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = num;
			result.M32 = 0f;
			result.M33 = num2;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <returns>A rotation matrix.</returns>
		public static Matrix CreateRotationZ(float radians)
		{
			Matrix matrix;
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			matrix.M11 = num2;
			matrix.M12 = num;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = -num;
			matrix.M22 = num2;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a rotation matrix.
		/// </summary>
		/// <param name="radians">Rotation value.</param>
		/// <param name="result">A rotation matrix.</param>
		public static void CreateRotationZ(float radians, out Matrix result)
		{
			float num2 = (float)Math.Cos((double)radians);
			float num = (float)Math.Sin((double)radians);
			result.M11 = num2;
			result.M12 = num;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = -num;
			result.M22 = num2;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f;
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a rotation matrix around the specified axis.
		/// </summary>
		/// <param name="axis">Axis to rotate around</param>
		/// <param name="angle">Rotation angle.</param>
		/// <returns>A rotation matrix.</returns>
		public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
		{
			Matrix matrix;
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num2 = (float)Math.Sin(angle);
			float num = (float)Math.Cos(angle);
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;
			matrix.M11 = num11 + (num * (1f - num11));
			matrix.M12 = (num8 - (num * num8)) + (num2 * z);
			matrix.M13 = (num7 - (num * num7)) - (num2 * y);
			matrix.M14 = 0f;
			matrix.M21 = (num8 - (num * num8)) - (num2 * z);
			matrix.M22 = num10 + (num * (1f - num10));
			matrix.M23 = (num6 - (num * num6)) + (num2 * x);
			matrix.M24 = 0f;
			matrix.M31 = (num7 - (num * num7)) + (num2 * y);
			matrix.M32 = (num6 - (num * num6)) - (num2 * x);
			matrix.M33 = num9 + (num * (1f - num9));
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		/// <summary>
		/// Create a rotation matrix around the specified axis.
		/// </summary>
		/// <param name="axis">Axis to rotate around</param>
		/// <param name="angle">Rotation angle.</param>
		/// <param name="result">A rotation matrix.</param>
		public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix result)
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num2 = (float)Math.Sin((double)angle);
			float num = (float)Math.Cos((double)angle);
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;
			result.M11 = num11 + (num * (1f - num11));
			result.M12 = (num8 - (num * num8)) + (num2 * z);
			result.M13 = (num7 - (num * num7)) - (num2 * y);
			result.M14 = 0f;
			result.M21 = (num8 - (num * num8)) - (num2 * z);
			result.M22 = num10 + (num * (1f - num10));
			result.M23 = (num6 - (num * num6)) + (num2 * x);
			result.M24 = 0f;
			result.M31 = (num7 - (num * num7)) + (num2 * y);
			result.M32 = (num6 - (num * num6)) - (num2 * x);
			result.M33 = num9 + (num * (1f - num9));
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = 0f;
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a perspective matrix.
		/// </summary>
		/// <param name="fieldOfView">Field of view value.</param>
		/// <param name="aspectRatio">The view's aspect ratio.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane</param>
		/// <returns>A perspective matrix.</returns>
		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result;
			CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Create a perspective matrix.
		/// </summary>
		/// <param name="fieldOfView">Field of view value.</param>
		/// <param name="aspectRatio">The view's aspect ratio.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane</param>
		/// <param name="result">A perspective matrix.</param>
		public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
		{
			if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f))
			{
				throw new ArgumentOutOfRangeException("Invalid Field Of View.");
			}
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance.");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid far plane distance.");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance (farther than far plane).");
			}

			float num = 1f / ((float)Math.Tan((double)(fieldOfView * 0.5f)));
			float num9 = num / aspectRatio;
			result.M11 = num9;
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = num;
			result.M21 = result.M23 = result.M24 = 0f;
			result.M31 = result.M32 = 0f;
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.M34 = -1f;
			result.M41 = result.M42 = result.M44 = 0f;
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
		}

		/// <summary>
		/// Create a perspective matrix.
		/// </summary>
		/// <param name="width">Near plane width.</param>
		/// <param name="height">Near plane height.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane</param>
		/// <returns>A perspective matrix.</returns>
		public static Matrix CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result;
			CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Create a perspective matrix.
		/// </summary>
		/// <param name="width">Near plane width.</param>
		/// <param name="height">Near plane height.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane</param>
		/// <param name="result">A perspective matrix.</param>
		public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance.");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid far plane distance.");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance (farther than far plane).");
			}

			result.M11 = (2f * nearPlaneDistance) / width;
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = (2f * nearPlaneDistance) / height;
			result.M21 = result.M23 = result.M24 = 0f;
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.M31 = result.M32 = 0f;
			result.M34 = -1f;
			result.M41 = result.M42 = result.M44 = 0f;
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
		}

		/// <summary>
		/// Create a off-center perspective matrix.
		/// </summary>
		/// <param name="left">Near plane left side.</param>
		/// <param name="right">Near plane right side.</param>
		/// <param name="bottom">Near plane bottom.</param>
		/// <param name="top">Near plane top.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane.</param>
		/// <returns>A perspective matrix.</returns>
		public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result;
			CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out result);
			return result;
		}

		/// <summary>
		/// Create a off-center perspective matrix.
		/// </summary>
		/// <param name="left">Near plane left side.</param>
		/// <param name="right">Near plane right side.</param>
		/// <param name="bottom">Near plane bottom.</param>
		/// <param name="top">Near plane top.</param>
		/// <param name="nearPlaneDistance">Near plane.</param>
		/// <param name="farPlaneDistance">Far plane.</param>
		/// <param name="result">A perspective matrix.</param>
		public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance.");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Invalid far plane distance.");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("Invalid near plane distance (farther than far plane).");
			}
			result.M11 = (2f * nearPlaneDistance) / (right - left);
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = (2f * nearPlaneDistance) / (top - bottom);
			result.M21 = result.M23 = result.M24 = 0f;
			result.M31 = (left + right) / (right - left);
			result.M32 = (top + bottom) / (top - bottom);
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.M34 = -1f;
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
			result.M41 = result.M42 = result.M44 = 0f;
		}

		/// <summary>
		/// Create an orthographic perspective matrix.
		/// </summary>
		/// <param name="width">Width of the view volume.</param>
		/// <param name="height">Height of the view volume.</param>
		/// <param name="zNearPlane">Near plane.</param>
		/// <param name="zFarPlane">Far plane.</param>
		/// <returns>A perspective matrix.</returns>
		public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			Matrix result;
			CreateOrthographic(width, height, zNearPlane, zFarPlane, out result);
			return result;
		}

		/// <summary>
		/// Create an orthographic perspective matrix.
		/// </summary>
		/// <param name="width">Width of the view volume.</param>
		/// <param name="height">Height of the view volume.</param>
		/// <param name="zNearPlane">Near plane.</param>
		/// <param name="zFarPlane">Far plane.</param>
		/// <param name="result">A perspective matrix.</param>
		public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix result)
		{
			result.M11 = 2f / width;
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = 2f / height;
			result.M21 = result.M23 = result.M24 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M31 = result.M32 = result.M34 = 0f;
			result.M41 = result.M42 = 0f;
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;
		}

		/// <summary>
		/// Create an off-center orthographic perspective matrix.
		/// </summary>
		/// <param name="left">Left side of the view volume.</param>
		/// <param name="right">Right side of the view volume.</param>
		/// <param name="bottom">Bottom of the view volume.</param>
		/// <param name="top">Top of the view volume.</param>
		/// <param name="zNearPlane">Near plane.</param>
		/// <param name="zFarPlane">Far plane.</param>
		/// <returns>A perspective matrix.</returns>
		public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			Matrix result;
			CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out result);
			return result;
		}

		/// <summary>
		/// Create an off-center orthographic perspective matrix.
		/// </summary>
		/// <param name="left">Left side of the view volume.</param>
		/// <param name="right">Right side of the view volume.</param>
		/// <param name="bottom">Bottom of the view volume.</param>
		/// <param name="top">Top of the view volume.</param>
		/// <param name="zNearPlane">Near plane.</param>
		/// <param name="zFarPlane">Far plane.</param>
		/// <param name="result">A perspective matrix.</param>
		public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix result)
		{
			result.M11 = 2f / (right - left);
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = 2f / (top - bottom);
			result.M21 = result.M23 = result.M24 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M31 = result.M32 = result.M34 = 0f;
			result.M41 = (left + right) / (left - right);
			result.M42 = (top + bottom) / (bottom - top);
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;
		}

		/// <summary>
		/// Create a view matrix that looks at the specified target.
		/// </summary>
		/// <param name="cameraPosition">Camera position.</param>
		/// <param name="cameraTarget">Camera target.</param>
		/// <param name="cameraUpVector">The camera's up vector.</param>
		/// <returns>A view transform matrix.</returns>
		public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
		{
			Matrix result;
			CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out result);
			return result;
		}
		
		/// <summary>
		/// Create a view matrix that looks at the specified target.
		/// </summary>
		/// <param name="cameraPosition">Camera position.</param>
		/// <param name="cameraTarget">Camera target.</param>
		/// <param name="cameraUpVector">The camera's up vector.</param>
		/// <param name="result">A view transform matrix.</param>
		public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix result)
		{
			Vector3 vector = Vector3.Normalize(cameraPosition - cameraTarget);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			result.M11 = vector2.X;
			result.M12 = vector3.X;
			result.M13 = vector.X;
			result.M14 = 0f;
			result.M21 = vector2.Y;
			result.M22 = vector3.Y;
			result.M23 = vector.Y;
			result.M24 = 0f;
			result.M31 = vector2.Z;
			result.M32 = vector3.Z;
			result.M33 = vector.Z;
			result.M34 = 0f;
			result.M41 = -Vector3.Dot(vector2, cameraPosition);
			result.M42 = -Vector3.Dot(vector3, cameraPosition);
			result.M43 = -Vector3.Dot(vector, cameraPosition);
			result.M44 = 1f;
		}

		/// <summary>
		/// Creates a world transformation matrix.
		/// </summary>
		/// <param name="position">World position.</param>
		/// <param name="forward">Forward vector.</param>
		/// <param name="up">Up vector</param>
		/// <returns>A world transform matrix.</returns>
		public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
		{
			Matrix result;
			CreateWorld(ref position, ref forward, ref up, out result);
			return result;
		}

		/// <summary>
		/// Creates a world transformation matrix.
		/// </summary>
		/// <param name="position">World position.</param>
		/// <param name="forward">Forward vector.</param>
		/// <param name="up">Up vector</param>
		/// <param name="result">A world transform matrix.</param>
		public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix result)
		{
			Vector3 vector = Vector3.Normalize(-forward);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			result.M11 = vector2.X;
			result.M12 = vector2.Y;
			result.M13 = vector2.Z;
			result.M14 = 0f;
			result.M21 = vector3.X;
			result.M22 = vector3.Y;
			result.M23 = vector3.Z;
			result.M24 = 0f;
			result.M31 = vector.X;
			result.M32 = vector.Y;
			result.M33 = vector.Z;
			result.M34 = 0f;
			result.M41 = position.X;
			result.M42 = position.Y;
			result.M43 = position.Z;
			result.M44 = 1f;
		}
		
		/// <summary>
		/// Return the string representation of this vector.
		/// </summary>
		/// <returns>The string representation of this vector.</returns>
		public override string ToString()
		{
			return ("{ "
			        +
			        string.Format(
			        	"{{M11:{0} M12:{1} M13:{2} M14:{3}}} ",
			        	new object[]
			        		{
								TypeConvert.ToString(M11), TypeConvert.ToString(M12), TypeConvert.ToString(M13), TypeConvert.ToString(M14),
			        		})
			        +
			        string.Format(
			        	"{{M21:{0} M22:{1} M23:{2} M24:{3}}} ",
			        	new object[]
			        		{
			        			TypeConvert.ToString(M21), TypeConvert.ToString(M22), TypeConvert.ToString(M23), TypeConvert.ToString(M24),
			        		})
			        +
			        string.Format(
			        	"{{M31:{0} M32:{1} M33:{2} M34:{3}}} ",
			        	new object[]
			        		{
			        			TypeConvert.ToString(M31), TypeConvert.ToString(M32), TypeConvert.ToString(M33), TypeConvert.ToString(M34),
			        		})
			        +
			        string.Format(
			        	"{{M41:{0} M42:{1} M43:{2} M44:{3}}} ",
			        	new object[]
			        		{
			        			TypeConvert.ToString(M41), TypeConvert.ToString(M42), TypeConvert.ToString(M43), TypeConvert.ToString(M44),
			        		}) + "}");
		}

		/// <summary>
		/// Test if this matrix equals the specified matrix.
		/// </summary>
		/// <param name="other">Matrix to test for equality.</param>
		/// <returns>True if the matrices are equal.</returns>
		public bool Equals(Matrix other)
		{
			return 
				((((((M11 == other.M11) &&
				(M22 == other.M22)) && 
				((M33 == other.M33) && 
				(M44 == other.M44))) && 
				(((M12 == other.M12) &&
				(M13 == other.M13)) &&
				((M14 == other.M14) &&
				(M21 == other.M21)))) &&
				((((M23 == other.M23) && 
				(M24 == other.M24)) && 
				((M31 == other.M31) &&
				(M32 == other.M32))) &&
				(((M34 == other.M34) &&
				(M41 == other.M41)) &&
				(M42 == other.M42)))) &&
				(M43 == other.M43));
		}

		/// <summary>
		/// Test if this matrix equals the specified object.
		/// </summary>
		/// <param name="obj">Object to test for equality.</param>
		/// <returns>True if the specified object equals this matrix.</returns>
		public override bool Equals(object obj)
		{
			bool flag = false;
			if (obj is Matrix)
			{
				flag = Equals((Matrix)obj);
			}
			return flag;
		}

		/// <summary>
		/// The hash code value for this matrix.
		/// </summary>
		/// <returns>The hash code value for this matrix.</returns>
		public override int GetHashCode()
		{
			return (((((((((((((((M11.GetHashCode() + M12.GetHashCode()) + M13.GetHashCode())
			                    + M14.GetHashCode()) + M21.GetHashCode()) + M22.GetHashCode())
			                 + M23.GetHashCode()) + M24.GetHashCode()) + M31.GetHashCode())
			              + M32.GetHashCode()) + M33.GetHashCode()) + M34.GetHashCode()) + M41.GetHashCode())
			          + M42.GetHashCode()) + M43.GetHashCode()) + M44.GetHashCode());
		}

		/// <summary>
		/// Returns a transposed copy of the specified matrix.
		/// </summary>
		/// <param name="matrix">Matrix to transpose.</param>
		/// <returns>A transposed copy of the source matrix.</returns>
		public static Matrix Transpose(Matrix matrix)
		{
			Matrix matrix2;
			matrix2.M11 = matrix.M11;
			matrix2.M12 = matrix.M21;
			matrix2.M13 = matrix.M31;
			matrix2.M14 = matrix.M41;
			matrix2.M21 = matrix.M12;
			matrix2.M22 = matrix.M22;
			matrix2.M23 = matrix.M32;
			matrix2.M24 = matrix.M42;
			matrix2.M31 = matrix.M13;
			matrix2.M32 = matrix.M23;
			matrix2.M33 = matrix.M33;
			matrix2.M34 = matrix.M43;
			matrix2.M41 = matrix.M14;
			matrix2.M42 = matrix.M24;
			matrix2.M43 = matrix.M34;
			matrix2.M44 = matrix.M44;
			return matrix2;
		}

		/// <summary>
		/// Returns a transposed copy of the specified matrix.
		/// </summary>
		/// <param name="matrix">Matrix to transpose.</param>
		/// <param name="result">A transposed copy of the source matrix.</param>
		public static void Transpose(ref Matrix matrix, out Matrix result)
		{
			float num16 = matrix.M11;
			float num15 = matrix.M12;
			float num14 = matrix.M13;
			float num13 = matrix.M14;
			float num12 = matrix.M21;
			float num11 = matrix.M22;
			float num10 = matrix.M23;
			float num9 = matrix.M24;
			float num8 = matrix.M31;
			float num7 = matrix.M32;
			float num6 = matrix.M33;
			float num5 = matrix.M34;
			float num4 = matrix.M41;
			float num3 = matrix.M42;
			float num2 = matrix.M43;
			float num = matrix.M44;
			result.M11 = num16;
			result.M12 = num12;
			result.M13 = num8;
			result.M14 = num4;
			result.M21 = num15;
			result.M22 = num11;
			result.M23 = num7;
			result.M24 = num3;
			result.M31 = num14;
			result.M32 = num10;
			result.M33 = num6;
			result.M34 = num2;
			result.M41 = num13;
			result.M42 = num9;
			result.M43 = num5;
			result.M44 = num;
		}

		/// <summary>
		/// Calculate the determinant for this matrix.
		/// </summary>
		/// <returns>The determinant.</returns>
		public float Determinant()
		{
			float num22 = M11;
			float num21 = M12;
			float num20 = M13;
			float num19 = M14;
			float num12 = M21;
			float num11 = M22;
			float num10 = M23;
			float num9 = M24;
			float num8 = M31;
			float num7 = M32;
			float num6 = M33;
			float num5 = M34;
			float num4 = M41;
			float num3 = M42;
			float num2 = M43;
			float num = M44;
			float num18 = (num6 * num) - (num5 * num2);
			float num17 = (num7 * num) - (num5 * num3);
			float num16 = (num7 * num2) - (num6 * num3);
			float num15 = (num8 * num) - (num5 * num4);
			float num14 = (num8 * num2) - (num6 * num4);
			float num13 = (num8 * num3) - (num7 * num4);
			return ((((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16)))
			          - (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14))))
			         + (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13))))
			        - (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13))));
		}

		/// <summary>
		/// Invert a matrix.
		/// </summary>
		/// <param name="matrix">Matrix to invert.</param>
		/// <returns>The inverted matrix.</returns>
		public static Matrix Invert(Matrix matrix)
		{
			Matrix result;
			Invert(ref matrix, out result);
			return result;
		}

		/// <summary>
		/// Invert a matrix.
		/// </summary>
		/// <param name="matrix">Matrix to invert.</param>
		/// <param name="result">The inverted matrix.</param>
		public static void Invert(ref Matrix matrix, out Matrix result)
		{
			float num5 = matrix.M11;
			float num4 = matrix.M12;
			float num3 = matrix.M13;
			float num2 = matrix.M14;
			float num9 = matrix.M21;
			float num8 = matrix.M22;
			float num7 = matrix.M23;
			float num6 = matrix.M24;
			float num17 = matrix.M31;
			float num16 = matrix.M32;
			float num15 = matrix.M33;
			float num14 = matrix.M34;
			float num13 = matrix.M41;
			float num12 = matrix.M42;
			float num11 = matrix.M43;
			float num10 = matrix.M44;
			float num23 = (num15 * num10) - (num14 * num11);
			float num22 = (num16 * num10) - (num14 * num12);
			float num21 = (num16 * num11) - (num15 * num12);
			float num20 = (num17 * num10) - (num14 * num13);
			float num19 = (num17 * num11) - (num15 * num13);
			float num18 = (num17 * num12) - (num16 * num13);
			float num39 = ((num8 * num23) - (num7 * num22)) + (num6 * num21);
			float num38 = -(((num9 * num23) - (num7 * num20)) + (num6 * num19));
			float num37 = ((num9 * num22) - (num8 * num20)) + (num6 * num18);
			float num36 = -(((num9 * num21) - (num8 * num19)) + (num7 * num18));
			float num = 1f / ((((num5 * num39) + (num4 * num38)) + (num3 * num37)) + (num2 * num36));
			result.M11 = num39 * num;
			result.M21 = num38 * num;
			result.M31 = num37 * num;
			result.M41 = num36 * num;
			result.M12 = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
			result.M22 = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
			result.M32 = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
			result.M42 = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;
			float num35 = (num7 * num10) - (num6 * num11);
			float num34 = (num8 * num10) - (num6 * num12);
			float num33 = (num8 * num11) - (num7 * num12);
			float num32 = (num9 * num10) - (num6 * num13);
			float num31 = (num9 * num11) - (num7 * num13);
			float num30 = (num9 * num12) - (num8 * num13);
			result.M13 = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
			result.M23 = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
			result.M33 = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
			result.M43 = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;
			float num29 = (num7 * num14) - (num6 * num15);
			float num28 = (num8 * num14) - (num6 * num16);
			float num27 = (num8 * num15) - (num7 * num16);
			float num26 = (num9 * num14) - (num6 * num17);
			float num25 = (num9 * num15) - (num7 * num17);
			float num24 = (num9 * num16) - (num8 * num17);
			result.M14 = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
			result.M24 = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
			result.M34 = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
			result.M44 = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;
		}

		/// <summary>
		/// Linearly interpolate between two matrices.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="amount">Interpolation factor.</param>
		/// <returns>The interpolated matrix.</returns>
		public static Matrix Lerp(Matrix matrix1, Matrix matrix2, float amount)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
			matrix.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
			matrix.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
			matrix.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
			matrix.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
			matrix.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
			matrix.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
			matrix.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
			matrix.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
			matrix.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
			matrix.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
			matrix.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
			matrix.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
			matrix.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
			matrix.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
			matrix.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
			return matrix;
		}

		/// <summary>
		/// Linearly interpolate between two matrices.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="amount">Interpolation factor.</param>
		/// <param name="result">The interpolated matrix.</param>
		public static void Lerp(ref Matrix matrix1, ref Matrix matrix2, float amount, out Matrix result)
		{
			result.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
			result.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
			result.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
			result.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
			result.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
			result.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
			result.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
			result.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
			result.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
			result.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
			result.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
			result.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
			result.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
			result.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
			result.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
			result.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
		}

		/// <summary>
		/// Negate a matrix.
		/// </summary>
		/// <param name="matrix">Matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix Negate(Matrix matrix)
		{
			Matrix matrix2;
			matrix2.M11 = -matrix.M11;
			matrix2.M12 = -matrix.M12;
			matrix2.M13 = -matrix.M13;
			matrix2.M14 = -matrix.M14;
			matrix2.M21 = -matrix.M21;
			matrix2.M22 = -matrix.M22;
			matrix2.M23 = -matrix.M23;
			matrix2.M24 = -matrix.M24;
			matrix2.M31 = -matrix.M31;
			matrix2.M32 = -matrix.M32;
			matrix2.M33 = -matrix.M33;
			matrix2.M34 = -matrix.M34;
			matrix2.M41 = -matrix.M41;
			matrix2.M42 = -matrix.M42;
			matrix2.M43 = -matrix.M43;
			matrix2.M44 = -matrix.M44;
			return matrix2;
		}

		/// <summary>
		/// Negate a matrix.
		/// </summary>
		/// <param name="matrix">Matrix to negate.</param>
		/// <param name="result">The negated matrix.</param>
		public static void Negate(ref Matrix matrix, out Matrix result)
		{
			result.M11 = -matrix.M11;
			result.M12 = -matrix.M12;
			result.M13 = -matrix.M13;
			result.M14 = -matrix.M14;
			result.M21 = -matrix.M21;
			result.M22 = -matrix.M22;
			result.M23 = -matrix.M23;
			result.M24 = -matrix.M24;
			result.M31 = -matrix.M31;
			result.M32 = -matrix.M32;
			result.M33 = -matrix.M33;
			result.M34 = -matrix.M34;
			result.M41 = -matrix.M41;
			result.M42 = -matrix.M42;
			result.M43 = -matrix.M43;
			result.M44 = -matrix.M44;
		}

		/// <summary>
		/// Add two matrices together.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The combined matrix.</returns>
		public static Matrix Add(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 + matrix2.M11;
			matrix.M12 = matrix1.M12 + matrix2.M12;
			matrix.M13 = matrix1.M13 + matrix2.M13;
			matrix.M14 = matrix1.M14 + matrix2.M14;
			matrix.M21 = matrix1.M21 + matrix2.M21;
			matrix.M22 = matrix1.M22 + matrix2.M22;
			matrix.M23 = matrix1.M23 + matrix2.M23;
			matrix.M24 = matrix1.M24 + matrix2.M24;
			matrix.M31 = matrix1.M31 + matrix2.M31;
			matrix.M32 = matrix1.M32 + matrix2.M32;
			matrix.M33 = matrix1.M33 + matrix2.M33;
			matrix.M34 = matrix1.M34 + matrix2.M34;
			matrix.M41 = matrix1.M41 + matrix2.M41;
			matrix.M42 = matrix1.M42 + matrix2.M42;
			matrix.M43 = matrix1.M43 + matrix2.M43;
			matrix.M44 = matrix1.M44 + matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Add two matrices together.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="result">The combined matrix.</param>
		public static void Add(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
		{
			result.M11 = matrix1.M11 + matrix2.M11;
			result.M12 = matrix1.M12 + matrix2.M12;
			result.M13 = matrix1.M13 + matrix2.M13;
			result.M14 = matrix1.M14 + matrix2.M14;
			result.M21 = matrix1.M21 + matrix2.M21;
			result.M22 = matrix1.M22 + matrix2.M22;
			result.M23 = matrix1.M23 + matrix2.M23;
			result.M24 = matrix1.M24 + matrix2.M24;
			result.M31 = matrix1.M31 + matrix2.M31;
			result.M32 = matrix1.M32 + matrix2.M32;
			result.M33 = matrix1.M33 + matrix2.M33;
			result.M34 = matrix1.M34 + matrix2.M34;
			result.M41 = matrix1.M41 + matrix2.M41;
			result.M42 = matrix1.M42 + matrix2.M42;
			result.M43 = matrix1.M43 + matrix2.M43;
			result.M44 = matrix1.M44 + matrix2.M44;
		}

		/// <summary>
		/// Subract one matrix from another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The subtracted matrix.</returns>
		public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 - matrix2.M11;
			matrix.M12 = matrix1.M12 - matrix2.M12;
			matrix.M13 = matrix1.M13 - matrix2.M13;
			matrix.M14 = matrix1.M14 - matrix2.M14;
			matrix.M21 = matrix1.M21 - matrix2.M21;
			matrix.M22 = matrix1.M22 - matrix2.M22;
			matrix.M23 = matrix1.M23 - matrix2.M23;
			matrix.M24 = matrix1.M24 - matrix2.M24;
			matrix.M31 = matrix1.M31 - matrix2.M31;
			matrix.M32 = matrix1.M32 - matrix2.M32;
			matrix.M33 = matrix1.M33 - matrix2.M33;
			matrix.M34 = matrix1.M34 - matrix2.M34;
			matrix.M41 = matrix1.M41 - matrix2.M41;
			matrix.M42 = matrix1.M42 - matrix2.M42;
			matrix.M43 = matrix1.M43 - matrix2.M43;
			matrix.M44 = matrix1.M44 - matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Subract one matrix from another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="result">The subtracted matrix.</param>
		public static void Subtract(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
		{
			result.M11 = matrix1.M11 - matrix2.M11;
			result.M12 = matrix1.M12 - matrix2.M12;
			result.M13 = matrix1.M13 - matrix2.M13;
			result.M14 = matrix1.M14 - matrix2.M14;
			result.M21 = matrix1.M21 - matrix2.M21;
			result.M22 = matrix1.M22 - matrix2.M22;
			result.M23 = matrix1.M23 - matrix2.M23;
			result.M24 = matrix1.M24 - matrix2.M24;
			result.M31 = matrix1.M31 - matrix2.M31;
			result.M32 = matrix1.M32 - matrix2.M32;
			result.M33 = matrix1.M33 - matrix2.M33;
			result.M34 = matrix1.M34 - matrix2.M34;
			result.M41 = matrix1.M41 - matrix2.M41;
			result.M42 = matrix1.M42 - matrix2.M42;
			result.M43 = matrix1.M43 - matrix2.M43;
			result.M44 = matrix1.M44 - matrix2.M44;
		}

		/// <summary>
		/// Multiply two matrices together.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31))
			             + (matrix1.M14 * matrix2.M41);
			matrix.M12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32))
			             + (matrix1.M14 * matrix2.M42);
			matrix.M13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33))
			             + (matrix1.M14 * matrix2.M43);
			matrix.M14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34))
			             + (matrix1.M14 * matrix2.M44);
			matrix.M21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31))
			             + (matrix1.M24 * matrix2.M41);
			matrix.M22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32))
			             + (matrix1.M24 * matrix2.M42);
			matrix.M23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33))
			             + (matrix1.M24 * matrix2.M43);
			matrix.M24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34))
			             + (matrix1.M24 * matrix2.M44);
			matrix.M31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31))
			             + (matrix1.M34 * matrix2.M41);
			matrix.M32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32))
			             + (matrix1.M34 * matrix2.M42);
			matrix.M33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33))
			             + (matrix1.M34 * matrix2.M43);
			matrix.M34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34))
			             + (matrix1.M34 * matrix2.M44);
			matrix.M41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31))
			             + (matrix1.M44 * matrix2.M41);
			matrix.M42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32))
			             + (matrix1.M44 * matrix2.M42);
			matrix.M43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33))
			             + (matrix1.M44 * matrix2.M43);
			matrix.M44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34))
			             + (matrix1.M44 * matrix2.M44);
			return matrix;
		}

		/// <summary>
		/// Multiply two matrices together.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="result">The resulting matrix.</param>
		public static void Multiply(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
		{
			float num16 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31))
			              + (matrix1.M14 * matrix2.M41);
			float num15 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32))
			              + (matrix1.M14 * matrix2.M42);
			float num14 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33))
			              + (matrix1.M14 * matrix2.M43);
			float num13 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34))
			              + (matrix1.M14 * matrix2.M44);
			float num12 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31))
			              + (matrix1.M24 * matrix2.M41);
			float num11 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32))
			              + (matrix1.M24 * matrix2.M42);
			float num10 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33))
			              + (matrix1.M24 * matrix2.M43);
			float num9 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34))
			             + (matrix1.M24 * matrix2.M44);
			float num8 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31))
			             + (matrix1.M34 * matrix2.M41);
			float num7 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32))
			             + (matrix1.M34 * matrix2.M42);
			float num6 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33))
			             + (matrix1.M34 * matrix2.M43);
			float num5 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34))
			             + (matrix1.M34 * matrix2.M44);
			float num4 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31))
			             + (matrix1.M44 * matrix2.M41);
			float num3 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32))
			             + (matrix1.M44 * matrix2.M42);
			float num2 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33))
			             + (matrix1.M44 * matrix2.M43);
			float num = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34))
			            + (matrix1.M44 * matrix2.M44);
			result.M11 = num16;
			result.M12 = num15;
			result.M13 = num14;
			result.M14 = num13;
			result.M21 = num12;
			result.M22 = num11;
			result.M23 = num10;
			result.M24 = num9;
			result.M31 = num8;
			result.M32 = num7;
			result.M33 = num6;
			result.M34 = num5;
			result.M41 = num4;
			result.M42 = num3;
			result.M43 = num2;
			result.M44 = num;
		}

		/// <summary>
		/// Multiply a matrix by a scaling factor.
		/// </summary>
		/// <param name="matrix1">Matrix to multiply.</param>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix Multiply(Matrix matrix1, float scaleFactor)
		{
			Matrix matrix;
			float num = scaleFactor;
			matrix.M11 = matrix1.M11 * num;
			matrix.M12 = matrix1.M12 * num;
			matrix.M13 = matrix1.M13 * num;
			matrix.M14 = matrix1.M14 * num;
			matrix.M21 = matrix1.M21 * num;
			matrix.M22 = matrix1.M22 * num;
			matrix.M23 = matrix1.M23 * num;
			matrix.M24 = matrix1.M24 * num;
			matrix.M31 = matrix1.M31 * num;
			matrix.M32 = matrix1.M32 * num;
			matrix.M33 = matrix1.M33 * num;
			matrix.M34 = matrix1.M34 * num;
			matrix.M41 = matrix1.M41 * num;
			matrix.M42 = matrix1.M42 * num;
			matrix.M43 = matrix1.M43 * num;
			matrix.M44 = matrix1.M44 * num;
			return matrix;
		}

		/// <summary>
		/// Multiply a matrix by a scaling factor.
		/// </summary>
		/// <param name="matrix1">Matrix to multiply.</param>
		/// <param name="scaleFactor">Scaling factor.</param>
		/// <param name="result">The scaled matrix.</param>
		public static void Multiply(ref Matrix matrix1, float scaleFactor, out Matrix result)
		{
			float num = scaleFactor;
			result.M11 = matrix1.M11 * num;
			result.M12 = matrix1.M12 * num;
			result.M13 = matrix1.M13 * num;
			result.M14 = matrix1.M14 * num;
			result.M21 = matrix1.M21 * num;
			result.M22 = matrix1.M22 * num;
			result.M23 = matrix1.M23 * num;
			result.M24 = matrix1.M24 * num;
			result.M31 = matrix1.M31 * num;
			result.M32 = matrix1.M32 * num;
			result.M33 = matrix1.M33 * num;
			result.M34 = matrix1.M34 * num;
			result.M41 = matrix1.M41 * num;
			result.M42 = matrix1.M42 * num;
			result.M43 = matrix1.M43 * num;
			result.M44 = matrix1.M44 * num;
		}

		/// <summary>
		/// Divide one matrix by another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix Divide(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 / matrix2.M11;
			matrix.M12 = matrix1.M12 / matrix2.M12;
			matrix.M13 = matrix1.M13 / matrix2.M13;
			matrix.M14 = matrix1.M14 / matrix2.M14;
			matrix.M21 = matrix1.M21 / matrix2.M21;
			matrix.M22 = matrix1.M22 / matrix2.M22;
			matrix.M23 = matrix1.M23 / matrix2.M23;
			matrix.M24 = matrix1.M24 / matrix2.M24;
			matrix.M31 = matrix1.M31 / matrix2.M31;
			matrix.M32 = matrix1.M32 / matrix2.M32;
			matrix.M33 = matrix1.M33 / matrix2.M33;
			matrix.M34 = matrix1.M34 / matrix2.M34;
			matrix.M41 = matrix1.M41 / matrix2.M41;
			matrix.M42 = matrix1.M42 / matrix2.M42;
			matrix.M43 = matrix1.M43 / matrix2.M43;
			matrix.M44 = matrix1.M44 / matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Divide one matrix by another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <param name="result">The resulting matrix.</param>
		public static void Divide(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
		{
			result.M11 = matrix1.M11 / matrix2.M11;
			result.M12 = matrix1.M12 / matrix2.M12;
			result.M13 = matrix1.M13 / matrix2.M13;
			result.M14 = matrix1.M14 / matrix2.M14;
			result.M21 = matrix1.M21 / matrix2.M21;
			result.M22 = matrix1.M22 / matrix2.M22;
			result.M23 = matrix1.M23 / matrix2.M23;
			result.M24 = matrix1.M24 / matrix2.M24;
			result.M31 = matrix1.M31 / matrix2.M31;
			result.M32 = matrix1.M32 / matrix2.M32;
			result.M33 = matrix1.M33 / matrix2.M33;
			result.M34 = matrix1.M34 / matrix2.M34;
			result.M41 = matrix1.M41 / matrix2.M41;
			result.M42 = matrix1.M42 / matrix2.M42;
			result.M43 = matrix1.M43 / matrix2.M43;
			result.M44 = matrix1.M44 / matrix2.M44;
		}

		/// <summary>
		/// Divide a matrix by a value.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="divider">Value to divide by.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix Divide(Matrix matrix1, float divider)
		{
			Matrix matrix;
			float num = 1f / divider;
			matrix.M11 = matrix1.M11 * num;
			matrix.M12 = matrix1.M12 * num;
			matrix.M13 = matrix1.M13 * num;
			matrix.M14 = matrix1.M14 * num;
			matrix.M21 = matrix1.M21 * num;
			matrix.M22 = matrix1.M22 * num;
			matrix.M23 = matrix1.M23 * num;
			matrix.M24 = matrix1.M24 * num;
			matrix.M31 = matrix1.M31 * num;
			matrix.M32 = matrix1.M32 * num;
			matrix.M33 = matrix1.M33 * num;
			matrix.M34 = matrix1.M34 * num;
			matrix.M41 = matrix1.M41 * num;
			matrix.M42 = matrix1.M42 * num;
			matrix.M43 = matrix1.M43 * num;
			matrix.M44 = matrix1.M44 * num;
			return matrix;
		}

		/// <summary>
		/// Divide a matrix by a value.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="divider">Value to divide by.</param>
		/// <param name="result">The resulting matrix.</param>
		public static void Divide(ref Matrix matrix1, float divider, out Matrix result)
		{
			float num = 1f / divider;
			result.M11 = matrix1.M11 * num;
			result.M12 = matrix1.M12 * num;
			result.M13 = matrix1.M13 * num;
			result.M14 = matrix1.M14 * num;
			result.M21 = matrix1.M21 * num;
			result.M22 = matrix1.M22 * num;
			result.M23 = matrix1.M23 * num;
			result.M24 = matrix1.M24 * num;
			result.M31 = matrix1.M31 * num;
			result.M32 = matrix1.M32 * num;
			result.M33 = matrix1.M33 * num;
			result.M34 = matrix1.M34 * num;
			result.M41 = matrix1.M41 * num;
			result.M42 = matrix1.M42 * num;
			result.M43 = matrix1.M43 * num;
			result.M44 = matrix1.M44 * num;
		}

		/// <summary>
		/// Negate a matrix.
		/// </summary>
		/// <param name="matrix1">Matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix operator -(Matrix matrix1)
		{
			Matrix matrix;
			matrix.M11 = -matrix1.M11;
			matrix.M12 = -matrix1.M12;
			matrix.M13 = -matrix1.M13;
			matrix.M14 = -matrix1.M14;
			matrix.M21 = -matrix1.M21;
			matrix.M22 = -matrix1.M22;
			matrix.M23 = -matrix1.M23;
			matrix.M24 = -matrix1.M24;
			matrix.M31 = -matrix1.M31;
			matrix.M32 = -matrix1.M32;
			matrix.M33 = -matrix1.M33;
			matrix.M34 = -matrix1.M34;
			matrix.M41 = -matrix1.M41;
			matrix.M42 = -matrix1.M42;
			matrix.M43 = -matrix1.M43;
			matrix.M44 = -matrix1.M44;
			return matrix;
		}

		/// <summary>
		/// Test if two matrices are equal.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>True if the two matrices are equal.</returns>
		public static bool operator ==(Matrix matrix1, Matrix matrix2)
		{
			return ((((((matrix1.M11 == matrix2.M11) && (matrix1.M22 == matrix2.M22))
			           && ((matrix1.M33 == matrix2.M33) && (matrix1.M44 == matrix2.M44)))
			          &&
			          (((matrix1.M12 == matrix2.M12) && (matrix1.M13 == matrix2.M13))
			           && ((matrix1.M14 == matrix2.M14) && (matrix1.M21 == matrix2.M21))))
			         &&
			         ((((matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24))
			           && ((matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32)))
			          && (((matrix1.M34 == matrix2.M34) && (matrix1.M41 == matrix2.M41)) && (matrix1.M42 == matrix2.M42))))
			        && (matrix1.M43 == matrix2.M43));
		}

		/// <summary>
		/// Test if two matrices are not equal.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>True if the two matrices are not equal.</returns>
		public static bool operator !=(Matrix matrix1, Matrix matrix2)
		{
			if (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12))
			      && ((matrix1.M13 == matrix2.M13) && (matrix1.M14 == matrix2.M14)))
			     &&
			     (((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22))
			      && ((matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24))))
			    &&
			    ((((matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32))
			      && ((matrix1.M33 == matrix2.M33) && (matrix1.M34 == matrix2.M34)))
			     && (((matrix1.M41 == matrix2.M41) && (matrix1.M42 == matrix2.M42)) && (matrix1.M43 == matrix2.M43))))
			{
				return !(matrix1.M44 == matrix2.M44);
			}
			return true;
		}

		/// <summary>
		/// Add two matrices.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 + matrix2.M11;
			matrix.M12 = matrix1.M12 + matrix2.M12;
			matrix.M13 = matrix1.M13 + matrix2.M13;
			matrix.M14 = matrix1.M14 + matrix2.M14;
			matrix.M21 = matrix1.M21 + matrix2.M21;
			matrix.M22 = matrix1.M22 + matrix2.M22;
			matrix.M23 = matrix1.M23 + matrix2.M23;
			matrix.M24 = matrix1.M24 + matrix2.M24;
			matrix.M31 = matrix1.M31 + matrix2.M31;
			matrix.M32 = matrix1.M32 + matrix2.M32;
			matrix.M33 = matrix1.M33 + matrix2.M33;
			matrix.M34 = matrix1.M34 + matrix2.M34;
			matrix.M41 = matrix1.M41 + matrix2.M41;
			matrix.M42 = matrix1.M42 + matrix2.M42;
			matrix.M43 = matrix1.M43 + matrix2.M43;
			matrix.M44 = matrix1.M44 + matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Subtract one matrix from another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 - matrix2.M11;
			matrix.M12 = matrix1.M12 - matrix2.M12;
			matrix.M13 = matrix1.M13 - matrix2.M13;
			matrix.M14 = matrix1.M14 - matrix2.M14;
			matrix.M21 = matrix1.M21 - matrix2.M21;
			matrix.M22 = matrix1.M22 - matrix2.M22;
			matrix.M23 = matrix1.M23 - matrix2.M23;
			matrix.M24 = matrix1.M24 - matrix2.M24;
			matrix.M31 = matrix1.M31 - matrix2.M31;
			matrix.M32 = matrix1.M32 - matrix2.M32;
			matrix.M33 = matrix1.M33 - matrix2.M33;
			matrix.M34 = matrix1.M34 - matrix2.M34;
			matrix.M41 = matrix1.M41 - matrix2.M41;
			matrix.M42 = matrix1.M42 - matrix2.M42;
			matrix.M43 = matrix1.M43 - matrix2.M43;
			matrix.M44 = matrix1.M44 - matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Multiply one matrix by another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31))
			             + (matrix1.M14 * matrix2.M41);
			matrix.M12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32))
			             + (matrix1.M14 * matrix2.M42);
			matrix.M13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33))
			             + (matrix1.M14 * matrix2.M43);
			matrix.M14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34))
			             + (matrix1.M14 * matrix2.M44);
			matrix.M21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31))
			             + (matrix1.M24 * matrix2.M41);
			matrix.M22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32))
			             + (matrix1.M24 * matrix2.M42);
			matrix.M23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33))
			             + (matrix1.M24 * matrix2.M43);
			matrix.M24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34))
			             + (matrix1.M24 * matrix2.M44);
			matrix.M31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31))
			             + (matrix1.M34 * matrix2.M41);
			matrix.M32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32))
			             + (matrix1.M34 * matrix2.M42);
			matrix.M33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33))
			             + (matrix1.M34 * matrix2.M43);
			matrix.M34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34))
			             + (matrix1.M34 * matrix2.M44);
			matrix.M41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31))
			             + (matrix1.M44 * matrix2.M41);
			matrix.M42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32))
			             + (matrix1.M44 * matrix2.M42);
			matrix.M43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33))
			             + (matrix1.M44 * matrix2.M43);
			matrix.M44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34))
			             + (matrix1.M44 * matrix2.M44);
			return matrix;
		}

		/// <summary>
		/// Multiply a matrix by a value.
		/// </summary>
		/// <param name="matrix">Vector to multiply.</param>
		/// <param name="scaleFactor">Scaling value.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator *(Matrix matrix, float scaleFactor)
		{
			Matrix matrix2;
			float num = scaleFactor;
			matrix2.M11 = matrix.M11 * num;
			matrix2.M12 = matrix.M12 * num;
			matrix2.M13 = matrix.M13 * num;
			matrix2.M14 = matrix.M14 * num;
			matrix2.M21 = matrix.M21 * num;
			matrix2.M22 = matrix.M22 * num;
			matrix2.M23 = matrix.M23 * num;
			matrix2.M24 = matrix.M24 * num;
			matrix2.M31 = matrix.M31 * num;
			matrix2.M32 = matrix.M32 * num;
			matrix2.M33 = matrix.M33 * num;
			matrix2.M34 = matrix.M34 * num;
			matrix2.M41 = matrix.M41 * num;
			matrix2.M42 = matrix.M42 * num;
			matrix2.M43 = matrix.M43 * num;
			matrix2.M44 = matrix.M44 * num;
			return matrix2;
		}

		/// <summary>
		/// Multiply a matrix by a value.
		/// </summary>
		/// <param name="scaleFactor">Scaling value.</param>
		/// <param name="matrix">Vector to multiply.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator *(float scaleFactor, Matrix matrix)
		{
			Matrix matrix2;
			float num = scaleFactor;
			matrix2.M11 = matrix.M11 * num;
			matrix2.M12 = matrix.M12 * num;
			matrix2.M13 = matrix.M13 * num;
			matrix2.M14 = matrix.M14 * num;
			matrix2.M21 = matrix.M21 * num;
			matrix2.M22 = matrix.M22 * num;
			matrix2.M23 = matrix.M23 * num;
			matrix2.M24 = matrix.M24 * num;
			matrix2.M31 = matrix.M31 * num;
			matrix2.M32 = matrix.M32 * num;
			matrix2.M33 = matrix.M33 * num;
			matrix2.M34 = matrix.M34 * num;
			matrix2.M41 = matrix.M41 * num;
			matrix2.M42 = matrix.M42 * num;
			matrix2.M43 = matrix.M43 * num;
			matrix2.M44 = matrix.M44 * num;
			return matrix2;
		}

		/// <summary>
		/// Divide one matrix by another.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="matrix2">Second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator /(Matrix matrix1, Matrix matrix2)
		{
			Matrix matrix;
			matrix.M11 = matrix1.M11 / matrix2.M11;
			matrix.M12 = matrix1.M12 / matrix2.M12;
			matrix.M13 = matrix1.M13 / matrix2.M13;
			matrix.M14 = matrix1.M14 / matrix2.M14;
			matrix.M21 = matrix1.M21 / matrix2.M21;
			matrix.M22 = matrix1.M22 / matrix2.M22;
			matrix.M23 = matrix1.M23 / matrix2.M23;
			matrix.M24 = matrix1.M24 / matrix2.M24;
			matrix.M31 = matrix1.M31 / matrix2.M31;
			matrix.M32 = matrix1.M32 / matrix2.M32;
			matrix.M33 = matrix1.M33 / matrix2.M33;
			matrix.M34 = matrix1.M34 / matrix2.M34;
			matrix.M41 = matrix1.M41 / matrix2.M41;
			matrix.M42 = matrix1.M42 / matrix2.M42;
			matrix.M43 = matrix1.M43 / matrix2.M43;
			matrix.M44 = matrix1.M44 / matrix2.M44;
			return matrix;
		}

		/// <summary>
		/// Divide one matrix by a scaling value.
		/// </summary>
		/// <param name="matrix1">First matrix.</param>
		/// <param name="divider">Value to divide by.</param>
		/// <returns>The resulting matrix.</returns>
		public static Matrix operator /(Matrix matrix1, float divider)
		{
			Matrix matrix;
			float num = 1f / divider;
			matrix.M11 = matrix1.M11 * num;
			matrix.M12 = matrix1.M12 * num;
			matrix.M13 = matrix1.M13 * num;
			matrix.M14 = matrix1.M14 * num;
			matrix.M21 = matrix1.M21 * num;
			matrix.M22 = matrix1.M22 * num;
			matrix.M23 = matrix1.M23 * num;
			matrix.M24 = matrix1.M24 * num;
			matrix.M31 = matrix1.M31 * num;
			matrix.M32 = matrix1.M32 * num;
			matrix.M33 = matrix1.M33 * num;
			matrix.M34 = matrix1.M34 * num;
			matrix.M41 = matrix1.M41 * num;
			matrix.M42 = matrix1.M42 * num;
			matrix.M43 = matrix1.M43 * num;
			matrix.M44 = matrix1.M44 * num;
			return matrix;
		}
	}
}