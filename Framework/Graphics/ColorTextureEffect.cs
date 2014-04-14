using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	public class ColorTextureEffect : ColorEffect
	{
		private int textureSamplerUniform;
		private int textureCoordinatesAttribute;

		public int TextureCoordinatesAttribute { get { return textureCoordinatesAttribute; } }
		
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
