using System;
using System.Collections.Generic;
using System.Diagnostics;

using MG.Framework.Numerics;
using MG.Framework.Utility;

using OpenTK;
using OpenTK.Input;

namespace MG.Framework.Input
{
	/// <summary>
	/// Main input handler. Manages input contexts and dispatches input events every frame.
	/// </summary>
	public class InputHandler
	{
		private Dictionary<string, InputContext> contexts = new Dictionary<string, InputContext>();
		private List<InputContext> activeContexts = new List<InputContext>();
		private GameWindow window;
		private Input input = new Input();
		private HashSet<string> tempActions = new HashSet<string>();
		internal delegate void SystemValueChangedFunction();
		
		// Note: InputHandler is static per se, but should not have static (=global) access to all game classes. Thus, make only its internals static.
		internal static KeyModifier ModifierState;
		internal static Dictionary<string, double> SystemValues = new Dictionary<string, double>();
		internal static event SystemValueChangedFunction SystemValuesChanged;

		private KeyboardState lastKeyboardState;

		private bool triggerLeftActive;
		private bool triggerRightActive;
		private bool dpadLeftActive;
		private bool dpadRightActive;
		private bool dpadUpActive;
		private bool dpadDownActive;
		private int currentGamepadIndex = -1;
		private GamePadState lastGamepadState;
		private bool usingGamepad = false;
		private float lastWheel = 0;

		public delegate void InputCallback(Input input);
		public event InputCallback InputReceived;

		public void LoadContext(string contextFile, string contextName, int priority)
		{
			try
			{
				var context = new InputContext(contextName, priority);
				context.Load(contextFile);

				if (contexts.ContainsKey(context.Name))
				{
					Log.Warning("Duplicate input context name: " + context.Name);
				}
				else
				{
					contexts.Add(context.Name, context);
				}
			}
			catch (Exception e)
			{
				Log.Error("Error: " + e.Message);
			}
		}
		
		public void PushContext(string contextName)
		{
			InputContext context;
			if (contexts.TryGetValue(contextName, out context))
			{
				for (int i = 0; i < activeContexts.Count; i++)
				{
					var c = activeContexts[i];
					if (c.Priority > context.Priority)
					{
						activeContexts.Insert(i, context);
						return;
					}
				}

				activeContexts.Add(context);
				return;
			}

			Log.Error("Trying to add unknown context: " + contextName);
		}

		public void PopContext(string contextName)
		{
			if (activeContexts.Count > 0)
			{
				for (int i = activeContexts.Count - 1; i >= 0; i--)
				{
					var context = activeContexts[i];
					if (context.Name == contextName)
					{
						activeContexts.RemoveAt(i);
						input.ReleaseAll();
						return;
					}
				}

				Log.Error("Input context not on stack: " + contextName);
				return;
			}

			Log.Error("Trying to pop context from empty stack.");
		}
		
		public void UpdateScreenSize(MG.Framework.Numerics.Vector2 screenSize, RectangleF normalizedScreenArea)
		{
			SystemValues["{ScreenSizeX}"] = screenSize.X;
			SystemValues["{ScreenSizeY}"] = screenSize.Y;
			SystemValues["{NormalizedScreenAreaLeft}"] = normalizedScreenArea.Left;
			SystemValues["{NormalizedScreenAreaRight}"] = normalizedScreenArea.Right;
			SystemValues["{NormalizedScreenAreaTop}"] = normalizedScreenArea.Top;
			SystemValues["{NormalizedScreenAreaBottom}"] = normalizedScreenArea.Bottom;

			OnSystemValuesChanged();
		}

		public override string ToString()
		{
			string s = "";
			s += "Contexts: " + activeContexts.Count + "\n";
			for (int i = activeContexts.Count - 1; i >= 0; i--)
			{
				var context = activeContexts[i];
				s += i + ". " + context.Name + "\n";
			}

			s += "\n\nActions:\n";

			var activeActions = new List<string>();
			input.GetActiveActions(activeActions);
			foreach (var action in activeActions)
			{
				s += action + "\n";
			}

			s += "\n\nRanges:\n";

			foreach (var range in input.ActiveRanges)
			{
				s += range.Key + ": " + range.Value.Value + "\n";
			}

			return s;
		}

		internal InputHandler(GameWindow window)
		{
			this.window = window;
			
			window.Keyboard.KeyDown += WindowOnKeyDown;
			window.KeyPress += WindowOnKeyPress;
			window.Keyboard.KeyRepeat = true;
			
			window.Mouse.ButtonDown += MouseOnButtonDown;
			window.Mouse.ButtonUp += MouseOnButtonUp;
		}
		
