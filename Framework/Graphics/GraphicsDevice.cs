using System;

using MG.Framework.Numerics;
using MG.Framework.Utility;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// The graphics device holds the global rendering state for the current application.
	/// </summary>
	public static class GraphicsDevice
	{
		private static Color clearColor;
		private static Viewport viewport;
		private static Rectangle? scissorTestArea;
		private static readonly int vertexArrayObject;
		private static readonly Version apiVersion;
		
		/// <summary>
		/// The color used to clear the background.
		/// </summary>
		public static Color ClearColor
		{
			get { return clearColor; }

			set
			{
				clearColor = value;
				GL.ClearColor(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
			}
		}

		/// <summary>
		/// Get the current viewport.
		/// </summary>
		/// <returns>The current viewport.</returns>
		public static Viewport GetViewport()
		{
			return viewport;
		}

		/// <summary>
		/// Set the viewport.
		/// </summary>
		/// <param name="viewport"></param>
		/// <param name="screen"></param>
		/// <returns></returns>
		public static void SetViewport(Viewport viewport, Screen screen)
		{
			if (viewport.X < 0 || viewport.Y < 0 || viewport.Width <= 0 || viewport.Height <= 0)
			{
				throw new ArgumentException("Invalid viewport.");
			}

			if (GraphicsDevice.viewport.Equals(viewport))
			{
				return;
			}

			RenderQueue.Flush(); // Render batches should use the render settings at the start of the batch
			GraphicsDevice.viewport = viewport;
			GL.Viewport(viewport.X, screen.ScreenSize.Y - viewport.Y - viewport.Height, viewport.Width, viewport.Height);
		}

		/// <summary>
		/// Get the current scissor test rectangle.
		/// </summary>
		/// <returns>The scissor test rectangle.</returns>
		public static Rectangle? GetScissorTest()
		{
			return scissorTestArea;
		}
		
		/// <summary>
		/// Disable the scissor test. 
		/// </summary>
		public static void ClearScissorTest()
		{
			scissorTestArea = null;
			GL.Disable(EnableCap.ScissorTest);
		}

		/// <summary>
		/// Set the scissor test rectangle.
		/// </summary>
		/// <param name="rectangle">The new scissor test rectangle, in normalized screen coordinates.</param>
		/// <param name="screen">The screen to apply the test to.</param>
		public static void SetScissorTest(Rectangle rectangle, Screen screen)
		{
			if (rectangle.X < 0 || rectangle.Y < 0 || rectangle.Width < 0 || rectangle.Height < 0)
			{
				throw new ArgumentException("Invalid viewport.");
			}

			if (rectangle == scissorTestArea)
			{
				return;
			}

			RenderQueue.Flush(); // Render batches should use the render settings at the start of the batch
			scissorTestArea = rectangle;

			if (scissorTestArea != null)
			{
				var r = scissorTestArea.Value;
				GL.Enable(EnableCap.ScissorTest);
				GL.Scissor(r.X, (int)screen.ScreenSize.Y -r.Y - r.Height, r.Width, r.Height);
			}
			else
			{
				ClearScissorTest();
			}
		}

		/// <summary>
		/// Enable or disable error checking. Typically very expensive.
		/// </summary>
		public static bool ErrorChecking
		{
			get { return GraphicsContext.CurrentContext.ErrorChecking; }
			set { GraphicsContext.CurrentContext.ErrorChecking = value; }
		}
		
		/// <summary>
		/// Make the shared graphics context the current one.
		/// </summary>
		public static void MakeCurrent(Screen screen)
		{
			screen.Context.MakeCurrent(screen.WindowInfo);
		}

		/// <summary>
		/// Swap the buffers, presenting the rendered scene.
		/// </summary>
		public static void SwapBuffers(Screen screen)
		{
			screen.Context.SwapBuffers();
		}

		/// <summary>
		/// Clear the background using the current ClearColor.
		/// </summary>
		public static void Clear()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Clear the background, setting the new ClearColor.
		/// </summary>
		/// <param name="color">New ClearColor.</param>
		public static void Clear(Color color)
		{
			ClearColor = color;
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		/// <summary>
		/// Fetch the entire backbuffer at it's current state.
		/// </summary>
		/// <returns></returns>
		public static int[] GetBackBufferData(Screen screen)
		{
			var s = screen.ScreenSize;
			var b = new int[s.X * s.Y];
			GL.ReadPixels(0, 0, s.X, s.Y, PixelFormat.Rgba, PixelType.UnsignedByte, b);
			return b;
		}

		static GraphicsDevice()
		{
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

		//public static void Deinitialize()
		//{
		//    if (vertexArrayObject != 0)
		//    {
		//        GL.BindVertexArray(0);
		//        GL.DeleteVertexArray(vertexArrayObject);
		//    }
		//}
		
		private static bool SupportsExtension(string extension)
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