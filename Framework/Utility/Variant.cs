using System;
using System.Collections.Generic;
using System.Globalization;

namespace MG.Framework.Utility
{
	/// <summary>
	/// A multi-type, single value container. Optimized for read-performance.
	/// </summary>
	public class Variant
	{
		private Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
		private readonly string stringValue;

		public Variant(string value)
		{
			stringValue = value;
		}
		
		public T Get<T>()
		{
			var desiredType = typeof(T);
			object v;

			if (cachedValues.TryGetValue(desiredType, out v))
			{
				return (T)v;
			}

			v = Convert.ChangeType(stringValue, desiredType, CultureInfo.InvariantCulture);

			if (v != null)
			{
				cachedValues.Add(desiredType, v);
			}
			
			return (T)v;
		}
	}
}
