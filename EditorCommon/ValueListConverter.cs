using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using MG.Framework.Utility;

namespace MG.EditorCommon
{
	public class ValueListConverter : TypeConverter
	{
		private List<KeyValuePair<string, Any>> values;
		private List<string> valueKeys = new List<string>();

		public ValueListConverter(List<KeyValuePair<string, Any>> values)
		{
			this.values = values;

			if (values == null) return;

			foreach (var kvp in values)
			{
				valueKeys.Add(kvp.Key);
			}
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(valueKeys);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() != typeof(string))
				return base.ConvertFrom(context, culture, value);

			var s = (string)value;

			for (int i = 0; i < values.Count; i++)
			{
				if (values[i].Key == s) return values[i].Value.GetAsObject();
			}

			return "";
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}

			var v = value;
			if (v is Any)
			{
				v = ((Any)v).GetAsObject();
			}

			if ((destinationType == typeof(string)))
			{
				for (int i = 0; i < values.Count; i++)
				{
					if (values[i].Value.GetAsObject().Equals(v)) return values[i].Key;
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