		internal void Update(Time time)
		{
			var keyboard = window.Keyboard;

			ModifierState = KeyModifier.None;
			if (keyboard[Key.ControlLeft] || keyboard[Key.ControlRight]) ModifierState |= KeyModifier.Ctrl;
			if (keyboard[Key.ShiftLeft] || keyboard[Key.ShiftRight]) ModifierState |= KeyModifier.Shift;
			if (keyboard[Key.AltLeft] || keyboard[Key.AltRight]) ModifierState |= KeyModifier.Alt;

			if (window.Focused)
			{
				HandleKeyboard();
				HandleGamepads();
			}

			var wheel = window.Mouse.WheelPrecise;

			SetRange("MouseX", window.Mouse.X);
			SetRange("MouseY", window.Mouse.Y);
			SetRange("MouseWheel", wheel - lastWheel);
			
			lastWheel = wheel;
			input.Update(time);

			OnInputReceived();
		}
		
		internal void Clear()
		{
			input.Clear();
		}
		
		private void WindowOnKeyDown(object sender, KeyboardKeyEventArgs keyboardKeyEventArgs)
		{
			usingGamepad = false;
			KeyDown(keyboardKeyEventArgs.Key);
		}
		
		private void MouseOnButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			usingGamepad = false;
			ButtonDown(InputTranslation.Translate(mouseButtonEventArgs.Button));
		}

