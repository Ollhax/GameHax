using System;
using System.Collections.Generic;
using MG.Framework.Numerics;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Draws a line with a specific texture.
	/// </summary>
	public class TexturePath : IDisposable
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
		
		private ColorTextureEffect currentEffect;
		private ColorTextureEffect defaultEffect;

		private Vertex2Tex2Color[] vertices;
		private ushort[] indices;
		private Texture2D texture;
		private uint[] vertexBufferObjects = new uint[2];

		public Matrix Projection;

		public TexturePath()
		{
			defaultEffect = new ColorTextureEffect(defaultVs, defaultFs);
			
			//Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.BackBufferSize.X, graphicsDevice.BackBufferSize.Y, 0, -1024, 1024);

			GL.GenBuffers(2, vertexBufferObjects);
		}

		public void Dispose()
		{
			Deconstruct();
			defaultEffect.Dispose();

			GL.DeleteBuffers(2, vertexBufferObjects);
		}
		
		public void SetPath(List<Vector2> points, Texture2D texture, float widthScale, Color color)
		{
			Construct(points, texture, widthScale, null, color);
		}

		public void SetPath(List<Vector2> points, Texture2D texture, float widthScale, List<Color> colors)
		{
			if (points.Count != colors.Count)
			{
				throw new ArgumentException("Point and color counts must match.");
			}

			Construct(points, texture, widthScale, colors, Color.White);
		}

		public void Draw(BlendMode blendMode = BlendMode.BlendmodeAlpha, ColorTextureEffect customEffect = null)
		{
			Draw(Matrix.Identity, blendMode, customEffect);
		}

		public void Draw(Matrix transformMatrix, BlendMode blendMode = BlendMode.BlendmodeAlpha, ColorTextureEffect customEffect = null)
		{
			currentEffect = customEffect;
			if (currentEffect == null)
			{
				currentEffect = defaultEffect;
			}
			
			if (vertices == null || texture == null) return;

			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Texture2D);
			//GL.Viewport(graphicsDevice.Viewport.X, graphicsDevice.BackBufferSize.Y - graphicsDevice.Viewport.Y - graphicsDevice.Viewport.Height, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

			currentEffect.Projection = Projection;
			currentEffect.View = transformMatrix;
			currentEffect.Enable();
			
			GL.BindTexture(TextureTarget.Texture2D, texture.textureId);
			RenderHelpers.SetSamplers(TextureSampling.LinearWrap);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);
			
			GL.VertexAttribPointer(currentEffect.VertexAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), 0);
			GL.VertexAttribPointer(currentEffect.ColorAttribute, 4, VertexAttribPointerType.UnsignedByte, true, Vertex2Tex2Color.GetSize(), sizeof(float) * 2);
			GL.VertexAttribPointer(currentEffect.TextureCoordinatesAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), sizeof(float) * 2 + sizeof(byte) * 4);
			GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedShort, 0);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			currentEffect.Disable();
		}

		private void GetAdjustedEdge(List<Vector2> points, int index, float halfLineWidth, float maxBendAngle, out Line straightLeft, out Line straightRight)
		{
			var straightCore = new Line(points[index], points[index + 1]);
			var straightCoreDir = straightCore.SafeDirection();
			var straightCoreDirPerp = straightCoreDir.GetPerpendicular();
			straightLeft = new Line(straightCore.Start - straightCoreDirPerp * halfLineWidth, straightCore.End - straightCoreDirPerp * halfLineWidth);
			straightRight = new Line(straightCore.Start + straightCoreDirPerp * halfLineWidth, straightCore.End + straightCoreDirPerp * halfLineWidth);

			bool closedShape = (MathTools.Equals(points[0], points[points.Count - 1]));

			// Adjust for previous angle
			if (index > 0 || closedShape)
			{
				Line otherCore;
				if (index > 0)
				{
					otherCore = new Line(points[index - 1], points[index]);
				}
				else
				{
					otherCore = new Line(points[points.Count - 2], points[points.Count - 1]);
				}

				var otherCoreDir = otherCore.SafeDirection();
				var otherCoreDirPerp = otherCoreDir.GetPerpendicular();
				var otherStraightLeft = new Line(otherCore.Start - otherCoreDirPerp * halfLineWidth, otherCore.End - otherCoreDirPerp * halfLineWidth);
				var otherStraightRight = new Line(otherCore.Start + otherCoreDirPerp * halfLineWidth, otherCore.End + otherCoreDirPerp * halfLineWidth);

				var angle = (MathTools.GetSmallestAngleDelta(straightCoreDir.Angle(), otherCoreDir.Angle()));
				if (Math.Abs(angle) < maxBendAngle)
				{
					Vector2 intersection;
					if (GetLineIntersection(otherStraightLeft, straightLeft, out intersection))
					{
						straightLeft.Start = intersection;
					}

					if (GetLineIntersection(otherStraightRight, straightRight, out intersection))
					{
						straightRight.Start = intersection;
					}
				}
			}

			// Adjust for upcoming angle
			if (index < points.Count - 2 || closedShape)
			{
				Line otherCore;
				if (index < points.Count - 2)
				{
					otherCore = new Line(points[index + 1], points[index + 2]);
				}
				else
				{
					otherCore = new Line(points[0], points[1]);
				}

				var otherCoreDir = otherCore.SafeDirection();
				var otherCoreDirPerp = otherCoreDir.GetPerpendicular();
				var otherStraightLeft = new Line(otherCore.Start - otherCoreDirPerp * halfLineWidth, otherCore.End - otherCoreDirPerp * halfLineWidth);
				var otherStraightRight = new Line(otherCore.Start + otherCoreDirPerp * halfLineWidth, otherCore.End + otherCoreDirPerp * halfLineWidth);

				var angle = (MathTools.GetSmallestAngleDelta(straightCoreDir.Angle(), otherCoreDir.Angle()));
				if (Math.Abs(angle) < maxBendAngle)
				{
					Vector2 intersection;
					if (GetLineIntersection(otherStraightLeft, straightLeft, out intersection))
					{
						straightLeft.End = intersection;
					}

					if (GetLineIntersection(otherStraightRight, straightRight, out intersection))
					{
						straightRight.End = intersection;
					}
				}
			}
		}

		private void Construct(List<Vector2> points, Texture2D texture, float widthScale, List<Color> colors, Color defaultColor)
		{
			Deconstruct();
			
			if (points.Count <= 1) return;
			
			this.texture = texture;
			float halfLineWidth = texture.Height * 0.5f * widthScale;
			var straights = points.Count - 1;
			var vertexCount = straights * 4;
			var indexCount = straights * 6;

			vertices = new Vertex2Tex2Color[vertexCount];
			indices = new ushort[indexCount];
			
			// Create vertices
			for (var i = 0; i < points.Count - 1; i++)
			{
				Line straightLeft;
				Line straightRight;

				const float maxBendAngle = MathTools.PiOver2 * 1.5f;
				GetAdjustedEdge(points, i, halfLineWidth, maxBendAngle, out straightLeft, out straightRight);

				var startColor = defaultColor;
				var endColor = defaultColor;
				if (colors != null)
				{
					startColor = colors[i - 1];
					endColor = colors[i];
				}

				vertices[i * 4 + 0] = new Vertex2Tex2Color(straightLeft.Start, startColor, new Vector2(0, 0));
				vertices[i * 4 + 1] = new Vertex2Tex2Color(straightRight.Start, startColor, new Vector2(0, 1));
				vertices[i * 4 + 2] = new Vertex2Tex2Color(straightLeft.End, endColor, new Vector2(1, 0));
				vertices[i * 4 + 3] = new Vertex2Tex2Color(straightRight.End, endColor, new Vector2(1, 1));
			}
			
			// Create indices
			for (var i = 0; i < straights; i++)
			{
				indices[i * 6 + 0] = (ushort)(i * 4 + 0);
				indices[i * 6 + 1] = (ushort)(i * 4 + 2);
				indices[i * 6 + 2] = (ushort)(i * 4 + 1);
				indices[i * 6 + 3] = (ushort)(i * 4 + 1);
				indices[i * 6 + 4] = (ushort)(i * 4 + 2);
				indices[i * 6 + 5] = (ushort)(i * 4 + 3);	
			}
			
			// Set texture
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexCount * Vertex2Tex2Color.GetSize()), vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(ushort)), indices, BufferUsageHint.StaticDraw);
		}
		
		private bool GetLineIntersection(Line l1, Line l2, out Vector2 intersection)
		{
			// http://stackoverflow.com/a/563275

			var e = l1.End - l1.Start;
			var f = l2.End - l2.Start;
			var p = e.GetPerpendicular();
			var f_dot_p = Vector2.Dot(f, p);

			if (f_dot_p == 0)
			{
				intersection = Vector2.Zero;
				return false;
			}

			var h = Vector2.Dot((l1.Start - l2.Start), p) / f_dot_p;

			intersection = l2.Start + f * h;
			return true;
		}

		private void Deconstruct()
		{
			vertices = null;
			indices = null;
		}
	}
}