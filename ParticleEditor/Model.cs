using System;
using System.Collections.Generic;

using MG.EditorCommon;
using MG.Framework.Particle;

namespace MG.ParticleEditor
{
	class Model
	{
		public ParticleDeclarationTable Declaration;
		public ParticleDefinitionTable Definition;
		public ParticleSystem ParticleSystem;

		public string StatusText;

		public int DefinitionIdCounter = 1;
		
		public ParticleDefinition GetDefinitionById(int id)
		{
			foreach (var def in Definition.Definitions)
			{
				if (def.Value.InternalId == id) return def.Value;
			}

			return null;
		}
	}
}
