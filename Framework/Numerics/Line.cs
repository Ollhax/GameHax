using System;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A line segment made up of two floating point vectors.
	/// </summary>
	public struct Line
	{
		/// <summary>
		/// Starting point of line.
		/// </summary>
		public Vector2 Start;

		/// <summary>
		/// End point of line.
		/// </summary>
		public Vector2 End;

		/// <summary>
		/// Return zero-length line.
		/// </summary>
		public static Line Zero { get { return new Line(new Vector2(0, 0), new Vector2(0, 0)); } }
		
		/// <summary>
		/// Create a copy of the specified line.
		/// </summary>
		/// <param name="other">Line to copy.</param>
		public Line(Line other)
		{
			Start = other.Start;
			End = other.End;
		}

		/// <summary>
		/// Create a line with the specified start and end points.
		/// </summary>
		/// <param name="start">Start point.</param>
		/// <param name="end">End point.</param>
		public Line(Vector2 start, Vector2 end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Create a line with the specified coordinates.
		/// </summary>
		/// <param name="x1">Start x coordinate.</param>
		/// <param name="y1">Start y coordinate.</param>
		/// <param name="x2">End x coordinate.</param>
		/// <param name="y2">End y coordinate.</param>
		public Line(float x1, float y1, float x2, float y2)
		{
			Start = new Vector2(x1, y1);
			End = new Vector2(x2, y2);
		}

		/// <summary>
		/// Returns the direction of the line, checking for zero-length lines.
		/// </summary>
		/// <returns>The direction of the line.</returns>
		public Vector2 SafeDirection()
		{
			return (End - Start).SafeNormalized();
		}

		/// <summary>
		/// Returns the direction of the line.
		/// </summary>
		/// <returns>The direction of the line.</returns>
		public Vector2 Direction()
		{
			return (End - Start).Normalized();
		}
		
		/// <summary>
		/// Fetch the perpendicular of the line's direction.
		/// </summary>
		/// <returns>A unit vector that is perpendicular to the line's direction.</returns>
		public Vector2 Normal()
		{
			var normal = SafeDirection();
			if (normal.IsZero)
				return Vector2.Zero;

			return normal.GetPerpendicular();
		}
		
		/// <summary>
		/// Get the length of this line.
		/// </summary>
		/// <returns>The length of this line.</returns>
		public float Length()
		{
			return (End - Start).Length();
		}

		/// <summary>
		/// Move this line by the specified offset.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public void Offset(float x, float y)
		{
			Start.X += x;
			Start.Y += y;
			End.X += x;
			End.Y += y;
		}

		/// <summary>
		/// Move this line by the specified offset.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		public void Offset(Vector2 offset)
		{
			Start += offset;
			End += offset;
		}

		/// <summary>
		/// Return a copy of this line that is moved by the specified offset.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		/// <returns>A moved copy.</returns>
		public Line Offseted(float x, float y)
		{
			var copy = new Line(this);
			copy.Start.X += x;
			copy.Start.Y += y;
			copy.End.X += x;
			copy.End.Y += y;
			return copy;
		}

		/// <summary>
		/// Return a copy of this line that is moved by the specified offset.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		/// <returns>A moved copy.</returns>
		public Line Offseted(Vector2 offset)
		{
			var copy = new Line(this);
			copy.Start += offset;
			copy.End += offset;
			return copy;
		}

		/// <summary>
		/// Fetch the normal of this line that is facing towards the specified point.
		/// </summary>
		/// <param name="point">A point.</param>
		/// <returns>The normal vector that faces the specified point.</returns>
		public Vector2 GetNormalFacingPoint(Vector2 point)
		{
			var normal = Normal();
			if (PointsOnSameSide(normal, point - Start))
			{
				return normal;
			}
			return -normal;
		}

		/// <summary>
		/// Returns true if the default normal of this line faces the specified point.
		/// </summary>
		/// <param name="point">A point.</param>
		/// <returns>True if the normal faces the specified point.</returns>
		public bool FacingPoint(Vector2 point)
		{
			var normal = Normal();
			return PointsOnSameSide(normal, point - Start);
		}

		/// <summary>
		/// Check if the two specified points are on the same side of this line. 
		/// </summary>
		/// <remarks>Treats the line as it was an infinite line rather than a segment.</remarks>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <returns>True if the two points are on the same side of this line.</returns>
		public bool PointsOnSameSide(Vector2 p1, Vector2 p2)
		{
			var diff = End - Start;
			diff = diff.GetPerpendicular();

			return Vector2.Dot(p1, diff) * Vector2.Dot(p2, diff) > 0.0f;
		}

		/// <summary>
		/// Fetch the point on this line that is closest to the specified point.
		/// </summary>
		/// <param name="point">Input point.</param>
		/// <returns>The point on this line that is closest to the specified point.</returns>
		public Vector2 ClosestPointOnLine(Vector2 point)
		{
			float length = Length();
			
			// Special case, zero length line
			if (length == 0)
			{
				return Start;
			}

			Vector2 dir = (End - Start).Normalized(length);
			point = point - Start;
			
			float r;
			Vector2.Dot(ref point, ref dir, out r);
			
			// Clamp
			if (r < 0) r = 0;
			else if (r > length) r = length;

			return Start + dir * r;
		}

		/// <summary>
		/// Check if this line intersects the specified circle.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(Circle other)
		{
			Vector2 p = ClosestPointOnLine(other.Center);
			return (p - other.Center).LengthSquared() < other.Radius * other.Radius;
		}

		/// <summary>
		/// Check if this line intersects the specified line.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(Line other)
		{
			Vector2 dummy;
			return GetIntersectionPoint(other, out dummy);
		}

		/// <summary>
		/// Check if this line intersects the specified rectangle.
		/// </summary>
		/// <param name="other">Shape to test against.</param>
		/// <returns>True if the two shapes intersect.</returns>
		public bool Intersects(RectangleF other)
		{
			Line l;

			// Top
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X + other.Width, other.Y));
			if (Intersects(l)) return true;

			// Right
			l = new Line(new Vector2(other.X + other.Width, other.Y), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (Intersects(l)) return true;

			// Bottom
			l = new Line(new Vector2(other.X, other.Y + other.Height), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (Intersects(l)) return true;

			// Left
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X, other.Y + other.Height));
			if (Intersects(l)) return true;

			return false;
		}

		/// <summary>
		/// Returns true if this line has no length (i.e. start and end are equal).
		/// </summary>
		/// <returns></returns>
		public bool ZeroLength()
		{
			return MathTools.Equals(Start, End);
		}

		/// <summary>
		/// Returns true if this line runs parallel with specified line.
		/// </summary>
		/// <param name="other">Line to test against.</param>
		/// <returns>True if this line is parallel with the specified line.</returns>
		public bool Parallel(Line other)
		{
			return (other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y) == 0;
		}
		
		/// <summary>
		/// Returns true if this line runs colinear with specified line.
		/// </summary>
		/// <param name="other">Line to test against.</param>
		/// <returns>True if this line is colinear with the specified line.</returns>
		public bool Colinear(Line other)
		{
			if (!Parallel(other)) return false;

			Line l = new Line(Start, other.Start);
			return l.Parallel(this);
		}

		/// <summary>
		/// Get the intersection point between a line and a rectangle.
		/// </summary>
		/// <returns>True if the lines collide.</returns>
		public bool GetIntersectionPoint(RectangleF other, out Vector2 intersectionPoint)
		{
			if (ZeroLength())
			{
				intersectionPoint = Vector2.Zero;
				return false;
			}

			Line l;
			
			// Top
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X + other.Width, other.Y));
			if (GetIntersectionPoint(l, out intersectionPoint)) return true;

			// Right
			l = new Line(new Vector2(other.X + other.Width, other.Y), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (GetIntersectionPoint(l, out intersectionPoint)) return true;

			// Bottom
			l = new Line(new Vector2(other.X, other.Y + other.Height), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (GetIntersectionPoint(l, out intersectionPoint)) return true;

			// Left
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X, other.Y + other.Height));
			if (GetIntersectionPoint(l, out intersectionPoint)) return true;

			intersectionPoint = Vector2.Zero;
			return false;
		}

		/// <summary>
		/// Get the intersection point between two lines.
		/// </summary>
		/// <returns>True if the lines collide.</returns>
		public bool GetIntersectionPoint(Line other, out Vector2 intersectionPoint)
		{
			intersectionPoint = new Vector2();
			float denom = (other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y);

			if (denom == 0)
			{				
				// Parallel lines, check for colinearity
				if (Colinear(other))
				{
					float l1;
					float l2;

					if ((End.X - Start.X) != 0)
					{
						l1 = (other.Start.X - Start.X) / (End.X - Start.X);
						l2 = (other.End.X - Start.X) / (End.X - Start.X);
					}
					else
					{
						l1 = (other.Start.Y - Start.Y) / (End.Y - Start.Y);
						l2 = (other.End.Y - Start.Y) / (End.Y - Start.Y);
					}
					
					// Parallel, colinear, but not coinciding.
					if ((l1 < 0 && l2 < 0) || (l1 > 1 && l2 > 1))
					{
						return false;
					}

					l1 = MathTools.Clamp<float>(l1, 0, 1);
					l2 = MathTools.Clamp<float>(l2, 0, 1);

					intersectionPoint = Start + Math.Min(l1, l2) * (End - Start);
					return true;
				}

				return false;
			}
			
			float r = (((other.End.X - other.Start.X) * (Start.Y - other.Start.Y)) - ((other.End.Y - other.Start.Y) * (Start.X - other.Start.X))) / denom;						
			if (r < 0 || r > 1)
			{
				return false;
			}

			float s = (((End.X - Start.X) * (Start.Y - other.Start.Y)) - ((End.Y - Start.Y) * (Start.X - other.Start.X))) / denom;
			if (s < 0 || s > 1)
			{
				return false;
			}

			intersectionPoint = Start + r * (End - Start);
			return true;
		}

		/// <summary>
		/// Returns the closest intersection point from the start to the target circle.
		/// </summary>
		/// <returns>True if this line and the circle intersect.</returns>
		public bool GetIntersectionPoint(Circle other, out Vector2 intersectionPoint)
		{
			Vector2 p = ClosestPointOnLine(other.Center);
			
			if ((p - other.Center).LengthSquared() < other.Radius * other.Radius)
			{
				float A = (p - other.Center).Length();
				float C = other.Radius;
				float B = (float)Math.Sqrt(C * C - A * A);

				Vector2 dir = SafeDirection();
				Vector2 p1 = p - dir * B;
				Vector2 p2 = p + dir * B;

				intersectionPoint = ((p1 - Start).LengthSquared() < (p2 - Start).LengthSquared()) ? p1 : p2;
				
				return true;
			}

			intersectionPoint = new Vector2();
			return false;
		}

		/// <summary>
		/// Fetch the smallest bounding box that contains this line.
		/// </summary>
		/// <returns>The smallest bounding box containing this line.</returns>
		public RectangleF Bounds()
		{
			return new RectangleF(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y), Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y));
		}

		/// <summary>
		/// Convert this line to a string representation.
		/// </summary>
		/// <returns>String representation of this line.</returns>
		public override string ToString()
		{
			return "{" + Start.ToString() + "}, {" + End.ToString() + "}";
		}
		
		/// <summary>
		/// Get the hash code value for this line.
		/// </summary>
		/// <returns>Hash code value.</returns>
		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ End.GetHashCode();
		}
	}
}
