
using System.Diagnostics;

using MG.Framework.Numerics;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	static class RenderHelpers
	{
		internal static void SetBlendMode(BlendMode blendMode)
		{
			switch (blendMode)
			{
				case BlendMode.BlendmodeOpaque:
					GL.Disable(EnableCap.Blend);
					break;

				case BlendMode.BlendmodeAlpha:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
					break;

				case BlendMode.BlendmodeNonPremultiplied:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					break;

				case BlendMode.BlendmodeAdditive:
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
					break;
			}
		}
		
		internal static void SetSamplers(TextureSampling sampling)
		{
			int minFilter = 0;
			int magFilter = 0;
			switch (sampling.Filter)
			{
				case TextureSampling.TextureFilter.Linear:
					minFilter = (int)TextureMinFilter.Linear;
					magFilter = (int)TextureMagFilter.Linear;
					break;

				case TextureSampling.TextureFilter.Nearest:
					minFilter = (int)TextureMinFilter.Nearest;
					magFilter = (int)TextureMagFilter.Nearest;
					break;

				default: Debug.Assert(false);
					break;
			}

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);

			int wrapS = GetAddressMode(sampling.AddressModeU);
			int wrapT = GetAddressMode(sampling.AddressModeV);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapS);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapT);
		}

		internal static void ToNativeMatrix(ref Matrix m, float[] entries)
		{
			entries[0] = m.M11; entries[1] = m.M12; entries[2] = m.M13; entries[3] = m.M14;
			entries[4] = m.M21; entries[5] = m.M22; entries[6] = m.M23; entries[7] = m.M24;
			entries[8] = m.M31; entries[9] = m.M32; entries[10] = m.M33; entries[11] = m.M34;
			entries[12] = m.M41; entries[13] = m.M42; entries[14] = m.M43; entries[15] = m.M44;
		}

		private static int GetAddressMode(TextureSampling.TextureAddressMode mode)
		{
			switch (mode)
			{
				case TextureSampling.TextureAddressMode.Clamp:
					return (int)TextureWrapMode.Clamp;
				case TextureSampling.TextureAddressMode.Wrap:
					return (int)TextureWrapMode.Repeat;
			}

			Debug.Assert(false);
			return 0;
		}
	}
}
