using System;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.Framework.Utility;

namespace MG.ParticleEditor
{
	class Model
	{
		public UndoHandler UndoHandler;
		public ParticleDefinition CurrentDefinition;
		public ParticleDeclarationTable DeclarationTable;
		public ParticleDefinitionTable DefinitionTable;
		public ParticleSystem ParticleSystem;
		public ParticleManager ParticleManager;

		public bool DocumentOpen;
		public FilePath DocumentFile;
		public int DefinitionIdCounter;
		public bool Modified;

		public void Clear()
		{
			DocumentOpen = false;
			DocumentFile = null;
			DefinitionTable = new ParticleDefinitionTable();
			ParticleSystem = null;
			UndoHandler.Clear();
			CurrentDefinition = null;
			DefinitionIdCounter = 1;
			Modified = false;
		}
	}
}
