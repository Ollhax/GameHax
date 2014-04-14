using MG.Framework.Utility;

using NUnit.Framework;

namespace MG.Framework.Numerics
{
	[TestFixture]
	class RectangleFTest
	{
		[SetUp]
		public void Init()
		{
			GlobalSettings.DefaultFloatingPointTolerance = TestTools.FloatingPointTolerance;
		}

		[Test]
		public void ClosestPointOnEdgeTest()
		{
			RectangleF r;
			r = new RectangleF(1, 1, 4, 4);
			TestTools.VectorEquals(new Vector2(5, 3), r.ClosestPointOnEdge(new Vector2(6, 3)));
			TestTools.VectorEquals(new Vector2(5, 3), r.ClosestPointOnEdge(new Vector2(5, 3)));
			TestTools.VectorEquals(new Vector2(5, 3), r.ClosestPointOnEdge(new Vector2(4, 3)));

			TestTools.VectorEquals(new Vector2(5, 1), r.ClosestPointOnEdge(new Vector2(6, 0)));
			TestTools.VectorEquals(new Vector2(1, 1), r.ClosestPointOnEdge(new Vector2(0, 0)));
		}

		[Test]
		public void SweepMoveLineTest()
		{
			var r = new RectangleF();
			var l = new Line();
			var p = new Vector2();
			var n = new Vector2();

			// No collision
			l = new Line(5, 3, 8, 3);
			r = new RectangleF(1, 1, 3, 3);
			Assert.IsTrue(r.SweepMove(new Vector2(1, 1), l, out n, out p));
			TestTools.VectorEquals(new Vector2(1, 1), r.Position);

			r = new RectangleF(1, 1, 3, 3);
			Assert.IsTrue(r.SweepMove(new Vector2(0, 0), l, out n, out p));
			TestTools.VectorEquals(new Vector2(0, 0), r.Position);

			r = new RectangleF(1, 1, 3, 3);
			Assert.IsTrue(r.SweepMove(new Vector2(2, 0), l, out n, out p));
			TestTools.VectorEquals(new Vector2(2, 0), r.Position);

			r = new RectangleF(1, 1, 3, 3);
			Assert.IsTrue(r.SweepMove(new Vector2(2, 3), l, out n, out p));
			TestTools.VectorEquals(new Vector2(2, 3), r.Position);

			// Start with line inside
			r = new RectangleF(0, 0, 2, 2);
			l = new Line(1, 1, 3, 1);
			Assert.IsFalse(r.SweepMove(new Vector2(3, 3), l, out n, out p));
			TestTools.VectorEquals(new Vector2(0, 0), n);
			TestTools.VectorEquals(new Vector2(0, 0), r.Position);

			r = new RectangleF(0, 0, 2, 2);
			l = new Line(-1, 1, 3, 1);
			Assert.IsFalse(r.SweepMove(new Vector2(3, 3), l, out n, out p));
			TestTools.VectorEquals(new Vector2(0, 0), n);
			TestTools.VectorEquals(new Vector2(0, 0), r.Position);

			// Different collision tests
			r = new RectangleF(1, 1, 3, 3);
			l = new Line(5, 3, 8, 3);
			Assert.IsFalse(r.SweepMove(new Vector2(3, 1), l, out n, out p));
			TestTools.VectorEquals(new Vector2(-1, 0), n);
			TestTools.VectorEquals(new Vector2(2, 1), r.Position);
			
			r = new RectangleF(9, 1, 3, 3);
			l = new Line(5, 3, 8, 3);
			Assert.IsFalse(r.SweepMove(new Vector2(3, 1), l, out n, out p));
			TestTools.VectorEquals(new Vector2(1, 0), n);
			TestTools.VectorEquals(new Vector2(8, 1), r.Position);

			r = new RectangleF(-1, 2, 2, 2);
			l = new Line(0, 0, 0, 1);
			Assert.IsFalse(r.SweepMove(new Vector2(-1, 0), l, out n, out p));
			TestTools.VectorEquals(new Vector2(0, 1), n);
			TestTools.VectorEquals(new Vector2(-1, 1), r.Position);

			r = new RectangleF(-1, -3, 2, 2);
			l = new Line(0, 0, 0, 1);
			Assert.IsFalse(r.SweepMove(new Vector2(-1, 0), l, out n, out p));
			TestTools.VectorEquals(new Vector2(0, -1), n);
			TestTools.VectorEquals(new Vector2(-1, -2), r.Position);
			
			r = new RectangleF(1, 1, 3, 3);
			l = new Line(5, 3, 8, 3);
			Assert.IsFalse(r.SweepMove(new Vector2(20, 1), l, out n, out p));
			TestTools.VectorEquals(new Vector2(-1, 0), n);
			TestTools.VectorEquals(new Vector2(2, 1), r.Position);

			r = new RectangleF(1, 2, 1, 1);
			l = new Line(2, 4, 4, 2);
			Assert.IsFalse(r.SweepMove(new Vector2(4, 2), l, out n, out p));
			TestTools.VectorEquals(new Vector2(-1, -1).Normalized(), n);
			TestTools.VectorEquals(new Vector2(2, 2), r.Position);

			r = new RectangleF(4, 3, 1, 1);
			l = new Line(2, 4, 4, 2);
			Assert.IsFalse(r.SweepMove(new Vector2(0, 3), l, out n, out p));
			TestTools.VectorEquals(new Vector2(1, 1).Normalized(), n);
			TestTools.VectorEquals(new Vector2(3, 3), r.Position);
		}
	}
}