using System.Collections.Generic;

using MG.Framework.Numerics;

using OpenTK.Graphics;
using OpenTK.Platform;

namespace MG.Framework.Graphics
{
	/// <summary>
	/// A screen representation.
	/// </summary>
	/// <remarks>
	/// Does not necessarily need to represent a real screen; for example, in a window with multiple
	/// views, you'd have one screen per view.
	/// </remarks>
	public class Screen
	{
		private static List<Screen> screens = new List<Screen>();

		/// <summary>
		/// Get the number of registered screens.
		/// </summary>
		public static int ScreenCount { get { return screens.Count; } }

		/// <summary>
		/// Get the primary screen.
		/// </summary>
		public static Screen PrimaryScreen { get { return screens[0]; } }

		/// <summary>
		/// Get a specific screen.
		/// </summary>
		/// <param name="screenIndex">Screen index.</param>
		/// <returns>The specified screen, or null if the index is invalid.</returns>
		public static Screen GetScreen(int screenIndex)
		{
			if (screenIndex >= 0 && screenIndex < screens.Count)
			{
				return screens[screenIndex];
			}

			return null;
		}

		/// <summary>
		/// Register a screen.
		/// </summary>
		/// <param name="screen">The screen to register.</param>
		public static void AddScreen(Screen screen)
		{
			screens.Add(screen);
		}
		
		private Vector2I screenSize;
		
		/// <summary>
		/// Get the OpenTK WindowInfo.
		/// </summary>
		public IWindowInfo WindowInfo;
		
		/// <summary>
		/// Get the OpenTK context.
		/// </summary>
		public IGraphicsContext Context;

		/// <summary>
		/// Get the default projection matrix.
		/// </summary>
		public Matrix DefaultProjection;

		/// <summary>
		/// Virtual screen size, e.g. 1920x1080.
		/// </summary>
		public Vector2 VirtualScreenSize;

		/// <summary>
		/// Screen area, when converted to virtual screen area. Mainly accounts for screen aspect ratio.
		/// </summary>
		public RectangleF NormalizedScreenArea;

		/// <summary>
		/// Get or set the real size of this screen, e.g. 960x540 pixels.
		/// </summary>
		/// <remarks>Rarely needed; you'd usually want to use normalized or virtual coordinates.</remarks>
		public Vector2I ScreenSize
		{
			get { return screenSize; }
			set { if (value != screenSize) SetSize(value); }
		}
		
		/// <summary>
		/// Convert the specific virtual area to real screen area.
		/// </summary>
		/// <param name="virtualArea">Input virtual area.</param>
		/// <returns>Real screen coordinates.</returns>
		public RectangleF VirtualToScreen(RectangleF virtualArea)
		{
			var s = ScreenSize.X / NormalizedScreenArea.Width;
			return new RectangleF((virtualArea.X - NormalizedScreenArea.X) * s, (virtualArea.Y - NormalizedScreenArea.Y) * s, virtualArea.Width * s, virtualArea.Height * s);
		}

		/// <summary>
		/// Get the size of the virtual screen.
		/// </summary>
		public RectangleF VirtualScreenArea { get { return new RectangleF(0, 0, VirtualScreenSize.X, VirtualScreenSize.Y); } }
		
		private void SetSize(Vector2I size)
		{
			screenSize = size;
			
			// Unscaled projection
			//if (desktop != null)
			//{
			//    desktop.Area = new RectangleF(0, 0, size.X, size.Y);
			//}

			//if (clientComponents != null)
			//{
			//    UpdateProjection((Vector2)size, Vector2.Zero);
			//}

			var s = (Vector2)size;
			var aspectRatio = s.Y > 0 ? s.X / s.Y : 0;
			var desiredRatio = VirtualScreenSize.X / VirtualScreenSize.Y;
			var diffX = 0.0f;
			var diffY = 0.0f;

			if (aspectRatio < desiredRatio)
			{
				diffX = VirtualScreenSize.X - (aspectRatio / desiredRatio) * VirtualScreenSize.X;
			}
			else if (aspectRatio > desiredRatio)
			{
				diffY = VirtualScreenSize.Y - (desiredRatio / aspectRatio) * VirtualScreenSize.Y;
			}

			NormalizedScreenArea = new RectangleF(diffX / 2, diffY / 2, VirtualScreenSize.X - diffX, VirtualScreenSize.Y - diffY);

			UpdateProjection();
		}

		private void UpdateProjection()
		{
			var size = NormalizedScreenArea.Size;
			var offset = NormalizedScreenArea.Position;
			DefaultProjection = Matrix.CreateTranslation(-offset.X, -offset.Y, 0) * Matrix.CreateOrthographicOffCenter(0, size.X, size.Y, 0, -1024, 1024);
		}
	}
}
