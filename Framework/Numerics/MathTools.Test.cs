using MG.Framework.Utility;

using NUnit.Framework;

namespace MG.Framework.Numerics
{
	[TestFixture]
	class MathToolsTest
	{
		[SetUp]
		public void Init()
		{
			GlobalSettings.DefaultFloatingPointTolerance = TestTools.FloatingPointTolerance;
		}

		[Test]
		public void FromAngleTest()
		{
			TestTools.VectorEquals(new Vector2(1, 0), MathTools.FromAngle(0));
			TestTools.VectorEquals(new Vector2(0, 1), MathTools.FromAngle(MathTools.PiOver2));
			TestTools.VectorEquals(new Vector2(-1, 0), MathTools.FromAngle(MathTools.Pi));
			TestTools.VectorEquals(new Vector2(0, -1), MathTools.FromAngle(MathTools.PiOver2 * 3));
		}

		[Test]
		public void DeltaDirectionTest()
		{
			TestTools.VectorEquals(new Vector2(0, 1), MathTools.DeltaDirection(new Vector2(1, 0), MathTools.PiOver2));
		}

		[Test]
		public void AffineTransformTest()
		{
			// Simple translation
			TestTools.VectorEquals(new Vector2(2, 0), MathTools.AffineTransform(new Vector2(1, 0), 1, 0, 0));
			TestTools.VectorEquals(new Vector2(2, 1), MathTools.AffineTransform(new Vector2(1, 0), 1, 1, 0));

			// Simple rotation
			TestTools.VectorEquals(new Vector2(0, 1), MathTools.AffineTransform(new Vector2(1, 0), 0, 0, MathTools.PiOver2));
			TestTools.VectorEquals(new Vector2(-1, 0), MathTools.AffineTransform(new Vector2(1, 0), 0, 0, MathTools.Pi));

			// Translation and rotation
			TestTools.VectorEquals(new Vector2(2, 2), MathTools.AffineTransform(new Vector2(1, 0), 2, 1, MathTools.PiOver2));
			TestTools.VectorEquals(new Vector2(2, 2), MathTools.AffineTransform(new Vector2(0, 0), 2, 2, MathTools.PiOver2));
			TestTools.VectorEquals(new Vector2(0, 0), MathTools.AffineTransform(new Vector2(0, 1), 0, 1, MathTools.Pi));

			// Inverses
			var tx = 0.0f;
			var ty = 0.0f;
			var r = 0.0f;
			var v = new Vector2(0, 0);
			TestTools.VectorEquals(v, MathTools.InverseAffineTransform(MathTools.AffineTransform(v, tx, ty, r), tx, ty, r));

			tx = 1.0f; ty = 1.0f; r = MathTools.Pi; v = new Vector2(1, 0);
			TestTools.VectorEquals(v, MathTools.InverseAffineTransform(MathTools.AffineTransform(v, tx, ty, r), tx, ty, r));

			tx = 10.0f; ty = 1.0f; r = MathTools.PiOver2; v = new Vector2(53, 0);
			TestTools.VectorEquals(v, MathTools.InverseAffineTransform(MathTools.AffineTransform(v, tx, ty, r), tx, ty, r));
		}
	}
}