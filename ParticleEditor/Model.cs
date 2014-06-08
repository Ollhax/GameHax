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
		public string CurrentParameter;
		public string CurrentSubParameter;
		public int CurrentDefinitionId;
		public ParticleDeclarationTable DeclarationTable;
		public ParticleDefinitionTable DefinitionTable;
		public ParticleSystem ParticleSystem;
		public ParticleManager ParticleManager;

		public ParticleDefinition CurrentDefinition { get { return DefinitionTable.Definitions.GetById(CurrentDefinitionId); } }

		public bool DocumentOpen;
		public FilePath DocumentFile;
		public int DefinitionIdCounter;
		public bool Modified;

		public void Clear()
		{
			DocumentOpen = false;
			DocumentFile = null;
			DefinitionTable = new ParticleDefinitionTable();
			ParticleManager.Clear();
			ParticleSystem = null;
			UndoHandler.Clear();
			CurrentParameter = null;
			CurrentSubParameter = null;
			CurrentDefinitionId = 0;
			DefinitionIdCounter = 1;
			Modified = false;
		}
	}
}
