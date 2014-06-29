using System.Collections.Generic;
using System.IO;

using MG.Framework.Assets;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleManager
	{
		private readonly Dictionary<string, ParticleDefinitionTable> particleDefinitionTables = new Dictionary<string, ParticleDefinitionTable>();
		private readonly ParticleSystemPool particleSystemPool;
		
		public ParticleManager(AssetHandler assetHandler, FilePath path)
		{
			particleSystemPool = new ParticleSystemPool(assetHandler);
			
			string[] files = Directory.GetFiles(assetHandler.GetFullPath(path), "*.pe", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var name = PathHelper.GetFileNameWithoutPathOrExtension(file);
				
				var table = new ParticleDefinitionTable();
				if (table.Load(file))
				{
					particleDefinitionTables.Add(name, table);
				}
			}
		}
		
		public ParticleSystem Create(string library, string definition)
		{
			ParticleDefinitionTable table;
			if (particleDefinitionTables.TryGetValue(library, out table))
			{
				var particleDefinition = table.Definitions.GetByName(definition);
				if (particleDefinition != null)
				{
					return particleSystemPool.Create(particleDefinition);
				}
			}

			return null;
		}
		
		public void Destroy(ParticleSystem particleSystem)
		{
			particleSystemPool.Destroy(particleSystem);
		}
	}
}
