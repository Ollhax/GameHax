using System;
using System.Text;

using MG.Framework.Utility;

namespace MG.Framework.Input
{
	struct InputBinding
	{
		public readonly Button Button;
		public readonly KeyModifier KeyModifier;

		public InputBinding(Button button, KeyModifier modifier)
		{
			Button = button;
			KeyModifier = modifier;
			
			switch (Button)
			{
				case Button.KeyShiftLeft:
				case Button.KeyShiftRight:
					KeyModifier &= ~KeyModifier.Shift;
					break;

				case Button.KeyControlLeft:
				case Button.KeyControlRight:
					KeyModifier &= ~KeyModifier.Ctrl;
					break;

				case Button.KeyAltLeft:
				case Button.KeyAltRight:
					KeyModifier &= ~KeyModifier.Alt;
					break;
			}
		}

		public InputBinding(string input)
		{
			// Parse modifier
			if (input.Contains("+"))
			{
				var tokens = input.Split(new[] { "+" }, StringSplitOptions.None);
				KeyModifier m = KeyModifier.None;

				// Parse all modifiers
				for (int i = 0; i < tokens.Length - 1; i++)
				{
					try
					{
						KeyModifier modifier = tokens[i].TryParse<string, KeyModifier>();
						m &= ~KeyModifier.Any; // If we've set a modifier, we don't want the Any-flag set (or the newly set modifier would not matter).
						m |= modifier;
					}
					catch (Exception)
					{
						throw new ArgumentException("Invalid key modifier: " + tokens[i]);
					}
				}

				KeyModifier = m;

				// Final token is the input
				input = tokens[tokens.Length - 1];
			}
			else
			{
				KeyModifier = KeyModifier.Any;
			}

			// Parse button
			Button = ButtonHelpers.FromString(input);
		}
		
		public override string ToString()
		{
			var builder = new StringBuilder();

			if (KeyModifier != KeyModifier.Any)
			{
				if (KeyModifier == KeyModifier.None) builder.Append("None+");
				if ((KeyModifier & KeyModifier.Ctrl) != 0) builder.Append("Ctrl+");
				if ((KeyModifier & KeyModifier.Shift) != 0) builder.Append("Shift+");
				if ((KeyModifier & KeyModifier.Alt) != 0) builder.Append("Alt+");
			}

			if (Button != Button.LastButton)
			{
				builder.Append(ButtonHelpers.ToString(Button));
			}
			
			return builder.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var other = (InputBinding)obj;
			return (Button == other.Button && KeyModifier == other.KeyModifier);
		}

		public override int GetHashCode()
		{
			return Button.GetHashCode() * (int)KeyModifier;
		}
	}
}
