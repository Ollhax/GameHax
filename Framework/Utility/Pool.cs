using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MG.Framework.Utility
{
	/// <summary>
	/// A pre-allocated pool of objects with the ability to sort
	/// and maintain objects as they become invalidated.
	/// </summary>
	/// <typeparam name="T">The type of object the pool will hold.</typeparam>
	public class Pool<T> : IDisposable, IEnumerable<T>
		where T : class
	{
		#region Delegates

		/// <summary>
		/// Creates a new object during initialization.
		/// </summary>
		/// <returns>The newly created object. Cannot return null.</returns>
		public delegate T CreateNewObject();

		/// <summary>
		/// Checks if an object should be flagged as
		/// invalidated, allowing it's space in the pool
		/// to be given to another object.
		/// </summary>
		/// <param name="obj">The object to check</param>
		/// <returns>True if the object should be kept; false otherwise</returns>
		public delegate bool ValidateObject(T obj);

		#endregion

		private int numberOfValidObjects;
		private CreateNewObject objectCreate;
		private List<T> objects;

		/// <summary>
		/// Create a new pool with enough space for the allocated number of objects
		/// </summary>
		/// <param name="defaultCapacity">The number of objects to allocate space for</param>
		/// <param name="objectCreate">The method used to create new objects</param>
		public Pool(int defaultCapacity, CreateNewObject objectCreate = null)
		{
			this.objectCreate = objectCreate;
			
			numberOfValidObjects = 0;
			Reallocate(defaultCapacity);
		}

		/// <summary>
		/// Returns a valid object at the given index. Throws an exception
		/// if the index points to an invalid object space.
		/// </summary>
		/// <param name="index">The index of the valid object to get</param>
		/// <returns>A valid object found at the index</returns>
		public T this[int index]
		{
			get
			{
				if (index >= Count || index < 0)
				{
					throw new IndexOutOfRangeException("invalid index");
				}

				return objects[index];
			}
		}

		/// <summary>
		/// Gets the current capacity of this pool.
		/// </summary>
		public int Capacity
		{
			get { return objects.Count; }
		}

		/// <summary>
		/// Gets the number of valid objects (filled spaces) in the pool.
		/// </summary>
		public int Count
		{
			get { return numberOfValidObjects; }
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes this object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Retrieve an enumerator for this pool.
		/// </summary>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Retrieve an enumerator for this pool.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Creates a new valid object to fill the spot of an invalidated
		/// object if possible. Always returns a valid value; pool is 
		/// enlarged if necessary.
		/// </summary>
		/// <returns>A reference to the next valid object.</returns>
		public T New()
		{
			if (numberOfValidObjects >= objects.Count)
			{
				Reallocate(Math.Max((int)(objects.Count * 1.5f), objects.Count + 4));
			}

			return objects[numberOfValidObjects++];
		}

		/// <summary>
		/// Remove the specified object, returning it to the pool.
		/// </summary>
		/// <param name="t"></param>
		public void Delete(T t)
		{
			for (int i = 0; i < numberOfValidObjects; i++)
			{
				if (objects[i] == t)
				{
					for (int j = i + 1; j < numberOfValidObjects; j++)
					{
						objects[j - 1] = objects[j];
					}
					objects[numberOfValidObjects - 1] = t;
					numberOfValidObjects--;
					return;
				}
			}
		}
		
		/// <summary>
		/// Sort all valid objects of this pool.
		/// </summary>
		public void Sort()
		{
			objects.Sort(0, numberOfValidObjects, null);
		}

		/// <summary>
		/// Retrieve an enumerator for this pool.
		/// </summary>
		public PoolEnumerator<T> GetEnumerator()
		{
			return new PoolEnumerator<T>(this);
		}

		/// <summary>
		/// Disposes this object.
		/// </summary>
		/// <param name="disposing">
		/// True if this method was called as part of the Dispose method.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			objects.Clear();
			objectCreate = null;
		}
		
		private void Reallocate(int newSize)
		{
			int newObjects = 0;
			if (objects != null)
			{
				Debug.Assert(newSize > objects.Count);
				List<T> oldList = objects;

				objects = new List<T>(newSize);
				objects.AddRange(oldList);
				newObjects = newSize - oldList.Count;
			}
			else
			{
				objects = new List<T>(newSize);
				newObjects = newSize;
			}

			for (int i = 0; i < newObjects; i++)
			{
				if (objectCreate != null)
				{
					T t = objectCreate();

					if (t == null)
						throw new ArgumentException("CreateNewObject delegate cannot return null.");

					objects.Add(t);
				}
				else
				{
					var types = new Type[0];
					if (typeof (T).GetConstructor(types) != null)
					{
						objects.Add((T)typeof (T).GetConstructor(types).Invoke(null));
					}
					else
					{
						throw new ArgumentException(
							"CreateNewObject delegate must be specified or T (" + typeof (T) + ") " +
							"must implement a parameterless constructor.");
					}
				}
			}
		}

		#region Nested type: PoolEnumerator

		/// <summary>
		/// Pool enumerator type.
		/// </summary>
		public struct PoolEnumerator<PoolT> : IEnumerator<PoolT>
			where PoolT : class
		{
			public Pool<PoolT> pool;

			// Enumerators are positioned before the first element
			// until the first MoveNext() call.
			private int position;

			public PoolEnumerator(Pool<PoolT> pool)
			{
				this.pool = pool;
				position = -1;
			}

			public PoolT Current
			{
				get { return pool[position]; }
			}

			#region IEnumerator<PoolT> Members

			public bool MoveNext()
			{
				position++;
				return (position < pool.Count);
			}

			public void Reset()
			{
				position = -1;
			}

			public void Dispose()
			{
				pool = null;
			}

			object IEnumerator.Current
			{
				get { return pool[position]; }
			}

			PoolT IEnumerator<PoolT>.Current
			{
				get { return pool[position]; }
			}

			#endregion
		}

		#endregion
	}
}