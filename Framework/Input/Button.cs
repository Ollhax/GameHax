using System;
using System.Collections.Generic;

namespace MG.Framework.Input
{
	/// <summary>
	/// A physical button.
	/// </summary>
	public enum Button
	{
		// Keyboard buttons
		KeyShiftLeft,
		KeyShiftRight,
		KeyControlLeft,
		KeyControlRight,
		KeyAltLeft,
		KeyAltRight,
		KeyWinLeft,
		KeyWinRight,
		KeyMenu,
		KeyF1,
		KeyF2,
		KeyF3,
		KeyF4,
		KeyF5,
		KeyF6,
		KeyF7,
		KeyF8,
		KeyF9,
		KeyF10,
		KeyF11,
		KeyF12,
		KeyF13,
		KeyF14,
		KeyF15,
		KeyF16,
		KeyF17,
		KeyF18,
		KeyF19,
		KeyF20,
		KeyF21,
		KeyF22,
		KeyF23,
		KeyF24,
		KeyF25,
		KeyF26,
		KeyF27,
		KeyF28,
		KeyF29,
		KeyF30,
		KeyF31,
		KeyF32,
		KeyF33,
		KeyF34,
		KeyF35,
		KeyUp,
		KeyDown,
		KeyLeft,
		KeyRight,
		KeyEnter,
		KeyEscape,
		KeySpace,
		KeyTab,
		KeyBackSpace,
		KeyInsert,
		KeyDelete,
		KeyPageUp,
		KeyPageDown,
		KeyHome,
		KeyEnd,
		KeyCapsLock,
		KeyScrollLock,
		KeyPrintScreen,
		KeyPause,
		KeyNumLock,
		KeyClear,
		KeySleep,
		KeyKeypad0,
		KeyKeypad1,
		KeyKeypad2,
		KeyKeypad3,
		KeyKeypad4,
		KeyKeypad5,
		KeyKeypad6,
		KeyKeypad7,
		KeyKeypad8,
		KeyKeypad9,
		KeyKeypadDivide,
		KeyKeypadMultiply,
		KeyKeypadSubtract,
		KeyKeypadAdd,
		KeyKeypadDecimal,
		KeyKeypadEnter,
		KeyA,
		KeyB,
		KeyC,
		KeyD,
		KeyE,
		KeyF,
		KeyG,
		KeyH,
		KeyI,
		KeyJ,
		KeyK,
		KeyL,
		KeyM,
		KeyN,
		KeyO,
		KeyP,
		KeyQ,
		KeyR,
		KeyS,
		KeyT,
		KeyU,
		KeyV,
		KeyW,
		KeyX,
		KeyY,
		KeyZ,
		KeyNumber0,
		KeyNumber1,
		KeyNumber2,
		KeyNumber3,
		KeyNumber4,
		KeyNumber5,
		KeyNumber6,
		KeyNumber7,
		KeyNumber8,
		KeyNumber9,
		KeyTilde,
		KeyMinus,
		KeyPlus,
		KeyBracketLeft,
		KeyBracketRight,
		KeySemicolon,
		KeyQuote,
		KeyComma,
		KeyPeriod,
		KeySlash,
		KeyBackSlash,

		// Mouse buttons
		MouseLeft,
		MouseMiddle,
		MouseRight,
		MouseButton1,
		MouseButton2,
		MouseButton3,
		MouseButton4,
		MouseButton5,
		MouseButton6,
		MouseButton7,
		MouseButton8,
		MouseButton9,

		// Gamepad buttons
		GamepadA,
		GamepadB,
		GamepadX,
		GamepadY,
		GamepadBack,
		GamepadStart,
		GamepadGuide,
		GamepadStickLeft,
		GamepadStickRight,
		GamepadBumperLeft,
		GamepadBumperRight,
		GamepadTriggerLeft,
		GamepadTriggerRight,
		GamepadDpadLeft,
		GamepadDpadRight,
		GamepadDpadUp,
		GamepadDpadDown,

		LastButton
	}

	public static class ButtonHelpers
	{
		private static Dictionary<Button, string> buttonToName = new Dictionary<Button, string>();
		private static Dictionary<string, Button> nameToButton = new Dictionary<string, Button>(StringComparer.InvariantCultureIgnoreCase);
		
		private static void Register(Button b, string s)
		{
			buttonToName.Add(b, s);
			nameToButton.Add(s, b);
		}

