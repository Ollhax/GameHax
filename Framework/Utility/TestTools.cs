using System;
using MG.Framework.Numerics;
using NUnit.Framework;

namespace MG.Framework.Utility
{
	public class TestTools
	{
		public static readonly double FloatingPointTolerance = 0.00001;

		/// <summary>
		/// Compare two vectors, returns true if they are equal.
		/// </summary>
		public static void VectorEquals(Vector2 expected, Vector2 actual)
		{
			var epsilon = (float)FloatingPointTolerance;
			var v1 = expected;
			var v2 = actual;

			Assert.IsTrue(
				Math.Abs(v1.X - v2.X) < epsilon && 
				Math.Abs(v1.Y - v2.Y) < epsilon,
				"{0} is not equal to {1}", v1, v2);
		}

		/// <summary>
		/// Compare two vectors, returns true if they are equal.
		/// </summary>
		public static void VectorEquals(Vector3 expected, Vector3 actual)
		{
			var epsilon = (float)FloatingPointTolerance;
			var v1 = expected;
			var v2 = actual;

			Assert.IsTrue(
				Math.Abs(v1.X - v2.X) < epsilon &&
				Math.Abs(v1.Y - v2.Y) < epsilon &&
				Math.Abs(v1.Z - v2.Z) < epsilon,
				"{0} is not equal to {1}", v1, v2);
		}

		/// <summary>
		/// Compare two rectangles, returns true if they are equal.
		/// </summary>
		public static void RectangleEquals(RectangleF expected, RectangleF actual)
		{
			var epsilon = (float)FloatingPointTolerance;
			var r1 = expected;
			var r2 = actual;

			Assert.IsTrue(
				Math.Abs(r1.X - r2.X) < epsilon &&
				Math.Abs(r1.Y - r2.Y) < epsilon && 
				Math.Abs(r1.Width - r2.Width) < epsilon && 
				Math.Abs(r1.Height - r2.Height) < epsilon,
				"{0} is not equal to {1}", r1, r2);
		}
		
		/// <summary>
		/// Compare two matrices, returns true if they are equal.
		/// </summary>
		public static void MatrixEquals(Matrix expected, Matrix actual)
		{
			float epsilon = (float)FloatingPointTolerance;
			var v1 = expected;
			var v2 = actual;

			Assert.IsTrue(			
				Math.Abs(v1.M11 - v2.M11) < epsilon &&
				Math.Abs(v1.M12 - v2.M12) < epsilon &&
				Math.Abs(v1.M13 - v2.M13) < epsilon &&
				Math.Abs(v1.M14 - v2.M14) < epsilon &&
				Math.Abs(v1.M21 - v2.M21) < epsilon &&
				Math.Abs(v1.M22 - v2.M22) < epsilon &&
				Math.Abs(v1.M23 - v2.M23) < epsilon &&
				Math.Abs(v1.M24 - v2.M24) < epsilon &&
				Math.Abs(v1.M31 - v2.M31) < epsilon &&
				Math.Abs(v1.M32 - v2.M32) < epsilon &&
				Math.Abs(v1.M33 - v2.M33) < epsilon &&
				Math.Abs(v1.M34 - v2.M34) < epsilon &&
				Math.Abs(v1.M41 - v2.M41) < epsilon &&
				Math.Abs(v1.M42 - v2.M42) < epsilon &&
				Math.Abs(v1.M43 - v2.M43) < epsilon &&
				Math.Abs(v1.M44 - v2.M44) < epsilon,
				"{0} is not equal to {1}", v1, v2);
		}
	}
}