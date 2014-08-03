using System;
using System.Collections.Generic;
using System.ComponentModel;
using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// An entry in the gradient.
	/// </summary>
	public class GradientEntry : IComparer<GradientEntry>, IEquatable<GradientEntry>
	{
		/// <summary>
		/// The position value of this entry.
		/// </summary>
		public readonly float Position;

		/// <summary>
		/// The color value of this entry.
		/// </summary>
		public readonly Color Color;

		/// <summary>
		/// Create an entry.
		/// </summary>
		/// <param name="position">This entry's position.</param>
		/// <param name="color">This entry's color.</param>
		public GradientEntry(float position, Color color)
		{
			Position = position;
			Color = color;
		}

		/// <summary>
		/// Copy another entry.
		/// </summary>
		/// <param name="other">Entry to copy.</param>
		public GradientEntry(GradientEntry other)
		{
			Position = other.Position;
			Color = other.Color;
		}

		/// <summary>
		/// Compare these two entries.
		/// </summary>
		/// <param name="a">The first entry.</param>
		/// <param name="b">The second entry.</param>
		public int Compare(GradientEntry a, GradientEntry b)
		{
			return a.Position.CompareTo(b.Position);
		}

		/// <summary>
		/// Test if two GradientEntries are equal.
		/// </summary>
		/// <param name="other">GradientEntry to test against.</param>
		/// <returns>True if the two GradientEntries are equal.</returns>
		public bool Equals(GradientEntry other)
		{
			return Position == other.Position && Color == other.Color;
		}
	}

	/// <summary>
	/// A representation of a gradient color curve.
	/// </summary>
	[TypeConverter(typeof(GradientConverter))]
	public class Gradient : ICollection<GradientEntry>, IEquatable<Gradient>
	{
		private readonly List<GradientEntry> entries = new List<GradientEntry>();
		
		/// <summary>
		/// Return the first entry, or null if the gradient has no entries.
		/// </summary> 
		public GradientEntry Front
		{
			get { if (entries.Count > 0) return entries[0]; return null; }
		}

		/// <summary>
		/// Return the last entry, or null if the gradient has no entries.
		/// </summary> 
		public GradientEntry End
		{
			get { if (entries.Count > 0) return entries[entries.Count - 1]; return null; }
		}

		/// <summary>
		/// Create an empty gradient.
		/// </summary>
		public Gradient()
		{

		}

		/// <summary>
		/// Create a copy of another gradient.
		/// </summary>
		/// <param name="other">Gradient to copy.</param>
		public Gradient(Gradient other)
		{
			entries.Capacity = other.Count;
			foreach (var entry in other)
			{
				entries.Add(new GradientEntry(entry));
			}
		}

		/// <summary>
		/// Evaluate the value of this gradient at a given point.
		/// </summary>
		/// <param name="x">The X value.</param>
		/// <returns>The result value.</returns>
		public Color Evaluate(float x)
		{
			var count = entries.Count;
			if (count == 0) return Color.White;
			if (count == 1) return entries[0].Color;

			var firstEntry = entries[0];
			var lastEntry = entries[count - 1];
			if (x <= firstEntry.Position)
			{
				return firstEntry.Color;
			}

			if (x > lastEntry.Position)
			{
				return lastEntry.Color;
			}
			
			var index = SearchForEntry(x);
			var leftEntry = entries[index - 1];
			var rightEntry = entries[index];

			var spanX = rightEntry.Position - leftEntry.Position;
			float fraction = 0;

			if (spanX > 0)
			{
				fraction = (x - leftEntry.Position) / spanX;
			}
			
			return Color.Lerp(leftEntry.Color, rightEntry.Color, fraction);
		}
		
		private int SearchForEntry(float value)
		{
			var min = 0;
			var max = entries.Count - 1;

			while (min < max)
			{
				int mid = (max + min) / 2;
				
				if (entries[mid].Position < value)
				{
					min = mid + 1;
				}
				else
				{
					max = mid - 1;
				}
			}

			if (entries[min].Position < value) min++;
			return min;
		}
		
		/// <summary>
		/// Add an entry to this gradient.
		/// </summary>
		/// <param name="entry">Entry to add.</param>
		public void Add(GradientEntry entry)
		{
			var result = entries.BinarySearch(entry, entry);
			if (result < 0)
			{
				int index = ~result;
				entries.Insert(index, entry);
			}
			else if (result == 0)
			{
				entries.Insert(0, entry);
			}
			else
			{
				entries.Add(entry);
			}
		}

		/// <summary>
		/// Remove all entries from this gradient.
		/// </summary>
		public void Clear()
		{
			entries.Clear();
		}

		/// <summary>
		/// Test if the specified entry exists in this gradient.
		/// </summary>
		/// <param name="entry">Entry to test for.</param>
		/// <returns>True if the specified entry exists in this gradient.</returns>
		public bool Contains(GradientEntry entry)
		{
			return entries.Contains(entry);
		}

		/// <summary>
		/// Copy all entries of this gradient to an array.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="arrayIndex">Starting index of the destination array.</param>
		public void CopyTo(GradientEntry[] array, int arrayIndex)
		{
			entries.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Return the number of elements in this gradient.
		/// </summary>
		public int Count
		{
			get { return entries.Count; }
		}

		/// <summary>
		/// Fetch whether or not this gradient is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}
		
		/// <summary>
		/// Remove the specified entry from this gradient.
		/// </summary>
		/// <param name="entry">Entry to remove</param>
		/// <returns></returns>
		public bool Remove(GradientEntry entry)
		{
			return entries.Remove(entry);
		}

		/// <summary>
		/// Get an enumerator that walks through all entries of this gradient.
		/// </summary>
		/// <returns>An enumerator that walks through all entries of this gradient.</returns>
		IEnumerator<GradientEntry> IEnumerable<GradientEntry>.GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		/// <summary>
		/// Get an enumerator that walks through all entries of this gradient.
		/// </summary>
		/// <returns>An enumerator that walks through all entries of this gradient.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		/// <summary>
		/// Test if this gradient equals another gradient.
		/// </summary>
		/// <param name="other">Gradient to test against.</param>
		/// <returns>True if the two gradients are equal.</returns>
		public bool Equals(Gradient other)
		{
			if (entries.Count != other.entries.Count) return false;
			for (int i = 0; i < entries.Count; i++)
			{
				if (!entries[i].Equals(other.entries[i])) return false;
			}
			return true;
		}

		/// <summary>
		/// Test if this gradient equals another object.
		/// </summary>
		/// <param name="obj">Gradient to test against.</param>
		/// <returns>True if the two gradients are equal.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as Gradient;
			if (other != null)
			{
				return Equals(other);
			}
			return false;
		}
	}
}
