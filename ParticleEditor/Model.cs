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
		
		public int DefinitionIdCounter = 1;
		
		public ParticleDefinition GetDefinitionById(int id)
		{
			foreach (var def in DefinitionTable.Definitions)
			{
				if (def.Value.InternalId == id) return def.Value;
			}

			return null;
		}
	}
}
