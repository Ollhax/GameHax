using System.Collections.Generic;

using MG.Framework.Assets;
using MG.Framework.Utility;

namespace MG.Framework.Particle
{
	public class ParticleEffectPool
	{
		private readonly AssetHandler assetHandler;
		private readonly Dictionary<int, Pool<ParticleEffect>> particlePools = new Dictionary<int, Pool<ParticleEffect>>();

		public ParticleEffectPool(AssetHandler assetHandler)
		{
			this.assetHandler = assetHandler;
		}

		public ParticleEffect Create(ParticleDefinition definition, ParticleCommon common = null)
		{
			var particlePool = GetPool(definition);
			var particleEffect = particlePool.New();
			particleEffect.Reload(common ?? new ParticleCommon());
			
			return particleEffect;
		}

		public void Destroy(ParticleEffect particleEffect)
		{
			var particlePool = GetPool(particleEffect.Definition);
			particleEffect.Clear();
			particlePool.Delete(particleEffect);
		}

		public void Clear()
		{
			particlePools.Clear();
		}

		private Pool<ParticleEffect> GetPool(ParticleDefinition definition)
		{
			// Get (or create) the pool for this type of particle
			Pool<ParticleEffect> particlePool;
			if (!particlePools.TryGetValue(definition.Id, out particlePool))
			{
				particlePool = new Pool<ParticleEffect>(8, () => new ParticleEffect(assetHandler, this, definition));
				particlePools.Add(definition.Id, particlePool);
			}
			return particlePool;
		}
	}
}
