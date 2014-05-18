using System;
using System.Collections.Generic;

namespace MG.EditorCommon.Undo
{
	public class UndoHandler
	{
		private List<IUndoableAction> undoStack;
		private List<IUndoableAction> redoStack;
		private UndoableActionGroup currentGroup;
		
		/// <summary>
		/// Fetch the maximum number of undo steps saved by this handler.
		/// </summary>
		public int MaxUndoSteps { get; protected set; }

		/// <summary>
		/// Fetch the current number of undo steps.
		/// </summary>
		public int UndoSteps { get { return undoStack.Count; } }

		/// <summary>
		/// Fetch the current number of redo steps.
		/// </summary>
		public int RedoSteps { get { return redoStack.Count; } }

		/// <summary>
		/// Event called after actions are executed, undone or redone.
		/// </summary>
		public event Action AfterStateChanged;

		/// <summary>
		/// Event called before actions are executed, undone or redone.
		/// </summary>
		public event Action BeforeStateChanged;
		
		/// <summary>
		/// Event called when actions are undone.
		/// </summary>
		public event Action UndoEvent;

		/// <summary>
		/// Event called when actions are redone.
		/// </summary>
		public event Action RedoEvent;
		
		public UndoHandler(int maxUndoSteps)
		{
			if (maxUndoSteps < 0)
			{
				throw new ArgumentOutOfRangeException("Must have 0 or more undo steps!");
			}

			MaxUndoSteps = maxUndoSteps;
			undoStack = new List<IUndoableAction>(MaxUndoSteps);
			redoStack = new List<IUndoableAction>(MaxUndoSteps);
		}
		
		/// <summary>
		/// Clear entire undo state.
		/// </summary>
		public void Clear()
		{
			OnBeforeStateChanged();
			currentGroup = null;
			undoStack.Clear();
			redoStack.Clear();
			OnAfterStateChanged();
		}
		
		/// <summary>
		/// Execute a simple undoable action. It will be added to the undo stack if it succeeds (returns true).
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public bool ExecuteAction(IUndoableAction action)
		{
			return ExecuteActionInternal(action, true);
		}
		
		protected bool ExecuteActionInternal(IUndoableAction action, bool clearRedoStack)
		{
			OnBeforeStateChanged();

			if (!action.Execute())
			{
				return false;
			}
			
			if (clearRedoStack)
			{
				redoStack.Clear();
			}

			var undoGroup = action.GetUndoGroup();

			// Should we group together this undo step with more of the same kind?
			if (undoGroup != 0)
			{
				if (currentGroup == null || currentGroup.CaughtGroup != undoGroup)
				{
					currentGroup = new UndoableActionGroup(undoGroup);
					AddToUndoStack(currentGroup);
				}

				currentGroup.Actions.Add(action);
			}
			else
			{
				AddToUndoStack(action);
			}

			OnAfterStateChanged();
			return true;
		}

		private void AddToUndoStack(IUndoableAction action)
		{
			// Clear oldest actions if we're out of room
			if (undoStack.Count > 0 && undoStack.Count >= MaxUndoSteps)
			{
				undoStack.RemoveAt(0);
			}

			// Note: not certain that we got room, MaxUndoSteps may be zero
			if (undoStack.Count < MaxUndoSteps)
			{
				undoStack.Add(action);
			}
		}

		/// <summary>
		/// Force the current undo group to stop. For example, if you're painting some tiles and
		/// stop for a moment, you might want to split the undo group at that point in order
		/// to separate undo steps by stroke. Call this function to do so.
		/// </summary>
		/// <param name="group">Id of the group we want to end.</param>
		public void EndUndoGroup(int group)
		{
			if (currentGroup != null && currentGroup.CaughtGroup == group)
			{
				currentGroup = null;
			}
		}
		
		/// <summary>
		/// Undo a single action.
		/// </summary>
		public void Undo()
		{
			currentGroup = null;

			if (undoStack.Count > 0)
			{
				OnBeforeStateChanged();

				var action = undoStack[undoStack.Count-1];
				undoStack.RemoveAt(undoStack.Count - 1);
				
				action.Undo();
				redoStack.Add(action);
				
				OnUndoEvent();
				OnAfterStateChanged();
			}
		}
		
		/// <summary>
		/// Redo a single action.
		/// </summary>
		public void Redo()
		{
			currentGroup = null;

			if (redoStack.Count > 0)
			{
				var action = redoStack[redoStack.Count - 1];
				redoStack.RemoveAt(redoStack.Count - 1);
				
				ExecuteActionInternal(action, false);
				OnRedoEvent();
			}
		}
		
		protected void OnAfterStateChanged()
		{
			if (AfterStateChanged != null)
			{
				AfterStateChanged();
			}
		}

		protected void OnBeforeStateChanged()
		{
			if (BeforeStateChanged != null)
			{
				BeforeStateChanged();
			}
		}

		protected void OnUndoEvent()
		{
			if (UndoEvent != null)
			{
				UndoEvent();
			}
		}

		protected void OnRedoEvent()
		{
			if (RedoEvent != null)
			{
				RedoEvent();
			}
		}
	}
}
