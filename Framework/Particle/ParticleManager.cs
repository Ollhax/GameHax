using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleManager
	{
		private AssetHandler assetHandler;
		private Dictionary<string, Pool<ParticleSystem>> particlePools = new Dictionary<string, Pool<ParticleSystem>>();

		public ParticleManager(AssetHandler assetHandler)
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

		private Pool<ParticleSystem> GetPool(ParticleDefinition definition)
		{
			// Get (or create) the pool for this type of particle
			Pool<ParticleSystem> particlePool;
			if (!particlePools.TryGetValue(definition.Name, out particlePool))
			{
				particlePool = new Pool<ParticleSystem>(8, () => new ParticleSystem(assetHandler, this, definition));
				particlePools.Add(definition.Name, particlePool);
			}
			return particlePool;
		}
	}
}
