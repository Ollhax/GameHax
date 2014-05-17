namespace MG.EditorCommon.Undo
{
	public interface IUndoableAction
	{
		/// <summary>
		/// Execute this action.
		/// </summary>
		/// <returns>True if the action was successfully executed and should be added to the undo stack.</returns>
		bool Execute();

		/// <summary>
		/// Undo this action.
		/// </summary>
		void Undo();

		/// <summary>
		/// Specify undo group. All actions with a group > 0 will be unmade at the same time.
		/// </summary>
		int GetUndoGroup();
	}
}
