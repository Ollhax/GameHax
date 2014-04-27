using System;

using MG.Framework.Numerics;
using MG.Framework.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// The graphics device wraps global state for the underlying renderer.
	/// </summary>
	public class GraphicsDevice : IDisposable
	{
		private readonly IGraphicsContext context;
		private readonly GameWindow window;
		private readonly IWindowInfo windowInfo;
		private Color clearColor;
		private Viewport viewport;
		private Rectangle? scissorTestArea;
		private readonly int vertexArrayObject;
		private readonly Version apiVersion;

		/// <summary>
		/// Fetch the size of the backbuffer.
		/// </summary>
		public Vector2I BackBufferSize { get { return new Vector2I(window.ClientSize.Width, window.ClientSize.Height); } }

		/// <summary>
		/// The color used to clear the background.
		/// </summary>
		public Color ClearColor
		{
			get { return clearColor; }

			set
			{
				clearColor = value;
				GL.ClearColor(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
			}
		}

		/// <summary>
		/// The shared context, generally not used except for loading shared textures and such.
		/// </summary>
		public IGraphicsContext Context { get { return context; } }
		
		/// <summary>
		/// Change the viewport setting.
		/// </summary>
		public Viewport Viewport
		{
			get { return viewport; }

			set
			{
				if (value.X < 0 || value.Y < 0 || value.Width <= 0 || value.Height <= 0)
				{
					throw new ArgumentException("Invalid viewport.");
				}

				if (viewport.Equals(value))
				{
					return;
				}

				RenderQueue.Flush(); // Render batches should use the render settings at the start of the batch
				viewport = value;
				GL.Viewport(viewport.X, window.ClientSize.Height - viewport.Y - viewport.Height, viewport.Width, viewport.Height);
			}
		}

		/// <summary>
		/// Set the scissor test rectangle. If set to null, scissor testing is disabled.
		/// </summary>
		public Rectangle? ScissorTest
		{
			get { return scissorTestArea; }

			set
			{
				if (value.HasValue && (value.Value.X < 0 || value.Value.Y < 0 || value.Value.Width < 0 || value.Value.Height < 0))
				{
					throw new ArgumentException("Invalid viewport.");
				}

				if (value == scissorTestArea)
				{
					return;
				}

				RenderQueue.Flush(); // Render batches should use the render settings at the start of the batch
				scissorTestArea = value;

				if (scissorTestArea != null)
				{
					var r = scissorTestArea.Value;
					GL.Enable(EnableCap.ScissorTest);

					GL.Scissor(r.X, window.ClientSize.Height - r.Y - r.Height, r.Width, r.Height);
				}
				else
				{
					GL.Disable(EnableCap.ScissorTest);
				}
			}
		}

		/// <summary>
		/// Enable or disable error checking. Typically very expensive.
		/// </summary>
		public bool ErrorChecking
		{
			get { return Context.ErrorChecking; }
			set { Context.ErrorChecking = value; }
		}
		
		internal GraphicsDevice(GameWindow window)
		{
			this.window = window;
			context = window.Context;
			windowInfo = window.WindowInfo;
			context.MakeCurrent(windowInfo);

			Log.Info("Initializing GraphicsDevice.");

			// Parse version
			var fullVersionString = GL.GetString(StringName.Version);
			var versionTokens = fullVersionString.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			var versionString = versionTokens[0];
			
			if (string.IsNullOrEmpty(versionString) || !Version.TryParse(versionString, out apiVersion))
			{
				Log.Error("Could not parse version string: " + fullVersionString);
				apiVersion = new Version(0, 0);
			}
			
			Log.Info("OpenGL version: " + apiVersion + " (string: " + fullVersionString + ")");

			// For OpenGL 4 compatibility, bind a vertex array object and use it all the time.
			// Will need to revise this plan should we ever want to use VAO "for real".
			if (apiVersion.Major >= 4 && SupportsExtension("GL_ARB_vertex_array_object"))
			{
				GL.GenVertexArrays(1, out vertexArrayObject);
				GL.BindVertexArray(vertexArrayObject);
			}
		}

		/// <summary>
		/// Destroy the GraphicsDevice.
		/// </summary>
		public void Dispose()
		{
			if (vertexArrayObject != 0)
			{
				GL.BindVertexArray(0);
				GL.DeleteVertexArray(vertexArrayObject);
			}

			context.Dispose();
		}

		/// <summary>
		/// Make the shared graphics context the current one.
		/// </summary>
		public void MakeCurrent()
		{
			context.MakeCurrent(windowInfo);
		}

		/// <summary>
		/// Swap the buffers, presenting the rendered scene.
		/// </summary>
		public void SwapBuffers()
		{
			context.SwapBuffers();
		}

		/// <summary>
		/// Clear the background using the current ClearColor.
		/// </summary>
		static public void Clear()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Clear the background, setting the new ClearColor.
		/// </summary>
		/// <param name="color">New ClearColor.</param>
		public void Clear(Color color)
		{
			ClearColor = color;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Fetch the entire backbuffer at it's current state.
		/// </summary>
		/// <returns></returns>
		public int[] GetBackBufferData()
		{
			Vector2I s = BackBufferSize;
			var b = new int[s.X * s.Y];
			GL.ReadPixels(0, 0, s.X, s.Y, PixelFormat.Rgba, PixelType.UnsignedByte, b);
			return b;
		}

		private bool SupportsExtension(string extension)
		{
			// Can probably make this more optimized, if need be.

			if (apiVersion.Major < 3)
			{
				string[] extensions = GL.GetString(StringName.Extensions).Split(' ');
				foreach (var s in extensions)
				{
					if (s == extension) return true;
				}
			}
			else
			{
				int extensions;
				GL.GetInteger(GetPName.NumExtensions, out extensions);

				for (int i = extensions - 1; i >= 0; i--)
				{
					//Log.P(GL.GetString(StringName.Extensions, i));
					if (GL.GetString(StringName.Extensions, i) == extension) return true;
				}
			}

			return false;
		}
	}
}