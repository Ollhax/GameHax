using System;
using System.Diagnostics;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A circle made up of a point and a radius.
	/// </summary>
	public struct Circle
	{
		/// <summary>
		/// Center of circle.
		/// </summary>
		public Vector2 Center;

		/// <summary>
		/// Radius of circle.
		/// </summary>
		public float Radius;

		/// <summary>
		/// Return a circle centered on origin with zero radius.
		/// </summary>
		public static Circle Zero { get { return new Circle(Vector2.Zero, 0); } }

		/// <summary>
		/// Create a new circle.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="radius">Radius of circle.</param>
		public Circle(float x, float y, float radius)
		{
			Center.X = x;
			Center.Y = y;
			Radius = radius;
		}

		/// <summary>
		/// Create a new circle.
		/// </summary>
		/// <param name="center">Center position.</param>
		/// <param name="radius">Radius of circle.</param>
		public Circle(Vector2 center, float radius)
		{
			Center = center;
			Radius = radius;
		}

		/// <summary>
		/// Copy another circle.
		/// </summary>
		/// <param name="other">Circle to copy.</param>
		public Circle(Circle other)
		{
			Center = other.Center;
			Radius = other.Radius;
		}

		/// <summary>
		/// Fetch the smallest box containing this circle.
		/// </summary>
		public RectangleF Bounds { get { return new RectangleF(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2); } }

		/// <summary>
		/// Expand the radius by the specified distance.
		/// </summary>
		/// <param name="distance">Distance to expand the circle.</param>
		public void Expand(float distance)
		{
			Radius += distance;
		}
		
		/// <summary>
		/// Create a copy of this circle and increase its radius.
		/// </summary>
		/// <param name="distance">Distance to expand the copied circle.</param>
		/// <returns>The expanded copy.</returns>
		public Circle Expanded(float distance)
		{
			return new Circle(Center, Radius + distance);
		}

		/// <summary>
		/// Test if the specified vector is contained within this circle.
		/// </summary>
		/// <param name="other">Vector to test against.</param>
		/// <returns>True if the vector is within the circle.</returns>
		public bool Contains(Vector2 other)
		{
			return (other - Center).LengthSquared() <= Radius * Radius;
		}
		
		/// <summary>
		/// Test if the specified circle is fully contained within this circle.
		/// </summary>
		/// <param name="other">Circle to test against.</param>
		/// <returns>True if the specified circle is fully contained within this circle.</returns>
		public bool Contains(Circle other)
		{
			if (other.Radius > Radius) return false;
			if (!Contains(other.Center)) return false;
			return Contains((other.Center - Center).SafeNormalized() * other.Radius + other.Center);
		}

		/// <summary>
		/// Test if the specified circle intersects with this circle.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(Circle other)
		{
			return (other.Center - Center).LengthSquared() < (Radius + other.Radius) * (Radius + other.Radius);
		}

		/// <summary>
		/// Test if the specified line intersects with this circle.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(Line other)
		{
			return other.Intersects(this);
		}
		
		/// <summary>
		/// Test if the specified rectangle intersects with this circle.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(RectangleF other)
		{
			return other.Contains(Center) || new Line(other.TopLeft, other.TopRight).Intersects(this)
			       || new Line(other.TopRight, other.BottomRight).Intersects(this)
			       || new Line(other.BottomRight, other.BottomLeft).Intersects(this)
			       || new Line(other.BottomLeft, other.TopLeft).Intersects(this);
		}

		/// <summary>
		/// Convert this circle to a string representation.
		/// </summary>
		/// <returns>String representation for this circle.</returns>
		public override string ToString()
		{
			return "{c=" + Center.ToString() + ", r=" + Radius.ToString() + "}";
		}

		/// <summary>
		/// Get the hash value for this circle.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Center.GetHashCode() ^ Radius.GetHashCode();
		}

		/// <summary>
		/// Return a point on the edge of this circle.
		/// </summary>
		/// <param name="delta">Angle, in radians.</param>
		/// <returns>A point on the edge of this circle.</returns>
		public Vector2 PointOnEdge(float delta)
		{
			return Center + Radius * new Vector2((float)Math.Cos(delta), (float)Math.Sin(delta));
		}

		/// <summary>
		/// Fetch a random point within this circle.
		/// </summary>
		/// <returns></returns>
		public Vector2 RandomPointInside()
		{
			float theta = MathTools.Random().NextFloat(0, MathTools.TwoPi);
			float radius = (float)Math.Sqrt(MathTools.Random().NextDouble()) * Radius;

			return new Vector2(Center.X + (float)Math.Cos(theta) * radius, Center.Y + (float)Math.Sin(theta) * radius);
		}

		/// <summary>
		/// Calculates how much this circle is penetrating the target rectangle.
		/// Will return the length and direction of the penetration of the least
		/// penetrating axis.
		/// </summary>
		/// <param name="other">Calculate overlapse against this rectangle.</param>
		/// <returns>Shortest overlapse distance.</returns>
		public Vector2 ShortestOverlapse(RectangleF other)
		{
			// 1. Check which voronoi diagram region the center of the circle lies in.
			// 2. If on an edge, test against x and y axis.
			// 3. If on a corner, project the rectangle's bounds unto an axis made up
			//    from the line that cross both the rectangle and circles centers.
			//    Compare this projection with that of the circle; if they collide,
			//    the amount of collision is the penetration we seek.
			//
			// For a visual representation, see
			// http://www.metanetsoftware.com/technique/tutorialA.html
			
			var region = other.GetRegion(Center);

			switch (region)
			{
				// Sides
				case 1: case 3: case 5: case 7: case 4:
					{
						// Project sides of rectangle and circle
						var s1 = GetOverlapse(other.Top, other.Bottom, Center.Y - Radius, Center.Y + Radius);
						if (s1 == 0)
						{
							return Vector2.Zero;
						}
						
						var s2 = GetOverlapse(other.Left, other.Right, Center.X - Radius, Center.X + Radius);
						if (s2 == 0)
						{
							return Vector2.Zero;
						}

						if (Math.Abs(s1) < Math.Abs(s2))
						{
							return new Vector2(0, s1);
						}

						return new Vector2(s2, 0);
					} 

				// Corners
				case 0: case 2: case 6: case 8:
					{
						var center = Vector2.Zero;
						var end = Vector2.Zero;

						if (region == 0) { center = other.TopLeft; end = other.BottomRight; }
						if (region == 2) { center = other.TopRight; end = other.BottomLeft; }
						if (region == 6) { center = other.BottomLeft; end = other.TopRight; }
						if (region == 8) { center = other.BottomRight; end = other.TopLeft; }

						var diff = Center - center;
						var length = diff.Length();
						diff.X /= length;
						diff.Y /= length;
						
						var r1 = Vector2.Dot(end - center, diff);
						var r2 = 0.0f;
						
						return diff * GetOverlapse(r1, r2, length - Radius, length + Radius);
					}

				default:
					Debug.Assert(false);
					break;
			}

			return Vector2.Zero;
		}

		private float GetOverlapse(float startA, float endA, float startB, float endB)
		{
			if (endB < startA || startB > endA)
				return 0;

			var centerA = startA + (endA - startA) / 2;
			var centerB = startB + (endB - startB) / 2;

			if (centerA < centerB)
			{
				return endA - startB;
			}

			return startA - endB;
		}
		
		/// <summary>
		/// Test for intersection against a point when sweeping this circle from its current location to the target one.
		/// </summary>
		/// <param name="newPosition">New location.</param>
		/// <param name="other">Point to test collision against.</param>
		/// <returns>True if the swept shape intersects with the point.</returns>
		public bool SweepIntersects(Vector2 newPosition, Vector2 other)
		{
			return SweepIntersects(newPosition, other, Radius);
		}

		/// <summary>
		/// Test for intersection against another circle when sweeping this circle from its current location to the target one.
		/// </summary>
		/// <param name="newPosition">New location.</param>
		/// <param name="other">Circle to test collision against.</param>
		/// <returns>True if the swept shape intersects with the circle.</returns>
		public bool SweepIntersects(Vector2 newPosition, Circle other)
		{
			return SweepIntersects(newPosition, other.Center, Radius + other.Radius);
		}

		private bool SweepIntersects(Vector2 newPosition, Vector2 other, float maxDistance)
		{
			Line l = new Line(Center, newPosition);
			Vector2 p = l.ClosestPointOnLine(other);

			return (p - other).LengthSquared() < maxDistance * maxDistance;
		}

		/// <summary>
		/// Try to sweep move this shape from its current position to the new one,
		/// colliding against the target shape.
		/// </summary>
		/// <param name="newPosition">Try to move to this position.</param>
		/// <param name="other">Collide against this shape.</param>
		/// <returns>True if we could move all the way, false if we got stuck (i.e. collided against the other shape)</returns>		
		public bool SweepMove(Vector2 newPosition, Circle other)
		{
			// The first circle moves between P1 and P2 with the functions f(t) and g(t).
			// f(t) = x0 + t(x1 - x0)
			// g(t) = y0 + t(y1 - y0)
			// Find distance d from other points center (ox and oy) such that d = sqrt((ox - f(t)^2 + (oy - g(t))^2).
			
			// Special case, intersects at start.
			if (Intersects(other))
			{
				return false;
			}

			float x0 = Center.X;
			float y0 = Center.Y;
			float x1 = newPosition.X;
			float y1 = newPosition.Y;
			float xd = x1 - x0;
			float yd = y1 - y0;			
			float ox = other.Center.X;
			float oy = other.Center.Y;
			float d = Radius + other.Radius;
						
			float A = 
				xd * xd + 
				yd * yd;
			
			if (A == 0)
			{
				// Special case, we did not move.
				return true;
			}
			
			float B = 
				(-2 * ox * xd) + (2 * x0 * xd) + 
				(-2 * oy * yd) + (2 * y0 * yd);

			float C = - d * d +
				ox * ox - 2 * ox * x0 + x0 * x0 +
				oy * oy - 2 * oy * y0 + y0 * y0;

			float Bp = B * B - 4 * A * C;
			
			// If Bp is <= 0, we did not collide. Move to the new location.
			if (Bp <= 0)
			{
				Center = newPosition;
				return true;
			}

			// Can get up to two results, but ignore any ones 
			float res1 = (-B + (float)Math.Sqrt(Bp)) / (2 * A);
			float res2 = (-B - (float)Math.Sqrt(Bp)) / (2 * A);

			float distance = 1;
			if (res1 > 0 && res1 < 1)
				distance = res1;
			if (res2 > 0 && res2 < 1)
				distance = Math.Min(distance, res2);

			if (MathTools.Equals(distance, 1))
			{
				// Special case, we scraped by the other shape but did not collide with it
				Center = newPosition;
				return true;
			}

			// Move by the specified distance
			Vector2 dir = newPosition - Center;
			float length = dir.Length();
			Center = Center + dir.SafeNormalized(length) * length * distance;

			return false;
		}
	}
}
