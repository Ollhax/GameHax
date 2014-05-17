namespace MG.EditorCommon.Undo
{
	/// <summary>
	/// Helper for passing delegates instead of having to create a specific action class.
	/// </summary>
	public class GenericUndoableAction : IUndoableAction
	{
		public delegate bool ExecuteMethodDelegate();
		public delegate void UndoMethodDelegate();

		private ExecuteMethodDelegate ExecuteMethod;
		private UndoMethodDelegate UndoMethod;
		private int undoGroup;

		public GenericUndoableAction(ExecuteMethodDelegate executeMethod, UndoMethodDelegate undoMethod, int undoGroup = 0)
		{
			this.ExecuteMethod = executeMethod;
			this.UndoMethod = undoMethod;
			this.undoGroup = undoGroup;
		}

		public bool Execute()
		{
			if (ExecuteMethod != null)
			{
				return ExecuteMethod();
			}
			return false;
		}

		public void Undo()
		{
			if (UndoMethod != null)
			{
				UndoMethod();
			}
		}

		public int GetUndoGroup()
		{
			return undoGroup;
		}
	}
}
