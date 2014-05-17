namespace MG.EditorCommon.Undo
{
	/// <summary>
	/// Basic undoable action implementation with events called on execute/undo.
	/// </summary>
	public abstract class UndoableAction : IUndoableAction
	{
		public delegate void CallbackMethod(UndoableAction action);

		public event CallbackMethod ExecuteEvent;

		public event CallbackMethod UndoEvent;

		/// <summary>
		/// Called on Execute. Override this instead of Execute()!
		/// </summary>
		protected abstract bool CallExecute();

		/// <summary>
		/// Called on Undo. Override this instead of Undo()!
		/// </summary>
		protected abstract void CallUndo();

		public bool Execute()
		{
			bool ret = CallExecute();
			if (ExecuteEvent != null)
			{
				ExecuteEvent(this);
			}
			return ret;
		}

		public void Undo()
		{
			CallUndo();
			if (UndoEvent != null)
			{
				UndoEvent(this);
			}
		}

		public virtual int GetUndoGroup()
		{
			return 0;
		}
	}
}
