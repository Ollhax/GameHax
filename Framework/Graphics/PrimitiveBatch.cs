using System;
using System.Collections.Generic;
using System.Diagnostics;

using MG.Framework.Numerics;
using MG.Framework.Utility;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// Draws groups of primitive graphics (lines, circles and so on) to the screen.
	/// </summary>
	public class PrimitiveBatch : IDisposable
	{
		private struct PrimitiveEntry
		{
			public Circle Circle;
			public Line Line;
			public Vector2 Point;
			public RectangleF Rect;
			public Polygon Polygon;

			public Color Color;
			public PrimitiveType Type;
		};

		private enum PrimitiveType
		{
			Points,
			Lines,
			Quads,
			Circles,
			FilledQuads,
			FilledCircles,
			Polygon,
		};

		private const string defaultVs = @"
#version 110

attribute vec4 a_position;
attribute vec4 a_color;

uniform		mat4 u_MVMatrix;
uniform		mat4 u_PMatrix;

varying vec4 v_fragmentColor;

void main()
{
	gl_Position = u_PMatrix * (u_MVMatrix * a_position);	
	v_fragmentColor = a_color;
}
";

		private const string defaultFs = @"
#version 110

varying vec4 v_fragmentColor;
void main()
{
	gl_FragColor = v_fragmentColor;
}
";

		private class Batch : RenderQueue.Batch
		{
			public List<PrimitiveEntry> Entries = new List<PrimitiveEntry>();
		}

		private Pool<Batch> batchPool = new Pool<Batch>(1);
		private Batch currentBatch;

		private ColorEffect defaultEffect;
		private ColorEffect currentEffect;
		private uint[] vertexBufferObjects = new uint[1];
		private Vertex2Color[] vertexStorage = new Vertex2Color[128];
		private Matrix transform;
		private bool hasBegun;

		/// <summary>
		/// Override default projection.
		/// </summary>
		public Matrix? Projection;

		internal RenderContext CurrentContext;

		public PrimitiveBatch()
		{
			defaultEffect = new ColorEffect(defaultVs, defaultFs);
			currentEffect = defaultEffect;

			GL.GenBuffers(1, vertexBufferObjects);
		}
		
		public void Dispose()
		{
			defaultEffect.Dispose();
			GL.DeleteBuffers(1, vertexBufferObjects);
		}
		
		public void Render(RenderQueue.Batch renderBatch)
		{
			var batch = (Batch)renderBatch;
			var entries = batch.Entries;

			if (entries.Count == 0)
			{
				return;
			}

			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.CullFace);

			currentEffect.Projection = Projection ?? CurrentContext.ActiveScreen.DefaultProjection;
			currentEffect.View = transform;
			currentEffect.Enable();

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjects[0]);
			GL.VertexAttribPointer(currentEffect.VertexAttribute, 2, VertexAttribPointerType.Float, false, Vertex2Color.GetSize(), 0);
			GL.VertexAttribPointer(currentEffect.ColorAttribute, 4, VertexAttribPointerType.UnsignedByte, true, Vertex2Color.GetSize(), 2 * sizeof(float));

			for (int entryIndex = 0; entryIndex < entries.Count; )
			{
				var startingEntry = entries[entryIndex];
				var mode = GetMode(startingEntry);
				int index = 0;

				AddEntry(startingEntry, ref index);

				for (int nextIndex = entryIndex + 1; nextIndex < entries.Count; ++nextIndex)
				{
					var nextEntry = entries[nextIndex];
					if (GetMode(nextEntry) == mode)
					{
						AddEntry(nextEntry, ref index);
						entryIndex++;
					}
					else
					{
						break;
					}
				}

				if (index > 0)
				{
					GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(index * Vertex2Color.GetSize()), vertexStorage, BufferUsageHint.DynamicDraw);
					GL.DrawArrays(mode, 0, index);
				}

				entryIndex++;
			}
			
			entries.Clear();
			currentEffect.Disable();
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			// This group is done, recycle it to the pool
			batchPool.Delete(batch);
		}

		private int GetCircleEdges(float radius)
		{
			return (int)Math.Max(3, (Math.Log(radius, 2) * 4));
		}

		private void AddEntry(PrimitiveEntry entry, ref int index)
		{
			var halfPoint = new Vector2(0.5f, 0.5f);
			int vertexCount = 0;
			
			switch (entry.Type)
			{
				case PrimitiveType.Points: vertexCount = 1; break;
				case PrimitiveType.Lines: vertexCount = 2; break;
				case PrimitiveType.Quads: vertexCount = 8; break;				
				case PrimitiveType.FilledQuads: vertexCount = 6; break;
				case PrimitiveType.Circles: vertexCount = 2 * GetCircleEdges(entry.Circle.Radius); break;
				case PrimitiveType.FilledCircles: vertexCount = 3 * GetCircleEdges(entry.Circle.Radius); break;

				case PrimitiveType.Polygon:
					{
						if (entry.Polygon.Count < 2) return;
						vertexCount = entry.Polygon.Count * 2;
					}
					break;
			}

			var vertices = GetVertices(index + vertexCount);

			switch (entry.Type)
			{
				case PrimitiveType.Points:
					{
						vertices[index].Position = entry.Point + halfPoint;
						vertices[index].Color = entry.Color;
					}
					break;

				case PrimitiveType.Lines:
					{
						vertices[index + 0].Position = entry.Line.Start + halfPoint;
						vertices[index + 1].Position = entry.Line.End + halfPoint;
						vertices[index + 0].Color = entry.Color;
						vertices[index + 1].Color = entry.Color;
					}
					break;

				case PrimitiveType.Quads:
					{
						vertices[index + 0].Position.X = entry.Rect.X + halfPoint.X;
						vertices[index + 0].Position.Y = entry.Rect.Y + halfPoint.Y;
						vertices[index + 1].Position.X = entry.Rect.X + halfPoint.X;
						vertices[index + 1].Position.Y = entry.Rect.Y + entry.Rect.Height - 1.0f + halfPoint.Y;

						vertices[index + 2].Position.X = vertices[index + 1].Position.X;
						vertices[index + 2].Position.Y = vertices[index + 1].Position.Y;
						vertices[index + 3].Position.X = entry.Rect.X + entry.Rect.Width - 1.0f + halfPoint.X;
						vertices[index + 3].Position.Y = entry.Rect.Y + entry.Rect.Height - 1.0f + halfPoint.Y;

						vertices[index + 4].Position.X = vertices[index + 3].Position.X;
						vertices[index + 4].Position.Y = vertices[index + 3].Position.Y;
						vertices[index + 5].Position.X = entry.Rect.X + entry.Rect.Width - 1.0f + halfPoint.X;
						vertices[index + 5].Position.Y = entry.Rect.Y + halfPoint.Y;

						vertices[index + 6].Position.X = vertices[index + 5].Position.X;
						vertices[index + 6].Position.Y = vertices[index + 5].Position.Y;
						vertices[index + 7].Position.X = vertices[index + 0].Position.X;
						vertices[index + 7].Position.Y = vertices[index + 0].Position.Y;

						vertices[index + 0].Color = entry.Color;
						vertices[index + 1].Color = entry.Color;
						vertices[index + 2].Color = entry.Color;
						vertices[index + 3].Color = entry.Color;
						vertices[index + 4].Color = entry.Color;
						vertices[index + 5].Color = entry.Color;
						vertices[index + 6].Color = entry.Color;
						vertices[index + 7].Color = entry.Color;						
					}
					break;

				case PrimitiveType.Circles:
					{
						int edges = vertexCount / 2;

						vertices[index].Position = new Vector2(entry.Circle.Center.X + entry.Circle.Radius + halfPoint.X, entry.Circle.Center.Y + halfPoint.Y);
						vertices[index].Color = entry.Color;
							
						for (int i = 1; i < edges; i++)
						{
							float a = ((float)i / edges) * MathTools.TwoPi;
							var p = new Vector2(entry.Circle.Center.X + entry.Circle.Radius * (float)Math.Cos(a), entry.Circle.Center.Y + entry.Circle.Radius * (float)Math.Sin(a));
							var v = new Vertex2Color(p + halfPoint, entry.Color);

							vertices[index + i * 2 - 1] = v;
							vertices[index + i * 2] = v;
						}

						vertices[index + edges * 2 - 1] = vertices[index];
					} break;

				case PrimitiveType.Polygon:
					{
						int edges = vertexCount / 2;
						vertices[index] = new Vertex2Color(entry.Polygon[0] + halfPoint, entry.Color);
						
						for (int i = 1; i < edges; i++)
						{
							var p = entry.Polygon[i];
							var v = new Vertex2Color(p + halfPoint, entry.Color);

							vertices[index + i * 2 - 1] = v;
							vertices[index + i * 2] = v;
						}

						vertices[index + edges * 2 - 1] = vertices[index];
					} break;

				case PrimitiveType.FilledQuads:
					{
						vertices[index + 0].Position.X = entry.Rect.X + halfPoint.X;
						vertices[index + 0].Position.Y = entry.Rect.Y + halfPoint.Y;
						vertices[index + 1].Position.X = entry.Rect.X + halfPoint.X;
						vertices[index + 1].Position.Y = entry.Rect.Y + entry.Rect.Height - 1.0f + halfPoint.Y;
						vertices[index + 2].Position.X = entry.Rect.X + entry.Rect.Width - 1.0f + halfPoint.X;
						vertices[index + 2].Position.Y = entry.Rect.Y + halfPoint.Y;
						vertices[index + 3].Position = vertices[index + 2].Position;
						vertices[index + 4].Position = vertices[index + 1].Position;
						vertices[index + 5].Position.X = entry.Rect.X + entry.Rect.Width - 1.0f + halfPoint.X;
						vertices[index + 5].Position.Y = entry.Rect.Y + entry.Rect.Height - 1.0f + halfPoint.Y;

						for (int i = 0; i < vertexCount; i++)
						{
							vertices[index + i].Color = entry.Color;
						}
					}
					break;

				case PrimitiveType.FilledCircles:
					{
						int edges = vertexCount / 3;

						var center = new Vertex2Color(entry.Circle.Center + halfPoint, entry.Color);

						vertices[index] = new Vertex2Color(new Vector2(entry.Circle.Center.X + entry.Circle.Radius + halfPoint.X, entry.Circle.Center.Y + halfPoint.Y), entry.Color);
						vertices[index + 1] = center;
							
						for (int i = 1; i < edges; i++)
						{
							float a = ((float)i / edges) * MathTools.TwoPi;
							var p = new Vector2(entry.Circle.Center.X + entry.Circle.Radius * (float)Math.Cos(a), entry.Circle.Center.Y + entry.Circle.Radius * (float)Math.Sin(a));
							var v = new Vertex2Color(p + halfPoint, entry.Color);
								
							vertices[index + i * 3 - 1] = v;
							vertices[index + i * 3] = v;
							vertices[index + i * 3 + 1] = center;
						}

						vertices[index + edges * 3 - 1] = vertices[index];
					} break;
			}

			index += vertexCount;
		}

		private BeginMode GetMode(PrimitiveEntry entry)
		{
			switch (entry.Type)
			{
				case PrimitiveType.Points: return BeginMode.Points;
				case PrimitiveType.Lines: return BeginMode.Lines;
				case PrimitiveType.Quads: return BeginMode.Lines;
				case PrimitiveType.Polygon: return BeginMode.Lines;
				case PrimitiveType.FilledQuads: return BeginMode.Triangles;
				case PrimitiveType.Circles: return BeginMode.Lines;
				case PrimitiveType.FilledCircles: return BeginMode.Triangles;
			}

			Debug.Assert(false);
			return BeginMode.Points;
		}

		private Vertex2Color[] GetVertices(int minCount)
		{
			if (vertexStorage.Length < minCount)
			{
				var oldStorage = vertexStorage;
				vertexStorage = new Vertex2Color[minCount];
				oldStorage.CopyTo(vertexStorage, 0);
			}
			return vertexStorage;
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

		public void Begin(Matrix transform)
		{
			Debug.Assert(!hasBegun, "Called Begin() twice");
			hasBegun = true;

			this.transform = transform;
		}

		public void End()
		{
			Debug.Assert(hasBegun, "Called End() without calling Begin()");
			RenderQueue.Flush();
			hasBegun = false;
		}

		public void Draw(Line shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Line = shape;
			e.Type = PrimitiveType.Lines;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void Draw(Vector2 shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Point = shape;
			e.Type = PrimitiveType.Points;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void Draw(Rectangle shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Rect = new RectangleF(shape);
			e.Type = PrimitiveType.Quads;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void Draw(RectangleF shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Rect = shape;
			e.Type = PrimitiveType.Quads;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void Draw(Circle shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Circle = shape;
			e.Type = PrimitiveType.Circles;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void Draw(Polygon shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Polygon = shape;
			e.Type = PrimitiveType.Polygon;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void DrawFilled(Circle shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Circle = shape;
			e.Type = PrimitiveType.FilledCircles;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}

		public void DrawFilled(RectangleF shape, Color color)
		{
			PrepareGroup();

			var e = new PrimitiveEntry();
			e.Rect = shape;
			e.Type = PrimitiveType.FilledQuads;
			e.Color = color;
			currentBatch.Entries.Add(e);
		}
	}
}