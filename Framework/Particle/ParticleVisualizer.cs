using MG.Framework.Graphics;
using MG.Framework.Numerics;

namespace MG.Framework.Particle
{
	public static class ParticleVisualizer
	{
		public static void Draw(ParticleEffect particleEffect, RenderContext renderContext)
		{
			Draw(particleEffect, renderContext, Matrix.Identity);
		}

		public static void Draw(ParticleEffect particleEffect, RenderContext renderContext, Matrix transform)
		{
			if (particleEffect.ParamQualityLevel > ParticleDefinition.GlobalQualityLevel) return;

			DrawCurrent(particleEffect, renderContext, transform);

			for (int i = particleEffect.SubSystems.Count - 1; i >= 0; i--)				
			{
				var system = particleEffect.SubSystems[i];
				Draw(system, renderContext, transform);
			}
		}

		public static void DrawCurrent(ParticleEffect particleEffect, RenderContext renderContext, Matrix transform)
		{
			//var primitiveBatch = renderContext.PrimitiveBatch;
			//primitiveBatch.Begin();
			//var turbulence = 1;
			//if (turbulence != 0)
			//{
			//    for (int y = 0; y < 100; y++)
			//    {
			//        for (int x = 0; x < 100; x++)
			//        {
			//            var p = new Vector2(x, y) * 10;
			//            var v = SampleTurbulence(p) * 10;
			//            primitiveBatch.Draw(new Line(p, p+v), Color.Red);
			//        }
			//    }
			//}
			//primitiveBatch.End();

			var quadBatch = renderContext.QuadBatch;

			BlendMode blendMode = BlendMode.BlendmodeAlpha;
			if (particleEffect.ParamBlendMode == BlendMode.BlendmodeOpaque)
			{
				blendMode = BlendMode.BlendmodeOpaque;
			}

			quadBatch.Begin(transform, blendMode);

			for (int i = 0; i < particleEffect.ParticleData.ActiveParticles; i++)
			{
				var p = particleEffect.ParticlePosition[i];
				var a = particleEffect.ParticleAge[i];
				var l = particleEffect.ParticleLife[i];
				var r = particleEffect.ParticleRotation[i];
				var particleLifeFraction = a / l;
				var emitterLifeFraction = particleEffect.LifeFractional;
				var color = particleEffect.ParamParticleColor.Evaluate(particleLifeFraction);
				var s = particleEffect.ParticleScale[i] * particleEffect.ParamParticleScale.Get(emitterLifeFraction, particleLifeFraction);
				var sx = particleEffect.ParamParticleScaleX.Get(emitterLifeFraction, particleLifeFraction);
				var sy = particleEffect.ParamParticleScaleY.Get(emitterLifeFraction, particleLifeFraction);

				// TODO: Precalculate this
				if (particleEffect.ParamBlendMode == BlendMode.BlendmodeAlpha || particleEffect.ParamBlendMode == BlendMode.BlendmodeAdditive)
				{
					color.R = (byte)((((int)color.R) * (int)color.A) / 255);
					color.G = (byte)((((int)color.G) * (int)color.A) / 255);
					color.B = (byte)((((int)color.B) * (int)color.A) / 255);

					if (particleEffect.ParamBlendMode == BlendMode.BlendmodeAdditive)
					{
						color.A = 0;
					}
				}

				int clonedCount = 0;
				if (particleEffect.ParamSegmentSpawnType == ParticleEffect.SegmentSpawnType.CloneAll && particleEffect.SegmentTransforms != null)
				{
					clonedCount = particleEffect.SegmentTransforms.Count - 1;
				}
				for (int clone = 0; clone <= clonedCount; clone++)
				{
					int segmentIndex = particleEffect.ParticleSegmentIndex[i];
					segmentIndex += clone;
					if (particleEffect.ParamParticleOrientToVelocity && segmentIndex >= 0 && particleEffect.SegmentTransforms != null && segmentIndex < particleEffect.SegmentTransforms.Count)
					{
						Matrix segmentTransform = particleEffect.SegmentTransforms[segmentIndex];
						Matrix velocityMatrix = MathTools.Create2DAffineMatrix(particleEffect.ParticleWorldForceVelocity[i].X, particleEffect.ParticleWorldForceVelocity[i].Y, 1.0f, 1.0f, 0.0f);
						velocityMatrix = velocityMatrix * Matrix.Invert(segmentTransform);
						r = (velocityMatrix.TranslationXY + particleEffect.ParticleVelocity[i]).Angle() + MathTools.PiOver2;
					}
					Matrix particleTransform = MathTools.Create2DAffineMatrix(p.X, p.Y, s * sx, s * sy, r);
					if (segmentIndex >= 0 && particleEffect.SegmentTransforms != null && segmentIndex < particleEffect.SegmentTransforms.Count)
					{
						Matrix segmentTransform = particleEffect.SegmentTransforms[segmentIndex];
						particleTransform = particleTransform * segmentTransform;
						if (particleEffect.ParamSegmentKeepRotation)
						{
							particleTransform = MathTools.Create2DAffineMatrix(particleTransform.TranslationXY.X, particleTransform.TranslationXY.Y, s * sx, s * sy, r);
						}
					}

					if (particleEffect.ParamParticleRelativeToParent)
					{
						particleTransform.TranslationXY += particleEffect.Position;
					}
					else
					{
						particleTransform.TranslationXY += particleEffect.ParticleOrigin[i];
					}

					// Offset position with world forces
					particleTransform.TranslationXY += particleEffect.ParticleWorldForceOffset[i];

					var sourceArea = new RectangleF(0, 0, particleEffect.ParticleTexture.Width, particleEffect.ParticleTexture.Height);
					var maxCells = particleEffect.AnimationCells;

					int frame = -1;
					if (maxCells > 1)
					{
						float timelineFrame = particleEffect.ParamTextureFrameTimeline.Get(0, a);
						if (timelineFrame >= 1.0f)
						{
							// 1.0, 2.0, 3.0, etc should not be interpreted as frame 0.
							timelineFrame -= MathTools.Epsilon;
						}
						frame = (int)((particleEffect.ParticleStartFrame[i] + timelineFrame) * maxCells) % maxCells;
					}
					if (frame >= 0)
					{
						int frameX = frame % particleEffect.ParamTextureCells.X;
						int frameY = frame / particleEffect.ParamTextureCells.X;
						float cellX = particleEffect.ParticleTexture.Width / (float)particleEffect.ParamTextureCells.X;
						float cellY = particleEffect.ParticleTexture.Height / (float)particleEffect.ParamTextureCells.Y;

						sourceArea = new RectangleF(frameX * cellX, frameY * cellY, cellX, cellY);
					}

					quadBatch.Draw(particleEffect.ParticleTexture, particleTransform, sourceArea, color, sourceArea.Size * particleEffect.ParamTextureAnchor, QuadEffects.None, 0);
				}
			}

			quadBatch.End();
		}
	}
}
