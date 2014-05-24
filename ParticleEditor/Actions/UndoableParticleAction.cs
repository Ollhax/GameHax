using MG.EditorCommon.Undo;

namespace MG.ParticleEditor.Actions
{
	public abstract class UndoableParticleAction : UndoableAction
	{
		public int CurrentDefinitionId = -1;
	}
}
