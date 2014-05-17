using System.Collections.Generic;

namespace MG.EditorCommon.Undo
{
	/// <summary>
	/// A group of undoable actions.
	/// </summary>
	public class UndoableActionGroup : IUndoableAction
	{
		public List<IUndoableAction> Actions = new List<IUndoableAction>();
		public int CaughtGroup { get; protected set; }

		public UndoableActionGroup(int caughtGroup)
		{
			CaughtGroup = caughtGroup;
		}

		public bool Execute()
		{
			var anyExectuted = false;
			foreach (var action in Actions)
			{
				anyExectuted |= action.Execute();
			}

			return anyExectuted;
		}

		public void Undo()
		{
			for (int i = Actions.Count - 1; i >= 0; i--)
			{
				Actions[i].Undo();
			}
		}

		public int GetUndoGroup()
		{
			return 0;
		}
	}
}
