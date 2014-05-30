using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;

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
		
		public int DefinitionIdCounter = 1;
	}
}
