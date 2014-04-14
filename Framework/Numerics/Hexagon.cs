using System;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A generic regular flat-top hexagon representation.
	/// </summary>
	[Serializable]
	public struct Hexagon
	{
		public Vector2 Center;
		public float Radius;
		public float Rotation;

		public static Vector2[] Units = new Vector2[] { new Vector2(-1, 0), new Vector2(0.5f, (float)Math.Sqrt(3.0f) / 2.0f), new Vector2(0.5f, -(float)Math.Sqrt(3.0f) / 2.0f) };

		public static Hexagon Zero { get { return new Hexagon(0, 0, 0, 0); } }

		public Circle BoundingCircle { get { return new Circle(Center, Radius); }}

		public Hexagon(Hexagon other)
			: this(other.Center.X, other.Center.Y, other.Radius, other.Rotation)
		{

		}

		public Hexagon(Vector2 center, float radius, float rotation)
			: this(center.X, center.Y, radius, rotation)
		{

		}
		
		public Hexagon(float x, float y, float radius, float rotation)
		{
			Center.X = x;
			Center.Y = y;
			Radius = radius;
			Rotation = rotation;
		}
		
		public Hexagon Offseted(Vector2 offset)
		{
			return new Hexagon(Center + offset, Radius, Rotation);
		}

		public Vector2 GetCorner(int index)
		{
			return Center + MathTools.FromAngle(Rotation + index * MathTools.TwoPi / 6) * Radius;
		}

		public Vector2 GetPoint(float angle)
		{
			angle = MathTools.WrapAngle(angle);

			const float hexAngle = MathTools.TwoPi / 6;
			int segment = (int)(angle / hexAngle);
			var localAngle = angle % hexAngle;

			float a = (float)Math.Tan(localAngle);
			float h = (float)Math.Sqrt(3);
			float x = h / (a + h);
			float y = a * x;

			return Center + new Vector2(x * Radius, y * Radius).Rotated(segment * hexAngle + Rotation);
		}
		
		public Line ClosestEdge(Vector2 point)
		{
			const float hexAngle = MathTools.TwoPi / 6;
			int region = (int)(MathTools.WrapAngle((point - Center).DefaultTo().Angle() - Rotation) / hexAngle);
			return new Line(GetCorner(region), GetCorner(region + 1));
		}

		public Vector2 ClosestPointOnEdge(Vector2 point)
		{
			return ClosestEdge(point).ClosestPointOnLine(point);
		}

		public Vector2 RandomPointInside()
		{
			var rhombus = MathTools.Random().Next(3);
			var v1 = Units[rhombus];
			var v2 = Units[(rhombus + 1) % 3];

			if (Rotation != 0)
			{
				v1 = v1.Rotated(Rotation);
				v2 = v2.Rotated(Rotation);
			}

			var x = MathTools.Random().NextFloat();
			var y = MathTools.Random().NextFloat();

			return Center + Radius * new Vector2(x * v1.X + y * v2.X, x * v1.Y + y * v2.Y);
		}

		public bool Contains(Vector2 point)
		{
			// Early-out: point not inside bounding circle
			var boundingCircle = BoundingCircle;
			if (!boundingCircle.Contains(point)) return false;

			// Early-in: point inside inner circle
			boundingCircle.Radius *= (float)Math.Sqrt(3) / 2.0f;
			if (boundingCircle.Contains(point)) return true;
			
			// Worst case: determine maxiumum distance for a point at that angle, compare with the point's distance
			var maxDist = (GetPoint((point - Center).DefaultTo().Angle() - Rotation) - Center).LengthSquared();
			return (point - Center).LengthSquared() < maxDist;
		}

		public bool Contains(Circle circle)
		{
			// Early-out: circle not inside bounding circle
			var boundingCircle = BoundingCircle;
			if (!boundingCircle.Contains(circle)) return false;

			// Early-in: circle inside inner circle
			boundingCircle.Radius *= (float)Math.Sqrt(3) / 2.0f;
			if (boundingCircle.Contains(circle)) return true;

			// Get closest edge, check if we cross it
			var closest = ClosestPointOnEdge(circle.Center);
			var dist = (closest - circle.Center).LengthSquared();
			if (circle.Radius * circle.Radius > dist) return false;

			// Finally, make sure our center lies closer to the center than the closest point
			// (This avoids false positives when the circle is very small compared to the hexagon. Can probably be made prettier.)
			return (closest - Center).Length() > (circle.Center - Center).Length();
		}

		public bool Intersects(Circle other)
		{
			var boundingCircle = BoundingCircle;

			// If our bounding areas do not meet, we never collide
			if (!other.Intersects(boundingCircle)) return false;
			
			// If we contain the circle center, we always collide
			if (Contains(other.Center)) return true;
			
			// Now, the tricky part. The circle may graze at the edge of the hexagon,
			// yet still not collide. We need to find the point on the hexagon closest
			// to the circle, then compare distances.
			var nearestPoint = ClosestPointOnEdge(other.Center);
			return other.Contains(nearestPoint);
		}

		public bool GetOverlapse(Circle other, out Vector2 overlapse)
		{
			overlapse = Vector2.Zero;

			// Quick rejection
			if (!other.Intersects(BoundingCircle)) return false;

			float minOverlapse = float.MaxValue;

			// Run SAT on all six edges and corners
			for (int i = 0; i < 12; i++)
			{
				Line line;
				if (i % 2 == 0) // Corner
				{
					var tangent = MathTools.FromAngle(Rotation + (MathTools.TwoPi / 6) * i / 2).GetPerpendicular();

					var corner = GetCorner(i / 2);
					line = new Line(corner - tangent, corner + tangent);
				}
				else // Edge
				{
					line = new Line(GetCorner(i / 2), GetCorner(i / 2 + 1));
				}

				Vector2 satAxis = line.Direction().GetPerpendicular();

				//DebugDraw.Instance.Draw(line, Color.Yellow);

				float a1 = 0;
				float a2 = Vector2.Dot(GetCorner(i + 3) - line.Start, satAxis);
				if (a2 < a1) MathTools.Swap(ref a1, ref a2);

				float b1 = Vector2.Dot(other.Center - line.Start + satAxis * other.Radius, satAxis);
				float b2 = Vector2.Dot(other.Center - line.Start - satAxis * other.Radius, satAxis);
				if (b2 < b1) MathTools.Swap(ref b1, ref b2);

				//DebugDraw.Instance.Draw(new Circle(line.Start + satAxis * a1, 3), Color.Red);
				//DebugDraw.Instance.Draw(new Circle((line.Start + satAxis * a2), 3), Color.Green);
				//DebugDraw.Instance.Draw(new Circle((line.Start + satAxis * b1), 3), Color.Blue);
				//DebugDraw.Instance.Draw(new Circle((line.Start + satAxis * b2), 3), Color.White);

				if (b2 < a1 || a1 > b2)
					return false;

				float v = b2 - a1;
				//DebugDraw.Instance.Draw(new Line(line.Start, line.Start + satAxis * v), Color.Teal);

				if (v < minOverlapse)
				{
					minOverlapse = v;
					overlapse = satAxis * -v;
				}
			}

			return true;
		}
		
		public bool Intersects(Circle other, out Vector2 collisionPoint, out Vector2 overlapse)
		{
			if (GetOverlapse(other, out overlapse))
			{
				collisionPoint = ClosestPointOnEdge(other.Center);
				return true;
			}

			collisionPoint = Vector2.Zero;
			return false;
		}
	}
}
