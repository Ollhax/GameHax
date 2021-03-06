﻿using MG.Framework.Numerics;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	public class ColorEffect : Effect
	{
		private int projectionUniform;
		private int viewUniform;
		private int vertexAttribute;
		private int? colorAttribute;
		
		public Matrix Projection;
		public Matrix View;
		public int VertexAttribute { get { return vertexAttribute; } }
		public int? ColorAttribute { get { return colorAttribute; } }

		private float[] matrix = new float[16];

		public ColorEffect(string vertexShader, string fragmentShader) : base(vertexShader, fragmentShader)
		{
			projectionUniform = Uniforms["u_PMatrix"];
			viewUniform = Uniforms["u_MVMatrix"];
			vertexAttribute = Attributes["a_position"];
			
			int colorAttributeTemp;
			if (Attributes.TryGetValue("a_color", out colorAttributeTemp))
			{
				colorAttribute = colorAttributeTemp;
			}
		}

		public override void Enable()
		{
			base.Enable();

			RenderHelpers.ToNativeMatrix(ref Projection, matrix);
			GL.UniformMatrix4(projectionUniform, 1, false, matrix);

			RenderHelpers.ToNativeMatrix(ref View, matrix);
			GL.UniformMatrix4(viewUniform, 1, false, matrix);

			// Enable vertex arrays
			GL.EnableVertexAttribArray(vertexAttribute);

			if (colorAttribute.HasValue)
			{
				GL.EnableVertexAttribArray(colorAttribute.Value);
			}
		}

		public override void Disable()
		{
			base.Disable();
			
			GL.DisableVertexAttribArray(vertexAttribute);

			if (colorAttribute.HasValue)
			{
				GL.DisableVertexAttribArray(colorAttribute.Value);
			}
		}
	}
}
