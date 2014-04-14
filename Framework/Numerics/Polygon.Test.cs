using System;
using System.Collections.Generic;
using MG.Framework.Utility;

using NUnit.Framework;

namespace MG.Framework.Numerics
{
	[TestFixture]
	class PolygonTest
	{
		[SetUp]
		public void Init()
		{
			GlobalSettings.DefaultFloatingPointTolerance = TestTools.FloatingPointTolerance;
		}

		[Test]
		public void BasicTests()
		{
			var points = new[] 
			{ 
				new Vector2(0, 0), 
				new Vector2(0, 10),
				new Vector2(10, 0),
				new Vector2(0, 0),
			};

			var p1 = new Polygon(points);
			RunBasicTests(p1);
			
			var p2 = new Polygon(p1);
			RunBasicTests(p2);
		}

		void RunBasicTests(Polygon p)
		{
			Assert.AreEqual(4, p.Count);
			Assert.AreEqual(10 + 10 + Math.Sqrt(10 * 10 + 10 * 10), p.Length);
			TestTools.VectorEquals(new Vector2(0, 0), p[0]);
			TestTools.VectorEquals(new Vector2(0, 10), p[1]);
			TestTools.VectorEquals(new Vector2(10, 0), p[2]);
			TestTools.VectorEquals(new Vector2(0, 0), p[3]);
			TestTools.RectangleEquals(new RectangleF(0, 0, 10, 10), p.Bounds);

			var points = new List<Vector2>(4);
			foreach (var point in p)
			{
				points.Add(point);
			}
			Assert.AreEqual(4, points.Count);
			TestTools.VectorEquals(new Vector2(0, 0), points[0]);
			TestTools.VectorEquals(new Vector2(0, 10), points[1]);
			TestTools.VectorEquals(new Vector2(10, 0), points[2]);
			TestTools.VectorEquals(new Vector2(0, 0), points[3]);
		}
		
		[Test]
		public void TestPointOnEdge()
		{
			var points = new[] 
			{ 
				new Vector2(0, 0), 
				new Vector2(0, 10),
				new Vector2(10, 0),
				new Vector2(0, 0),
			};

			var p1 = new Polygon(points);
			RunPointOnEdgeTest(p1, p1.Offset);

			p1.Offset = new Vector2(5, 10);
			RunPointOnEdgeTest(p1, p1.Offset);
		}

		void RunPointOnEdgeTest(Polygon p, Vector2 offset)
		{
			TestTools.VectorEquals(new Vector2(0, 0) + offset, p.ClosestPointOnEdge(new Vector2(-10, -10) + offset));
			TestTools.VectorEquals(new Vector2(5, 0) + offset, p.ClosestPointOnEdge(new Vector2(5, -10) + offset));
			TestTools.VectorEquals(new Vector2(10, 0) + offset, p.ClosestPointOnEdge(new Vector2(10, -10) + offset));
			TestTools.VectorEquals(new Vector2(0, 10) + offset, p.ClosestPointOnEdge(new Vector2(0, 100) + offset));
			TestTools.VectorEquals(new Vector2(0, 5) + offset, p.ClosestPointOnEdge(new Vector2(0.1f, 5) + offset));
		}

		[Test]
		public void TestEdgeIntersection()
		{
			var points = new[] 
			{ 
				new Vector2(0, 0), 
				new Vector2(0, 10),
				new Vector2(10, 0),
				new Vector2(0, 0),
			};

			var l = new Line(new Vector2(5, -5), new Vector2(5, 1));
			var p1 = new Polygon(points);
			var b = false;
			var intersection = Vector2.Zero;
			var normal = Vector2.Zero;
			
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(5, 0), intersection);
			TestTools.VectorEquals(new Vector2(0, -1), normal);

			l = new Line(new Vector2(5, -20), new Vector2(5, 20));
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(5, 0), intersection);
			TestTools.VectorEquals(new Vector2(0, -1), normal);

			l = new Line(new Vector2(-5, 5), new Vector2(20, 5));
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(0, 5), intersection);
			TestTools.VectorEquals(new Vector2(-1, 0), normal);

			l = new Line(new Vector2(-5, -5), new Vector2(-4, -4));
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsFalse(b);

			l = new Line(new Vector2(1, 1), new Vector2(2, 2));
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsFalse(b);

			p1.Offset = new Vector2(5, 5);
			l = new Line(new Vector2(10, 0), new Vector2(10, 20));
			b = p1.GetEdgeIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(10, 5), intersection);
		}

		[Test]
		public void TestIntersection()
		{
			var points = new[] 
			{ 
				new Vector2(0, 0), 
				new Vector2(0, 10),
				new Vector2(10, 0),
				new Vector2(0, 0),
			};

			var l = new Line();
			var p1 = new Polygon(points);
			var b = false;
			var intersection = Vector2.Zero;
			var normal = Vector2.Zero;

			l = new Line(new Vector2(5, -5), new Vector2(5, 1));
			b = p1.GetIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(5, 0), intersection);
			TestTools.VectorEquals(new Vector2(0, -1), normal);

			l = new Line(new Vector2(5, 1), new Vector2(5, -1));
			b = p1.GetIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(5, 1), intersection);
			TestTools.VectorEquals(new Vector2(0, -1), normal);

			l = new Line(new Vector2(1, 5), new Vector2(-1, 5));
			b = p1.GetIntersectionPoint(l, out intersection, out normal);
			Assert.IsTrue(b);
			TestTools.VectorEquals(new Vector2(1, 5), intersection);
			TestTools.VectorEquals(new Vector2(-1, 0), normal);
		}

		[Test]
		public void TestPointInside()
		{
			var points = new[] 
			{ 
				new Vector2(0, 0), 
				new Vector2(0, 10),
				new Vector2(10, 0),
				new Vector2(0, 0),
			};
			
			var p1 = new Polygon(points);
			var p = new Vector2();

			p = new Vector2(1, 1); Assert.IsTrue(p1.Contains(p));
			p = new Vector2(4, 4); Assert.IsTrue(p1.Contains(p));
			p = new Vector2(6, 6); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(20, 20); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(-20, -20); Assert.IsFalse(p1.Contains(p));

			points = new[] 
			{ 
				new Vector2(0, 0),
				new Vector2(10, 0),
				new Vector2(10, 5),
				new Vector2(5, 5),
				new Vector2(5, 10),
				new Vector2(0, 10),
				new Vector2(0, 0),
			};

			p1 = new Polygon(points);
			p = new Vector2(1, 1); Assert.IsTrue(p1.Contains(p));
			p = new Vector2(9, 1); Assert.IsTrue(p1.Contains(p));
			p = new Vector2(1, 9); Assert.IsTrue(p1.Contains(p));
			p = new Vector2(9, 9); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(6, 6); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(11, 3); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(3, 11); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(3, -1); Assert.IsFalse(p1.Contains(p));
			p = new Vector2(-1, 3); Assert.IsFalse(p1.Contains(p));
		}
	}
}