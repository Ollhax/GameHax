using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class CurveConverter : TypeConverter
	{
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
				var curve = new Curve();
				string[] entries = ((string)value).Split(new [] { culture.TextInfo.ListSeparator }, StringSplitOptions.None);
				foreach (var entryText in entries)
				{
					var p = entryText.Split(new[] { EntrySeparator });

					if (p.Length == 3 && p[0] == "lin")
					{
						var position = new Vector2(Convert.ToSingle(p[1], culture), Convert.ToSingle(p[2], culture));
						var entry = new CurveEntry(position);
						curve.Add(entry);
					}

					if (p.Length == 7 && p[1] == "bez")
					{
						var position = new Vector2(Convert.ToSingle(p[1], culture), Convert.ToSingle(p[2], culture));
						var left = new Vector2(Convert.ToSingle(p[3], culture), Convert.ToSingle(p[4], culture));
						var right = new Vector2(Convert.ToSingle(p[5], culture), Convert.ToSingle(p[6], culture));
						var entry = new CurveEntry(position, left, right);
						curve.Add(entry);
					}
				}

				return curve;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var curve = (Curve)value;
				var builder = new StringBuilder();
				int count = curve.Count;
				foreach (var entry in curve)
				{
					switch (entry.Type)
					{
						case CurveEntry.EntryType.Linear:
							builder.Append("lin");
							break;
						case CurveEntry.EntryType.Bezier:
							builder.Append("bez");
							break;
					}
					builder.Append(EntrySeparator);
					builder.Append(entry.Value.X.ToString(culture));
					builder.Append(EntrySeparator);
					builder.Append(entry.Value.Y.ToString(culture));
					count--;

					if (count >= 1)
					{
						builder.Append(culture.TextInfo.ListSeparator);
					}
				}

				return builder.ToString();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}


