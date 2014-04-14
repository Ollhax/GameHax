using System;
using System.ComponentModel;
using System.Globalization;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A generic relative position vector. It's basically a vector with an origin that depends on a parent object's size. 
	/// You specify the relative position, usually in the range 0-1 (but it can exceed this range), and may also set
	/// an offset to this.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(RelativePosition2Converter))]
	public struct RelativePosition2 : IEquatable<RelativePosition2>
	{
		private static readonly RelativePosition2 zero;
		private static readonly RelativePosition2 center;

		/// <summary>
		/// X Offset value for this position.
		/// </summary>
		public float OffsetX;

		/// <summary>
		/// Y Offset value for this position.
		/// </summary>
		public float OffsetY;

		/// <summary>
		/// X Relative value for this position. This is a value relative to some parent size and will usually span between [0.0-1.0].
		/// </summary>
		public float RelativeX;

		/// <summary>
		/// Y Relative value for this position. This is a value relative to some parent size and will usually span between [0.0-1.0].
		/// </summary>
		public float RelativeY;

		/// <summary>
		/// Return the zero position.
		/// </summary>
		public static RelativePosition2 Zero { get { return zero; } }

		/// <summary>
		/// Return the center position.
		/// </summary>
		public static RelativePosition2 Center { get { return center; } }

		static RelativePosition2()
		{
			zero = new RelativePosition2(0, 0, 0, 0);
			center = new RelativePosition2(0, 0, 0.5f, 0.5f);
		}

		/// <summary>
		/// Create the position structure.
		/// </summary>
		/// <param name="offsetX">X offset, usually measured in pixels.</param>
		/// <param name="offsetY">Y offset, usually measured in pixels.</param>
		/// <param name="relativeX">Relative X position, usually in the range [0-1].</param>
		/// <param name="relativeY">Relative Y position, usually in the range [0-1].</param>
		public RelativePosition2(float offsetX, float offsetY, float relativeX, float relativeY)
		{
			OffsetX = offsetX;
			OffsetY = offsetY;
			RelativeX = relativeX;
			RelativeY = relativeY;
		}

		/// <summary>
		/// Test if the specified position equals this one.
		/// </summary>
		/// <param name="other">Position to test against.</param>
		/// <returns>True if we equal the specified position.</returns>
		public bool Equals(RelativePosition2 other)
		{
			return OffsetX == other.OffsetX && 
				OffsetY == other.OffsetY &&
				RelativeX == other.RelativeX &&
				RelativeY == other.RelativeY;
		}

		/// <summary>
		/// Given the specified parent size, return the absolute position.
		/// </summary>
		/// <param name="parentSize">Size of the parent.</param>
		/// <returns>An absolute position.</returns>
		public Vector2 Expand(Vector2 parentSize)
		{
			return new Vector2(parentSize.X * RelativeX + OffsetX, parentSize.Y * RelativeY + OffsetY);
		}

		/// <summary>
		/// Return a string representation of this position.
		/// </summary>
		/// <returns>A string representation of this position.</returns>
		public override string ToString()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return string.Format(
				currentCulture,
				"{{X:{0} Y:{1} RX:{2} RY:{3} }}",
				new object[]
					{
						OffsetX.ToString(currentCulture), OffsetY.ToString(currentCulture), RelativeX.ToString(currentCulture),
						RelativeY.ToString(currentCulture)
					});
		}

		/// <summary>
		/// Test if the specified object is equal to this position.
		/// </summary>
		/// <param name="obj">Specified object.</param>
		/// <returns>True if the object is equal to this position.</returns>
		public override bool Equals(Object obj)
		{
			return (obj is RelativePosition2) && Equals((RelativePosition2)obj);
		}

		/// <summary>
		/// Get the hash code for this position.
		/// </summary>
		/// <returns>Hash code for this position.</returns>
		public override int GetHashCode()
		{
			return OffsetX.GetHashCode() + OffsetY.GetHashCode() + RelativeX.GetHashCode() + RelativeY.GetHashCode();
		}

		/// <summary>
		/// Test if the two positions are equal.
		/// </summary>
		/// <param name="value1">First position.</param>
		/// <param name="value2">Second position.</param>
		/// <returns>True if the two positions are equal.</returns>
		public static bool operator ==(RelativePosition2 value1, RelativePosition2 value2)
		{
			return value1.OffsetX.Equals(value2.OffsetX) && value1.OffsetY.Equals(value2.OffsetY)
			       && value1.RelativeX.Equals(value2.RelativeX) && value1.RelativeY.Equals(value2.RelativeY);
		}

		/// <summary>
		/// Test if the two positions are not equal.
		/// </summary>
		/// <param name="value1">First position.</param>
		/// <param name="value2">Second position.</param>
		/// <returns>True if the two positions are not equal.</returns>
		public static bool operator !=(RelativePosition2 value1, RelativePosition2 value2)
		{
			return !value1.OffsetX.Equals(value2.OffsetX) || !value1.OffsetY.Equals(value2.OffsetY)
			       || !value1.RelativeX.Equals(value2.RelativeX) || !value1.RelativeY.Equals(value2.RelativeY);
		}
	}
}