using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleSystemPool
	{
		private readonly AssetHandler assetHandler;
		private readonly Dictionary<int, Pool<ParticleSystem>> particlePools = new Dictionary<int, Pool<ParticleSystem>>();

		public ParticleSystemPool(AssetHandler assetHandler)
		{
			this.assetHandler = assetHandler;
		}

		public ParticleSystem Create(ParticleDefinition definition)
		{
			var particlePool = GetPool(definition);
			var particleSystem = particlePool.New();
			particleSystem.Reload();
			return particleSystem;
		}

		public void Destroy(ParticleSystem particleSystem)
		{
			var particlePool = GetPool(particleSystem.Definition);
			particleSystem.Clear();
			particlePool.Delete(particleSystem);
		}

		public void Clear()
		{
			particlePools.Clear();
		}

		private Pool<ParticleSystem> GetPool(ParticleDefinition definition)
		{
			// Get (or create) the pool for this type of particle
			Pool<ParticleSystem> particlePool;
			if (!particlePools.TryGetValue(definition.Id, out particlePool))
			{
				particlePool = new Pool<ParticleSystem>(8, () => new ParticleSystem(assetHandler, this, definition));
				particlePools.Add(definition.Id, particlePool);
			}
			return particlePool;
		}
	}
}