		private void MouseOnButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			usingGamepad = false;
			ButtonUp(InputTranslation.Translate(mouseButtonEventArgs.Button));
		}
		
		private void WindowOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
		{
			usingGamepad = false;
			KeyPress(keyPressEventArgs.KeyChar);
		}
		
		internal void OnSystemValuesChanged()
		{
			if (SystemValuesChanged != null)
			{
				SystemValuesChanged.Invoke();
			}
		}

		internal bool SetRange(string range, double value)
		{
			for (int i = activeContexts.Count - 1; i >= 0; i--)
			{
				var context = activeContexts[i];
				if (context.TranslateAxis(range, value, input.ActiveRanges))
				{
					return true;
				}
			}
			return false;
		}
		
		internal void ButtonDown(Button button)
		{
			for (int i = activeContexts.Count - 1; i >= 0; i--)
			{
				var context = activeContexts[i];
				if (context.TranslateButton(button, ModifierState, tempActions))
				{
					foreach (var action in tempActions)
					{
						input.SetAction(action, true);
					}

					tempActions.Clear();
					break;
				}
			}
		}

		internal void ButtonUp(Button button)
		{
			for (int i = activeContexts.Count - 1; i >= 0; i--)
			{
				var context = activeContexts[i];
				if (context.TranslateButton(button, KeyModifier.Any, tempActions)) // Don't care about modifier when it comes to releasing buttons
				{
					foreach (var action in tempActions)
					{
						input.SetAction(action, false);
					}

					tempActions.Clear();
					break;
				}
			}
		}

		internal void KeyDown(Key key)
		{
			switch (key)
			{
				case Key.BackSpace:
					KeyPress('\b');
					break;

				case Key.Enter:
					KeyPress('\r');
					break;
			}
		}
		
		internal void KeyPress(char key)
		{
			input.RawCharacters.Append(key);
		}
		
		private void HandleKeyboard()
		{
			var state = Keyboard.GetState();

			for (int i = 0; i < (int)Key.LastKey; i++)
			{
				var key = (Key)i;
				bool down = state.IsKeyDown(key);
				bool lastDown = lastKeyboardState.IsKeyDown(key);

				if (down && !lastDown)
				{
					ButtonDown(InputTranslation.Translate(key));
				}
				if (!down && lastDown)
				{
					ButtonUp(InputTranslation.Translate(key));
				}
			}
			lastKeyboardState = state;
		}

		private void HandleGamepads()
		{
			// Handle non-connected or disconnected gamepads
			if (currentGamepadIndex < 0 || !GamePad.GetState(currentGamepadIndex).IsConnected)
			{
				currentGamepadIndex = -1;
				
				for (int i = 0; i < 4; i++)
				{
					var state = GamePad.GetState(i);
					if (state.IsConnected)
					{
						currentGamepadIndex = i;
						lastGamepadState = state;
						break;
					}
				}

				if (currentGamepadIndex >= 0)
				{
					usingGamepad = true;
					Log.Info("Gamepad connected: " + currentGamepadIndex + "\n" + GamePad.GetCapabilities(currentGamepadIndex));
				}
			}

			if (currentGamepadIndex < 0) usingGamepad = false;

			// Poll gamepad
			SetRange("Gamepad.Enabled", usingGamepad ? 1 : 0);

			if (currentGamepadIndex >= 0)
			{
				var newState = GamePad.GetState(currentGamepadIndex);
				if (!newState.Equals(lastGamepadState))
				{
					usingGamepad = true;
					if      (newState.Buttons.A == ButtonState.Pressed  && lastGamepadState.Buttons.A == ButtonState.Released) ButtonDown(Button.GamepadA);
					else if (newState.Buttons.A == ButtonState.Released && lastGamepadState.Buttons.A == ButtonState.Pressed)  ButtonUp  (Button.GamepadA);
					if      (newState.Buttons.B == ButtonState.Pressed  && lastGamepadState.Buttons.B == ButtonState.Released) ButtonDown(Button.GamepadB);
					else if (newState.Buttons.B == ButtonState.Released && lastGamepadState.Buttons.B == ButtonState.Pressed)  ButtonUp  (Button.GamepadB);
					if      (newState.Buttons.X == ButtonState.Pressed  && lastGamepadState.Buttons.X == ButtonState.Released) ButtonDown(Button.GamepadX);
					else if (newState.Buttons.X == ButtonState.Released && lastGamepadState.Buttons.X == ButtonState.Pressed)  ButtonUp  (Button.GamepadX);
					if      (newState.Buttons.Y == ButtonState.Pressed  && lastGamepadState.Buttons.Y == ButtonState.Released) ButtonDown(Button.GamepadY);
					else if (newState.Buttons.Y == ButtonState.Released && lastGamepadState.Buttons.Y == ButtonState.Pressed)  ButtonUp  (Button.GamepadY);
					if      (newState.Buttons.Start == ButtonState.Pressed  && lastGamepadState.Buttons.Start == ButtonState.Released) ButtonDown(Button.GamepadStart);
					else if (newState.Buttons.Start == ButtonState.Released && lastGamepadState.Buttons.Start == ButtonState.Pressed)  ButtonUp  (Button.GamepadStart);
					if      (newState.Buttons.Back == ButtonState.Pressed  && lastGamepadState.Buttons.Back == ButtonState.Released) ButtonDown(Button.GamepadStart);
					else if (newState.Buttons.Back == ButtonState.Released && lastGamepadState.Buttons.Back == ButtonState.Pressed)  ButtonUp  (Button.GamepadBack);
					if      (newState.Buttons.BigButton == ButtonState.Pressed  && lastGamepadState.Buttons.BigButton == ButtonState.Released) ButtonDown(Button.GamepadGuide);
					else if (newState.Buttons.BigButton == ButtonState.Released && lastGamepadState.Buttons.BigButton == ButtonState.Pressed)  ButtonUp  (Button.GamepadGuide);
					if      (newState.Buttons.LeftShoulder == ButtonState.Pressed  && lastGamepadState.Buttons.LeftShoulder == ButtonState.Released) ButtonDown(Button.GamepadBumperLeft);
					else if (newState.Buttons.LeftShoulder == ButtonState.Released && lastGamepadState.Buttons.LeftShoulder == ButtonState.Pressed)  ButtonUp  (Button.GamepadBumperLeft);
					if      (newState.Buttons.RightShoulder == ButtonState.Pressed && lastGamepadState.Buttons.RightShoulder == ButtonState.Released) ButtonDown(Button.GamepadBumperRight);
					else if (newState.Buttons.RightShoulder == ButtonState.Released && lastGamepadState.Buttons.RightShoulder == ButtonState.Pressed) ButtonUp  (Button.GamepadBumperRight);
					if      (newState.Buttons.LeftStick == ButtonState.Pressed && lastGamepadState.Buttons.LeftStick == ButtonState.Released) ButtonDown(Button.GamepadStickLeft);
					else if (newState.Buttons.LeftStick == ButtonState.Released && lastGamepadState.Buttons.LeftStick == ButtonState.Pressed) ButtonUp  (Button.GamepadStickLeft);
					if      (newState.Buttons.RightStick == ButtonState.Pressed && lastGamepadState.Buttons.RightStick == ButtonState.Released) ButtonDown(Button.GamepadStickRight);
					else if (newState.Buttons.RightStick == ButtonState.Released && lastGamepadState.Buttons.RightStick == ButtonState.Pressed) ButtonUp(Button.GamepadStickRight);
					
					if      (newState.DPad.Left == ButtonState.Pressed &&  lastGamepadState.DPad.Left == ButtonState.Released) ButtonDown(Button.GamepadDpadLeft);
					else if (newState.DPad.Left == ButtonState.Released && lastGamepadState.DPad.Left == ButtonState.Pressed) ButtonUp   (Button.GamepadDpadLeft);
					if      (newState.DPad.Right == ButtonState.Pressed &&  lastGamepadState.DPad.Right == ButtonState.Released) ButtonDown(Button.GamepadDpadRight);
					else if (newState.DPad.Right == ButtonState.Released && lastGamepadState.DPad.Right == ButtonState.Pressed) ButtonUp(Button.GamepadDpadRight);
					if      (newState.DPad.Down == ButtonState.Pressed &&  lastGamepadState.DPad.Down == ButtonState.Released) ButtonDown(Button.GamepadDpadDown);
					else if (newState.DPad.Down == ButtonState.Released && lastGamepadState.DPad.Down == ButtonState.Pressed) ButtonUp(Button.GamepadDpadDown);
					if      (newState.DPad.Up == ButtonState.Pressed &&  lastGamepadState.DPad.Up == ButtonState.Released) ButtonDown(Button.GamepadDpadUp);
					else if (newState.DPad.Up == ButtonState.Released && lastGamepadState.DPad.Up == ButtonState.Pressed) ButtonUp(Button.GamepadDpadUp);
					
					lastGamepadState = newState;
				}
				
				SetRange("Gamepad.LeftStickX", newState.ThumbSticks.Left.X);
				SetRange("Gamepad.LeftStickY", -newState.ThumbSticks.Left.Y);
				SetRange("Gamepad.RightStickX", newState.ThumbSticks.Right.X);
				SetRange("Gamepad.RightStickY", -newState.ThumbSticks.Right.Y);
				
				const float triggerAmount = 0.3f;
				if (newState.Triggers.Left > triggerAmount)
				{
					if (!triggerLeftActive)
					{
						ButtonDown(Button.GamepadTriggerLeft);
						triggerLeftActive = true;
					}
				}
				else if (triggerLeftActive)
				{
					ButtonUp(Button.GamepadTriggerLeft);
					triggerLeftActive = false;
				}

				if (newState.Triggers.Right > triggerAmount)
				{
					if (!triggerRightActive)
					{
						ButtonDown(Button.GamepadTriggerRight);
						triggerRightActive = true;
					}
				}
				else if (triggerRightActive)
				{
					ButtonUp(Button.GamepadTriggerRight);
					triggerRightActive = false;
				}

				if (newState.ThumbSticks.Left.X > triggerAmount)
				{
					if (!dpadRightActive)
					{
						ButtonDown(Button.GamepadDpadRight);
						dpadRightActive = true;
					}					
				}
				else if (dpadRightActive)
				{
					ButtonUp(Button.GamepadDpadRight);
					dpadRightActive = false;
				}

				if (newState.ThumbSticks.Left.X < -triggerAmount)
				{
					if (!dpadLeftActive)
					{
						ButtonDown(Button.GamepadDpadLeft);
						dpadLeftActive = true;
					}					
				}
				else if (dpadLeftActive)
				{
					ButtonUp(Button.GamepadDpadLeft);
					dpadLeftActive = false;
				}

				if (newState.ThumbSticks.Left.Y > triggerAmount)
				{
					if (!dpadUpActive)
					{
						ButtonDown(Button.GamepadDpadUp);
						dpadUpActive = true;
					}
				}
				else if (dpadUpActive)
				{
					ButtonUp(Button.GamepadDpadUp);
					dpadUpActive = false;
				}

				if (newState.ThumbSticks.Left.Y < -triggerAmount)
				{
					if (!dpadDownActive)
					{
						ButtonDown(Button.GamepadDpadDown);
						dpadDownActive = true;
					}					
				}
				else if (dpadDownActive)
				{
					ButtonUp(Button.GamepadDpadDown);
					dpadDownActive = false;
				}
			}
			
			//// Add new joysticks
			//foreach (var joystick in window.Joysticks)
			//{
			//    if (!joysticks.Contains(joystick))
			//    {
			//        Log.Info("New joystick found: " + joystick.Description);

			//        joystick.ButtonDown += JoystickButtonDown;
			//        joystick.ButtonUp += JoystickButtonUp;
			//        joysticks.Add(joystick);
			//    }
			//}

			//// Remove old joysticks
			//for (int i = joysticks.Count - 1; i >= 0; i--)
			//{
			//    var joystick = joysticks[i];
				
			//    //if (!joystick.Connected)
			//    //{
			//    //    Log.Info("Removed joystick: " + joystick.Description);

			//    //    joystick.ButtonDown -= JoystickButtonDown;
			//    //    joystick.ButtonUp -= JoystickButtonUp;
			//    //    joysticks.RemoveAt(i);
			//    //}
			//}
		}

		private void OnInputReceived()
		{
			if (InputReceived != null)
			{
				InputReceived.Invoke(input);
			}
		}
	}
}