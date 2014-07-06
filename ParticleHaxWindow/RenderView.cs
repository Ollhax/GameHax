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

		private HaxGLWidget glWidget;
		internal Widget Widget { get { return glWidget; } }
		
		public RenderView()
		{
			glWidget = new HaxGLWidget();
			glWidget.Name = "MainGL";
			glWidget.SingleBuffer = false;
			glWidget.ColorBPP = 0;
			glWidget.AccumulatorBPP = 0;
			glWidget.DepthBPP = 0;
			glWidget.StencilBPP = 0;
			glWidget.Samples = 0;
			glWidget.Stereo = false;
			glWidget.GlVersionMajor = 2;
			glWidget.GlVersionMinor = 1;
			glWidget.Load += () => Load();
			glWidget.Draw += context => Draw(context);

			glWidget.Events |= EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.ButtonMotionMask;
			glWidget.ButtonPressEvent += OnButtonPressEvent;
			glWidget.ButtonReleaseEvent += OnButtonReleaseEvent;
			glWidget.MotionNotifyEvent += OnMotionNotifyEvent;

			// We do not get mac mouse events, so hook up our "alternative solution":
			// Bug report: https://bugzilla.gnome.org/show_bug.cgi?id=730415
			glWidget.MacMouseDown += OnLeftMouseDown;
			glWidget.MacMouseDragged += OnLeftMouseDragged;
			glWidget.MacMouseUp += OnLeftMouseUp;
		}

		private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 1)
			{
				OnLeftMouseDown(new Vector2((float)args.Event.X, (float)args.Event.Y));
			}
		}

		private void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button == 1)
			{
				OnLeftMouseUp(new Vector2((float)args.Event.X, (float)args.Event.Y));
			}
		}

		private void OnMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			OnLeftMouseDragged(new Vector2((float)args.Event.X, (float)args.Event.Y));
		}

		private void OnLeftMouseDown(Vector2 pos)
		{
			pressed = true;
			LeftMousePress.Invoke(pos);
		}

		private void OnLeftMouseDragged(Vector2 pos)
		{
			if (pressed)
			{
				LeftMousePress.Invoke(pos);
			}
		}

		private void OnLeftMouseUp(Vector2 pos)
		{
			pressed = false;
		}

		public void Refresh()
		{
			Widget.QueueDraw();
		}
	}
}
