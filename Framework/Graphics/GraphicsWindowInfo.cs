using MG.Framework.Numerics;

using OpenTK.Graphics;
using OpenTK.Platform;

namespace MG.Framework.Graphics
{
	public class GraphicsWindowInfo
	{
		public delegate Vector2 WindowSizeDelegate();

		public readonly IWindowInfo WindowInfo;
		public readonly IGraphicsContext GraphicsContext;
		public readonly WindowSizeDelegate GetWindowSize;

		public GraphicsWindowInfo(IWindowInfo windowInfo, IGraphicsContext graphicsContext, WindowSizeDelegate getWindowSize)
		{
			WindowInfo = windowInfo;
			GraphicsContext = graphicsContext;
			GetWindowSize = getWindowSize;
		}
	}
}
