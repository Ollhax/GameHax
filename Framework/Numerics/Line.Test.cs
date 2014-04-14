using MG.Framework.Utility;

using NUnit.Framework;

namespace MG.Framework.Numerics
{
	[TestFixture]
	public class LineTest
	{
		[SetUp]
		public void Init()
		{
			GlobalSettings.DefaultFloatingPointTolerance = TestTools.FloatingPointTolerance;
		}

		[Test]
		public void ClosestPointOnLineTest()
		{
			Line l = new Line();

			{
				// Endpoints
				l.Start = new Vector2(10, 10);
				l.End = new Vector2(30, 30);				
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(40, 40)), l.End);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(30, 40)), l.End);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(40, 30)), l.End);

				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(0, 0)), l.Start);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(10, 0)), l.Start);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(0, 15)), l.Start);

				// Inside the line
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(30, 10)), new Vector2(20, 20));

				l.Start = new Vector2(300, 400);
				l.End = new Vector2(400, 400);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(350, 450)), new Vector2(350, 400));
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(380, -430)), new Vector2(380, 400));
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(320, 730)), new Vector2(320, 400));

				// Special case
				l.Start = new Vector2(0, 0);
				l.End = new Vector2(0, 0);
				TestTools.VectorEquals(l.ClosestPointOnLine(new Vector2(123, 456)), new Vector2(0, 0));
			}
		}

		[Test]
		public void IntersectsTest()
		{
			Line l1 = new Line();
			Line l2 = new Line();
			Circle c = new Circle();
			RectangleF r = new RectangleF();
			
			{
				// Positive tests
				l1.Start = new Vector2(30, 30);
				l1.End = new Vector2(0, 0);
				c.Center = new Vector2(0, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.Intersects(c));
				Assert.IsTrue(c.Intersects(l1));

				l1.Start = new Vector2(30, 30);
				l1.End = new Vector2(2, 2);
				c.Center = new Vector2(0, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.Intersects(c));
				Assert.IsTrue(c.Intersects(l1));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(70, 70);
				c.Center = new Vector2(30, 30);
				c.Radius = 15;
				Assert.IsTrue(l1.Intersects(c));
				Assert.IsTrue(c.Intersects(l1));

				// Negative tests
				l1.Start = new Vector2(7, 8);
				l1.End = new Vector2(4, 4);
				c.Center = new Vector2(0, 0);
				c.Radius = 5;
				Assert.IsFalse(l1.Intersects(c));
				Assert.IsFalse(c.Intersects(l1));

				l1.Start = new Vector2(10, 10);
				l1.End = new Vector2(10, -10);
				c.Center = new Vector2(0, 0);
				c.Radius = 10;
				Assert.IsFalse(l1.Intersects(c));
				Assert.IsFalse(c.Intersects(l1));

				// Special cases
				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(0, 0);
				c.Center = new Vector2(0, 0);
				c.Radius = 0;
				Assert.IsFalse(l1.Intersects(c));
				Assert.IsFalse(c.Intersects(l1));
			}

			{
				// Positive tests
				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(30, 30);
				l2.Start = new Vector2(0, 30);
				l2.End = new Vector2(30, 0);			
				Assert.IsTrue(l1.Intersects(l2));
				Assert.IsTrue(l2.Intersects(l1));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(1, 0);
				l2.Start = new Vector2(0, 0);
				l2.End = new Vector2(0, 1);
				Assert.IsTrue(l1.Intersects(l2));
				Assert.IsTrue(l2.Intersects(l1));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(0, 2);
				l2.Start = new Vector2(0, 1);
				l2.End = new Vector2(0, 3);
				Assert.IsTrue(l1.Intersects(l2));
				Assert.IsTrue(l2.Intersects(l1));
				
				// Negative tests
				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(10, 0);
				l2.Start = new Vector2(0, 1);
				l2.End = new Vector2(10, 1);
				Assert.IsFalse(l1.Intersects(l2));
				Assert.IsFalse(l2.Intersects(l1));
				
				// Special cases
				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(1, 0);
				Assert.IsTrue(l1.Intersects(l1));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(0, 0);
				Assert.IsTrue(l1.Intersects(l1)); // Actually, degenerate lines should intersect 

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(0, 0);
				l2.Start = new Vector2(1, 1);
				l2.End = new Vector2(1, 1);
				Assert.IsFalse(l1.Intersects(l2));
			}

			{
				r.X = 10;
				r.Y = 10;
				r.Width = 20;
				r.Height = 20;
				l1.End = new Vector2(20, 20);

				// Positive tests
				l1.Start = new Vector2(20, 0);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(0, 20);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(40, 20);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(20, 40);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(40, 40);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(0, 10);
				l1.End = new Vector2(40, 10);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(10, 0);
				l1.End = new Vector2(10, 40);
				Assert.IsTrue(l1.Intersects(r));
				
				// Parallel tests
				l1.Start = new Vector2(0, 10);
				l1.End = new Vector2(40, 10);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(0, 30);
				l1.End = new Vector2(40, 30);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(30, 0);
				l1.End = new Vector2(30, 40);
				Assert.IsTrue(l1.Intersects(r));

				l1.Start = new Vector2(10, 0);
				l1.End = new Vector2(10, 40);
				Assert.IsTrue(l1.Intersects(r));

				// Negative tests
				l1.Start = new Vector2(30, 0);
				l1.End = new Vector2(40, 10);
				Assert.IsFalse(l1.Intersects(r));
			}
		}

		[Test]
		public void GetIntersectionPointTest()
		{
			var l1 = new Line();
			var l2 = new Line();
			var c = new Circle();
			var r = new RectangleF();
			Vector2 p;

			{
				// Positive tests
				l1.Start = new Vector2(10, 10);
				l1.End = new Vector2(10, 0);
				c.Center = new Vector2(10, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.GetIntersectionPoint(c, out p));
				TestTools.VectorEquals(l1.ClosestPointOnLine(p), p);
				TestTools.VectorEquals(p, new Vector2(10, 5));
								
				l1.Start = new Vector2(10, 0);
				l1.End = new Vector2(10, 10);
				c.Center = new Vector2(10, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.GetIntersectionPoint(c, out p));
				TestTools.VectorEquals(l1.ClosestPointOnLine(p), p);
				TestTools.VectorEquals(p, new Vector2(10, 5));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(20, 0);
				c.Center = new Vector2(10, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.GetIntersectionPoint(c, out p));
				TestTools.VectorEquals(l1.ClosestPointOnLine(p), p);
				TestTools.VectorEquals(p, new Vector2(5, 0));

				l1.End = new Vector2(0, 0);
				l1.Start = new Vector2(20, 0);
				c.Center = new Vector2(10, 0);
				c.Radius = 5;
				Assert.IsTrue(l1.GetIntersectionPoint(c, out p));
				TestTools.VectorEquals(l1.ClosestPointOnLine(p), p);
				TestTools.VectorEquals(p, new Vector2(15, 0));				
				
				// Negative tests
				l1.End = new Vector2(0, 0);
				l1.Start = new Vector2(20, 0);
				c.Center = new Vector2(10, 10);
				c.Radius = 10;
				Assert.IsFalse(l1.GetIntersectionPoint(c, out p));

				// Special cases
				l1.End = new Vector2(0, 0);
				l1.Start = new Vector2(0, 0);
				c.Center = new Vector2(10, 0);
				c.Radius = 5;
				Assert.IsFalse(l1.GetIntersectionPoint(c, out p));
			}

			{
				// Positive tests
				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(30, 30);
				l2.Start = new Vector2(0, 30);
				l2.End = new Vector2(30, 0);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(p, new Vector2(15, 15));

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(1, 0);
				l2.Start = new Vector2(0, 0);
				l2.End = new Vector2(0, 1);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(new Vector2(0, 0), p);
				Assert.IsTrue(l2.GetIntersectionPoint(l1, out p));
				TestTools.VectorEquals(new Vector2(0, 0), p);

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(0, 2);
				l2.Start = new Vector2(0, 1);
				l2.End = new Vector2(0, 3);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(new Vector2(0, 1), p);
				Assert.IsTrue(l2.GetIntersectionPoint(l1, out p));
				TestTools.VectorEquals(new Vector2(0, 1), p);

				l2.Start = new Vector2(0, 3);
				l2.End = new Vector2(0, 1);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(new Vector2(0, 1), p);
				Assert.IsTrue(l2.GetIntersectionPoint(l1, out p));
				TestTools.VectorEquals(new Vector2(0, 2), p);

				l1.Start = new Vector2(0, 0);
				l1.End = new Vector2(2, 2);
				l2.Start = new Vector2(1, 1);
				l2.End = new Vector2(3, 3);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(new Vector2(1, 1), p);
				Assert.IsTrue(l2.GetIntersectionPoint(l1, out p));
				TestTools.VectorEquals(new Vector2(1, 1), p);

				l1.Start = new Vector2(0, 720);
				l1.End = new Vector2(1280, 720);
				l2.Start = new Vector2(500, 500);
				l2.End = new Vector2(500, 1000);
				Assert.IsTrue(l1.GetIntersectionPoint(l2, out p));
				TestTools.VectorEquals(new Vector2(500, 720), p);
				Assert.IsTrue(l2.GetIntersectionPoint(l1, out p));
				TestTools.VectorEquals(new Vector2(500, 720), p);
			}

			// Test against rectangle
			{
				r = new RectangleF(1, 1, 2, 2);
				l1 = new Line(0, 2, 2, 2);
				Assert.IsTrue(l1.GetIntersectionPoint(r, out p));
				TestTools.VectorEquals(new Vector2(1, 2), p);

				r = new RectangleF(1, 1, 2, 2);
				l1 = new Line(2, 0, 2, 2);
				Assert.IsTrue(l1.GetIntersectionPoint(r, out p));
				TestTools.VectorEquals(new Vector2(2, 1), p);

				r = new RectangleF(1, 1, 2, 2);
				l1 = new Line(4, 2, 2, 2);
				Assert.IsTrue(l1.GetIntersectionPoint(r, out p));
				TestTools.VectorEquals(new Vector2(3, 2), p);

				r = new RectangleF(1, 1, 2, 2);
				l1 = new Line(2, 4, 2, 2);
				Assert.IsTrue(l1.GetIntersectionPoint(r, out p));
				TestTools.VectorEquals(new Vector2(2, 3), p);
			}
		}

		[Test]
		public void PointsOnSameSideTest()
		{
			var l = new Line();
			Vector2 p1;
			Vector2 p2;

			l.Start = new Vector2(0, 0);
			l.End = new Vector2(0, 1);
			p1 = new Vector2(1, 0);
			p2 = new Vector2(-1, 0);
			Assert.IsFalse(l.PointsOnSameSide(p1, p2));

			l.Start = new Vector2(0, 0);
			l.End = new Vector2(0, -1);
			p1 = new Vector2(1, 0);
			p2 = new Vector2(-1, 0);
			Assert.IsFalse(l.PointsOnSameSide(p1, p2));

			l.Start = new Vector2(0, 0);
			l.End = new Vector2(0, -1);
			Assert.IsTrue(l.PointsOnSameSide(p1, p1));
			Assert.IsTrue(l.PointsOnSameSide(p2, p2));

			l.Start = new Vector2(-1, 0);
			l.End = new Vector2(1, 0);
			p1 = new Vector2(-100, -10);
			p2 = new Vector2(100, -500);
			Assert.IsTrue(l.PointsOnSameSide(p1, p2));

			p1 = new Vector2(100, 10);
			p2 = new Vector2(100, 500);
			Assert.IsTrue(l.PointsOnSameSide(p1, p2));
			
			l.Start = new Vector2(1000, 1000);
			l.End = new Vector2(2000, 2000);
			p1 = new Vector2(2, 1);
			p2 = new Vector2(1, 2);
			Assert.IsFalse(l.PointsOnSameSide(p1, p2));
			Assert.IsFalse(l.PointsOnSameSide(p2, p1));
		}

		[Test]
		public void GetNormalFacingPointTest()
		{
			var l = new Line();
			Vector2 p;

			l = new Line(new Vector2(0, 0), new Vector2(0, 1));
			p = new Vector2(1, 0);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(1, 0));
			
			p = new Vector2(-1, 0);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(-1, 0));

			l = new Line(new Vector2(1, 0), new Vector2(0, 0));
			p = new Vector2(0, 1);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(0, 1));

			p = new Vector2(0, -1);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(0, -1));

			p = new Vector2(100, -1);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(0, -1));
			
			p = new Vector2(-100, -1);
			TestTools.VectorEquals(l.GetNormalFacingPoint(p), new Vector2(0, -1));
		}

		[Test]
		public void FacingPointTest()
		{
			var l = new Line();
			Vector2 p;

			l = new Line(new Vector2(0, 1), new Vector2(0, 0));
			p = new Vector2(1, 0);
			Assert.IsTrue(l.FacingPoint(p));

			p = new Vector2(1, -1);
			Assert.IsTrue(l.FacingPoint(p));

			p = new Vector2(-1, 0);
			Assert.IsFalse(l.FacingPoint(p));

			l = new Line(new Vector2(11, 0), new Vector2(10, 0));
			p = new Vector2(0, -1);
			Assert.IsTrue(l.FacingPoint(p));

			p = new Vector2(0, 1);
			Assert.IsFalse(l.FacingPoint(p));
		}
	}
}