using System;
using EditorCommon;
using MG.Framework.Graphics;

namespace MG.ParticleEditorWindow
{
	public class RenderView
	{
		public event Action Load = delegate { };
		public event Action<RenderContext> Draw = delegate { };

		internal HaxGLWidget Widget;
		
		public RenderView()
		{
			Widget = new HaxGLWidget();
			Widget.Name = "MainGL";
			Widget.SingleBuffer = false;
			Widget.ColorBPP = 0;
			Widget.AccumulatorBPP = 0;
			Widget.DepthBPP = 0;
			Widget.StencilBPP = 0;
			Widget.Samples = 0;
			Widget.Stereo = false;
			Widget.GlVersionMajor = 2;
			Widget.GlVersionMinor = 1;

			Widget.Load += () => Load();
			Widget.Draw += context => Draw(context);
		}

		public void Refresh()
		{
			Widget.QueueDraw();
		}
	}
}
