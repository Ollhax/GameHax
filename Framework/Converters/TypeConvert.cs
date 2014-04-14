using System;
using System.ComponentModel;
using MG.Framework.Numerics;
using System.Globalization;

namespace MG.Framework.Converters
{
	/// <summary>
	/// Type conversion helper.
	/// </summary>
	public static class TypeConvert
	{
		private static string listSeparator = CultureInfo.InvariantCulture.TextInfo.ListSeparator;
		private static string listSeparatorAndSpace = CultureInfo.InvariantCulture.TextInfo.ListSeparator + " ";
		
		//////////////////////////////////////////////////////////////////////////
		// bool
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(bool v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out bool ret)
		{
			try
			{
				ret = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception)
			{
				ret = false;
				return false;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// int
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(int v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out int ret)
		{
			try
			{
				ret = Convert.ToInt32(s, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception)
			{
				ret = 0;
				return false;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// uint
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(uint v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out uint ret)
		{
			try
			{
				ret = Convert.ToUInt32(s, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception)
			{
				ret = 0;
				return false;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// long
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(long v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out long ret)
		{
			try
			{
				ret = Convert.ToInt64(s, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception)
			{
				ret = 0;
				return false;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// float
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(float v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out float ret)
		{
			ret = 0;

			if (s.Length == 0)
				return false;

			float f;
			bool successful = false;

			try
			{
				f = Convert.ToSingle(s, CultureInfo.InvariantCulture);
				ret = f;
				successful = true;
			}
			catch (Exception)
			{
				successful = false;
			}

			return successful;
		}
		
		//////////////////////////////////////////////////////////////////////////
		// double
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(double v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out double ret)
		{
			ret = 0;

			if (s.Length == 0)
				return false;

			double f;
			bool successful = false;

			try
			{
				f = Convert.ToDouble(s, CultureInfo.InvariantCulture);
				ret = f;
				successful = true;
			}
			catch (Exception)
			{
				successful = false;
			}

			return successful;
		}

		//////////////////////////////////////////////////////////////////////////
		// Vector2
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(Vector2 v)
		{
			return v.X.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace + v.Y.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out Vector2 ret)
		{
			ret = Vector2.Zero;
			string[] parms = s.Split(new [] { listSeparator }, StringSplitOptions.None);

			if (parms.Length == 2)
			{
				float x = 0;
				float y = 0;

				if (FromString(parms[0], out x) && FromString(parms[1], out y))
				{
					ret = new Vector2(x, y);
					return true;
				}
			}

			return false;
		}

		//////////////////////////////////////////////////////////////////////////
		// RelativePosition2
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(RelativePosition2 v)
		{
			return 
				v.OffsetX.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace + 
				v.OffsetY.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.RelativeX.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.RelativeY.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out RelativePosition2 ret)
		{
			ret = RelativePosition2.Zero;
			string[] parms = s.Split(new[] { listSeparator }, StringSplitOptions.None);

			if (parms.Length == 4)
			{
				float x;
				float y;
				float rx;
				float ry;

				if (FromString(parms[0], out x) && 
					FromString(parms[1], out y) &&
					FromString(parms[2], out rx) &&
					FromString(parms[3], out ry)
					)
				{
					ret = new RelativePosition2(x, y, rx, ry);
					return true;
				}
			}

			return false;
		}

		//////////////////////////////////////////////////////////////////////////
		// Rectangle
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(Rectangle v)
		{			
			return
				v.X.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.Y.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.Width.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.Height.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out Rectangle ret)
		{
			ret = Rectangle.Empty;
			string[] parms = s.Split(new[] { listSeparator }, StringSplitOptions.None);

			if (parms.Length == 4)
			{
				try
				{
					ret.X = Convert.ToInt32(parms[0]);
					ret.Y = Convert.ToInt32(parms[1]);
					ret.Width = Convert.ToInt32(parms[2]);
					ret.Height = Convert.ToInt32(parms[3]);
				}
				catch (Exception)
				{
					return false;
				}

				return true;				
			}

			return false;
		}

		//////////////////////////////////////////////////////////////////////////
		// Color
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(Color v)
		{
			return
				v.R.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.G.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.B.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.A.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out Color ret)
		{
			ret = Color.White;
			string[] parms = s.Split(new[] { listSeparator }, StringSplitOptions.None);

			if (parms.Length == 4)
			{
				try
				{
					ret.R = Convert.ToByte(parms[0]);
					ret.G = Convert.ToByte(parms[1]);
					ret.B = Convert.ToByte(parms[2]);
					ret.A = Convert.ToByte(parms[3]);
				}
				catch (Exception)
				{
					return false;
				}

				return true;
			}

			return false;
		}
		
		//////////////////////////////////////////////////////////////////////////
		// Vector2I
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Convert value to a safely formated string.
		/// </summary>
		/// <param name="v">Value to convert.</param>
		/// <returns>A string without culture variation.</returns>
		public static string ToString(Vector2I v)
		{
			return
				v.X.ToString(CultureInfo.InvariantCulture) + listSeparatorAndSpace +
				v.Y.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert value from a correct string.
		/// </summary>
		/// <param name="s">String to convert.</param>
		/// <param name="ret">Output value.</param>
		/// <returns>True if conversion was successful.</returns>
		public static bool FromString(string s, out Vector2I ret)
		{
			ret = Vector2I.Zero;
			string[] parms = s.Split(new[] { listSeparator }, StringSplitOptions.None);

			if (parms.Length == 2)
			{
				try
				{
					ret.X = Convert.ToInt32(parms[0]);
					ret.Y = Convert.ToInt32(parms[1]);
				}
				catch (Exception)
				{
					return false;
				}

				return true;
			}

			return false;
		}

		public static T ChangeType<T>(object value)
		{
			return (T)ChangeType(typeof(T), value);
		}
		
		public static object ChangeType(Type t, object value)
		{
			TypeConverter tc = TypeDescriptor.GetConverter(t);
			return tc.ConvertFrom(value);
		}

		public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
		{
			TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
		}

		/// <summary>
		/// Convert values to string.
		/// </summary>
		public static string ConvertFromValues<T>(ITypeDescriptorContext context, CultureInfo culture, T[] values)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string separator = culture.TextInfo.ListSeparator + " ";
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			string[] strArray = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				strArray[i] = converter.ConvertToString(context, culture, values[i]);
			}
			return string.Join(separator, strArray);
		}

		/// <summary>
		/// Convert values from string.
		/// </summary>'
		public static T[] ConvertToValues<T>(ITypeDescriptorContext context, CultureInfo culture, object value, int arrayCount, params string[] expectedParams)
		{
			string str = value as string;
			if (str == null)
			{
				return null;
			}
			str = str.Trim();
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string[] strArray = str.Split(new string[] { culture.TextInfo.ListSeparator }, StringSplitOptions.None);
			T[] localArray = new T[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			for (int i = 0; i < localArray.Length; i++)
			{
				try
				{
					localArray[i] = (T)converter.ConvertFromString(context, culture, strArray[i]);
				}
				catch (Exception exception)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid string format", new object[] { string.Join(culture.TextInfo.ListSeparator, expectedParams) }), exception);
				}
			}
			if (localArray.Length != arrayCount)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid string format", new object[] { string.Join(culture.TextInfo.ListSeparator, expectedParams) }));
			}
			return localArray;
		}
	}
}

