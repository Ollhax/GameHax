using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A 2d polygon consisting of a number of points.
	/// </summary>
	/// <remarks>
	/// This implementation does not allow modifications of the polygon points
	/// after creation. This allows for some optimizations. You may, however,
	/// set an offset delta to the whole polygon.
	/// </remarks>
	public class Polygon : IEnumerable, IEnumerable<Vector2>
	{
		private List<Vector2> points;
		private List<Vector2> normals;
		private float length;
		private RectangleF unrotatedBounds;
		
		/// <summary>
		/// Fetch the number of points in this polygon.
		/// </summary>
		public int Count { get { return points.Count; }}
		
		/// <summary>
		/// Fetch a point.
		/// </summary>
		/// <param name="index">Index of point.</param>
		/// <returns>An offsetted polygon point.</returns>
		public Vector2 this[int index]
		{
			get
			{
				if (index >= points.Count || index < 0)
				{
					throw new IndexOutOfRangeException("invalid index");
				}

				if (Rotation == 0)
				{
					return points[index] + Offset;
				}

				return points[index].Rotated(Rotation) + Offset;
			}
		}
		
		/// <summary>
		/// Fetch the normal at the specified index.
		/// </summary>
		/// <param name="index">Normal to fetch. Valid range is [0, Count - 2].</param>
		/// <returns></returns>
		public Vector2 Normal(int index)
		{
			if (Rotation == 0)
			{
				return normals[index];
			}
			return normals[index].Rotated(Rotation);
		}

		/// <summary>
		/// Offset of this polygon.
		/// </summary>
		public Vector2 Offset;

		/// <summary>
		/// Rotation of this polygon. Applied _before_ the offset is applied.
		/// </summary>
		public float Rotation;

		/// <summary>
		/// Fetch the total length of the polygon.
		/// </summary>
		public float Length { get { return length; }}

		/// <summary>
		/// Fetch an axis-aligned bounding box containing this polygon.
		/// </summary>
		public RectangleF Bounds
		{
			get
			{
				if (Rotation == 0)
				{
					return unrotatedBounds.Offseted(Offset);
				}
				return CalculateBounds(Rotation).Offseted(Offset);
			}
		}
		
		/// <summary>
		/// Create a polygon from the specified points.
		/// </summary>
		/// <param name="points">A collection of points used to generate the polygon.</param>
		public Polygon(IEnumerable<Vector2> points)
		{
			this.points = new List<Vector2>(points);
			normals = new List<Vector2>();
			Initialize();
		}

		/// <summary>
		/// Create a copy of the specified polygon.
		/// </summary>
		/// <param name="other">Polygon to copy.</param>
		public Polygon(Polygon other)
		{
			points = new List<Vector2>(other.points);
			normals = new List<Vector2>(other.normals);
			length = other.length;
			unrotatedBounds = other.unrotatedBounds;
			Offset = other.Offset;
			Rotation = other.Rotation;
		}

		/// <summary>
		/// Reassign the points of this polygon.
		/// </summary>
		/// <param name="newPoints"></param>
		public void Reinitialize(List<Vector2> newPoints)
		{
			points.Clear();
			
			for (int i = 0; i < newPoints.Count; i++)
			{
				points.Add(newPoints[i]);
			}
			
			Initialize();
		}

		/// <summary>
		/// Changes this polygon into the target one. This operation
		/// assumes that the two polygons have the same number of
		/// points.
		/// </summary>
		/// <param name="other">Polygon to morph to.</param>
		public bool Morph(Polygon other)
		{
			if (other.points.Count != points.Count)
			{
				return false;
			}

			for (var i = 0; i < points.Count; ++i)
			{
				points[i] = other.points[i];
			}

			for (var i = 0; i < normals.Count; ++i)
			{
				normals[i] = other.normals[i];
			}

			length = other.length;
			unrotatedBounds = other.unrotatedBounds;
			Offset = other.Offset;
			Rotation = other.Rotation;
			return true;
		}

		private void Initialize()
		{
			CalculateNormals();
			length = CalculateLength();
			unrotatedBounds = CalculateBounds(0);
		}
		
		/// <summary>
		/// Fetch the point on this polygon that is closest to the input point.
		/// </summary>
		/// <param name="point">Input point.</param>
		/// <returns>The point on this polygon that is closest to the input point.</returns>
		public Vector2 ClosestPointOnEdge(Vector2 point)
		{
			Vector2 closestPointOnEdge;
			InternalClosestEdge(point, out closestPointOnEdge, out MathTools.Vector2Dummy, out MathTools.LineDummy);

			return closestPointOnEdge;
		}

		/// <summary>
		/// Given an input point, get the point on this line that is closest to it. Also return the normal vector at that point.
		/// </summary>
		/// <param name="point">Input point.</param>
		/// <param name="closestPointOnEdge">The point on this polygon that is closest to the input point.</param>
		/// <param name="closestNormal">The normal vector by the closest point.</param>
		public void ClosestEdge(Vector2 point, out Vector2 closestPointOnEdge, out Vector2 closestNormal)
		{
			InternalClosestEdge(point, out closestPointOnEdge, out closestNormal, out MathTools.LineDummy);
		}

		/// <summary>
		/// Given an input point, get the point on this line that is closest to it. Also return the whole line that the point was found on.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="closestPointOnEdge"></param>
		/// <param name="closestEdge"></param>
		public void ClosestEdge(Vector2 point, out Vector2 closestPointOnEdge, out Line closestEdge)
		{
			InternalClosestEdge(point, out closestPointOnEdge, out MathTools.Vector2Dummy, out closestEdge);
		}

		private void InternalClosestEdge(Vector2 point, out Vector2 closestPointOnEdge, out Vector2 closestNormal, out Line closestEdge)
		{
			closestPointOnEdge = Vector2.Zero;
			closestNormal = Vector2.Zero;
			closestEdge = Line.Zero;
			if (Count < 2)
				return;

			var shortestLength = float.MaxValue;
			for (var i = 1; i < Count; ++i)
			{
				var start = this[i - 1];
				var end = this[i];
				var dp = point;

				var delta = end - start;
				var length = delta.Length();

				var dir = (delta).Normalized(length);
				dp -= start;

				float r;
				Vector2.Dot(ref dp, ref dir, out r);

				// Clamp
				if (r < 0) r = 0;
				else if (r > length) r = length;

				var p = start + dir * r;

				length = (p - point).Length();
				if (length < shortestLength)
				{
					shortestLength = length;
					closestNormal = Normal(i - 1);
					closestPointOnEdge = p;
					closestEdge = new Line(start, end);
				}
			}
		}

		/// <summary>
		/// Test if this polygon contains the specified point.
		/// </summary>
		/// <param name="point">Point to test against.</param>
		/// <returns>True if the polygon contains the specified point.</returns>
		public bool Contains(Vector2 point)
		{
			// Fetched at http://alienryderflex.com/polygon
			
			var j = Count - 1;
			var oddNodes = false;

			for (var i = 0; i < Count; i++)
			{
				var polyI = this[i];
				var polyJ = this[j];

				if (polyI.Y < point.Y && polyJ.Y >= point.Y || polyJ.Y < point.Y && polyI.Y >= point.Y)
				{
					if (polyI.X + (point.Y - polyI.Y) / (polyJ.Y - polyI.Y) * (polyJ.X - polyI.X) < point.X)
					{
						oddNodes = !oddNodes;
					}
				}

				j = i;
			}

			return oddNodes;
		}

		/// <summary>
		/// Test if this polygon intersects with the specified line.
		/// </summary>
		/// <param name="other">Line to test against.</param>
		/// <returns>True if this polygon intersects with the specified line.</returns>
		public bool Intersects(Line other)
		{
			Vector2 dummy;
			return GetEdgeIntersectionPoint(other, out dummy, out dummy);
		}

		/// <summary>
		/// Test if this polygon intersects with the specified rectangle.
		/// </summary>
		/// <param name="other">Rectangle to test against.</param>
		/// <returns>True if this polygon intersects with the specified rectangle.</returns>
		public bool Intersects(RectangleF other)
		{
			Line l;
			Vector2 dummy;

			// Top
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X + other.Width, other.Y));
			if (GetEdgeIntersectionPoint(l, out dummy, out dummy)) return true;

			// Right
			l = new Line(new Vector2(other.X + other.Width, other.Y), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (GetEdgeIntersectionPoint(l, out dummy, out dummy)) return true;

			// Bottom
			l = new Line(new Vector2(other.X, other.Y + other.Height), new Vector2(other.X + other.Width, other.Y + other.Height));
			if (GetEdgeIntersectionPoint(l, out dummy, out dummy)) return true;

			// Left
			l = new Line(new Vector2(other.X, other.Y), new Vector2(other.X, other.Y + other.Height));
			if (GetEdgeIntersectionPoint(l, out dummy, out dummy)) return true;

			return false;
		}

		/// <summary>
		/// Test if this polygon either intersects with the specified rectangle, or if any of the rectangle's corners are within the polygon.
		/// </summary>
		/// <param name="other">Rectangle to test against.</param>
		/// <returns>True if there is intersection, or if the rectangle lies within the polygon.</returns>
		public bool IntersectsOrContains(RectangleF other)
		{
			// Early-out: Check bounding box for intersection
			if (!other.Intersects(Bounds))
				return false;
			
			// Check if any of the points of the poly is inside the rect
			foreach (var point in points)
			{
				if (other.Contains(point))
					return true;
			}

			// Check if any point of the rect is inside the poly, then finally check for edge collisions
			return (Contains(other.TopLeft) ||
			        Contains(other.TopRight) ||
			        Contains(other.BottomLeft) ||
			        Contains(other.BottomRight) ||
			        Intersects(other));
		}
		
		/// <summary>
		/// Get the intersection point between the edges and surface of this polygon and the
		/// specified line. Will return the point of intersection closest to the starting
		/// point of the line and the normal of the edge that is closest to the intersection
		/// point.
		/// </summary>
		/// <param name="other">Rectangle to test against.</param>
		/// <param name="intersectionPoint">Point of intersection.</param>
		/// <param name="normal">Normal vector at the point of intersection.</param>
		/// <returns>True if there was an intersection.</returns>
		public bool GetIntersectionPoint(Line other, out Vector2 intersectionPoint, out Vector2 normal)
		{
			// Check if we're starting out in the polygon
			if (Contains(other.Start))
			{
				intersectionPoint = other.Start;
				Vector2 dummy;
				ClosestEdge(other.Start, out dummy, out normal);
				return true;
			}

			// Okay, just check edges for intersection points
			return GetEdgeIntersectionPoint(other, out intersectionPoint, out normal);
		}

		/// <summary>
		/// Get the intersection point between any edge of this polygon and the
		/// specified line. If there are many intersections, the point closest to the start of 
		/// the line is returned.
		/// </summary>
		/// <param name="other">Line to test against.</param>
		/// <param name="intersectionPoint">Point of intersection.</param>
		/// <param name="normal">Normal vector at the point of intersection.</param>
		/// <returns>True if there was an intersection.</returns>
		public bool GetEdgeIntersectionPoint(Line other, out Vector2 intersectionPoint, out Vector2 normal)
		{
			intersectionPoint = Vector2.Zero;
			normal = Vector2.Zero;

			if (Count < 2)
				return false;
			
			var bestPoint = Vector2.Zero;
			var bestNormal = Vector2.Zero;
			var shortestLength = float.MaxValue;
			var hasPoint = false;

			for (var i = 1; i < Count; ++i)
			{
				var l = new Line(this[i - 1], this[i]);
				Vector2 p;
				if (l.GetIntersectionPoint(other, out p))
				{
					// Better than the last?
					length = (p - other.Start).Length();
					if (length < shortestLength)
					{
						shortestLength = length;
						bestPoint = p;
						bestNormal = normals[i - 1];
						hasPoint = true;
					}
				}
			}

			if (hasPoint)
			{
				intersectionPoint = bestPoint;
				normal = bestNormal;
				return true;
			}

			return false;
		}
		
		/// <summary>
		/// Return the string representation of this polygon.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string s = "{";
			for (int i = 0; i < Count; i++)
			{
				s += this[i];
				if (i < Count - 1)
				{
					s += " ";
				}
			}

			return s + "}";
		}

		/// <summary>
		/// Return the hash code of this polygon.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			var hash = 0;
			foreach (var v in this)
			{
				hash ^= v.GetHashCode();
			}
			return hash;
		}
		
		/// <summary>
		/// Return an enumerator for this polygon's points.
		/// </summary>
		/// <returns>Point enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Retrieve an enumerator for this polygon's points.
		/// </summary>
		public PolygonEnumerator GetEnumerator()
		{
			return new PolygonEnumerator(this);
		}

		IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator()
		{
			return new PolygonEnumerator(this);
		}

		/// <summary>
		/// Create a polygon shape.
		/// </summary>
		public static Polygon Create(RectangleF shape)
		{
			var edgePoints = new Vector2[5];
			edgePoints[0] = shape.TopLeft;
			edgePoints[1] = shape.TopRight;
			edgePoints[2] = shape.BottomRight;
			edgePoints[3] = shape.BottomLeft;
			edgePoints[4] = edgePoints[0];

			return new Polygon(edgePoints);
		}

		/// <summary>
		/// Polygon enumerator type.
		/// </summary>
		public struct PolygonEnumerator : IEnumerator<Vector2>
		{
		    public Polygon polygon;

		    // Enumerators are positioned before the first element
		    // until the first MoveNext() call.
		    private int position;

			public PolygonEnumerator(Polygon polygon)
		    {
				this.polygon = polygon;
		        position = -1;
		    }

		    public Vector2 Current
		    {
		        get { return polygon[position]; }
		    }
		
		    public bool MoveNext()
		    {
		        position++;
				return (position < polygon.Count);
		    }

		    public void Reset()
		    {
		        position = -1;
		    }

		    public void Dispose()
		    {
		        polygon = null;
		    }

		    object IEnumerator.Current
		    {
				get { return polygon[position]; }
		    }

			Vector2 IEnumerator<Vector2>.Current
		    {
				get { return polygon[position]; }
		    }
		}

		private void CalculateNormals()
		{
			if (points.Count < 2)
				return;

			normals.Clear();
			for (var i = 1; i < points.Count; ++i)
			{
				var normal = (points[i] - points[i - 1]).SafeNormalized().GetPerpendicular();
				normals.Add(normal);
			}
		}

		private float CalculateLength()
		{
			if (points.Count < 2)
				return 0;

			float totalLength = 0;
			var lastPoint = points[0];
			for (var i = 1; i < points.Count; ++i)
			{
				var p = points[i];
				totalLength += (p - lastPoint).Length();
				lastPoint = p;
			}

			return totalLength;
		}

		private RectangleF CalculateBounds(float rotation)
		{
			// Note: does not take offset into account

			if (points.Count == 0)
			{
				return RectangleF.Zero;
			}
			if (points.Count == 1)
			{
				return new RectangleF(points[0].X, points[0].Y, 0, 0);
			}

			var topLeft = new Vector2(float.MaxValue, float.MaxValue);
			var bottomRight = new Vector2(float.MinValue, float.MinValue);

			if (rotation == 0)
			{
				foreach (var p in points)
				{
					if (p.X < topLeft.X)
					{
						topLeft.X = p.X;
					}
					if (p.X > bottomRight.X)
					{
						bottomRight.X = p.X;
					}
					if (p.Y < topLeft.Y)
					{
						topLeft.Y = p.Y;
					}
					if (p.Y > bottomRight.Y)
					{
						bottomRight.Y = p.Y;
					}
				}
			}
			else
			{
				foreach (var po in points)
				{
					var p = po.Rotated(rotation);
					if (p.X < topLeft.X)
					{
						topLeft.X = p.X;
					}
					if (p.X > bottomRight.X)
					{
						bottomRight.X = p.X;
					}
					if (p.Y < topLeft.Y)
					{
						topLeft.Y = p.Y;
					}
					if (p.Y > bottomRight.Y)
					{
						bottomRight.Y = p.Y;
					}
				}
			}

			return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
		}
	}
}
