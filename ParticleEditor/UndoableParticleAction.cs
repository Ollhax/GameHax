using MG.EditorCommon.Undo;

namespace MG.ParticleEditor
{
	public abstract class UndoableParticleAction : UndoableAction
	{
		public int CurrentDefinitionId = -1;
	}
}
