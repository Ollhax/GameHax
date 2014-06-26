using MG.EditorCommon.Undo;

namespace MG.ParticleHax.Actions
{
	public abstract class UndoableParticleAction : UndoableAction
	{
		public int CurrentDefinitionId = -1;
		public string CurrentParameter = null;
	}
}
