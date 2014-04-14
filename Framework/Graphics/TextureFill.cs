using System;
using MG.Framework.Numerics;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Draws an area with a repeating texture.
	/// </summary>
	public class TextureFill : IDisposable
	{
		private const string defaultVs = @"
#version 120

attribute vec4 a_position;
attribute vec2 a_texCoord;
attribute vec4 a_color;

uniform		mat4 u_MVMatrix;
uniform		mat4 u_PMatrix;

varying vec4 v_fragmentColor;
varying vec2 v_texCoord;

void main()
{
	gl_Position = u_PMatrix * (u_MVMatrix * a_position);	
	v_fragmentColor = a_color;
	v_texCoord = a_texCoord;
}
";

		private const string defaultFs = @"
#version 120

varying vec4 v_fragmentColor;
varying vec2 v_texCoord;
uniform sampler2D u_texture;

void main()
{
	gl_FragColor = v_fragmentColor * texture2D(u_texture, v_texCoord);
}
";
		
		private readonly GraphicsDevice graphicsDevice;
		private ColorTextureEffect currentEffect;
		private ColorTextureEffect defaultEffect;

		private Vertex2Tex2Color[] vertices;
		private ushort[] indices;
		private uint[] vertexBufferObjects = new uint[2];

		public Matrix Projection;

		public TextureFill(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			defaultEffect = new ColorTextureEffect(defaultVs, defaultFs);
			
			Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.BackBufferSize.X, graphicsDevice.BackBufferSize.Y, 0, -1024, 1024);

			GL.GenBuffers(2, vertexBufferObjects);

			indices = new ushort[6];
			indices[0] = 0;
			indices[1] = 2;
			indices[2] = 1;
			indices[3] = 1;
			indices[4] = 2;
			indices[5] = 3;

			vertices = new Vertex2Tex2Color[4];
			
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(ushort)), indices, BufferUsageHint.StaticDraw);
		}

		public void Dispose()
		{
			defaultEffect.Dispose();
			GL.DeleteBuffers(2, vertexBufferObjects);
		}
		
		public void Draw(Texture2D texture, RectangleF area, BlendMode blendMode = BlendMode.BlendmodeAlpha, ColorTextureEffect customEffect = null)
		{
			Draw(texture, area, Matrix.Identity, blendMode, customEffect);
		}

		public void Draw(Texture2D texture, RectangleF area, Matrix transformMatrix, BlendMode blendMode = BlendMode.BlendmodeAlpha, ColorTextureEffect customEffect = null)
		{
			currentEffect = customEffect;
			if (currentEffect == null)
			{
				currentEffect = defaultEffect;
			}

			var textureSize = texture.Size;
			var scale = area.Size / textureSize;

			var color = Color.White;

			transformMatrix.Translation /= new Vector3(area.Width, area.Height, 1);
			transformMatrix *= Matrix.CreateScale(scale.X, scale.Y, 1);

			vertices[0] = new Vertex2Tex2Color(area.TopLeft, color, Vector2.Transform(new Vector2(0, 0), transformMatrix));
			vertices[1] = new Vertex2Tex2Color(area.BottomLeft, color, Vector2.Transform(new Vector2(0, 1), transformMatrix));
			vertices[2] = new Vertex2Tex2Color(area.TopRight, color, Vector2.Transform(new Vector2(1, 0), transformMatrix));
			vertices[3] = new Vertex2Tex2Color(area.BottomRight, color, Vector2.Transform(new Vector2(1, 1), transformMatrix));
			
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Viewport(graphicsDevice.Viewport.X, graphicsDevice.BackBufferSize.Y - graphicsDevice.Viewport.Y - graphicsDevice.Viewport.Height, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

			currentEffect.Projection = Projection;
			currentEffect.View = Matrix.Identity;
			currentEffect.Enable();
			
			GL.BindTexture(TextureTarget.Texture2D, texture.textureId);
			RenderHelpers.SetSamplers(TextureSampling.LinearWrap);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);
			
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex2Tex2Color.GetSize()), vertices, BufferUsageHint.DynamicDraw);

			GL.VertexAttribPointer(currentEffect.VertexAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), 0);
			GL.VertexAttribPointer(currentEffect.ColorAttribute, 4, VertexAttribPointerType.UnsignedByte, true, Vertex2Tex2Color.GetSize(), sizeof(float) * 2);
			GL.VertexAttribPointer(currentEffect.TextureCoordinatesAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), sizeof(float) * 2 + sizeof(byte) * 4);
			GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedShort, 0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			currentEffect.Disable();
		}
	}
}