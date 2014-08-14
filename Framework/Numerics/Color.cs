using System;
using System.ComponentModel;
using System.Globalization;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A four-channel, 8 bit / channel color representation.
	/// </summary>
	[TypeConverter(typeof(ColorConverter))]
	[Serializable]
	public struct Color : IEquatable<Color>
	{
		private uint packedValue;

		/// <summary>
		/// Create a color with the specified red, green and blue channels. Alpha will be set to 255.
		/// </summary>
		/// <param name="r">Red color channel, ranging between 0-255.</param>
		/// <param name="g">Green color channel, ranging between 0-255.</param>
		/// <param name="b">Blue color channel, ranging between 0-255.</param>
		public Color(int r, int g, int b)
		{
			// Clamp values if needed
			if ((((r | g) | b) & -256) != 0)
			{
				r = r < 0 ? 0 : (r > 255 ? 255 : r);
				g = g < 0 ? 0 : (g > 255 ? 255 : g);
				b = b < 0 ? 0 : (b > 255 ? 255 : b);
			}

			packedValue = (uint)(((r | g << 8) | b << 16) | -16777216);
		}

		/// <summary>
		/// Create a color with the specified red, green, blue and alpha channels.
		/// </summary>
		/// <param name="r">Red color channel, ranging between 0-255.</param>
		/// <param name="g">Green color channel, ranging between 0-255.</param>
		/// <param name="b">Blue color channel, ranging between 0-255.</param>
		/// <param name="a">Alpha color channel, ranging between 0-255.</param>
		public Color(int r, int g, int b, int a)
		{
			// Clamp values if needed
			if (((((r | g) | b) | a) & -256) != 0)
			{
				r = r < 0 ? 0 : (r > 255 ? 255 : r);
				g = g < 0 ? 0 : (g > 255 ? 255 : g);
				b = b < 0 ? 0 : (b > 255 ? 255 : b);
				a = a < 0 ? 0 : (a > 255 ? 255 : a);
			}

			packedValue = (uint)(((r | g << 8) | b << 16) | a << 24);
		}

		/// <summary>
		/// Create a color with the specified red, green and blue channels. Alpha will be set to 1.0.
		/// </summary>
		/// <param name="r">Red color channel, ranging between 0 - 1.</param>
		/// <param name="g">Green color channel, ranging between 0 - 1.</param>
		/// <param name="b">Blue color channel, ranging between 0 - 1.</param>
		public Color(float r, float g, float b)
		{
			packedValue = ColorPack(r, g, b, 1.0f);
		}

		/// <summary>
		/// Create a color with the specified red, green, blue and alpha channels.
		/// </summary>
		/// <param name="r">Red color channel, ranging between 0 - 1.</param>
		/// <param name="g">Green color channel, ranging between 0 - 1.</param>
		/// <param name="b">Blue color channel, ranging between 0 - 1.</param>
		/// <param name="a">Alpha color channel, ranging between 0 - 1.</param>
		public Color(float r, float g, float b, float a)
		{
			packedValue = ColorPack(r, g, b, a);
		}

		/// <summary>
		/// Create a color with the specified red, green and blue channels. Alpha will be set to 1.0.
		/// </summary>
		/// <param name="vector">Vector with R, G and B channels.</param>
		public Color(Vector3 vector)
			: this(vector.X, vector.Y, vector.Z)
		{
		}

		/// <summary>
		/// Create a color from the packed, internal value representation.
		/// </summary>
		private Color(uint packedValue)
		{
			this.packedValue = packedValue;
		}

		/// <summary>
		/// Create a vector containing the R, G and B color channels.
		/// </summary>
		/// <returns>A vector of the R, G and B color channels.</returns>
		public Vector3 ToVector3()
		{
			Vector3 result;

			result.X = (packedValue & 255);
			result.Y = (packedValue >> 8 & 255);
			result.Z = (packedValue >> 16 & 255);

			result /= 0xff;

			return result;
		}

		/// <summary>
		/// The packed, internal color representation.
		/// </summary>
		public uint PackedValue { get { return packedValue; } set { packedValue = value; } }

		/// <summary>
		/// Get or set the red color channel.
		/// </summary>
		public byte R { get { return (byte)packedValue; } set { packedValue = (packedValue & 0xffffff00) | value; } }

		/// <summary>
		/// Get or set the green color channel.
		/// </summary>
		public byte G { get { return (byte)(packedValue >> 8); } set { packedValue = (packedValue & 0xffff00ff) | ((uint)(value << 8)); } }

		/// <summary>
		/// Get or set the blue color channel.
		/// </summary>
		public byte B { get { return (byte)(packedValue >> 0x10); } set { packedValue = (packedValue & 0xff00ffff) | ((uint)(value << 0x10)); } }

		/// <summary>
		/// Get or set the alpha color channel.
		/// </summary>
		public byte A { get { return (byte)(packedValue >> 0x18); } set { packedValue = (packedValue & 0xffffff) | ((uint)(value << 0x18)); } }
		
		public static Color AliceBlue { get { return new Color(240, 248, 255, 255); } }

		public static Color AntiqueWhite { get { return new Color(250, 235, 215, 255); } }

		public static Color Aqua { get { return new Color(0, 255, 255, 255); } }

		public static Color Aquamarine { get { return new Color(127, 255, 212, 255); } }

		public static Color Azure { get { return new Color(240, 255, 255, 255); } }

		public static Color Beige { get { return new Color(245, 245, 220, 255); } }

		public static Color Bisque { get { return new Color(255, 228, 196, 255); } }

		public static Color Black { get { return new Color(0, 0, 0, 255); } }

		public static Color BlanchedAlmond { get { return new Color(255, 235, 205, 255); } }

		public static Color Blue { get { return new Color(0, 0, 255, 255); } }

		public static Color BlueViolet { get { return new Color(138, 43, 226, 255); } }

		public static Color Brown { get { return new Color(165, 42, 42, 255); } }

		public static Color BurlyWood { get { return new Color(222, 184, 135, 255); } }

		public static Color CadetBlue { get { return new Color(95, 158, 160, 255); } }

		public static Color Chartreuse { get { return new Color(127, 255, 0, 255); } }

		public static Color Chocolate { get { return new Color(210, 105, 30, 255); } }

		public static Color Coral { get { return new Color(255, 127, 80, 255); } }

		public static Color CornflowerBlue { get { return new Color(0xffed9564); } }

		public static Color Cornsilk { get { return new Color(255, 248, 220, 255); } }

		public static Color Crimson { get { return new Color(220, 20, 60, 255); } }

		public static Color Cyan { get { return new Color(0, 255, 255, 255); } }

		public static Color DarkBlue { get { return new Color(0, 0, 139, 255); } }

		public static Color DarkCyan { get { return new Color(0, 139, 139, 255); } }

		public static Color DarkGoldenrod { get { return new Color(184, 134, 11, 255); } }

		public static Color DarkGray { get { return new Color(169, 169, 169, 255); } }

		public static Color DarkGreen { get { return new Color(0, 100, 0, 255); } }

		public static Color DarkKhaki { get { return new Color(189, 183, 107, 255); } }

		public static Color DarkMagenta { get { return new Color(139, 0, 139, 255); } }

		public static Color DarkOliveGreen { get { return new Color(85, 107, 47, 255); } }

		public static Color DarkOrange { get { return new Color(255, 140, 0, 255); } }

		public static Color DarkOrchid { get { return new Color(153, 50, 204, 255); } }

		public static Color DarkRed { get { return new Color(139, 0, 0, 255); } }

		public static Color DarkSalmon { get { return new Color(233, 150, 122, 255); } }

		public static Color DarkSeaGreen { get { return new Color(143, 188, 139, 255); } }

		public static Color DarkSlateBlue { get { return new Color(72, 61, 139, 255); } }

		public static Color DarkSlateGray { get { return new Color(47, 79, 79, 255); } }

		public static Color DarkTurquoise { get { return new Color(0, 206, 209, 255); } }

		public static Color DarkViolet { get { return new Color(148, 0, 211, 255); } }

		public static Color DeepPink { get { return new Color(255, 20, 147, 255); } }

		public static Color DeepSkyBlue { get { return new Color(0, 191, 255, 255); } }

		public static Color DimGray { get { return new Color(105, 105, 105, 255); } }

		public static Color DodgerBlue { get { return new Color(30, 144, 255, 255); } }

		public static Color Firebrick { get { return new Color(178, 34, 34, 255); } }

		public static Color FloralWhite { get { return new Color(255, 250, 240, 255); } }

		public static Color ForestGreen { get { return new Color(34, 139, 34, 255); } }

		public static Color Fuchsia { get { return new Color(255, 0, 255, 255); } }

		public static Color Gainsboro { get { return new Color(220, 220, 220, 255); } }

		public static Color GhostWhite { get { return new Color(248, 248, 255, 255); } }

		public static Color Gold { get { return new Color(255, 215, 0, 255); } }

		public static Color Goldenrod { get { return new Color(218, 165, 32, 255); } }

		public static Color Gray { get { return new Color(128, 128, 128, 255); } }

		public static Color Green { get { return new Color(0, 128, 0, 255); } }

		public static Color GreenYellow { get { return new Color(173, 255, 47, 255); } }

		public static Color Honeydew { get { return new Color(240, 255, 240, 255); } }

		public static Color HotPink { get { return new Color(255, 105, 180, 255); } }

		public static Color IndianRed { get { return new Color(205, 92, 92, 255); } }

		public static Color Indigo { get { return new Color(75, 0, 130, 255); } }

		public static Color Ivory { get { return new Color(255, 255, 240, 255); } }

		public static Color Khaki { get { return new Color(240, 230, 140, 255); } }

		public static Color Lavender { get { return new Color(230, 230, 250, 255); } }

		public static Color LavenderBlush { get { return new Color(255, 240, 245, 255); } }

		public static Color LawnGreen { get { return new Color(124, 252, 0, 255); } }

		public static Color LemonChiffon { get { return new Color(255, 250, 205, 255); } }

		public static Color LightBlue { get { return new Color(173, 216, 230, 255); } }

		public static Color LightCoral { get { return new Color(240, 128, 128, 255); } }

		public static Color LightCyan { get { return new Color(224, 255, 255, 255); } }

		public static Color LightGoldenrodYellow { get { return new Color(250, 250, 210, 255); } }

		public static Color LightGray { get { return new Color(211, 211, 211, 255); } }

		public static Color LightGreen { get { return new Color(144, 238, 144, 255); } }

		public static Color LightPink { get { return new Color(255, 182, 193, 255); } }

		public static Color LightSalmon { get { return new Color(255, 160, 122, 255); } }

		public static Color LightSeaGreen { get { return new Color(32, 178, 170, 255); } }

		public static Color LightSkyBlue { get { return new Color(135, 206, 250, 255); } }

		public static Color LightSlateGray { get { return new Color(119, 136, 153, 255); } }

		public static Color LightSteelBlue { get { return new Color(176, 196, 222, 255); } }

		public static Color LightYellow { get { return new Color(255, 255, 224, 255); } }

		public static Color Lime { get { return new Color(0, 255, 0, 255); } }

		public static Color LimeGreen { get { return new Color(50, 205, 50, 255); } }

		public static Color Linen { get { return new Color(250, 240, 230, 255); } }

		public static Color Magenta { get { return new Color(255, 0, 255, 255); } }

		public static Color Maroon { get { return new Color(128, 0, 0, 255); } }

		public static Color MediumAquamarine { get { return new Color(102, 205, 170, 255); } }

		public static Color MediumBlue { get { return new Color(0, 0, 205, 255); } }

		public static Color MediumOrchid { get { return new Color(186, 85, 211, 255); } }

		public static Color MediumPurple { get { return new Color(147, 112, 219, 255); } }

		public static Color MediumSeaGreen { get { return new Color(60, 179, 113, 255); } }

		public static Color MediumSlateBlue { get { return new Color(123, 104, 238, 255); } }

		public static Color MediumSpringGreen { get { return new Color(0, 250, 154, 255); } }

		public static Color MediumTurquoise { get { return new Color(72, 209, 204, 255); } }

		public static Color MediumVioletRed { get { return new Color(199, 21, 133, 255); } }

		public static Color MidnightBlue { get { return new Color(25, 25, 112, 255); } }

		public static Color MintCream { get { return new Color(245, 255, 250, 255); } }

		public static Color MistyRose { get { return new Color(255, 228, 225, 255); } }

		public static Color Moccasin { get { return new Color(255, 228, 181, 255); } }

		public static Color NavajoWhite { get { return new Color(255, 222, 173, 255); } }

		public static Color Navy { get { return new Color(0, 0, 128, 255); } }

		public static Color OldLace { get { return new Color(253, 245, 230, 255); } }

		public static Color Olive { get { return new Color(128, 128, 0, 255); } }

		public static Color OliveDrab { get { return new Color(107, 142, 35, 255); } }

		public static Color Orange { get { return new Color(255, 165, 0, 255); } }

		public static Color OrangeRed { get { return new Color(255, 69, 0, 255); } }

		public static Color Orchid { get { return new Color(218, 112, 214, 255); } }

		public static Color PaleGoldenrod { get { return new Color(238, 232, 170, 255); } }

		public static Color PaleGreen { get { return new Color(152, 251, 152, 255); } }

		public static Color PaleTurquoise { get { return new Color(175, 238, 238, 255); } }

		public static Color PaleVioletRed { get { return new Color(219, 112, 147, 255); } }

		public static Color PapayaWhip { get { return new Color(255, 239, 213, 255); } }

		public static Color PeachPuff { get { return new Color(255, 218, 185, 255); } }

		public static Color Peru { get { return new Color(205, 133, 63, 255); } }

		public static Color Pink { get { return new Color(255, 192, 203, 255); } }

		public static Color Plum { get { return new Color(221, 160, 221, 255); } }

		public static Color PowderBlue { get { return new Color(176, 224, 230, 255); } }

		public static Color Purple { get { return new Color(128, 0, 128, 255); } }

		public static Color Red { get { return new Color(255, 0, 0, 255); } }

		public static Color RosyBrown { get { return new Color(188, 143, 143, 255); } }

		public static Color RoyalBlue { get { return new Color(65, 105, 225, 255); } }

		public static Color SaddleBrown { get { return new Color(139, 69, 19, 255); } }

		public static Color Salmon { get { return new Color(250, 128, 114, 255); } }

		public static Color SandyBrown { get { return new Color(244, 164, 96, 255); } }

		public static Color SeaGreen { get { return new Color(46, 139, 87, 255); } }

		public static Color SeaShell { get { return new Color(255, 245, 238, 255); } }

		public static Color Sienna { get { return new Color(160, 82, 45, 255); } }

		public static Color Silver { get { return new Color(192, 192, 192, 255); } }

		public static Color SkyBlue { get { return new Color(135, 206, 235, 255); } }

		public static Color SlateBlue { get { return new Color(106, 90, 205, 255); } }

		public static Color SlateGray { get { return new Color(112, 128, 144, 255); } }

		public static Color Snow { get { return new Color(255, 250, 250, 255); } }

		public static Color SpringGreen { get { return new Color(0, 255, 127, 255); } }

		public static Color SteelBlue { get { return new Color(70, 130, 180, 255); } }

		public static Color Tan { get { return new Color(210, 180, 140, 255); } }

		public static Color Teal { get { return new Color(0, 128, 128, 255); } }

		public static Color Thistle { get { return new Color(216, 191, 216, 255); } }

		public static Color Tomato { get { return new Color(255, 99, 71, 255); } }

		public static Color Transparent { get { return new Color(0, 0, 0, 0); } }

		public static Color Turquoise { get { return new Color(64, 224, 208, 255); } }

		public static Color Violet { get { return new Color(238, 130, 238, 255); } }

		public static Color Wheat { get { return new Color(245, 222, 179, 255); } }

		public static Color White { get { return new Color(255, 255, 255, 255); } }

		public static Color WhiteSmoke { get { return new Color(245, 245, 245, 255); } }

		public static Color Yellow { get { return new Color(255, 255, 0, 255); } }

		public static Color YellowGreen { get { return new Color(154, 205, 50, 255); } }

		/// <summary>
		/// Test if this color equals another color.
		/// </summary>
		/// <param name="other">Color to test against.</param>
		/// <returns>True if this color is the same as the other color.</returns>
		public bool Equals(Color other)
		{
			return packedValue.Equals(other.packedValue);
		}

		/// <summary>
		/// Test if this color equals another color.
		/// </summary>
		/// <param name="other">Color to test against.</param>
		/// <returns>True if this color is the same as the other color.</returns>
		public override bool Equals(Object other)
		{
			if (other is Color)
			{
				return Equals((Color)other);
			}

			return false;
		}
		
		/// <summary>
		/// Test two colors for equality.
		/// </summary>
		/// <param name="a">First color.</param>
		/// <param name="b">Second color.</param>
		/// <returns>True if the two colors are equal.</returns>
		public static bool operator ==(Color a, Color b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Test two colors for inequality.
		/// </summary>
		/// <param name="a">First color.</param>
		/// <param name="b">Second color.</param>
		/// <returns>True if the two colors are not equal.</returns>
		public static bool operator !=(Color a, Color b)
		{
			return !a.Equals(b);
		}

		/// <summary>
		/// Create a color from a non-premultiplied source. In other words,
		/// multiply the alpha into the color channels and return the resulting color.
		/// </summary>
		/// <param name="r">Red color channel.</param>
		/// <param name="g">Green color channel.</param>
		/// <param name="b">Blue color channel.</param>
		/// <param name="a">Alpha color channel.</param>
		/// <returns>A premulitplied color structure.</returns>
		public static Color FromNonPremultiplied(int r, int g, int b, int a)
		{
			Color color;

			r = r < 0 ? 0 : (r > 255 ? 255 : r);
			g = g < 0 ? 0 : (g > 255 ? 255 : g);
			b = b < 0 ? 0 : (b > 255 ? 255 : b);

			r = (r * a) / 255;
			g = (g * a) / 255;
			b = (b * a) / 255;
			
			color.packedValue = (uint)(((r | g << 8) | b << 16) | a << 24);
			return color;
		}

		/// <summary>
		/// Create a color from a non-premultiplied source. In other words,
		/// multiply the alpha into the color channels and return the resulting color.
		/// </summary>
		/// <returns>A premulitplied color structure.</returns>
		public static Color FromNonPremultiplied(Color color)
		{
			return FromNonPremultiplied(color.R, color.G, color.B, color.A);
		}

		/// <summary>
		/// Fetch the hash code value.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return packedValue.GetHashCode();
		}

		/// <summary>
		/// Linearly interpolate between the two specified colors.
		/// </summary>
		/// <param name="value1">First color.</param>
		/// <param name="value2">Second color.</param>
		/// <param name="amount">Interpolation amount, usually ranging between 0-1.</param>
		/// <returns>The interpolated color.</returns>
		public static Color Lerp(Color value1, Color value2, float amount)
		{
			Color color;

			byte r1 = (byte)value1.packedValue;
			byte g1 = (byte)(value1.packedValue >> 8);
			byte b1 = (byte)(value1.packedValue >> 16);
			byte a1 = (byte)(value1.packedValue >> 24);

			byte r2 = (byte)value2.packedValue;
			byte g2 = (byte)(value2.packedValue >> 8);
			byte b2 = (byte)(value2.packedValue >> 16);
			byte a2 = (byte)(value2.packedValue >> 24);

			int factor = (int)Pack(65536f, amount);

			int r3 = r1 + (((r2 - r1) * factor) >> 16);
			int g3 = g1 + (((g2 - g1) * factor) >> 16);
			int b3 = b1 + (((b2 - b1) * factor) >> 16);
			int a3 = a1 + (((a2 - a1) * factor) >> 16);

			color.packedValue = (uint)(((r3 | (g3 << 8)) | (b3 << 16)) | (a3 << 24));

			return color;
		}

		/// <summary>
		/// Multiply the color value by the specified scale.
		/// </summary>
		/// <param name="value">Color to multiply.</param>
		/// <param name="scale">Scale value.</param>
		/// <returns>The modified color.</returns>
		public static Color Multiply(Color value, float scale)
		{
			Color color;

			uint r = (byte)value.packedValue;
			uint g = (byte)(value.packedValue >> 8);
			uint b = (byte)(value.packedValue >> 16);
			uint a = (byte)(value.packedValue >> 24);

			uint uintScale = (uint)MathTools.Clamp(scale * 65536f, 0, 0xffffff);

			r = (r * uintScale) >> 16;
			g = (g * uintScale) >> 16;
			b = (b * uintScale) >> 16;
			a = (a * uintScale) >> 16;

			r = r > 255 ? 255 : r;
			g = g > 255 ? 255 : g;
			b = b > 255 ? 255 : b;
			a = a > 255 ? 255 : a;

			color.packedValue = ((r | (g << 8)) | (b << 0x10)) | (a << 0x18);

			return color;
		}

		/// <summary>
		/// Multiply the color value by the specified scale.
		/// </summary>
		/// <returns>The modified color.</returns>
		public static Color operator *(Color a, float scale)
		{
			return Multiply(a, scale);
		}

		/// <summary>
		/// Convert this color to a string representation.
		/// </summary>
		/// <returns>A string representation of this color.</returns>
		public override string ToString()
		{
			return string.Format(
				CultureInfo.CurrentCulture, "{{R:{0} G:{1} B:{2} A:{3}}}", new object[] { this.R, this.G, this.B, this.A });
		}

		private static uint ColorPack(float r, float g, float b, float a)
		{
			uint pr = Pack(255.0f, r);
			uint pg = Pack(255.0f, g) << 8;
			uint pb = Pack(255.0f, b) << 16;
			uint pa = Pack(255.0f, a) << 24;

			return (((pr | pg) | pb) | pa);
		}

		private static uint Pack(float bitmask, float value)
		{
			value *= bitmask;

			if (float.IsNaN(value))
			{
				return 0;
			}

			if (float.IsInfinity(value))
			{
				return (uint)(float.IsNegativeInfinity(value) ? 0 : bitmask);
			}

			if (value < 0)
			{
				return 0;
			}

			return (uint)value;
		}
	}
}