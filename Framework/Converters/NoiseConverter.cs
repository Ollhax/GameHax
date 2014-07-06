using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class NoiseConverter : TypeConverter
	{
		private const char ListSeparator = ';';

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
				var noise = new Noise();
				string[] entries = ((string)value).Split(new[] { ListSeparator }, StringSplitOptions.None);
				if (entries.Length == 6)
				{
					noise.Base = Convert.ToInt32(entries[0], culture);
					noise.Octaves = Convert.ToInt32(entries[1], culture);
					noise.Scale = Convert.ToSingle(entries[2], culture);
					noise.Amplitude = Convert.ToSingle(entries[3], culture);
					noise.Persistence = Convert.ToSingle(entries[4], culture);
					noise.Period = Convert.ToInt32(entries[5], culture);
				}

				return noise;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var noise = (Noise)value;
				var builder = new StringBuilder();
				builder.Append(noise.Base); builder.Append(ListSeparator);
				builder.Append(noise.Octaves); builder.Append(ListSeparator);
				builder.Append(noise.Scale); builder.Append(ListSeparator);
				builder.Append(noise.Amplitude); builder.Append(ListSeparator);
				builder.Append(noise.Persistence); builder.Append(ListSeparator);
				builder.Append(noise.Period);
				return builder.ToString();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}


