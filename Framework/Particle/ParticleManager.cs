﻿using System.Collections.Generic;
using System.IO;

using MG.Framework.Assets;
using MG.Framework.Utility;
using System;

namespace MG.Framework.Particle
{
	public class ParticleManager
	{
		private readonly Dictionary<string, ParticleDefinitionTable> particleDefinitionTables = new Dictionary<string, ParticleDefinitionTable>();
		private readonly ParticleEffectPool particleEffectPool;
		
		public ParticleManager(AssetHandler assetHandler, FilePath path)
		{
			particleEffectPool = new ParticleEffectPool(assetHandler);
			
			string[] files = Directory.GetFiles(assetHandler.GetFullPath(path), "*.pe", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				var name = PathHelper.GetFileNameWithoutPathOrExtension(file);
				
				var table = new ParticleDefinitionTable();
				try
				{
					table.Load(file);
					particleDefinitionTables.Add(name, table);
				}
				catch (Exception e)
				{
					Log.Error("Error on loading particle table: " + e.Message);
				}
			}
		}
		
		public ParticleEffect Create(string library, string definition)
		{
			ParticleDefinitionTable table;
			if (particleDefinitionTables.TryGetValue(library, out table))
			{
				var particleDefinition = table.Definitions.GetByName(definition);
				if (particleDefinition != null)
				{
					return particleEffectPool.Create(particleDefinition);
				}
			}

			return null;
		}
		
		public void Destroy(ParticleEffect particleEffect)
		{
			particleEffectPool.Destroy(particleEffect);
		}
	}
}
