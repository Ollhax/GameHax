using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// An entry in the curve.
	/// </summary>
	public class CurveEntry : IComparer<CurveEntry>
	{
		/// <summary>
		/// The type of entry determines how values are interpolated between this entry and its neighbours.
		/// </summary>
		public enum EntryType
		{
			Linear,
			Bezier
		}

		/// <summary>
		/// The type of entry.
		/// </summary>
		public readonly EntryType Type;

		/// <summary>
		/// The X and Y values of this entry.
		/// </summary>
		public readonly Vector2 Value;

		/// <summary>
		/// Create a linear entry.
		/// </summary>
		/// <param name="value">This entry's value.</param>
		public CurveEntry(Vector2 value)
		{
			Type = EntryType.Linear;
			Value = value;
		}

		/// <summary>
		/// Copy another entry.
		/// </summary>
		/// <param name="other">Entry to copy.</param>
		public CurveEntry(CurveEntry other)
		{
			Type = other.Type;
			Value = other.Value;
		}

		/// <summary>
		/// Compare these two entries.
		/// </summary>
		/// <param name="a">The first entry.</param>
		/// <param name="b">The second entry.</param>
		public int Compare(CurveEntry a, CurveEntry b)
		{
			return a.Value.X.CompareTo(b.Value.X);
		}
	}

	/// <summary>
	/// A representation of a graph curve predefined data entries that you can interpolate between.
	/// </summary>
	[TypeConverter(typeof(ComplexCurveConverter))]
	public class ComplexCurve : ICollection<CurveEntry>
	{
		private readonly List<CurveEntry> entries = new List<CurveEntry>();
		
		/// <summary>
		/// How extrapolation is performed beyond the lowest and highest entries.
		/// </summary>
		public enum ExtrapolateMode
		{
			Equal,
			Extrapolate
		}

		/// <summary>
		/// How values are calculated beyond the smallest entry of this curve.
		/// </summary>
		public ExtrapolateMode ExtrapolateModeLeft = ExtrapolateMode.Equal;

		/// <summary>
		/// How values are calculated beyond the largest entry of this curve.
		/// </summary>
		public ExtrapolateMode ExtrapolateModeRight = ExtrapolateMode.Equal;

		/// <summary>
		/// Create an empty curve.
		/// </summary>
		public ComplexCurve()
		{

		}

		/// <summary>
		/// Create a copy of another curve.
		/// </summary>
		/// <param name="other">Curve to copy.</param>
		public ComplexCurve(ComplexCurve other)
		{
			ExtrapolateModeLeft = other.ExtrapolateModeLeft;
			ExtrapolateModeRight = other.ExtrapolateModeRight;

			entries.Capacity = other.Count;
			foreach (var entry in other)
			{
				entries.Add(new CurveEntry(entry));
			}
		}

		/// <summary>
		/// Evaluate the value of this curve at a given point.
		/// </summary>
		/// <param name="x">The X value.</param>
		/// <returns>The result value.</returns>
		public float Evaluate(float x)
		{
			var count = entries.Count;
			if (count == 0) return 0;
			if (count == 1) return entries[0].Value.Y;

			var firstEntry = entries[0];
			var lastEntry = entries[count - 1];
			if (x <= firstEntry.Value.X)
			{
				// TODO: Extrapolate

				// Left bound
				return firstEntry.Value.Y;
			}

			if (x > lastEntry.Value.X)
			{
				// Right bound
				return lastEntry.Value.Y;
			}
			
			var index = SearchForEntry(x);
			var leftEntry = entries[index - 1];
			var rightEntry = entries[index];

			var spanX = rightEntry.Value.X - leftEntry.Value.X;
			float fraction = 0;

			if (spanX > 0)
			{
				fraction = (x - leftEntry.Value.X) / spanX;
			}

			// TODO: Bezier curves
			return leftEntry.Value.Y + fraction * (rightEntry.Value.Y - leftEntry.Value.Y);
		}

		private int SearchForEntry(float value)
		{
			var min = 0;
			var max = entries.Count - 1;

			while (min < max)
			{
				int mid = (max + min) / 2;
				
				if (entries[mid].Value.X < value)
				{
					min = mid + 1;
				}
				else
				{
					max = mid - 1;
				}
			}

			if (entries[min].Value.X < value) min++;
			return min;
		}
		
		/// <summary>
		/// Add an entry to this curve.
		/// </summary>
		/// <param name="entry">Entry to add.</param>
		public void Add(CurveEntry entry)
		{
			var result = entries.BinarySearch(entry, entry);
			if (result < 0)
			{
				int index = ~result;
				entries.Insert(index, entry);
			}
			else
			{
				entries.Add(entry);
			}
		}

		/// <summary>
		/// Remove all entries from this curve.
		/// </summary>
		public void Clear()
		{
			entries.Clear();
		}

		/// <summary>
		/// Test if the specified entry exists in this curve.
		/// </summary>
		/// <param name="entry">Entry to test for.</param>
		/// <returns>True if the specified entry exists in this curve.</returns>
		public bool Contains(CurveEntry entry)
		{
			return entries.Contains(entry);
		}

		/// <summary>
		/// Copy all entries of this curve to an array.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="arrayIndex">Starting index of the destination array.</param>
		public void CopyTo(CurveEntry[] array, int arrayIndex)
		{
			entries.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Return the number of elements in this curve.
		/// </summary>
		public int Count
		{
			get { return entries.Count; }
		}

		/// <summary>
		/// Fetch whether or not this curve is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}
		
		/// <summary>
		/// Remove the specified entry from this curve.
		/// </summary>
		/// <param name="entry">Entry to remove</param>
		/// <returns></returns>
		public bool Remove(CurveEntry entry)
		{
			return entries.Remove(entry);
		}

		/// <summary>
		/// Get an enumerator that walks through all entries of this curve.
		/// </summary>
		/// <returns>An enumerator that walks through all entries of this curve.</returns>
		IEnumerator<CurveEntry> IEnumerable<CurveEntry>.GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		/// <summary>
		/// Get an enumerator that walks through all entries of this curve.
		/// </summary>
		/// <returns>An enumerator that walks through all entries of this curve.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return entries.GetEnumerator();
		}

		/// <summary>
		/// Convert this curve to a string representation that does not change depending on culture.
		/// </summary>
		/// <returns>An invariant string representation.</returns>
		public string ToInvariantString()
		{
			var converter = TypeDescriptor.GetConverter(typeof(ComplexCurve));

			try
			{
				var s = converter.ConvertToString(null, CultureInfo.InvariantCulture, this);
				return s;
			}
			catch (Exception)
			{

			}

			return "";
		}

		/// <summary>
		/// Create a curve from a string.
		/// </summary>
		/// <param name="s">Source string.</param>
		/// <returns>A new curve, or null if the conversion was unsuccessful.</returns>
		public static ComplexCurve FromInvariantString(string s)
		{
			var converter = TypeDescriptor.GetConverter(typeof(ComplexCurve));

			try
			{
				var graph = converter.ConvertFromString(null, CultureInfo.InvariantCulture, s) as ComplexCurve;
				return graph;
			}
			catch (Exception)
			{

			}

			return null;
		}
	}
}
