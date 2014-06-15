using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class GradientConverter : TypeConverter
	{
		private const char ListSeparator = ';';
		private const char EntrySeparator = ':';

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				var gradient = new Gradient();
				string[] entries = ((string)value).Split(new[] { ListSeparator }, StringSplitOptions.None);
				foreach (var entryText in entries)
				{
					var p = entryText.Split(new[] { EntrySeparator });

					if (p.Length == 2)
					{
						var position = Convert.ToSingle(p[0], culture);
						var color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(context, culture, p[1]);
						var entry = new GradientEntry(position, color);
						gradient.Add(entry);
					}
				}

				return gradient;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var gradient = (Gradient)value;
				var builder = new StringBuilder();
				int count = gradient.Count;
				foreach (var entry in gradient)
				{
					builder.Append(entry.Position.ToString(culture));
					builder.Append(EntrySeparator);
					builder.Append(TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(context, culture, entry.Color));
					count--;

					if (count >= 1)
					{
						builder.Append(ListSeparator);
					}
				}

				return builder.ToString();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}


