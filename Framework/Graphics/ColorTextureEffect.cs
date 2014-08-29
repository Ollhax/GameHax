using System;
using System.IO;

using MG.Framework.Utility;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	public class ColorTextureEffect : ColorEffect
	{
		private int textureSamplerUniform;
		private int textureCoordinatesAttribute;

		public int TextureCoordinatesAttribute { get { return textureCoordinatesAttribute; } }
		
		public static ColorTextureEffect Load(FilePath vertexShaderFile, FilePath fragmentShaderFile)
		{
			var vs = Load(vertexShaderFile);
			var fs = Load(fragmentShaderFile);

			if (vs == null || fs == null) return null;

			try
			{
				return new ColorTextureEffect(vs.ReadToEnd(), fs.ReadToEnd());
			}
			catch (Exception e)
			{
				Log.Error("Error on loading shader: " + e.Message);
			}

			return null;
		}

		public ColorTextureEffect(string vertexShader, string fragmentShader) : base(vertexShader, fragmentShader)
		{
			textureSamplerUniform = Uniforms["u_texture"];
			textureCoordinatesAttribute = Attributes["a_texCoord"];
		}

		public override void Enable()
		{
			base.Enable();

			GL.Uniform1(textureSamplerUniform, 0);
			GL.EnableVertexAttribArray(textureCoordinatesAttribute);
			GL.ActiveTexture(TextureUnit.Texture0);
		}

		public override void Disable()
		{
			base.Disable();

			GL.DisableVertexAttribArray(textureCoordinatesAttribute);
		}
	}
}
