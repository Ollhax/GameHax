using System;
using System.Reflection;

namespace MG.Framework.Utility
{
	/// <summary>
	/// Helper classes for handling enumerations.
	/// </summary>
	public static class EnumHelper
	{
		/// <summary>
		/// Try parsing a type to a specific enum type.
		/// </summary>
		/// <typeparam name="TEnum">Enumeration type.</typeparam>
		/// <typeparam name="TInput">Type to parse.</typeparam>
		/// <param name="value">Value to parse.</param>
		/// <returns>A enum with the corresponding value. Returns null if there is an error.</returns>
		public static TEnum TryParse<TInput, TEnum>(this TInput value)
		{
			var type = typeof(TEnum);

			if (value == null)
			{
				throw new ArgumentException("Value is null or empty.", "value");
			}

			if (!type.IsEnum)
			{
				throw new ArgumentException("Enum expected.", "TEnum");
			}

			return (TEnum)Enum.Parse(type, value.ToString(), true);
		}

		/// <summary>
		/// Fetches all the values of an enumeration type.
		/// </summary>
		/// <returns>An array of enum values.</returns>
		public static EnumType[] GetValues<EnumType>()
		{
			return (EnumType[])Enum.GetValues(typeof(EnumType));
		}

		/// <summary>
		/// Fetches all the values of an enumeration.
		/// </summary>
		/// <param name="enumType">Enumeration to split up.</param>
		/// <returns>An array of enum values.</returns>
		public static Enum[] GetValues(Type enumType)
		{
			if (enumType.BaseType == typeof(Enum))
			{
				var info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
				var values = new Enum[info.Length];
				
				for (var i = 0; i < values.Length; ++i)
				{
					values[i] = (Enum)info[i].GetValue(null);
				}

				return values;
			}

			return new Enum[0];
		}
	}
}