using System;
using System.Runtime.InteropServices;
using MG.Framework.Numerics;

using System.Collections.Generic;

using MG.Framework.Utility;

using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Draws groups of textured quads to the screen.
	/// </summary>
	public class QuadBatch
	{
		private struct BatchEntry
		{
			public Texture2D Texture;
			public Color Color;
			public RectangleF SourceRect;
			public Vector2 Origin;
			public QuadEffects SpriteEffects;
			public Matrix Transform;
		};

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct Quad
		{
			public Vertex2Tex2Color TopLeft;
			public Vertex2Tex2Color TopRight;
			public Vertex2Tex2Color BottomLeft;
			public Vertex2Tex2Color BottomRight;

			public Quad(Vertex2Tex2Color topLeft, Vertex2Tex2Color topRight, Vertex2Tex2Color bottomLeft, Vertex2Tex2Color bottomRight)
			{
				TopLeft = topLeft;
				TopRight = topRight;
				BottomLeft = bottomLeft;
				BottomRight = bottomRight;
			}

			public static int GetSize()
			{
				return Vertex2Tex2Color.GetSize() * 4;
			}
		}

		private const string defaultVs = @"
#version 110

attribute vec4 a_position;
attribute vec2 a_texCoord;
attribute vec4 a_color;
		
uniform mat4 u_MVMatrix;
uniform mat4 u_PMatrix;
		
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
#version 110

varying vec4 v_fragmentColor;
varying vec2 v_texCoord;
uniform sampler2D u_texture;

void main()
{
	gl_FragColor = v_fragmentColor * texture2D(u_texture, v_texCoord);
}
";
		private class Batch : RenderQueue.Batch
		{
			public List<BatchEntry> Entries = new List<BatchEntry>();
		}
		
		private Pool<Batch> batchPool = new Pool<Batch>(1);
		private Batch currentBatch;
		
		private int capacity;
		private Quad[] quads;
		private ushort[] indices;
		private ColorTextureEffect currentEffect;
		private ColorTextureEffect defaultEffect;
		private FontLayout fontLayout;
		private uint[] vertexBufferObjects = new uint[2];
		private BlendMode blendMode;
		private TextureSampling textureSampling;
		private Matrix transform;
		private bool hasBegun;

		/// <summary>
		/// Override default projection.
		/// </summary>
		public Matrix? Projection;
		
		internal RenderContext CurrentContext;

		public QuadBatch(int defaultCapacity = 8192)
		{
			defaultEffect = new ColorTextureEffect(defaultVs, defaultFs);
			currentEffect = defaultEffect;

			capacity = defaultCapacity;
			Debug.Assert(capacity > 0);

			quads = new Quad[capacity];
			indices = new ushort[capacity * 6];
			fontLayout = new FontLayout();

			InitIndices();

			GL.GenBuffers(2, vertexBufferObjects);

			//GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
			//GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(capacity * 6 * Vertex2Tex2Color.GetSize()), IntPtr.Zero, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(ushort)), indices, BufferUsageHint.StaticDraw);
		}

		public void Dispose()
		{
			defaultEffect.Dispose();
			quads = null;
			indices = null;

			GL.DeleteBuffers(2, vertexBufferObjects);
		}

		private void InitIndices()
		{
			for (int i = 0; i < capacity; i++)
			{
				indices[i * 6 + 0] = (ushort)(i * 4 + 0);
				indices[i * 6 + 1] = (ushort)(i * 4 + 2);
				indices[i * 6 + 2] = (ushort)(i * 4 + 1);
				indices[i * 6 + 3] = (ushort)(i * 4 + 1);
				indices[i * 6 + 4] = (ushort)(i * 4 + 2);
				indices[i * 6 + 5] = (ushort)(i * 4 + 3);
			}
		}

		private int AddEntry(ref BatchEntry entry, int quadIndex)
		{
			if (quadIndex >= capacity)
			{
				return 0;
			}

			float contentScale = 1.0f;
			float w = entry.SourceRect.Width;
			float h = entry.SourceRect.Height;

			var bounds = entry.Texture.Bounds;
			Vector2I realSize = new Vector2I(bounds.Width, bounds.Height);
			float atlasWidth = realSize.X;
			float atlasHeight = realSize.Y;
			Quad q = new Quad();

			Vector2 topLeft = new Vector2(-entry.Origin.X, -entry.Origin.Y);
			Vector2 topRight = new Vector2(topLeft.X + w, topLeft.Y);
			Vector2 bottomLeft = new Vector2(topLeft.X, topLeft.Y + h);
			Vector2 bottomRight = new Vector2(topLeft.X + w, topLeft.Y + h);

			topLeft = Vector2.Transform(topLeft, entry.Transform);
			topRight = Vector2.Transform(topRight, entry.Transform);
			bottomLeft = Vector2.Transform(bottomLeft, entry.Transform);
			bottomRight = Vector2.Transform(bottomRight, entry.Transform);

			q.TopLeft.Position = topLeft;
			q.TopRight.Position = topRight;
			q.BottomLeft.Position = bottomLeft;
			q.BottomRight.Position = bottomRight;

			if ((entry.SpriteEffects & QuadEffects.RoundPositions) != 0)
			{
				q.TopLeft.Position.X = (int)q.TopLeft.Position.X;
				q.TopLeft.Position.Y = (int)q.TopLeft.Position.Y;
				q.TopRight.Position.X = (int)q.TopRight.Position.X;
				q.TopRight.Position.Y = (int)q.TopRight.Position.Y;
				q.BottomLeft.Position.X = (int)q.BottomLeft.Position.X;
				q.BottomLeft.Position.Y = (int)q.BottomLeft.Position.Y;
				q.BottomRight.Position.X = (int)q.BottomRight.Position.X;
				q.BottomRight.Position.Y = (int)q.BottomRight.Position.Y;
			}

			q.TopLeft.Color = entry.Color;
			q.TopRight.Color = entry.Color;
			q.BottomLeft.Color = entry.Color;
			q.BottomRight.Color = entry.Color;

			float left, right, top, bottom;
			{
				left = entry.SourceRect.X * contentScale / atlasWidth;
				right = left + entry.SourceRect.Width * contentScale / atlasWidth;
				top = entry.SourceRect.Y * contentScale / atlasHeight;
				bottom = top + entry.SourceRect.Height * contentScale / atlasHeight;

				if ((entry.SpriteEffects & QuadEffects.FlipHorizontally) != 0)
				{
					var temp = left;
					left = right;
					right = temp;
				}

				if ((entry.SpriteEffects & QuadEffects.FlipVertically) != 0)
				{
					var temp = top;
					top = bottom;
					bottom = temp;
				}

				q.BottomLeft.TextureCoordinate.X = left;
				q.BottomLeft.TextureCoordinate.Y = bottom;
				q.BottomRight.TextureCoordinate.X = right;
				q.BottomRight.TextureCoordinate.Y = bottom;
				q.TopLeft.TextureCoordinate.X = left;
				q.TopLeft.TextureCoordinate.Y = top;
				q.TopRight.TextureCoordinate.X = right;
				q.TopRight.TextureCoordinate.Y = top;
			}

			quads[quadIndex] = q;
			return 1;
		}

		private void Render(RenderQueue.Batch renderBatch)
		{
			var batch = (Batch)renderBatch;
			var entries = batch.Entries;

			if (entries.Count > 0)
			{
				RenderHelpers.SetBlendMode(blendMode);
				
				GL.Disable(EnableCap.CullFace);
				GL.Disable(EnableCap.DepthTest);
				//GL.Viewport(graphicsDevice.Viewport.X, graphicsDevice.BackBufferSize.Y - graphicsDevice.Viewport.Y - graphicsDevice.Viewport.Height, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

				currentEffect.Projection = Projection ?? CurrentContext.DefaultProjection;
				currentEffect.View = transform;

				currentEffect.Enable();

				for (int i = 0; i < entries.Count; )
				{
					int quadIndex = 0;
					BatchEntry firstEntry = entries[i];
					Texture2D texture = firstEntry.Texture;

					if (AddEntry(ref firstEntry, quadIndex++) == 0)
					{
						Debug.WriteLine("Warning: out of QuadBatch capacity!");
						break;
					}

					// Push in new quads until we reach a new texture
					int entryCount = 1;
					for (int j = i + 1; j < entries.Count; ++j)
					{
						BatchEntry futureEntry = entries[j];
						if (futureEntry.Texture != texture)
						{
							break;
						}

						int added = AddEntry(ref futureEntry, quadIndex++);
						if (added == 0)
						{
							Debug.WriteLine("Warning: out of QuadBatch capacity!");
						}
						entryCount += added; // If we're out of entry slots, do not increase entryCount
					}
					
					// Set texture
					GL.BindTexture(TextureTarget.Texture2D, texture.textureId);
					RenderHelpers.SetSamplers(textureSampling);

					GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
					GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(entryCount * 6 * Vertex2Tex2Color.GetSize()), quads, BufferUsageHint.DynamicDraw);
					//GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(entryCount * 6 * Vertex2Tex2Color.GetSize()), quads);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferObjects[1]);

					GL.VertexAttribPointer(currentEffect.VertexAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), 0);
					GL.VertexAttribPointer(currentEffect.ColorAttribute, 4, VertexAttribPointerType.UnsignedByte, true, Vertex2Tex2Color.GetSize(), sizeof(float) * 2);
					GL.VertexAttribPointer(currentEffect.TextureCoordinatesAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Tex2Color.GetSize(), sizeof(float) * 2 + sizeof(byte) * 4);
					GL.DrawElements(BeginMode.Triangles, entryCount * 6, DrawElementsType.UnsignedShort, 0);

					i += entryCount;
				}

				entries.Clear();

				currentEffect.Disable();
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			}

			// This group is done, recycle it to the pool
			batchPool.Delete(batch);
		}

		private void PrepareGroup()
		{
			Debug.Assert(hasBegun, "Called Draw() without calling Begin()");
			if (currentBatch == null || RenderQueue.CurrentBatch != currentBatch)
			{
				currentBatch = batchPool.New();

				if (currentBatch.Render == null)
				{
					currentBatch.Render = Render;
				}

				RenderQueue.Add(currentBatch);
			}
		}

		public void Begin()
		{
			Begin(Matrix.Identity);
		}

		public void Begin(Matrix transform, BlendMode blendMode = BlendMode.BlendmodeAlpha, TextureSampling textureSampling = null)
		{
			Debug.Assert(!hasBegun, "Called Begin() twice");
			hasBegun = true;

			this.transform = transform;
			this.blendMode = blendMode;
			this.textureSampling = textureSampling ?? TextureSampling.LinearClamp;
		}

		public void End()
		{
			Debug.Assert(hasBegun, "Called End() without calling Begin()");
			RenderQueue.Flush();
			hasBegun = false;
		}

		public void Draw(Texture2D texture, Vector2 position)
		{
			Draw(texture, Matrix.CreateTranslation(position.X, position.Y, 0), Color.White, Vector2.Zero, 0.0f);
		}

		public void Draw(Texture2D texture, Matrix transform, Color color, Vector2 origin, float layerDepth)
		{
			PrepareGroup();

			var bounds = texture.Bounds;
			var s = new Vector2I(bounds.Width, bounds.Height);

			var entry = new BatchEntry();
			entry.Texture = texture;
			entry.Color = color;
			entry.SourceRect = new RectangleF(0, 0, s.X, s.Y);
			entry.Transform = transform;
			entry.Origin = origin;

			currentBatch.Entries.Add(entry);
		}

		public void Draw(Texture2D texture, Matrix transform, RectangleF? sourceRectangle, Color color, Vector2 origin, QuadEffects effects, float layerDepth)
		{
			PrepareGroup();

			var bounds = texture.Bounds;
			var s = new Vector2I(bounds.Width, bounds.Height);

			var entry = new BatchEntry();
			entry.Texture = texture;
			entry.Color = color;
			entry.SourceRect = sourceRectangle ?? new RectangleF(0, 0, s.X, s.Y);
			entry.Origin = origin;
			entry.SpriteEffects = effects;
			entry.Transform = transform;

			currentBatch.Entries.Add(entry);
		}

		public void Draw(Texture2D texture, RectangleF targetRectangle, RectangleF? sourceRectangle, Color color, Vector2 origin, QuadEffects effects, float layerDepth)
		{
			PrepareGroup();

			if (targetRectangle.Width == 0 || targetRectangle.Height == 0)
			{
				// Empty draw, ignore it
				return;
			}

			var bounds = texture.Bounds;
			var s = new Vector2I(bounds.Width, bounds.Height);

			var entry = new BatchEntry();
			entry.Texture = texture;
			entry.Color = color;
			entry.SourceRect = sourceRectangle ?? new RectangleF(0, 0, s.X, s.Y);
			entry.Origin = origin;
			entry.SpriteEffects = effects;
			entry.Transform =
				Matrix.CreateScale(targetRectangle.Width / bounds.Width, targetRectangle.Height / bounds.Height, 1.0f) *
				Matrix.CreateTranslation(targetRectangle.X, targetRectangle.Y, 0);

			currentBatch.Entries.Add(entry);
		}

		public void Draw(FontLayout layout)
		{
			PrepareGroup();

			if (layout.Layout.Count == 0) return;

			var areaSize = layout.Area.Size;
			Vector2 origin = Vector2.Zero;
			switch (layout.Alignment)
			{
				case AlignmentType.NorthWest: break;
				case AlignmentType.SouthWest: origin.Y = areaSize.Y; break;
				case AlignmentType.West: origin.Y = areaSize.Y / 2; break;
				case AlignmentType.Center: origin = areaSize / 2; break;
				case AlignmentType.North: origin.X = areaSize.X / 2; break;
				case AlignmentType.NorthEast: origin.X = areaSize.X; break;
				case AlignmentType.East: origin.X = areaSize.X; origin.Y = areaSize.Y / 2; break;
				case AlignmentType.SouthEast: origin.X = areaSize.X; origin.Y = areaSize.Y; break;
				case AlignmentType.South: origin.X = areaSize.X / 2; origin.Y = areaSize.Y; break;
			}

			foreach (var c in layout.Layout)
			{
				var entry = new BatchEntry();
				entry.Texture = c.Texture;
				entry.Color = c.Color;
				entry.SourceRect = c.Source;
				entry.Origin = Vector2.Zero;
				entry.SpriteEffects = QuadEffects.RoundPositions;

				// TODO: Optimize
				entry.Transform =
					Matrix.CreateTranslation(-origin.X + c.Offset.X, -origin.Y + c.Offset.Y, 0)
					* Matrix.CreateScale(layout.Scale.X, layout.Scale.Y, 1)
					* Matrix.CreateTranslation(origin.X + layout.Position.X, origin.Y + layout.Position.Y, 0);

				currentBatch.Entries.Add(entry);
			}
		}

		public void Draw(Font font, string text, Vector2 position, Color? color = null, FontLayout.LayoutModeType layoutMode = FontLayout.LayoutModeType.Single, FontLayout.LayoutOptionsType layoutOptions = 0)
		{
			fontLayout.Set(font, text, new RectangleF(position.X, position.Y, float.MaxValue, float.MaxValue), color ?? Color.White, AlignmentType.NorthWest, layoutMode, layoutOptions);
			Draw(fontLayout);
		}

		public void Draw(Font font, string text, RectangleF area, Color? color = null, AlignmentType alignment = AlignmentType.NorthWest, FontLayout.LayoutModeType layoutMode = FontLayout.LayoutModeType.Single, FontLayout.LayoutOptionsType layoutOptions = 0)
		{
			fontLayout.Set(font, text, area, color ?? Color.White, alignment, layoutMode, layoutOptions);
			Draw(fontLayout);
		}
	}
}
