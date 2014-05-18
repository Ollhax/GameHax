using System;
using System.Globalization;

using MG.Framework.Numerics;

namespace MG.Framework.Utility
{
	/// <summary>
	/// A generic holder of values.
	/// </summary>
	public class Any : IEquatable<Any>
	{
		public Any(int value) { boxedValue = value; }
		public Any(bool value) { boxedValue = value; }
		public Any(float value) { boxedValue = value; }
		public Any(Tween value) { boxedValue = value; }
		public Any(Color value) { boxedValue = value; }
		public Any(string value) { boxedValue = value; }
		public Any(Vector2 value) { boxedValue = value; }
		public Any(Vector2I value) { boxedValue = value; }
		public Any(FilePath value) { boxedValue = value; }
		public Any(RectangleF value) { boxedValue = value; }
		public Any(RelativePosition2 value) { boxedValue = value; }
		public Any(Any other)
		{
			CopyFrom(other);
		}
		
		public Any(string valueAsString, string nameOfType)
		{
			if (nameOfType == typeof(int).Name) boxedValue = Convert.ToInt32(valueAsString, CultureInfo.InvariantCulture);
			else if (nameOfType == typeof(bool).Name) boxedValue = Convert.ToBoolean(valueAsString, CultureInfo.InvariantCulture);
			else if (nameOfType == typeof(float).Name) boxedValue = Convert.ToSingle(valueAsString, CultureInfo.InvariantCulture);
			else if (nameOfType == typeof(Tween).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Tween)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else if (nameOfType == typeof(Color).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else if (nameOfType == typeof(string).Name) boxedValue = valueAsString;
			else if (nameOfType == typeof(Vector2).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Vector2)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else if (nameOfType == typeof(Vector2I).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Vector2I)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else if (nameOfType == typeof(FilePath).Name) boxedValue = new FilePath(valueAsString);
			else if (nameOfType == typeof(RectangleF).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(RectangleF)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else if (nameOfType == typeof(RelativePosition2).Name) boxedValue = System.ComponentModel.TypeDescriptor.GetConverter(typeof(RelativePosition2)).ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
			else throw new Exception("Type unrecognized: " + nameOfType + " (value: " + valueAsString + ")");
		}

		public void CopyFrom(Any other)
		{
			if (other.boxedValue is int) boxedValue = (int)(other.boxedValue);
			else if (other.boxedValue is bool) boxedValue = (bool)(other.boxedValue);
			else if (other.boxedValue is float) boxedValue = (float)(other.boxedValue);
			else if (other.boxedValue is Tween) boxedValue = (Tween)(other.boxedValue);
			else if (other.boxedValue is Color) boxedValue = (Color)(other.boxedValue);
			else if (other.boxedValue is string) boxedValue = ((string)other.boxedValue).Clone();
			else if (other.boxedValue is Vector2) boxedValue = (Vector2)(other.boxedValue);
			else if (other.boxedValue is Vector2I) boxedValue = (Vector2I)(other.boxedValue);
			else if (other.boxedValue is FilePath) boxedValue = (FilePath)(other.boxedValue);
			else if (other.boxedValue is RectangleF) boxedValue = (RectangleF)(other.boxedValue);
			else if (other.boxedValue is RelativePosition2) boxedValue = (RelativePosition2)(other.boxedValue);
			else throw new Exception("Type unrecognized: " + other.GetTypeOfValue() + " (value: " + other.ToString() + ")");
		}

		public bool Equals(Any other)
		{
			return boxedValue.Equals(other.boxedValue);
		}

		public T Get<T>() { if (boxedValue is T) return (T)boxedValue; return default(T); }
		public int Get(int defaultValue) { if (boxedValue is int) return (int)boxedValue; return defaultValue; }
		public bool Get(bool defaultValue) { if (boxedValue is bool) return (bool)boxedValue; return defaultValue; }
		public float Get(float defaultValue) { if (boxedValue is float) return (float)boxedValue; return defaultValue; }
		public Tween Get(Tween defaultValue) { if (boxedValue is Tween) return (Tween)boxedValue; return defaultValue; }
		public Color Get(Color defaultValue) { if (boxedValue is Color) return (Color)boxedValue; return defaultValue; }
		public string Get(string defaultValue) { if (boxedValue is string) return (string)boxedValue; return defaultValue; }
		public Vector2 Get(Vector2 defaultValue) { if (boxedValue is Vector2) return (Vector2)boxedValue; return defaultValue; }
		public Vector2I Get(Vector2I defaultValue) { if (boxedValue is Vector2I) return (Vector2I)boxedValue; return defaultValue; }
		public FilePath Get(FilePath defaultValue) { if (boxedValue is FilePath) return (FilePath)boxedValue; return defaultValue; }
		public RectangleF Get(RectangleF defaultValue) { if (boxedValue is RectangleF) return (RectangleF)boxedValue; return defaultValue; }
		public RelativePosition2 Get(RelativePosition2 defaultValue) { if (boxedValue is RelativePosition2) return (RelativePosition2)boxedValue; return defaultValue; }
		
		public void Set(object value) { boxedValue = value; }
		public object GetAsObject() { return boxedValue; }
		public Type GetTypeOfValue() { return boxedValue.GetType(); }
		public override string ToString() { return System.ComponentModel.TypeDescriptor.GetConverter(boxedValue.GetType()).ConvertToString(null, CultureInfo.InvariantCulture, boxedValue); }

		public bool IsInt() { return boxedValue is int; }
		public bool IsBool() { return boxedValue is bool; }
		public bool IsFloat() { return boxedValue is float; }
		public bool IsTween() { return boxedValue is Tween; }
		public bool IsColor() { return boxedValue is Color; }
		public bool IsString() { return boxedValue is string; }
		public bool IsVector2() { return boxedValue is Vector2; }
		public bool IsVector2I() { return boxedValue is Vector2I; }
		public bool IsFilePath() { return boxedValue is FilePath; }
		public bool IsRectangleF() { return boxedValue is RectangleF; }
		public bool IsRelativePosition2() { return boxedValue is RelativePosition2; }
		
		private object boxedValue;
	}
}
