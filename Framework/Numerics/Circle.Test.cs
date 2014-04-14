using System;
using NUnit.Framework;
using MG.Framework.Utility;

namespace MG.Framework.Numerics
{
	[TestFixture]
	public class CircleTest
	{
		[SetUp]
		public void Init()
		{
			GlobalSettings.DefaultFloatingPointTolerance = TestTools.FloatingPointTolerance;
		}

		[Test]
		public void ShortestOverlapseTest()
		{
			var c = new Circle(new Vector2(0, 0), 4);
			var r = new RectangleF(10, 10, 10, 10);
			
			// Outside of collision
			c.Center = new Vector2(0, 0); TestTools.VectorEquals(new Vector2(0, 0), c.ShortestOverlapse(r));
			c.Center = new Vector2(100, 0); TestTools.VectorEquals(new Vector2(0, 0), c.ShortestOverlapse(r)); 
			c.Center = new Vector2(100, 100); TestTools.VectorEquals(new Vector2(0, 0), c.ShortestOverlapse(r));
			c.Center = new Vector2(0, 100); TestTools.VectorEquals(new Vector2(0, 0), c.ShortestOverlapse(r));
			
			// Sides
			c.Center = new Vector2(15, 9); TestTools.VectorEquals(new Vector2(0, -3), c.ShortestOverlapse(r));
			c.Center = new Vector2(21, 15); TestTools.VectorEquals(new Vector2(3, 0), c.ShortestOverlapse(r));
			c.Center = new Vector2(15, 21); TestTools.VectorEquals(new Vector2(0, 3), c.ShortestOverlapse(r));
			c.Center = new Vector2(9, 15); TestTools.VectorEquals(new Vector2(-3, 0), c.ShortestOverlapse(r));

			// Inside sides
			c.Center = new Vector2(15, 11); TestTools.VectorEquals(new Vector2(0, -5), c.ShortestOverlapse(r));
			c.Center = new Vector2(19, 15); TestTools.VectorEquals(new Vector2(5, 0), c.ShortestOverlapse(r));
			c.Center = new Vector2(15, 19); TestTools.VectorEquals(new Vector2(0, 5), c.ShortestOverlapse(r));
			c.Center = new Vector2(11, 15); TestTools.VectorEquals(new Vector2(-5, 0), c.ShortestOverlapse(r));

			// Corners
			var v = (float)((c.Radius - Math.Sqrt(2.0f)) * 1.0f / Math.Sqrt(2.0f));
			c.Center = new Vector2(9, 9); TestTools.VectorEquals(new Vector2(-v, -v), c.ShortestOverlapse(r));
			c.Center = new Vector2(21, 9); TestTools.VectorEquals(new Vector2(v, -v), c.ShortestOverlapse(r));
			c.Center = new Vector2(21, 21); TestTools.VectorEquals(new Vector2(v, v), c.ShortestOverlapse(r));
			c.Center = new Vector2(9, 21); TestTools.VectorEquals(new Vector2(-v, v), c.ShortestOverlapse(r));
		}

		[Test]
		public void ContainsTest()
		{
			var c = new Circle(new Vector2(5, 5), 5);
			
			// Contains point
			Assert.IsFalse(c.Contains(new Vector2(0, 0)));
			Assert.IsTrue(c.Contains(new Vector2(5, 5)));
			Assert.IsTrue(c.Contains(new Vector2(10, 5)));
			Assert.IsFalse(c.Contains(new Vector2(10.001f, 5)));
			Assert.IsTrue(c.Contains(new Vector2((float)Math.Sqrt(c.Radius), (float)Math.Sqrt(c.Radius))));

			// Contains circle
			Assert.IsTrue(c.Contains(new Circle(5, 5, 5)));
			Assert.IsFalse(c.Contains(new Circle(5, 5, 5.01f)));
			Assert.IsTrue(c.Contains(new Circle(5, 1, 1.0f)));
			Assert.IsFalse(c.Contains(new Circle(5, 1, 1.01f)));
			Assert.IsTrue(c.Contains(new Circle(5, 5, 0)));
			Assert.IsFalse(c.Contains(new Circle(100, 5, 1)));
		}
		
		[Test]
		public void IntersectsTest()
		{
			var c = new Circle();
			var r = new RectangleF();

			c = new Circle(5, 5, 5);

			// Rectangle intersection
			r = new RectangleF(5, 5, 0, 0);
			Assert.IsTrue(c.Intersects(r));

			r = new RectangleF(-1, -1, 6, 6);
			Assert.IsTrue(c.Intersects(r));
			
			r = new RectangleF(0, 0, 0, 0);
			Assert.IsFalse(c.Intersects(r));

			r = new RectangleF(0, 0, 1, 1);
			Assert.IsFalse(c.Intersects(r));

			r = new RectangleF(0, 0, 3, 3);
			Assert.IsTrue(c.Intersects(r));

			r = new RectangleF(0, -1, 10, 2);
			Assert.IsTrue(c.Intersects(r));

			r = new RectangleF(0, 9, 10, 2);
			Assert.IsTrue(c.Intersects(r));

			r = new RectangleF(-1, 0, 2, 10);
			Assert.IsTrue(c.Intersects(r));

			r = new RectangleF(9, 0, 2, 10);
			Assert.IsTrue(c.Intersects(r));

			// Circle intersection
			var c2 = new Circle(20, 5, 5);
			Assert.IsFalse(c.Intersects(c2));
			Assert.IsFalse(c2.Intersects(c));

			c2 = new Circle(15, 5, 5);
			Assert.IsFalse(c.Intersects(c2));
			Assert.IsFalse(c2.Intersects(c));

			c2 = new Circle(15, 5, 5.01f);
			Assert.IsTrue(c.Intersects(c2));
			Assert.IsTrue(c2.Intersects(c));

			c2 = new Circle(0, 0, 2.0f);
			Assert.IsFalse(c.Intersects(c2));
			Assert.IsFalse(c2.Intersects(c));

			c2 = new Circle(0, 0, 0);
			Assert.IsFalse(c2.Intersects(c2));
		}
	}
}