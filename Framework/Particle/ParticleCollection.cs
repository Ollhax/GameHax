using System;
using System.Collections.Generic;

namespace MG.Framework.Particle
{
	public class ParticleCollection : IList<ParticleDefinition>
	{
		private List<ParticleDefinition> children = new List<ParticleDefinition>();

		public ParticleDefinition GetById(int id)
		{
			foreach (var child in children)
			{
				if (child.Id == id) return child;
				var r = child.Children.GetById(id);
				if (r != null) return r;
			}

			return null;
		}

		public ParticleDefinition GetByName(string name)
		{
			foreach (var child in children)
			{
				if (child.Name == name) return child;
				var r = child.Children.GetByName(name);
				if (r != null) return r;
			}

			return null;
		}

		public ParticleCollection GetParentCollection(int id)
		{
			var r = GetParentCollectionInternal(id);
			if (r != null) return r;
			return this;
		}

		private ParticleCollection GetParentCollectionInternal(int id)
		{
			foreach (var child in this)
			{
				if (child.Id == id)
				{
					return this;
				}

				var r = child.Children.GetParentCollectionInternal(id);
				if (r != null) return r;
			}

			return null;
		} 

		public int IndexOfRecursive(ParticleDefinition item)
		{
			var index = children.IndexOf(item);
			if (index >= 0) return index;

			foreach (var child in children)
			{
				index = child.Children.IndexOfRecursive(item);
				if (index >= 0) return index;
			}

			return -1;
		}

		public int IndexOf(ParticleDefinition item)
		{
			return children.IndexOf(item);
		}

		public void Insert(int index, ParticleDefinition item)
		{
			children.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			children.RemoveAt(index);
		}

		public ParticleDefinition this[int index]
		{
			get { return children[index]; }
			set { children[index] = value; }
		}

		public void Add(ParticleDefinition item)
		{
			children.Add(item);
		}

		public void Clear()
		{
			children.Clear();
		}

		public bool Contains(ParticleDefinition item)
		{
			return children.Contains(item);
		}

		public void CopyTo(ParticleDefinition[] array, int arrayIndex)
		{
			children.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return children.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(ParticleDefinition item)
		{
			return children.Remove(item);
		}

		public void SortByName()
		{
			children.Sort((a, b) => String.Compare(a.Name, b.Name));
		}

		public void Sort(Comparer<ParticleDefinition> comparer)
		{
			children.Sort(comparer);
		}

		public IEnumerator<ParticleDefinition> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return children.GetEnumerator();
		}
	}
}
