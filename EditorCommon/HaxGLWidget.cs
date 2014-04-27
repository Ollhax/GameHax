using System;

using Gtk;

using MG.Framework.Graphics;
using System.ComponentModel;

using MG.Framework.Numerics;

using OpenTK.Graphics;

namespace EditorCommon
{
	[ToolboxItem(true)]
	public class HaxGLWidget : GLWidget
	{
		private Screen currentScreen;
		private RenderContext renderContext;

		private RenderContext PreparedContext
		{
			get
			{
				var area = Allocation;
				currentScreen.ScreenSize = new Vector2I(area.Width, area.Height);
				renderContext.Prepare(currentScreen);
				return renderContext;
			}
		}

		public event Action<RenderContext> Draw;
		public event System.Action Load;
		//public event Action<Time> Update;

		public HaxGLWidget()
		{
			GraphicsContextFlags = GraphicsContextFlags.ForwardCompatible;
			GraphicsContextInitialized += OnGraphicsContextInitialized;
		}

		private void OnGraphicsContextInitialized(object sender, EventArgs eventArgs)
		{
			currentScreen = new Screen();
			currentScreen.Context = GraphicsContext.CurrentContext;
			currentScreen.WindowInfo = WindowInfo;
			
#if DEBUG
			currentScreen.Context.ErrorChecking = true;
#endif

			renderContext = new RenderContext();
			
			if (Load != null)
			{
				Load();
			}
		}

		protected override void OnRenderFrame()
		{
			if (Draw != null)
			{
				Draw(PreparedContext);
			}

			base.OnRenderFrame();
		}
	}
}
