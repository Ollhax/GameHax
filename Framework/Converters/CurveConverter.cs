using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Converters
{
	public class CurveConverter : TypeConverter
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
				var curve = new Curve();
				string[] entries = ((string)value).Split(new[] { ListSeparator }, StringSplitOptions.None);
				foreach (var entryText in entries)
				{
					if (string.IsNullOrEmpty(entryText)) continue;

					var p = entryText.Split(new[] { EntrySeparator });

					if ((p.Length == 2) || (p.Length == 3 && p[0] == "lin")) // Backward compatibility
					{
						var position = new Vector2(Convert.ToSingle(p[p.Length - 2], culture), Convert.ToSingle(p[p.Length - 1], culture));
						var entry = new CurveEntry(position);
						curve.Add(entry);
					}
					else if ((p.Length == 6) || (p.Length == 7 && p[0] == "bez")) // Backward compatibility
					{
						var position = new Vector2(Convert.ToSingle(p[p.Length - 6], culture), Convert.ToSingle(p[p.Length - 5], culture));
						var left = new Vector2(Convert.ToSingle(p[p.Length - 4], culture), Convert.ToSingle(p[p.Length - 3], culture));
						var right = new Vector2(Convert.ToSingle(p[p.Length - 2], culture), Convert.ToSingle(p[p.Length - 1], culture));
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
					//switch (entry.Type)
					//{
					//    case CurveEntry.EntryType.Linear:
					//        builder.Append("lin");
					//        break;
					//    case CurveEntry.EntryType.Bezier:
					//        builder.Append("bez");
					//        break;
					//}
					//builder.Append(EntrySeparator);
					builder.Append(entry.Value.X.ToString(culture));
					builder.Append(EntrySeparator);
					builder.Append(entry.Value.Y.ToString(culture));

					switch (entry.Type)
					{						
						case CurveEntry.EntryType.Bezier:
							builder.Append(EntrySeparator);
							builder.Append(entry.LeftHandle.X.ToString(culture));
							builder.Append(EntrySeparator);
							builder.Append(entry.LeftHandle.Y.ToString(culture));
							builder.Append(EntrySeparator);
							builder.Append(entry.RightHandle.X.ToString(culture));
							builder.Append(EntrySeparator);
							builder.Append(entry.RightHandle.Y.ToString(culture));
							break;
					}
					
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


