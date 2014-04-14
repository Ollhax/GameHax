using System;
using System.Collections.Generic;
using System.Text;

using MG.Framework.Numerics;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// FontLayouts help you draw text to the screen. Given a font, text, wrapping rules and so on,
	/// the FontLayout creates a structure that other classes (such as the QuadBatch) can use to
	/// quickly output text.
	/// </summary>
	public class FontLayout
	{
		private string textString;
		private Font font;
		private Vector2 position;
		private Vector2 size;
		private Color color;
		private LayoutModeType layoutMode;
		private LayoutOptionsType layoutOptions;
		private AlignmentType alignment;
		private RectangleF textBounds;
		
		private struct StringProxy
		{
			private string text;
			private List<Character> layout;
			private StringBuilder stringBuilder;

			public StringProxy(string text)
			{
				this.text = text;
				this.layout = null;
				this.stringBuilder = null;
			}

			public StringProxy(List<Character> layout)
			{
				this.text = null;
				this.layout = layout;
				this.stringBuilder = null;
			}

			public StringProxy(StringBuilder stringBuilder)
			{
				this.text = null;
				this.layout = null;
				this.stringBuilder = stringBuilder;
			}


			public char this[int index]
			{
				get
				{
					if (text != null)
					{
						return text[index];
					}
					if (stringBuilder != null)
					{
						return stringBuilder[index];
					}

					return layout[index].Char;
				}
			}

			public int Length
			{
				get
				{
					if (text != null)
					{
						return text.Length;
					}
					if (stringBuilder != null)
					{
						return stringBuilder.Length;
					}

					return layout.Count;
				}
			}
		}

		/// <summary>
		/// Different modes for the layout engine.
		/// </summary>
		public enum LayoutModeType
		{
			Single, // Write the entire string in one line. Newline characters are ignored.
			Multiline, // Split the line if vertical room allows it. Newline characters are used.
			WordWrap, // Split the line if vertical room allows it, at word boundries. Newline characters are used.
		}
		
		/// <summary>
		/// Different options for the layout engine.
		/// </summary>
		[Flags]
		public enum LayoutOptionsType
		{
			None = 0,
			AutoScale = 1 << 0, // Changes the way the font handles overflowing text. Instead of just cropping it, it scales down the final text to fit within the specified bounds.
		}
		
		/// <summary>
		/// A layout character.
		/// </summary>
		public struct Character
		{
			public Texture2D Texture;
			public RectangleF Source;
			public Vector2 Offset;
			public Color Color;
			public int Line;
			public char Char;
		}
		
		/// <summary>
		/// The current layout.
		/// </summary>
		public List<Character> Layout { get; protected set; }

		/// <summary>
		/// Create a new layout instance.
		/// </summary>
		public FontLayout()
		{
			Layout = new List<Character>();
		}
		
		/// <summary>
		/// Create a new layout instance with all settings preset.
		/// </summary>
		/// <param name="font">Font to use.</param>
		/// <param name="text">The text to write.</param>
		/// <param name="area">The area in which the text should be written.</param>
		/// <param name="color">The color of the text.</param>
		/// <param name="alignment">How the text should be aligned within the text area.</param>
		/// <param name="layoutMode">How the engine should handle multiple lines.</param>
		/// <param name="layoutOptions">Different options for the layout engine.</param>
		public FontLayout(Font font, string text, RectangleF area, Color color, AlignmentType alignment = AlignmentType.NorthWest, LayoutModeType layoutMode = LayoutModeType.WordWrap, LayoutOptionsType layoutOptions = 0)
			: this()
		{
			Set(font, text, area, color, alignment, layoutMode, layoutOptions);
		}

		/// <summary>
		/// Create a copy of another layout.
		/// </summary>
		public FontLayout(FontLayout other)
			: this()
		{
			Set(other.font, other.textString, other.Area, other.color, other.alignment, other.layoutMode, other.layoutOptions);
		}
		
		/// <summary>
		/// Set a specific layout.
		/// </summary>
		/// <param name="font">Font to use.</param>
		/// <param name="text">The text to write.</param>
		/// <param name="area">The area in which the text should be written.</param>
		/// <param name="color">The color of the text.</param>
		/// <param name="alignment">How the text should be aligned within the text area.</param>
		/// <param name="layoutMode">How the engine should handle multiple lines.</param>
		/// <param name="layoutOptions">Different options for the layout engine.</param>
		public void Set(Font font, string text, RectangleF area, Color color, AlignmentType alignment = AlignmentType.NorthWest, LayoutModeType layoutMode = LayoutModeType.WordWrap, LayoutOptionsType layoutOptions = 0)
		{
			if (this.font != font ||
				textString != text ||
				!MathTools.Equals(position, area.TopLeft) ||
				!MathTools.Equals(size, area.Size) ||
				this.color != color ||
				this.alignment != alignment ||
				this.layoutMode != layoutMode ||
				this.layoutOptions != layoutOptions)
			{
				this.font = font;
				textString = text;
				position = area.TopLeft;
				size = area.Size;
				this.color = color;
				this.alignment = alignment;
				this.layoutMode = layoutMode;
				this.layoutOptions = layoutOptions;

				UpdateLayout();
			}
		}

		/// <summary>
		/// Clear out all values.
		/// </summary>
		public void Clear()
		{
			textString = "";
			Layout.Clear();
		}

		/// <summary>
		/// Get or set the font used by this layout.
		/// </summary>
		public Font Font
		{
			get { return font; }
			set
			{
				if (font != value)
				{
					font = value;
					UpdateLayout();
				}
			}
		}

		/// <summary>
		/// Get or set the position for this layout.
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		/// <summary>
		/// Get or set the scale for this layout.
		/// </summary>
		public Vector2 Scale { get; set; }

		/// <summary>
		/// Get or set the size of for this layout.
		/// </summary>
		public Vector2 Size
		{
			get { return size; }
			set
			{
				if (!MathTools.Equals(size, value))
				{
					size = value;
					UpdateLayout();
				}
			}
		}
		
		/// <summary>
		/// Get or set the area for this layout. 
		/// </summary>
		public RectangleF Area
		{
			get { return new RectangleF(position.X, position.Y, size.X, size.Y); }
			set
			{
				position = value.TopLeft;
				Size = value.Size;
			}
		}

		/// <summary>
		/// Get the smallest bounding box that contains all letters.
		/// </summary>
		public RectangleF TextBounds { get { return textBounds; } }

		/// <summary>
		/// Get or set the color for this layout.
		/// </summary>
		public Color Color
		{
			get { return color; }
			set
			{
				if (color != value)
				{
					color = value;
					UpdateLayout();
				}
			}
		}

		/// <summary>
		/// Get or set the layout mode for this layout.
		/// </summary>
		public LayoutModeType LayoutMode
		{
			get { return layoutMode; }
			set
			{
				if (layoutMode != value)
				{
					layoutMode = value;
					UpdateLayout();
				}
			}
		}

		/// <summary>
		/// Get or set the layout options for this layout.
		/// </summary>
		public LayoutOptionsType LayoutOptions
		{
			get { return layoutOptions; }
			set
			{
				if (layoutOptions != value)
				{
					layoutOptions = value;
					UpdateLayout();
				}
			}
		}

		/// <summary>
		/// Get or set the text alignment for this layout.
		/// </summary>
		public AlignmentType Alignment
		{
			get { return alignment; }
			set
			{
				if (alignment != value)
				{
					alignment = value;
					UpdateAlignment();
				}
			}
		}
		
		/// <summary>
		/// Measure a string's length. Handles newlines correctly.
		/// </summary>
		/// <param name="font">Font to use.</param>
		/// <param name="text">Text to measure.</param>
		/// <returns>The size of the display area.</returns>
		public static Vector2 MeasureString(Font font, string text) { return MeasureString(font, new StringProxy(text), 0, text.Length - 1); }

		/// <summary>
		/// Measure a string's length, using a StringBuilder. Handles newlines correctly.
		/// </summary>
		/// <param name="font">Font to use.</param>
		/// <param name="text">Text to measure.</param>
		/// <returns>The size of the display area.</returns>
		public static Vector2 MeasureString(Font font, StringBuilder text) { return MeasureString(font, new StringProxy(text), 0, text.Length - 1); }
		
		private static Vector2 MeasureString(Font font, StringProxy text, int start, int end)
		{
			if (text.Length == 0) return Vector2.Zero;

			float currentLength = 0;
			float maxLength = 0;
			
			int lines = 1;

			for (int i = start; i <= end; i++)
			{
				char character = text[i];

				if (character != '\r')
				{
					if (character != '\n')
					{
						var fontChar = font.GetFontChar(character);
						int length = fontChar.XAdvance;
						
						if (i < text.Length - 1)
						{
							length += font.GetKerning(character, text[i + 1]);
						}

						currentLength += length;
						maxLength = Math.Max(maxLength, currentLength);
					}
					else
					{
						currentLength = 0;
						lines++;
					}
				}
			}

			return new Vector2(maxLength, lines * font.Definition.Common.LineHeight);
		}
		
		private bool IsWordBreak(char c) { return c == ' ' || c == '\n'; }
		
		private void UpdateLayout()
		{
			Layout.Clear();
			//DebugCharacters.Clear();

			if (Size.X <= 0 || size.Y <= 0) return;
			if (string.IsNullOrEmpty(textString)) return;

			Scale = Vector2.One;
			var line = 0;
			var lineHeight = font.Definition.Common.LineHeight;
			var currentPosition = Vector2.Zero;
			
			for (int characterIndex = 0; characterIndex < textString.Length; )
			{
				// Get the current character
				char character = textString[characterIndex];

				// Ignore certain characters
				if (character == '\r')
				{
					characterIndex++;
					continue;
				}

				// If the character is a newline, create a newline and continue (consume the newline char).
				if (layoutMode != LayoutModeType.Single && character == '\n')
				{
					line++;
					currentPosition.X = 0;
					currentPosition.Y += lineHeight;
					characterIndex++;
					continue;
				}

				// Figure out how long the current word is
				int wordEnd = characterIndex;
				while (true)
				{
					if (wordEnd + 1 >= textString.Length || IsWordBreak(textString[wordEnd + 1])) break;
					wordEnd++;
				}

				float wordLength = MeasureString(font, new StringProxy(textString), characterIndex, wordEnd).X;
				
				// If the word fits within a single line, but not at the current location, start a new line.
				if (layoutMode == LayoutModeType.WordWrap && wordLength < size.X && wordLength + currentPosition.X > size.X)
				{
					line++;
					currentPosition.X = 0;
					currentPosition.Y += lineHeight;

					if (IsWordBreak(character))
					{
						characterIndex++; // Consume the following whitespace
						continue;
					}
				}

				// If we do not have vertical room for the character, quit
				if ((layoutOptions & LayoutOptionsType.AutoScale) == 0 && currentPosition.Y + lineHeight > size.Y)
				{
					break;
				}
				
				// If we do not have horizontal room for the character, create newline and restart (keep the current char).
				var fontChar = Font.GetFontChar(character);
				
				// Calculate the kerning
				int kerning = 0;
				if (characterIndex < textString.Length - 1)
				{
					kerning = Font.GetKerning(character, textString[characterIndex + 1]);
				}

				if ((layoutOptions & LayoutOptionsType.AutoScale) == 0 && currentPosition.X + fontChar.XAdvance > size.X)
				{
					if (layoutMode != LayoutModeType.Single)
					{
						line++;
						currentPosition.X = 0;
						currentPosition.Y += lineHeight;
						continue;
					}

					break;
				}

				// Create the character
				var page = Font.pages[fontChar.Page];
				var newChar = new Character
				{
					Color = color,
					Offset = new Vector2(currentPosition.X + fontChar.XOffset, currentPosition.Y + fontChar.YOffset),
					Source = new RectangleF(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height),
					Texture = page,
					Line = line,
					Char = character
				};

				Layout.Add(newChar);

				// Move ahead, applying kerning if there is any
				currentPosition.X += fontChar.XAdvance + kerning;
				characterIndex++;
			}
						
			// Scale text to fit the bounds
			if ((layoutOptions & LayoutOptionsType.AutoScale) != 0 && Layout.Count > 0)
			{
				// Figure out how large are we're covering
				var maxSize = Vector2.Zero;

				foreach (var c in Layout)
				{
					maxSize.X = Math.Max(maxSize.X, c.Offset.X + c.Source.Width);
					maxSize.Y = Math.Max(maxSize.Y, c.Offset.Y + c.Source.Height);
				}

				// Find out what is poking out the most
				var scale = maxSize / size;

				if (scale.X > 1 || scale.Y > 1)
				{
					float invScale = 1.0f / Math.Max(scale.X, scale.Y);
					Scale = new Vector2(invScale, invScale);
				}
			}

			UpdateAlignment();

			//for (int i = 0; i < Layout.Count; i++)
			//{
			//    //RectangleF r = new RectangleF();
			//    //r.SetPosition(Layout[i].Offset + position);
			//    //r.SetSize(Layout[i].Source.GetSize());

			//    var fontChar = Font.GetFontChar(Layout[i].Char);
			//    var bounds = new RectangleF();
			//    bounds.SetPosition(Layout[i].Offset - new Vector2(fontChar.XOffset, fontChar.YOffset) + position);
			//    bounds.SetSize(new Vector2(fontChar.XAdvance, Font.Definition.Common.LineHeight));

			//    DebugCharacters.Add(bounds);
			//}
		}

		//public List<RectangleF> DebugCharacters = new List<RectangleF>();

		private void UpdateAlignment()
		{
			if (Layout.Count == 0) return;

			// First, align horizontally
			for (int i = 0; i < Layout.Count; )
			{
				// Figure out the length of the current line
				int lineStart = i;
				int lineEnd = Layout.Count - 1;

				for (int j = lineStart; j < Layout.Count; j++)
				{
					if (Layout[i].Line != Layout[j].Line)
					{
						lineEnd = j - 1;
						break;
					}
				}
				
				// Get the bounds of the current line
				var lineArea = GetLayoutBounds(lineStart, lineEnd);
				var offset = Area.GetAlignmentOffset(alignment, lineArea.Size) - lineArea.Position;

				for (int j = lineStart; j <= lineEnd; j++)
				{
					var layout = Layout[j];
					layout.Offset.X += offset.X;
					Layout[j] = layout;
				}

				// Move ahead
				i = lineEnd + 1;
			}

			// Then, align vertically
			{
				var fullBounds = GetLayoutBounds(0, Layout.Count - 1);
				var offset = Area.GetAlignmentOffset(alignment, fullBounds.Size) - fullBounds.Position;
				
				for (int j = 0; j < Layout.Count; j++)
				{
					var layout = Layout[j];
					layout.Offset.Y += offset.Y;
					Layout[j] = layout;
				}
			}

			// Fetch latest bounds
			textBounds = GetLayoutBounds(0, Layout.Count - 1);
		}

		private RectangleF GetLayoutBounds(int start, int end)
		{
			var topLeft = new Vector2(float.MaxValue, float.MaxValue);
			var bottomRight = new Vector2(float.MinValue, float.MinValue);

			for (int i = start; i <= end; i++)
			{
				var fontChar = Font.GetFontChar(Layout[i].Char);
				var tl = Layout[i].Offset - new Vector2(fontChar.XOffset, fontChar.YOffset) + position;
				var br = tl + new Vector2(fontChar.XAdvance, Font.Definition.Common.LineHeight);

				if (tl.X < topLeft.X)
				{
					topLeft.X = tl.X;
				}
				if (tl.Y < topLeft.Y)
				{
					topLeft.Y = tl.Y;
				}
				if (br.X > bottomRight.X)
				{
					bottomRight.X = br.X;
				}
				if (br.Y > bottomRight.Y)
				{
					bottomRight.Y = br.Y;
				}
			}

			return RectangleF.ConstructSpanning(topLeft, bottomRight);
		}
	}
}
