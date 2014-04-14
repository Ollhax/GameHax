using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MG.Framework.Graphics
{
	public class Font
	{
		internal Dictionary<int, Texture2D> pages = new Dictionary<int, Texture2D>();
		private Dictionary<char, FontChar> fontDictionary;
		private Dictionary<int, Dictionary<int, int>> kernings;
		
		/// <summary>
		/// Fetch the FondDefinition used by this font.
		/// </summary>
		public FontDefinition Definition { get; protected set; }

		/// <summary>
		/// Fetch a cached FontChar by character. This is only for convenience/performance; it's the same information contained in Definition.
		/// </summary>
		public char DefaultCharacter { get; set; }
		
		/// <summary>
		/// Return a FontChar for the specified character, or the default FontChar if none is found.
		/// </summary>
		/// <param name="character">The character used to match against a FontChar.</param>
		/// <returns>A FontChar</returns>
		public FontChar GetFontChar(char character)
		{
			FontChar ret;
			if (fontDictionary.TryGetValue(character, out ret))
			{
				return ret;
			}

			if (fontDictionary.TryGetValue(DefaultCharacter, out ret))
			{
				return ret;
			}
			
			throw new ArgumentException("Font " + Definition.Info.Face + " does not contain the character: '" + character + "'.");
		}

		/// <summary>
		/// Return the kerning amount between two characters.
		/// </summary>
		/// <param name="first">First character.</param>
		/// <param name="second">First character.</param>
		/// <returns>The amount of kerning between the two characters. This may be zero.</returns>
		public int GetKerning(char first, char second)
		{
			Dictionary<int, int> d;
			if (kernings.TryGetValue(first, out d))
			{
				int kerningAmount;
				if (d.TryGetValue(second, out kerningAmount))
				{
					return kerningAmount;
				}
			}

			return 0;
		}

		/// <summary>
		/// Create a font from a specified font definition file.
		/// </summary>
		/// <param name="file">File to open.</param>
		public Font(string file)
		{
			DefaultCharacter = ' ';

			// Load definition
			using (var textReader = new StreamReader(file))
			{
				var deserializer = new XmlSerializer(typeof(FontDefinition));
				Definition = (FontDefinition)deserializer.Deserialize(textReader);
			}

			// Cache the fontchars
			fontDictionary = new Dictionary<char, FontChar>(Definition.Chars.Count);
			foreach (var c in Definition.Chars)
			{
				if (c.ID > char.MaxValue)
				{
					throw new ArgumentException("Invalid character id in font file: " + file + ", id:" + c.ID);
				}

				fontDictionary.Add((char)c.ID, c);
			}

			// Cache the kernings
			kernings = new Dictionary<int, Dictionary<int, int>>(Definition.Kernings.Count);
			foreach (var k in Definition.Kernings)
			{
				Dictionary<int, int> d;
				if (kernings.TryGetValue(k.First, out d))
				{
					d.Add(k.Second, k.Amount);
				}
				else
				{
					d = new Dictionary<int, int>();
					d.Add(k.Second, k.Amount);
					kernings.Add(k.First, d);
				}
			}

			// Load pages (textures)
			foreach (var page in Definition.Pages)
			{
				var texture = new Texture2D(Path.Combine(Path.GetDirectoryName(file), page.File));
				pages.Add(page.ID, texture);
			}
		}
	}
}