		static ButtonHelpers()
		{
			Register(Button.KeyShiftLeft, "Key.ShiftLeft");
			Register(Button.KeyShiftRight, "Key.ShiftRight");
			Register(Button.KeyControlLeft, "Key.ControlLeft");
			Register(Button.KeyControlRight, "Key.ControlRight");
			Register(Button.KeyAltLeft, "Key.AltLeft");
			Register(Button.KeyAltRight, "Key.AltRight");
			Register(Button.KeyWinLeft, "Key.WinLeft");
			Register(Button.KeyWinRight, "Key.WinRight");
			Register(Button.KeyMenu, "Key.Menu");
			Register(Button.KeyF1, "Key.F1");
			Register(Button.KeyF2, "Key.F2");
			Register(Button.KeyF3, "Key.F3");
			Register(Button.KeyF4, "Key.F4");
			Register(Button.KeyF5, "Key.F5");
			Register(Button.KeyF6, "Key.F6");
			Register(Button.KeyF7, "Key.F7");
			Register(Button.KeyF8, "Key.F8");
			Register(Button.KeyF9, "Key.F9");
			Register(Button.KeyF10, "Key.F10");
			Register(Button.KeyF11, "Key.F11");
			Register(Button.KeyF12, "Key.F12");
			Register(Button.KeyF13, "Key.F13");
			Register(Button.KeyF14, "Key.F14");
			Register(Button.KeyF15, "Key.F15");
			Register(Button.KeyF16, "Key.F16");
			Register(Button.KeyF17, "Key.F17");
			Register(Button.KeyF18, "Key.F18");
			Register(Button.KeyF19, "Key.F19");
			Register(Button.KeyF20, "Key.F20");
			Register(Button.KeyF21, "Key.F21");
			Register(Button.KeyF22, "Key.F22");
			Register(Button.KeyF23, "Key.F23");
			Register(Button.KeyF24, "Key.F24");
			Register(Button.KeyF25, "Key.F25");
			Register(Button.KeyF26, "Key.F26");
			Register(Button.KeyF27, "Key.F27");
			Register(Button.KeyF28, "Key.F28");
			Register(Button.KeyF29, "Key.F29");
			Register(Button.KeyF30, "Key.F30");
			Register(Button.KeyF31, "Key.F31");
			Register(Button.KeyF32, "Key.F32");
			Register(Button.KeyF33, "Key.F33");
			Register(Button.KeyF34, "Key.F34");
			Register(Button.KeyF35, "Key.F35");
			Register(Button.KeyUp, "Key.Up");
			Register(Button.KeyDown, "Key.Down");
			Register(Button.KeyLeft, "Key.Left");
			Register(Button.KeyRight, "Key.Right");
			Register(Button.KeyEnter, "Key.Enter");
			Register(Button.KeyEscape, "Key.Escape");
			Register(Button.KeySpace, "Key.Space");
			Register(Button.KeyTab, "Key.Tab");
			Register(Button.KeyBackSpace, "Key.BackSpace");
			Register(Button.KeyInsert, "Key.Insert");
			Register(Button.KeyDelete, "Key.Delete");
			Register(Button.KeyPageUp, "Key.PageUp");
			Register(Button.KeyPageDown, "Key.PageDown");
			Register(Button.KeyHome, "Key.Home");
			Register(Button.KeyEnd, "Key.End");
			Register(Button.KeyCapsLock, "Key.CapsLock");
			Register(Button.KeyScrollLock, "Key.ScrollLock");
			Register(Button.KeyPrintScreen, "Key.PrintScreen");
			Register(Button.KeyPause, "Key.Pause");
			Register(Button.KeyNumLock, "Key.NumLock");
			Register(Button.KeyClear, "Key.Clear");
			Register(Button.KeySleep, "Key.Sleep");
			Register(Button.KeyKeypad0, "Key.Keypad0");
			Register(Button.KeyKeypad1, "Key.Keypad1");
			Register(Button.KeyKeypad2, "Key.Keypad2");
			Register(Button.KeyKeypad3, "Key.Keypad3");
			Register(Button.KeyKeypad4, "Key.Keypad4");
			Register(Button.KeyKeypad5, "Key.Keypad5");
			Register(Button.KeyKeypad6, "Key.Keypad6");
			Register(Button.KeyKeypad7, "Key.Keypad7");
			Register(Button.KeyKeypad8, "Key.Keypad8");
			Register(Button.KeyKeypad9, "Key.Keypad9");
			Register(Button.KeyKeypadDivide, "Key.KeypadDivide");
			Register(Button.KeyKeypadMultiply, "Key.KeypadMultiply");
			Register(Button.KeyKeypadSubtract, "Key.KeypadSubtract");
			Register(Button.KeyKeypadAdd, "Key.KeypadAdd");
			Register(Button.KeyKeypadDecimal, "Key.KeypadDecimal");
			Register(Button.KeyKeypadEnter, "Key.KeypadEnter");
			Register(Button.KeyA, "Key.A");
			Register(Button.KeyB, "Key.B");
			Register(Button.KeyC, "Key.C");
			Register(Button.KeyD, "Key.D");
			Register(Button.KeyE, "Key.E");
			Register(Button.KeyF, "Key.F");
			Register(Button.KeyG, "Key.G");
			Register(Button.KeyH, "Key.H");
			Register(Button.KeyI, "Key.I");
			Register(Button.KeyJ, "Key.J");
			Register(Button.KeyK, "Key.K");
			Register(Button.KeyL, "Key.L");
			Register(Button.KeyM, "Key.M");
			Register(Button.KeyN, "Key.N");
			Register(Button.KeyO, "Key.O");
			Register(Button.KeyP, "Key.P");
			Register(Button.KeyQ, "Key.Q");
			Register(Button.KeyR, "Key.R");
			Register(Button.KeyS, "Key.S");
			Register(Button.KeyT, "Key.T");
			Register(Button.KeyU, "Key.U");
			Register(Button.KeyV, "Key.V");
			Register(Button.KeyW, "Key.W");
			Register(Button.KeyX, "Key.X");
			Register(Button.KeyY, "Key.Y");
			Register(Button.KeyZ, "Key.Z");
			Register(Button.KeyNumber0, "Key.Number0");
			Register(Button.KeyNumber1, "Key.Number1");
			Register(Button.KeyNumber2, "Key.Number2");
			Register(Button.KeyNumber3, "Key.Number3");
			Register(Button.KeyNumber4, "Key.Number4");
			Register(Button.KeyNumber5, "Key.Number5");
			Register(Button.KeyNumber6, "Key.Number6");
			Register(Button.KeyNumber7, "Key.Number7");
			Register(Button.KeyNumber8, "Key.Number8");
			Register(Button.KeyNumber9, "Key.Number9");
			Register(Button.KeyTilde, "Key.Tilde");
			Register(Button.KeyMinus, "Key.Minus");
			Register(Button.KeyPlus, "Key.Plus");
			Register(Button.KeyBracketLeft, "Key.BracketLeft");
			Register(Button.KeyBracketRight, "Key.BracketRight");
			Register(Button.KeySemicolon, "Key.Semicolon");
			Register(Button.KeyQuote, "Key.Quote");
			Register(Button.KeyComma, "Key.Comma");
			Register(Button.KeyPeriod, "Key.Period");
			Register(Button.KeySlash, "Key.Slash");
			Register(Button.KeyBackSlash, "Key.BackSlash");
			
			Register(Button.MouseLeft, "Mouse.Left");
			Register(Button.MouseMiddle, "Mouse.Middle");
			Register(Button.MouseRight, "Mouse.Right");
			Register(Button.MouseButton1, "Mouse.Button1");
			Register(Button.MouseButton2, "Mouse.Button2");
			Register(Button.MouseButton3, "Mouse.Button3");
			Register(Button.MouseButton4, "Mouse.Button4");
			Register(Button.MouseButton5, "Mouse.Button5");
			Register(Button.MouseButton6, "Mouse.Button6");
			Register(Button.MouseButton7, "Mouse.Button7");
			Register(Button.MouseButton8, "Mouse.Button8");
			Register(Button.MouseButton9, "Mouse.Button9");
			
			Register(Button.GamepadA, "Gamepad.A");
			Register(Button.GamepadB, "Gamepad.B");
			Register(Button.GamepadX, "Gamepad.X");
			Register(Button.GamepadY, "Gamepad.Y");
			Register(Button.GamepadBack, "Gamepad.Back");
			Register(Button.GamepadStart, "Gamepad.Start");
			Register(Button.GamepadGuide, "Gamepad.Guide");
			Register(Button.GamepadStickLeft, "Gamepad.StickLeft");
			Register(Button.GamepadStickRight, "Gamepad.StickRight");
			Register(Button.GamepadBumperLeft, "Gamepad.BumperLeft");
			Register(Button.GamepadBumperRight, "Gamepad.BumperRight");
			Register(Button.GamepadTriggerLeft, "Gamepad.TriggerLeft");
			Register(Button.GamepadTriggerRight, "Gamepad.TriggerRight");
			Register(Button.GamepadDpadLeft, "Gamepad.DpadLeft");
			Register(Button.GamepadDpadRight, "Gamepad.DpadRight");
			Register(Button.GamepadDpadUp, "Gamepad.DpadUp");
			Register(Button.GamepadDpadDown, "Gamepad.DpadDown");
			Register(Button.LastButton, "<invalid>");
		}

		public static string ToString(Button button)
		{
			return buttonToName[button];
		}

		public static Button FromString(string name)
		{
			Button b;
			if (nameToButton.TryGetValue(name, out b)) return b;
			return Button.LastButton;
		}
	}
}
