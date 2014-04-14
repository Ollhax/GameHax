using System;
using System.ComponentModel;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A floating point rectangle.
	/// </summary>
	[TypeConverter(typeof(RectangleFConverter))]
	[Serializable]
	public struct RectangleF
	{
		/// <summary>
		/// X position.
		/// </summary>
		public float X;

		/// <summary>
		/// Y position.
		/// </summary>
		public float Y;

		/// <summary>
		/// Height of the rectangle.
		/// </summary>
		public float Height;

		/// <summary>
		/// Width of the rectangle.
		/// </summary>
		public float Width;

		/// <summary>
		/// Return a rectangle centered on origin with zero size.
		/// </summary>
		public static RectangleF Zero { get { return new RectangleF(0, 0, 0, 0); } }
		
		/// <summary>
		/// Return a rectangle centered on origin with zero size.
		/// </summary>
		public static RectangleF Empty { get { return new RectangleF(0, 0, 0, 0); } }

		/// <summary>
		/// Construct a rectangle of the specified dimensions.
		/// </summary>
		/// <param name="x">X position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public RectangleF(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Creates a copy of the specified rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to copy.</param>
		public RectangleF(Rectangle rectangle)
		{
			X = rectangle.X;
			Y = rectangle.Y;
			Width = rectangle.Width;
			Height = rectangle.Height;
		}

		/// <summary>
		/// Creates a copy of the specified rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to copy.</param>
		public RectangleF(RectangleF rectangle)
		{
			X = rectangle.X;
			Y = rectangle.Y;
			Width = rectangle.Width;
			Height = rectangle.Height;
		}

		/// <summary>
		/// Construct the rectangle that spans between two vectors.
		/// </summary>
		/// <param name="vector1">First vector.</param>
		/// <param name="vector2">Second vector.</param>
		/// <returns>A rectangle that spans between the two input vectors.</returns>
		public static RectangleF ConstructSpanning(Vector2 vector1, Vector2 vector2)
		{
			var r = new RectangleF();
			r.X = Math.Min(vector1.X, vector2.X);
			r.Y = Math.Min(vector1.Y, vector2.Y);
			r.Width = Math.Abs(Math.Max(vector1.X, vector2.X) - r.X);
			r.Height = Math.Abs(Math.Max(vector1.Y, vector2.Y) - r.Y);

			return r;
		}

		/// <summary>
		/// Convert a rectangle to the target type.
		/// </summary>
		/// <param name="r">Source rectangle.</param>
		/// <returns>The converted rectangle.</returns>
		public static explicit operator Rectangle(RectangleF r)
		{
			return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
		}

		/// <summary>
		/// Convert the rectangle to string format.
		/// </summary>
		/// <returns>String representation for this rectangle.</returns>
		public override string ToString()
		{
			return "{X=" + TypeConvert.ToString(X) +
				", Y=" + TypeConvert.ToString(Y) +
				", W=" + TypeConvert.ToString(Width) +
				", H=" + TypeConvert.ToString(Height) + "}";
		}

		/// <summary>
		/// Get or set the position of this rectangle.
		/// </summary>
		public Vector2 Position
		{
			get
			{
				return new Vector2(X, Y);
			}

			set 
			{ 
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the top left position of this rectangle.
		/// </summary>
		public Vector2 TopLeft
		{
			get { return new Vector2(X, Y); }
			set 
			{ 
				Width += X - value.X;
				Height += Y - value.Y;
				X = value.X;
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the top right position of this rectangle.
		/// </summary>
		public Vector2 TopRight
		{
			get { return new Vector2(X + Width, Y); }
			set
			{
				Width = value.X - X;
				Height += Y - value.Y;				
				Y = value.Y;
			}
		}

		/// <summary>
		/// Get or set the bottom left position of this rectangle.
		/// </summary>
		public Vector2 BottomLeft
		{
			get { return new Vector2(X, Y + Height); }
			set
			{
				Height = value.Y - Y;
				Width += X - value.X;
				X = value.X;
			}
		}

		/// <summary>
		/// Get or set the bottom right position of this rectangle.
		/// </summary>
		public Vector2 BottomRight
		{
			get { return new Vector2(X + Width, Y + Height); }
			set
			{
				Width = value.X - X;
				Height = value.Y - Y;			
			}
		}

		/// <summary>
		/// Get or set the center position of this rectangle.
		/// </summary>
		public Vector2 Center
		{
			get { return new Vector2(X + Width/2, Y + Height/2); }
			set
			{
				X = value.X - Width / 2;
				Y = value.Y - Height / 2;				
			}
		}

		/// <summary>
		/// Get the center left position of this rectangle.
		/// </summary>
		public Vector2 CenterLeft
		{
			get { return new Vector2(X, Y + Height / 2); }
			//set
			//{
			//    X = value.X;
			//    Y = value.Y - Height / 2;
			//}
		}

		/// <summary>
		/// Get the center right position of this rectangle.
		/// </summary>
		public Vector2 CenterRight
		{
			get { return new Vector2(X + Width, Y + Height / 2); }
			//set
			//{
			//    X = value.X - Width;
			//    Y = value.Y - Height / 2;
			//}
		}

		/// <summary>
		/// Get the center top position of this rectangle.
		/// </summary>
		public Vector2 CenterTop
		{
			get { return new Vector2(X + Width / 2, Y); }
			//set
			//{
			//    X = value.X - Width / 2;
			//    Y = value.Y;
			//}
		}

		/// <summary>
		/// Get the center bottom position of this rectangle.
		/// </summary>
		public Vector2 CenterBottom
		{
			get { return new Vector2(X + Width / 2, Y + Height); }
			//set
			//{
			//    X = value.X - Width / 2;
			//    Y = value.Y - Height;
			//}
		}

		/// <summary>
		/// Get or set the left side of this rectangle.
		/// </summary>
		public float Left
		{
			get { return X; }
			set
			{
				Width += X - value;
				X = value;
			}
		}

		/// <summary>
		/// Get or set the right side of this rectangle.
		/// </summary>
		public float Right
		{
			get { return X + Width; }
			set
			{
				Width = value - X;				
			}
		}

		/// <summary>
		/// Get or set the top of this rectangle.
		/// </summary>
		public float Top
		{
			get { return Y; }
			set
			{
				Height += Y - value;
				Y = value;
			}
		}

		/// <summary>
		/// Get or set the bottom of this rectangle.
		/// </summary>
		public float Bottom
		{
			get { return Y + Height; }
			set
			{
				Height = value - Y;
			}
		}

		/// <summary>
		/// Test if this rectangle is empty.
		/// </summary>
		/// <returns>True if the rectangle is empty.</returns>
		public bool IsEmpty { get { return X == 0 && Y == 0 && Width == 0 && Height == 0; } }

		/// <summary>
		/// Get or set the size of this rectangle.
		/// </summary>
		public Vector2 Size
		{
			get
			{
				return new Vector2(Width, Height);
			}

			set 
			{ 
				Width = value.X;
				Height = value.Y;
			}
		}
		
		/// <summary>
		/// Move this rectangle by the specified offset.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public void Offset(float x, float y)
		{
			X += x;
			Y += y;
		}

		/// <summary>
		/// Move this rectangle by the specified offset.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		public void Offset(Vector2 offset)
		{
			X += offset.X;
			Y += offset.Y;
		}

		/// <summary>
		/// Return a copy of this rectangle that is offsetted 
		/// by the specified amount.
		/// </summary>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		/// <returns>An offsetted copy.</returns>
		public RectangleF Offseted(float x, float y)
		{
			var copy = new RectangleF(this);
			copy.X += x;
			copy.Y += y;
			return copy;
		}

		/// <summary>
		/// Return a copy of this rectangle that is offsetted 
		/// by the specified amount.
		/// </summary>
		/// <param name="offset">Offset vector.</param>
		/// <returns>An offsetted copy.</returns>
		public RectangleF Offseted(Vector2 offset)
		{
			var copy = new RectangleF(this);
			copy.X += offset.X;
			copy.Y += offset.Y;
			return copy;
		}

		/// <summary>
		/// Returns a copy of this rectangle at the specified location.
		/// </summary>
		/// <param name="position">Input location.</param>
		/// <returns>A moved rectangle.</returns>
		public RectangleF AtPosition(Vector2 position)
		{
			var copy = new RectangleF(this);
			copy.X = position.X;
			copy.Y = position.Y;
			return copy;
		}
		
		/// <summary>
		/// Return a copy of this rectangle that is united with the
		/// specified rectangle.
		/// </summary>
		/// <param name="other">Rectangle to unite with.</param>
		/// <returns>The rectangle that unites both rectangles.</returns>
		public RectangleF Unioned(RectangleF other)
		{
			var minX = X < other.X ? X : other.X;
			var minY = Y < other.Y ? Y : other.Y;
			return new RectangleF(minX, minY,
				Right > other.Right ? Right - minX : other.Right - minX,
				Bottom > other.Bottom ? Bottom - minY: other.Bottom - minY);		
		}
		
		/// <summary>
		/// Expand this rectangle in all directions by the specified distance. Negative distances are ok!
		/// </summary>
		public void Inflate(float distance)
		{
			X -= distance;
			Y -= distance;
			Width += distance * 2;
			Height += distance * 2;
		}

		/// <summary>
		/// Expand this rectangle by the specified distance vector. Negative distances are ok!
		/// </summary>
		public void Inflate(Vector2 distance)
		{
			X -= distance.X;
			Y -= distance.Y;
			Width += distance.X * 2;
			Height += distance.Y * 2;
		}

		/// <summary>
		/// Expand this rectangle in all directions by the specified distance. Negative distances are ok!
		/// </summary>
		public RectangleF Inflated(float distance)
		{
			return new RectangleF(X - distance, Y - distance, Width + distance * 2, Height + distance * 2);			
		}

		/// <summary>
		/// Expand this rectangle by the specified distance vector. Negative distances are ok!
		/// </summary>
		public RectangleF Inflated(Vector2 distance)
		{
			return new RectangleF(X - distance.X, Y - distance.Y, Width + distance.X * 2, Height + distance.Y * 2);
		}

		/// <summary>
		/// Return the intersecting area between this rectangle and the specified one.
		/// </summary>
		/// <param name="r">Rectangle to test intersection against.</param>
		/// <returns>An area containing the intersecting area. If the two areas do not intersect, an empty area will be returned.</returns>
		public RectangleF Intersection(RectangleF r)
		{
			// Compute the intersection boundaries
			float left = Math.Max(Left, r.Left);
			float top = Math.Max(Top, r.Top);
			float right = Math.Min(Right, r.Right);
			float bottom = Math.Min(Bottom, r.Bottom);

			// If the intersection is valid (positive non zero area), then there is an intersection
			if ((left < right) && (top < bottom))
			{
				return new RectangleF(left, top, right - left, bottom - top);
			}

			return Zero;
		}
		
		/// <summary>
		/// Test if this rectangle equals the specified value.
		/// </summary>
		/// <param name="obj">Object to test against.</param>
		/// <returns>True if the two values are equal.</returns>
		public override bool Equals(Object obj)
		{
			var result = false;
			
			if (obj is RectangleF)
			{
				var other = (RectangleF)obj;
				if (other != null && (other.X == X && other.Y == Y && other.Width == Width && other.Height == Height))
				{
					result = true;
				}
			}
			
			return result;
		}

		/// <summary>
		/// Test if this rectangle equals the specified one.
		/// </summary>
		/// <param name="other">Rectangle to test against.</param>
		/// <returns>True if the rectangles are equal.</returns>
		public bool Equals(RectangleF other)
		{
			return (other.X == X && other.Y == Y && other.Width == Width && other.Height == Height);
		}

		/// <summary>
		/// Test if two rectangles are equal.
		/// </summary>
		/// <param name="a">First rectangle.</param>
		/// <param name="b">Second rectangle.</param>
		/// <returns>True if the two rectangles are equal.</returns>
		public static bool operator ==(RectangleF v1, RectangleF v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Width == v2.Width && v1.Height == v2.Height;
		}

		/// <summary>
		/// Test if two rectangles are not equal.
		/// </summary>
		/// <param name="a">First rectangle.</param>
		/// <param name="b">Second rectangle.</param>
		/// <returns>True if the two rectangles are not equal.</returns>
		public static bool operator !=(RectangleF v1, RectangleF v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y || v1.Width != v2.Width || v1.Height != v2.Height;
		}
		
		/// <summary>
		/// Get the hash code for this rectangle.
		/// </summary>
		/// <returns>A hash code for this rectangle.</returns>
		public override int GetHashCode()
		{
			int hash = 37;
			hash = (hash * 397) ^ X.GetHashCode();
			hash = (hash * 397) ^ Y.GetHashCode();
			hash = (hash * 397) ^ Width.GetHashCode();
			hash = (hash * 397) ^ Height.GetHashCode();
			return hash;
		}

		/// <summary>
		/// Test if the specified shape is fully within this rectangle.
		/// </summary>
		/// <param name="other">Shape to test.</param>
		/// <returns>True if the specified shape is fully within this rectangle.</returns>
		public bool Contains(Vector2 other)
		{
			return 
				!(X > other.X ||
				Y > other.Y ||
				X + Width <= other.X ||
				Y + Height <= other.Y);
		}

		/// <summary>
		/// Test if the specified shape is fully within this rectangle.
		/// </summary>
		/// <param name="x">X position if input vector.</param>
		/// <param name="y">Y position if input vector.</param>
		/// <returns>True if the specified shape is fully within this rectangle.</returns>
		public bool Contains(float x, float y)
		{
			return
				!(X > x ||
				Y > y ||
				X + Width <= x ||
				Y + Height <= y);
		}

		/// <summary>
		/// Test if the specified shape is fully within this rectangle.
		/// </summary>
		/// <param name="other">Shape to test.</param>
		/// <returns>True if the specified shape is fully within this rectangle.</returns>
		public bool Contains(RectangleF other)
		{
			return (other.X >= X &&
			       	other.Y >= Y &&
			       	other.X + other.Width <= X + Width &&
			       	other.Y + other.Height <= Y + Height);
		}

		/// <summary>
		/// Test if the specified shape intersects this rectangle.
		/// </summary>
		/// <param name="other">Shape to test.</param>
		/// <returns>True if the specified shape intersects this rectangle.</returns>
		public bool Intersects(RectangleF other)
		{
			return
				!(X >= other.X + other.Width ||
				Y >= other.Y + other.Height ||
				X + Width <= other.X ||
				Y + Height <= other.Y);
		}

		/// <summary>
		/// Get the point on the edge of this rectangle that is closest to the input point.
		/// </summary>
		/// <param name="point">Input point.</param>
		/// <returns>The point on the edge of this rectangle that is closest to the input point.</returns>
		public unsafe Vector2 ClosestPointOnEdge(Vector2 point)
		{
			var edges = stackalloc Line[4];
			edges[0] = new Line(new Vector2(X, Y), new Vector2(X + Width, Y)); // top
			edges[1] = new Line(new Vector2(X + Width, Y), new Vector2(X + Width, Y + Height)); // right
			edges[2] = new Line(new Vector2(X, Y + Height), new Vector2(X + Width, Y + Height)); // bottom
			edges[3] = new Line(new Vector2(X, Y), new Vector2(X, Y + Height)); // left

			var closestDistance = float.MaxValue;
			var bestPoint = Vector2.Zero;
			for (int i = 0; i < 4; i++)
			{
				var closest = edges[i].ClosestPointOnLine(point);
				var length = (closest - point).Length();
				if (length < closestDistance)
				{
					bestPoint = closest;
					closestDistance = length;
				}
			}

			return bestPoint;
		}

		/// <summary>
		/// Calculates how much this rectangle is penetrating the target rectangle.
		/// Will return the length and direction of the penetration of the least
		/// penetrating axis.
		/// </summary>
		/// <param name="other">Calculate overlapse against this rectangle.</param>
		/// <returns>Shortest overlapse distance.</returns>
		public Vector2 ShortestOverlapse(RectangleF other)
		{
			float d0, d1, d2, d3;

			d0 = (X + Width) - other.X;
			if (d0 <= 0) return Vector2.Zero;

			d1 = (Y + Height) - other.Y;
			if (d1 <= 0) return Vector2.Zero;

			d2 = (other.X + other.Width) - X;
			if (d2 <= 0) return Vector2.Zero;

			d3 = (other.Y + other.Height) - Y;
			if (d3 <= 0) return Vector2.Zero;

			float minDistance = d0;
			Vector2 distance = new Vector2(d0, 0);
							
			if (d1 < minDistance)
			{
				minDistance = d1;
				distance.X = 0;
				distance.Y = d1;
			}

			if (d2 < minDistance)
			{
				minDistance = d2;
				distance.X = -d2;
				distance.Y = 0;
			}

			if (d3 < minDistance)
			{
				distance.X = 0;
				distance.Y = -d3;
			}

			return distance;
		}
		
		/// <summary>
		/// Try to sweep move this shape from its current position to the new one,
		/// colliding against the target shape.
		/// </summary>
		/// <param name="newPosition">Try to move to this position.</param>
		/// <param name="other">Collide against this shape.</param>
		/// <param name="normal">Normal of collision plane. Can be zero length if for example colliding at start!</param>
		/// <param name="intersectionPoint">Point of intersection.</param>
		/// <returns>True if we could move all the way, false if we got stuck (i.e. collided against the other shape)</returns>		
		public unsafe bool SweepMove(Vector2 newPosition, Line other, out Vector2 normal, out Vector2 intersectionPoint)
		{
			normal = Vector2.Zero;
			intersectionPoint = Vector2.Zero;

			// Check if we're colliding at start
			if (other.Intersects(this))
			{
				return false;
			}

			// Early out-test
			var newBox = new RectangleF(newPosition.X, newPosition.Y, Width, Height);
			var largeBox = Unioned(new RectangleF(newBox));
			if (!other.Intersects(largeBox) && !largeBox.Contains(other.Start) && !largeBox.Contains(other.End))
			{
				X = newPosition.X;
				Y = newPosition.Y;
				return true;
			}

			// The collision detection goes as follows:
			// Draw lines from all four corners of this rectangle to
			// the destination. If one or more lines intersect, select
			// the shortest one of them.
			//
			// Then, draw two lines from the edges of the line in
			// question with the inverse offset and check those lines
			// against the edges of the rectangle. If any of them
			// intersects, again choose the one with the closest
			// intersection point.
			//
			// Finally, choose the shortest intersection line of all
			// tests. This represents the distance we were able to move.
			
			// Preparations
			var corners = stackalloc Vector2[4];
			corners[0] = TopLeft;
			corners[1] = TopRight;
			corners[2] = BottomLeft;
			corners[3] = BottomRight;

			var edges = stackalloc Line[4];
			edges[0] = new Line(new Vector2(X, Y), new Vector2(X + Width, Y)); // top
			edges[1] = new Line(new Vector2(X + Width, Y), new Vector2(X + Width, Y + Height)); // right
			edges[2] = new Line(new Vector2(X, Y + Height), new Vector2(X + Width, Y + Height)); // bottom
			edges[3] = new Line(new Vector2(X, Y), new Vector2(X, Y + Height)); // left

			var normals = stackalloc Vector2[4];
			normals[0] = new Vector2(0, 1);
			normals[1] = new Vector2(-1, 0);
			normals[2] = new Vector2(0, -1);
			normals[3] = new Vector2(1, 0);
			
			var delta = new Vector2(newPosition.X - X, newPosition.Y - Y);
			var deltaLength = delta.Length();
			var dir = delta.SafeNormalized(deltaLength);
			var shortestDistanceEdge = deltaLength;
			var intersectionPointEdge = Vector2.Zero;
			var shortestDistanceLine = shortestDistanceEdge;
			var shortestDistanceLineNormal = Vector2.Zero;
			var intersectionPointLine = Vector2.Zero;
			Vector2 ip;
			var hasCollision = false;
			
			// Check edges against line
			for (int i = 0; i < 4; i++)
			{
				var corner = corners[i];
				if (new Line(corner, corner + delta).GetIntersectionPoint(other, out ip))
				{
					var d = (corner - ip).Length();
					if (d < shortestDistanceEdge)
					{
						intersectionPointEdge = ip;
						shortestDistanceEdge = d;
						hasCollision = true;
					}
				}
			}
			
			// Check line against edges
			for (var i = 0; i < 4; i++)
			{
				var edge = edges[i];
				
				if (new Line(other.Start, other.Start - delta).GetIntersectionPoint(edge, out ip))
				{
					var d = (other.Start - ip).Length();
					if (d < shortestDistanceLine)
					{
						intersectionPointLine = ip;
						shortestDistanceLine = d;
						shortestDistanceLineNormal = normals[i];
						hasCollision = true;
					}
				}

				if (new Line(other.End, other.End - delta).GetIntersectionPoint(edge, out ip))
				{
					var d = (other.End - ip).Length();
					if (d < shortestDistanceLine)
					{
						intersectionPointLine = ip;
						shortestDistanceLine = d;
						shortestDistanceLineNormal = normals[i];
						hasCollision = true;
					}
				}
			}

			// Did we collide at all?
			if (!hasCollision)
			{
				X = newPosition.X;
				Y = newPosition.Y;
				return true;
			}

			// Determine which normal we should use
			if (shortestDistanceEdge < shortestDistanceLine)
			{
				intersectionPoint = intersectionPointEdge;
				normal = other.GetNormalFacingPoint(Center);
				X += dir.X * shortestDistanceEdge;
				Y += dir.Y * shortestDistanceEdge;
			}
			else
			{
				intersectionPoint = intersectionPointLine;
				normal = shortestDistanceLineNormal;
				X += dir.X * shortestDistanceLine;
				Y += dir.Y * shortestDistanceLine;
			}

			return false;
		}
		
		/// <summary>
		/// Try to sweep move this shape from its current position to the new one,
		/// colliding against the target shape.
		/// </summary>
		/// <param name="newPosition">Try to move to this position.</param>
		/// <param name="other">Collide against this shape.</param>
		/// <returns>True if we could move all the way, false if we got stuck (i.e. collided against the other shape)</returns>		
		public bool SweepMove(Vector2 newPosition, RectangleF other)
		{
			return SweepMove(newPosition, other, out MathTools.Vector2Dummy);
		}
					
		/// <summary>
		/// Try to sweep move this shape from its current position to the new one,
		/// colliding against the target shape.
		/// </summary>
		/// <param name="newPosition">Try to move to this position.</param>
		/// <param name="other">Collide against this shape.</param>
		/// <param name="normal">Normal of collision plane. Can be zero  length if for example colliding at start!</param>
		/// <returns>True if we could move all the way, false if we got stuck (i.e. collided against the other shape)</returns>		
		public bool SweepMove(Vector2 newPosition, RectangleF other, out Vector2 normal)
		{
			// Check http://www.harveycartel.org/metanet/tutorials/tutorialA.html
			// for a good explaination of SAT.

			// Also see the Pollycoly tutorials (search on gamedev.net).

			normal = Vector2.Zero;

			// Calculate overlapse in value and y axis
			float x = CalculateOverlapse(new Vector2(1.0f, 0.0f), this, other, newPosition);
			float y = CalculateOverlapse(new Vector2(0.0f, 1.0f), this, other, newPosition);

			// Colliding at start
			if (x > 0 && y > 0)
			{
				return false;
			}

			// No overlapse on the way to the target
			if ((MathTools.Equals(x, -1.0f)) || (MathTools.Equals(y, -1.0f)))
			{
				Position = newPosition;
				return true;
			}

			// Colliding after some distance
			float d = Math.Min(x, y);
			Vector2 pos = Position;
			Vector2 dir = newPosition - pos;			
			float length = dir.Length();
			d = length * -d;			
			
			Position = (pos + dir.Normalized(length) * d);
						
			// Calculate normal vector
			if (length > 0)
			{
				if (x < y)
				{
					normal = new Vector2(dir.X < 0 ? 1.0f : -1.0f, 0.0f);
				}
				else if (y < x)
				{
					normal = new Vector2(0.0f, dir.Y < 0 ? 1.0f : -1.0f);
				}
				else
				{
					normal = new Vector2(dir.X < 0 ? 1.0f : -1.0f, dir.Y < 0 ? 1.0f : -1.0f);
					normal.Normalize();
				}
			}
			
			return false;
		}

		/// <summary>
		/// Returns the amount of overlapse in the specified axis between the two specified rectangles.
		/// If no initial overlapse, calculates the fraction of the distance r1 can move towards 
		/// newPosition before it hits r2 in the specified axis. This is returned as a negative value
		/// with the range [-0, -1].
		/// </summary>
		/// <param name="axis">Axis to test against.</param>
		/// <param name="r1">The first shape.</param>
		/// <param name="r2">The second shape.</param>
		/// <param name="newPosition">The first shape's new position.</param>
		/// <returns>Positive values = overlapse, negative values = movement fraction (see summary).</returns>
		private static float CalculateOverlapse(Vector2 axis, RectangleF r1, RectangleF r2, Vector2 newPosition)
		{
			// Retrieve all rectangle vectors
			Vector2 vr1s = new Vector2(r1.X, r1.Y);
			Vector2 vr1e = new Vector2(r1.X + r1.Width, r1.Y + r1.Height);
			Vector2 vr2s = new Vector2(r2.X, r2.Y);
			Vector2 vr2e = new Vector2(r2.X + r2.Width, r2.Y + r2.Height);

			// Project rectangle vectors to axis
			float r1s, r1e, r2s, r2e;

			Vector2.Dot(ref vr1s, ref axis, out r1s);
			Vector2.Dot(ref vr1e, ref axis, out r1e);
			Vector2.Dot(ref vr2s, ref axis, out r2s);
			Vector2.Dot(ref vr2e, ref axis, out r2e);

			//if (r1s > r1e) MathTools.Swap<float>(ref r1s, ref r1e);
			//if (r2s > r2e) MathTools.Swap<float>(ref r2s, ref r2e);
			if (r1s > r1e) throw new Exception("invalid rect");
			if (r2s > r2e) throw new Exception("invalid rect");

			// Check for intersection
			float p;
			float maxVal = Math.Min(r1e - r1s, r2e - r2s);
			if (r1s < r2s)
			{
				p = r1e - r2s;
				if (p < 0) p = 0; if (p > maxVal) p = maxVal;				
			}
			else
			{
				p = r2e - r1s;
				if (p < 0) p = 0; if (p > maxVal) p = maxVal;
			}

			// No intersection - determine when intersection will occur
			if (p == 0)
			{
				p = -1.0f;
				Vector2 vr1sp = newPosition;
				Vector2 vr1ep = newPosition + new Vector2(r1.Width, r1.Height);
				float r1sp, r1ep;

				Vector2.Dot(ref vr1sp, ref axis, out r1sp);
				Vector2.Dot(ref vr1ep, ref axis, out r1ep);

				//if (r1sp > r1ep) MathTools.Swap<float>(ref r1sp, ref r1ep);
				if (r1sp > r1ep) throw new Exception("invalid rect");

				if (!MathTools.Equals(r1s, r1sp))
				{
					if (r1s < r2s && r1ep > r2s)
					{
						// Calculate the distance until r1's end hits r2's start, divided by the distance r1 wants to go
						p = -(r2s - r1e) / Math.Abs(r1s - r1sp);
					}
					else if (r1s >= r2s && r1sp < r2e)
					{
						p = (r2e - r1s) / Math.Abs(r1s - r1sp);
					}
				}
			}

			return p;
		}

		/// <summary>
		/// Align another item to this rectangle according to the specified alignment.
		/// </summary>
		/// <param name="alignment">Alignment method.</param>
		/// <param name="otherItemSize">The size of the item to align.</param>
		/// <returns>Position of the other item after it has been aligned.</returns>
		public Vector2 GetAlignmentOffset(AlignmentType alignment, Vector2 otherItemSize)
		{			
			switch (alignment)
			{
				case AlignmentType.Center:
					return new Vector2(X + (Width - otherItemSize.X) / 2, Y + (Height - otherItemSize.Y) / 2);
				case AlignmentType.North:
					return new Vector2(X + (Width - otherItemSize.X) / 2, Y);
				case AlignmentType.NorthEast:
					return new Vector2(X + Width - otherItemSize.X, Y);
				case AlignmentType.East:
					return new Vector2(X + Width - otherItemSize.X, Y + (Height - otherItemSize.Y) / 2);
				case AlignmentType.SouthEast:
					return new Vector2(X + Width - otherItemSize.X, Y + Height - otherItemSize.Y);
				case AlignmentType.South:
					return new Vector2(X + (Width - otherItemSize.X) / 2, Y + Height - otherItemSize.Y);
				case AlignmentType.SouthWest:
					return new Vector2(X, Y + Height - otherItemSize.Y);
				case AlignmentType.West:
					return new Vector2(X, Y + (Height - otherItemSize.Y) / 2);
				case AlignmentType.NorthWest:
					return new Vector2(X, Y);
			}

			return Vector2.Zero;
		}
		
		/// <summary>
		/// Determine the Voronoi region index of the specified point relative to the this rectangle.
		/// </summary>
		/// <remarks>
		/// If the point is located on an edge, preference is given to the "side" regions (1, 3, 5, 7).
		/// </remarks>
		/// <param name="point">The tested point.</param>
		/// <returns>Voronoi region, ranging 0-8. 0 is top left, 4 is center and 8 is bottom right.</returns>
		public int GetRegion(Vector2 point)
		{
			if (point.X < X)
			{
				// 0, 3 or 6
				if (point.Y < Y)
				{
					return 0;
				}
				else if (point.Y > Y + Height)
				{
					return 6;
				}
				return 3;
			}
			else if (point.X > X + Width)
			{
				// 2, 5 or 8
				if (point.Y < Y)
				{
					return 2;
				}
				else if (point.Y > Y + Height)
				{
					return 8;
				}
				return 5;
			}
			else
			{
				// 1, 4 or 7
				if (point.Y < Y)
				{
					return 1;
				}
				else if (point.Y > Y + Height)
				{
					return 7;
				}
				return 4;
			}
		}
	}	
}
