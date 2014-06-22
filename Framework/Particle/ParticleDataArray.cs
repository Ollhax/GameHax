using System;
using System.Collections.Generic;
using System.Diagnostics;

using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	/// <summary>
	/// An array of particle instance data.
	/// </summary>
	/// <remarks>For now, this class only handles specific types. This helps avoid garbage generation and other performance issues.</remarks>
	public class GenericDataArray
	{
		/// <summary>
		/// Create a data array.
		/// </summary>
		/// <param name="type">Type of data.</param>
		/// <param name="initialSize">Initial array size.</param>
		public GenericDataArray(Type type, int initialSize)
		{
			if (type == typeof(float))
			{
				data = new List<float>(initialSize);
				Resize(initialSize);
				return;
			}

			if (type == typeof(Vector2))
			{
				data = new List<Vector2>(initialSize);
				Resize(initialSize);
				return;
			}

			if (type == typeof(int))
			{
				data = new List<int>(initialSize);
				Resize(initialSize);
				return;
			}

			throw new ArgumentException("Data type not handled: " + type);
		}

		/// <summary>
		/// Resize this array.
		/// </summary>
		/// <remarks>You can only increase the array size for now.</remarks>
		/// <param name="size">The new size.</param>
		public void Resize(int size)
		{
			if (size == currentSize) return;
			if (size < currentSize) throw new ArgumentException("Cannot reduce property array size.");

			currentSize = size;

			var floatList = data as List<float>;
			if (floatList != null)
			{
				var array = (List<float>)data;
				array.Capacity = size;
				while (array.Count < size)
				{
					array.Add(default(float));
				}

				return;
			}

			var vector2List = data as List<Vector2>;
			if (vector2List != null)
			{
				var array = (List<Vector2>)data;
				array.Capacity = size;
				while (array.Count < size)
				{
					array.Add(default(Vector2));
				}

				return;
			}

			var intList = data as List<int>;
			if (intList != null)
			{
				var array = (List<int>)data;
				array.Capacity = size;
				while (array.Count < size)
				{
					array.Add(default(int));
				}

				return;
			}
		}

		/// <summary>
		/// Fetch a specific list of data.
		/// </summary>
		/// <typeparam name="T">The type of data. This must be the same type as passed in the constructor.</typeparam>
		/// <returns>A list of the specific types. This list reference can be kept for quick access.</returns>
		public List<T> Get<T>()
		{
			return (List<T>)data;
		}

		/// <summary>
		/// Move one entry in this list to another location, overwriting the destination.
		/// </summary>
		/// <param name="from">Source location.</param>
		/// <param name="to">Destination location.</param>
		/// <remarks>The source data will not be modified by this operation.</remarks>
		public void Move(int from, int to)
		{
			var floatList = data as List<float>; if (floatList != null) { floatList[to] = floatList[from]; return; }
			var vector2List = data as List<Vector2>; if (vector2List != null) { vector2List[to] = vector2List[from]; return; }
			var intList = data as List<int>; if (intList != null) { intList[to] = intList[from]; return; }
		}

		/// <summary>
		/// Move a range of entries in this list one step forward or backward in the list.
		/// </summary>
		/// <param name="start">Start index.</param>
		/// <param name="end">End index.</param>		
		public void Shuffle(int start, int end)
		{
			var direction = start < end ? 1 : -1;
			var s = end - direction;
			var e = start - direction;
			
			var floatList = data as List<float>; 
			if (floatList != null)
			{
				for (int i = s; i != e; i -= direction) floatList[i + direction] = floatList[i];
				return;
			}

			var vector2List = data as List<Vector2>;
			if (vector2List != null)
			{
				for (int i = s; i != e; i -= direction) vector2List[i + direction] = vector2List[i];
				return;
			}

			var intList = data as List<int>;
			if (intList != null)
			{
				for (int i = s; i != e; i -= direction) intList[i + direction] = intList[i];
				return;
			}
		}

		private int currentSize;
		private object data;
	}
}
