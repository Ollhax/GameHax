using System.Collections.Generic;

using OpenTK.Input;

namespace MG.Framework.Input
{
	static class InputTranslation
	{
		private static Dictionary<Key, Button> translateKey = new Dictionary<Key, Button>();
		private static Dictionary<Button, Key> translateKeyInverse = new Dictionary<Button, Key>();

		private static Dictionary<MouseButton, Button> translateMouse = new Dictionary<MouseButton, Button>();
		private static Dictionary<Button, MouseButton> translateMouseInverse = new Dictionary<Button, MouseButton>();

		private static Dictionary<JoystickButton, Button> translateJoystick = new Dictionary<JoystickButton, Button>();
		private static Dictionary<Button, JoystickButton> translateJoystickInverse = new Dictionary<Button, JoystickButton>();

		static void Register(Key key, Button button)
		{
			translateKey.Add(key, button);
			translateKeyInverse.Add(button, key);
		}

		static void Register(MouseButton mouseButton, Button button)
		{
			translateMouse.Add(mouseButton, button);
			translateMouseInverse.Add(button, mouseButton);
		}

		static void Register(JoystickButton joystick, Button button)
		{
			translateJoystick.Add(joystick, button);
			translateJoystickInverse.Add(button, joystick);
		}

		static InputTranslation()
		{
			Register(Key.ShiftLeft, Button.KeyShiftLeft);
			Register(Key.ShiftRight, Button.KeyShiftRight);
			Register(Key.ControlLeft, Button.KeyControlLeft);
			Register(Key.ControlRight, Button.KeyControlRight);
			Register(Key.AltLeft, Button.KeyAltLeft);
			Register(Key.AltRight, Button.KeyAltRight);
			Register(Key.WinLeft, Button.KeyWinLeft);
			Register(Key.WinRight, Button.KeyWinRight);
			Register(Key.Menu, Button.KeyMenu);
			Register(Key.F1, Button.KeyF1);
			Register(Key.F2, Button.KeyF2);
			Register(Key.F3, Button.KeyF3);
			Register(Key.F4, Button.KeyF4);
			Register(Key.F5, Button.KeyF5);
			Register(Key.F6, Button.KeyF6);
			Register(Key.F7, Button.KeyF7);
			Register(Key.F8, Button.KeyF8);
			Register(Key.F9, Button.KeyF9);
			Register(Key.F10, Button.KeyF10);
			Register(Key.F11, Button.KeyF11);
			Register(Key.F12, Button.KeyF12);
			Register(Key.F13, Button.KeyF13);
			Register(Key.F14, Button.KeyF14);
			Register(Key.F15, Button.KeyF15);
			Register(Key.F16, Button.KeyF16);
			Register(Key.F17, Button.KeyF17);
			Register(Key.F18, Button.KeyF18);
			Register(Key.F19, Button.KeyF19);
			Register(Key.F20, Button.KeyF20);
			Register(Key.F21, Button.KeyF21);
			Register(Key.F22, Button.KeyF22);
			Register(Key.F23, Button.KeyF23);
			Register(Key.F24, Button.KeyF24);
			Register(Key.F25, Button.KeyF25);
			Register(Key.F26, Button.KeyF26);
			Register(Key.F27, Button.KeyF27);
			Register(Key.F28, Button.KeyF28);
			Register(Key.F29, Button.KeyF29);
			Register(Key.F30, Button.KeyF30);
			Register(Key.F31, Button.KeyF31);
			Register(Key.F32, Button.KeyF32);
			Register(Key.F33, Button.KeyF33);
			Register(Key.F34, Button.KeyF34);
			Register(Key.F35, Button.KeyF35);
			Register(Key.Up, Button.KeyUp);
			Register(Key.Down, Button.KeyDown);
			Register(Key.Left, Button.KeyLeft);
			Register(Key.Right, Button.KeyRight);
			Register(Key.Enter, Button.KeyEnter);
			Register(Key.Escape, Button.KeyEscape);
			Register(Key.Space, Button.KeySpace);
			Register(Key.Tab, Button.KeyTab);
			Register(Key.BackSpace, Button.KeyBackSpace);
			Register(Key.Insert, Button.KeyInsert);
			Register(Key.Delete, Button.KeyDelete);
			Register(Key.PageUp, Button.KeyPageUp);
			Register(Key.PageDown, Button.KeyPageDown);
			Register(Key.Home, Button.KeyHome);
			Register(Key.End, Button.KeyEnd);
			Register(Key.CapsLock, Button.KeyCapsLock);
			Register(Key.ScrollLock, Button.KeyScrollLock);
			Register(Key.PrintScreen, Button.KeyPrintScreen);
			Register(Key.Pause, Button.KeyPause);
			Register(Key.NumLock, Button.KeyNumLock);
			Register(Key.Clear, Button.KeyClear);
			Register(Key.Sleep, Button.KeySleep);
			Register(Key.Keypad0, Button.KeyKeypad0);
			Register(Key.Keypad1, Button.KeyKeypad1);
			Register(Key.Keypad2, Button.KeyKeypad2);
			Register(Key.Keypad3, Button.KeyKeypad3);
			Register(Key.Keypad4, Button.KeyKeypad4);
			Register(Key.Keypad5, Button.KeyKeypad5);
			Register(Key.Keypad6, Button.KeyKeypad6);
			Register(Key.Keypad7, Button.KeyKeypad7);
			Register(Key.Keypad8, Button.KeyKeypad8);
			Register(Key.Keypad9, Button.KeyKeypad9);
			Register(Key.KeypadDivide, Button.KeyKeypadDivide);
			Register(Key.KeypadMultiply, Button.KeyKeypadMultiply);
			Register(Key.KeypadSubtract, Button.KeyKeypadSubtract);
			Register(Key.KeypadAdd, Button.KeyKeypadAdd);
			Register(Key.KeypadDecimal, Button.KeyKeypadDecimal);
			Register(Key.KeypadEnter, Button.KeyKeypadEnter);
			Register(Key.A, Button.KeyA);
			Register(Key.B, Button.KeyB);
			Register(Key.C, Button.KeyC);
			Register(Key.D, Button.KeyD);
			Register(Key.E, Button.KeyE);
			Register(Key.F, Button.KeyF);
			Register(Key.G, Button.KeyG);
			Register(Key.H, Button.KeyH);
			Register(Key.I, Button.KeyI);
			Register(Key.J, Button.KeyJ);
			Register(Key.K, Button.KeyK);
			Register(Key.L, Button.KeyL);
			Register(Key.M, Button.KeyM);
			Register(Key.N, Button.KeyN);
			Register(Key.O, Button.KeyO);
			Register(Key.P, Button.KeyP);
			Register(Key.Q, Button.KeyQ);
			Register(Key.R, Button.KeyR);
			Register(Key.S, Button.KeyS);
			Register(Key.T, Button.KeyT);
			Register(Key.U, Button.KeyU);
			Register(Key.V, Button.KeyV);
			Register(Key.W, Button.KeyW);
			Register(Key.X, Button.KeyX);
			Register(Key.Y, Button.KeyY);
			Register(Key.Z, Button.KeyZ);
			Register(Key.Number0, Button.KeyNumber0);
			Register(Key.Number1, Button.KeyNumber1);
			Register(Key.Number2, Button.KeyNumber2);
			Register(Key.Number3, Button.KeyNumber3);
			Register(Key.Number4, Button.KeyNumber4);
			Register(Key.Number5, Button.KeyNumber5);
			Register(Key.Number6, Button.KeyNumber6);
			Register(Key.Number7, Button.KeyNumber7);
			Register(Key.Number8, Button.KeyNumber8);
			Register(Key.Number9, Button.KeyNumber9);
			Register(Key.Tilde, Button.KeyTilde);
			Register(Key.Minus, Button.KeyMinus);
			Register(Key.Plus, Button.KeyPlus);
			Register(Key.BracketLeft, Button.KeyBracketLeft);
			Register(Key.BracketRight, Button.KeyBracketRight);
			Register(Key.Semicolon, Button.KeySemicolon);
			Register(Key.Quote, Button.KeyQuote);
			Register(Key.Comma, Button.KeyComma);
			Register(Key.Period, Button.KeyPeriod);
			Register(Key.Slash, Button.KeySlash);
			Register(Key.BackSlash, Button.KeyBackSlash);

			Register(MouseButton.Left, Button.MouseLeft);
			Register(MouseButton.Middle, Button.MouseMiddle);
			Register(MouseButton.Right, Button.MouseRight);
			Register(MouseButton.Button1, Button.MouseButton1);
			Register(MouseButton.Button2, Button.MouseButton2);
			Register(MouseButton.Button3, Button.MouseButton3);
			Register(MouseButton.Button4, Button.MouseButton4);
			Register(MouseButton.Button5, Button.MouseButton5);
			Register(MouseButton.Button6, Button.MouseButton6);
			Register(MouseButton.Button7, Button.MouseButton7);
			Register(MouseButton.Button8, Button.MouseButton8);
			Register(MouseButton.Button9, Button.MouseButton9);
		}

		public static Button Translate(Key key)
		{
			Button b;
			if (translateKey.TryGetValue(key, out b)) return b;
			return Button.LastButton;
		}

		public static Button Translate(MouseButton mouseButton)
		{
			Button b;
			if (translateMouse.TryGetValue(mouseButton, out b)) return b;
			return Button.LastButton;
		}
	}
}
