using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.Framework.Collections
{
	/// <summary>
	/// A simple ordered dictionary that stores entries in both a list and a dictionary internally.
	/// Suitable for use cases with frequent access and infrequent changes.
	/// </summary>
	/// <typeparam name="TKey">Key type.</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private const int DefaultCapacity = 0;
		private readonly IEqualityComparer<TKey> comparer;
		private readonly int initialCapacity;

		private Dictionary<TKey, TValue> dictionary;
		private List<KeyValuePair<TKey, TValue>> list;

		private Dictionary<TKey, TValue> Dictionary
		{
			get
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<TKey, TValue>(initialCapacity, comparer);
				}
				return dictionary;
			}
		}

		/// <summary>
		/// Gets or sets a value at the specified index.
		/// </summary>
		/// <param name="index">Index of object to be set or returned.</param>
		/// <returns>The value at the specified index.</returns>
		public TValue this[int index]
		{
			get { return List[index].Value; }

			set
			{
				if (index >= Count || index < 0)
				{
					throw new ArgumentOutOfRangeException("index", "The value 'index' is out of bounds.");
				}

				TKey key = List[index].Key;

				List[index] = new KeyValuePair<TKey, TValue>(key, value);
				Dictionary[key] = value;
			}
		}

		private List<KeyValuePair<TKey, TValue>> List
		{
			get
			{
				if (list == null)
				{
					list = new List<KeyValuePair<TKey, TValue>>(initialCapacity);
				}
				return list;
			}
		}

		/// <summary>
		/// Creates a new dictionary.
		/// </summary>
		public OrderedDictionary()
			: this(DefaultCapacity, null)
		{
		}

		/// <summary>
		/// Creates a new dictionary with the specified capacity.
		/// </summary>
		/// <param name="capacity">Starting capacity.</param>
		public OrderedDictionary(int capacity)
			: this(capacity, null)
		{
		}

		/// <summary>
		/// Creates a new dictionary with a specific comparer.
		/// </summary>
		/// <param name="comparer">Equality comparer.</param>
		public OrderedDictionary(IEqualityComparer<TKey> comparer)
			: this(DefaultCapacity, comparer)
		{
		}

		/// <summary>
		/// Creates a new dictionary with the specified capacity and comparer.
		/// </summary>
		/// <param name="capacity">Starting capacity.</param>
		/// <param name="comparer">Equality comparer.</param>
		public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (0 > capacity)
			{
				throw new ArgumentOutOfRangeException("capacity", "'capacity' must be non-negative");
			}

			initialCapacity = capacity;
			this.comparer = comparer;
		}

		/// <summary>
		/// Creates a copy of another dictionary.
		/// </summary>
		/// <param name="other"></param>
		public OrderedDictionary(OrderedDictionary<TKey, TValue> other)
		{
			initialCapacity = other.initialCapacity;
			comparer = other.comparer;

			if (other.dictionary != null)
			{
				dictionary = new Dictionary<TKey, TValue>(other.dictionary);
			}

			if (other.list != null)
			{
				list = new List<KeyValuePair<TKey, TValue>>(other.list);
			}
		}

		/// <summary>
		/// Is this dictionary read-only?
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Get or sets a value by the specified key.
		/// </summary>
		/// <param name="key">Key of the object to be get or set.</param>
		/// <returns>The value by the specified key.</returns>
		public TValue this[TKey key]
		{
			get { return Dictionary[key]; }
			set
			{
				if (Dictionary.ContainsKey(key))
				{
					Dictionary[key] = value;
					List[IndexOfKey(key)] = new KeyValuePair<TKey, TValue>(key, value);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		/// <summary>
		/// Return the number of elements in this dictionary.
		/// </summary>
		public int Count { get { return List.Count; } }

		/// <summary>
		/// Return all keys of this dictionary.
		/// </summary>
		public ICollection<TKey> Keys { get { return Dictionary.Keys; } }

		/// <summary>
		/// Returns all values of this dictionary.
		/// </summary>
		public ICollection<TValue> Values { get { return Dictionary.Values; } }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			Add(key, value);
		}

		/// <summary>
		/// Remove all entries in this collection.
		/// </summary>
		public void Clear()
		{
			Dictionary.Clear();
			List.Clear();
		}

		/// <summary>
		/// Test if this dictionary contains a specific key.
		/// </summary>
		/// <param name="key">Key value to look for.</param>
		/// <returns>True if the key was found.</returns>
		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Remove a specific key/value pair by key.
		/// </summary>
		/// <param name="key">The key value to look for.</param>
		/// <returns>True if the pair was removed.</returns>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			int index = IndexOfKey(key);
			if (index >= 0)
			{
				if (Dictionary.Remove(key))
				{
					List.RemoveAt(index);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Try finding the value matching the specified key.
		/// </summary>
		/// <param name="key">Input key.</param>
		/// <param name="value">Output value.</param>
		/// <returns>True of the value was found.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}
		
		/// <summary>
		/// Insert a new key and value by the specified index.
		/// </summary>
		/// <param name="index">Insertion index.</param>
		/// <param name="key">Key to insert.</param>
		/// <param name="value">Value to insert.</param>
		public void Insert(int index, TKey key, TValue value)
		{
			if (index > Count || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			Dictionary.Add(key, value);
			List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
		}

		/// <summary>
		///  Remove a key/value pair at the specified index.
		/// </summary>
		/// <param name="index">The index of the key/value pair that should be removed.</param>
		public void RemoveAt(int index)
		{
			if (index >= Count || index < 0)
			{
				throw new ArgumentOutOfRangeException(
					"index", "'index' must be non-negative and less than the size of the collection");
			}

			TKey key = List[index].Key;

			List.RemoveAt(index);
			Dictionary.Remove(key);
		}

		/// <summary>
		/// Add a new key/value pair at the next available index.
		/// </summary>
		/// <param name="key">Key to insert.</param>
		/// <param name="value">Value to insert.</param>
		/// <returns>Index of the added pair.</returns>
		public int Add(TKey key, TValue value)
		{
			Dictionary.Add(key, value);
			List.Add(new KeyValuePair<TKey, TValue>(key, value));
			return Count - 1;
		}

		/// <summary>
		/// Test if this dictionary contains a specific value.
		/// </summary>
		/// <param name="value">Key value to look for.</param>
		/// <returns>True if the value was found.</returns>
		public bool ContainsValue(TValue value)
		{
			return Dictionary.ContainsValue(value);
		}

		/// <summary>
		/// Return the index of the specified key.
		/// </summary>
		/// <param name="key">Key value to look for.</param>
		/// <returns>Index of the key value.</returns>
		public int IndexOfKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			for (int index = 0; index < List.Count; index++)
			{
				KeyValuePair<TKey, TValue> entry = List[index];
				TKey next = entry.Key;
				if (null != comparer)
				{
					if (comparer.Equals(next, key))
					{
						return index;
					}
				}
				else if (next.Equals(key))
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns the key at the specified index.
		/// </summary>
		/// <param name="index">Index of the key.</param>
		/// <returns>The key at the specified index.</returns>
		public TKey KeyAt(int index)
		{
			return List[index].Key;
		}
	}
}