using System;
using System.Collections.Generic;
using System.Text;

using MG.Framework.Utility;

namespace MG.Framework.Input
{
	/// <summary>
	/// Represents the user's input for one frame.
	/// </summary>
	public class Input
	{
		internal class ActionState
		{
			[Flags]
			public enum StateType
			{
				None = 0,
				Active = 1 << 1,
				Down = 1 << 2,
				Pressed = 1 << 3,
				Released = 1 << 4,
				Repeat = 1 << 5,
			}

			public float KeyRepeatTime;
			public StateType State;

			public void Clear()
			{
				KeyRepeatTime = 0;
				State = StateType.None;
			}
		}

		internal Dictionary<string, Range> ActiveRanges = new Dictionary<string, Range>();

		private Dictionary<string, ActionState> activeActions = new Dictionary<string, ActionState>();
		private const float keyRepeatDelay = 0.4f;
		private const float keyRepeatInterval = 0.05f;

		public StringBuilder RawCharacters = new StringBuilder(128);
		
		public bool GetAction(string action)
		{
			return GetAction(action, ActionState.StateType.Down, true);
		}

		public bool GetActionRepeat(string action)
		{
			return GetAction(action, ActionState.StateType.Repeat | ActionState.StateType.Pressed, true);
		}

		public bool GetActionPressed(string action)
		{
			return GetAction(action, ActionState.StateType.Pressed, true);
		}

		public bool GetActionReleased(string action)
		{
			return GetAction(action, ActionState.StateType.Released, true);
		}

		public bool PeekAction(string action)
		{
			return GetAction(action, ActionState.StateType.Down, false);
		}

		public bool PeekActionRepeat(string action)
		{
			return GetAction(action, ActionState.StateType.Repeat | ActionState.StateType.Pressed, false);
		}

		public bool PeekActionPressed(string action)
		{
			return GetAction(action, ActionState.StateType.Pressed, false);
		}

		public bool PeekActionReleased(string action)
		{
			return GetAction(action, ActionState.StateType.Released, false);
		}

		public double GetRange(string range)
		{
			Range r;
			if (ActiveRanges.TryGetValue(range, out r))
			{
				return r.Value;
			}
			return 0;
		}

		public float GetRangeF(string range)
		{
			return (float)GetRange(range);
		}

		public void Reset()
		{
			foreach (var action in activeActions)
			{
				action.Value.State &= ~ActionState.StateType.Active;
			}
			RawCharacters.Clear();
		}

		internal void Clear()
		{
			foreach (var action in activeActions)
			{
				if ((action.Value.State & ~ActionState.StateType.Active) != 0)
					action.Value.State |= ActionState.StateType.Active;

				if ((action.Value.State & ActionState.StateType.Released) != 0)
				{
					action.Value.Clear();
				}

				action.Value.State &= ~(ActionState.StateType.Pressed | ActionState.StateType.Repeat);
			}

			RawCharacters.Clear();
		}

		private bool GetAction(string action, ActionState.StateType flags, bool deactivate)
		{
			ActionState s;
			if (activeActions.TryGetValue(action, out s))
			{
				if ((s.State & ActionState.StateType.Active) != 0 && (s.State & flags) != 0)
				{
					if (deactivate)
					{
						s.State &= ~ActionState.StateType.Active;
					}
					
					return true;
				}
			}

			return false;
		}

		internal void GetActiveActions(List<string> actions)
		{
			foreach (var action in activeActions)
			{
				if ((action.Value.State & ActionState.StateType.Active) != 0)
				{
					actions.Add(action.Key);
				}
			}
		}
		
		internal void SetAction(string action, bool down)
		{
			ActionState s;
			if (!activeActions.TryGetValue(action, out s))
			{
				s = new ActionState();
			}
			
			if (down)
			{
				s.State = ActionState.StateType.Active | ActionState.StateType.Down | ActionState.StateType.Pressed;
				s.KeyRepeatTime = keyRepeatDelay;
			}
			else
			{
				s.State = ActionState.StateType.Active | ActionState.StateType.Released;
			}

			activeActions[action] = s;
		}

		internal void Update(Time time)
		{
			foreach (var action in activeActions)
			{
				var state = action.Value;
				if ((state.State & ActionState.StateType.Down) != 0)
				{
					action.Value.KeyRepeatTime -= time.ElapsedSeconds;
					if (state.KeyRepeatTime <= 0)
					{
						action.Value.KeyRepeatTime += keyRepeatInterval;
						action.Value.State |= ActionState.StateType.Repeat;
					}
				}
			}
		}

		internal void ReleaseAll()
		{
			// Release all actions
			foreach (var action in activeActions)
			{
				if ((action.Value.State & ActionState.StateType.Down) != 0)
				{
					action.Value.State = ActionState.StateType.Active | ActionState.StateType.Released;
				}
			}

			ActiveRanges.Clear();
		}
	}
}
