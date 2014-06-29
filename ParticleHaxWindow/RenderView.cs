using System;
using EditorCommon;

using Gdk;

using Gtk;

using MG.Framework.Graphics;
using MG.Framework.Numerics;

using Action = System.Action;

namespace MG.ParticleEditorWindow
{
	public class RenderView
	{
		public event Action Load = delegate { };
		public event Action<RenderContext> Draw = delegate { };
		public event Action<Vector2> LeftMousePress = delegate { };

		private bool pressed;

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
			Widget.Events |= EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.ButtonMotionMask;
			Widget.ButtonPressEvent += OnButtonPressEvent;
			Widget.ButtonReleaseEvent += OnButtonReleaseEvent;
			Widget.MotionNotifyEvent += OnMotionNotifyEvent;

			Widget.Load += () => Load();
			Widget.Draw += context => Draw(context);
		}
		
		private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 1)
			{
				pressed = true;
				var mousePos = new Vector2((float)args.Event.X, (float)args.Event.Y);
				LeftMousePress.Invoke(mousePos);
			}
		}

		private void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 1)
			{
				pressed = false;
			}
		}

		private void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			if (pressed)
			{
				var mousePos = new Vector2((float)args.Event.X, (float)args.Event.Y);
				LeftMousePress.Invoke(mousePos);
			}
		}

		public void Refresh()
		{
			Widget.QueueDraw();
		}
	}
}
