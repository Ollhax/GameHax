using System;
using System.Diagnostics;
using System.Drawing;

using MG.Framework.Numerics;
using MG.Framework.Utility;
using MG.Framework.Graphics;
using MG.Framework.Input;

using OpenTK;
using OpenTK.Graphics;

namespace MG.Framework.Window
{
	/// <summary>
	/// A representation of a game window.
	/// </summary>
	public class Window : IDisposable
	{
		public enum WindowState
		{
			Normal = 0,
			Minimized,
			Maximized,
			Fullscreen
		}
		
		public event Action<RenderContext> Draw;
		public event Action Load;
		public event Action<Vector2I> Resize;
		public event Action<WindowState> StateChanged;
		public event Action Unload;
		public event Action Closed;
		public event Action<Time> Update;
		private float targetFps;
		private float secondsPerFrame;
		private double frameCounter;
		private readonly Stopwatch timeSinceStart;
		private readonly Stopwatch timeSinceLastFrame;
		private readonly GameWindow window;
		private VSyncMode vSyncMode = VSyncMode.Off;
		private RenderContext renderContext;
		private Screen currentScreen = new Screen();

		public string Name;
		
		public Vector2I ClientSize { get { return new Vector2I(window.ClientSize.Width, window.ClientSize.Height); } set { window.ClientSize = new Size(value.X, value.Y); } }
		
		public bool FixedWindow
		{
			get { return window.WindowBorder == WindowBorder.Fixed; }
			set { window.WindowBorder = value ? WindowBorder.Fixed : WindowBorder.Resizable; }
		}

		public bool Fullscreen
		{
			get { return window.WindowState == OpenTK.WindowState.Fullscreen; }
			set
			{
				if (value)
				{
					window.WindowState = OpenTK.WindowState.Fullscreen;
				}
				else
				{
					window.WindowState = OpenTK.WindowState.Normal;
				}
			}
		}

		public WindowState State
		{
			get { return (WindowState)window.WindowState; }
		}

		public InputHandler InputHandler { get; protected set; }

		public bool Vsync
		{
			get { return window.VSync == VSyncMode.On; }
			set
			{
				var newVal = value ? VSyncMode.On : VSyncMode.Off;
				if (vSyncMode != newVal)
				{
					window.VSync = newVal;
					vSyncMode = newVal;
				}
			}
		}

		public bool FixedUpdate { get; set; }

		private RenderContext PreparedContext
		{
			get
			{
				var size = window.ClientSize;
				currentScreen.Context = window.Context;
				currentScreen.ScreenSize = new Vector2I(size.Width, size.Height);
				currentScreen.WindowInfo = window.WindowInfo;
				renderContext.Prepare(currentScreen);
				return renderContext;
			}
		}

		public Window(string name, Vector2I size, float framesPerSecond)
		{
			Log.Info("Starting " + name + ". Window size: " + size);

			targetFps = framesPerSecond;
			secondsPerFrame = 1.0f / targetFps;

			Name = name;
			timeSinceStart = new Stopwatch();
			timeSinceStart.Start();

			timeSinceLastFrame = new Stopwatch();
			
			window = new GameWindow(
				size.X,
				size.Y,
				GraphicsMode.Default,
				name,
				GameWindowFlags.Default,
				DisplayDevice.Default,
				2,
				0,
				GraphicsContextFlags.Default);
			
			Log.Info("GraphicsMode: " + window.Context.GraphicsMode);

			window.VSync = vSyncMode;
			window.Keyboard.KeyRepeat = false;

			InputHandler = new InputHandler(window);

			window.RenderFrame += WindowOnRenderFrame;
			window.UpdateFrame += WindowOnUpdateFrame;
			window.Resize += WindowOnResize;
			window.Load += WindowOnLoad;
			window.Unload += WindowOnUnload;
			window.Closed += WindowOnClosed;
			window.WindowStateChanged += WindowOnStateChanged;
			
			Log.Info("Window created.");
		}
		
		public void Run()
		{
			window.Run(60, 60);
		}

		public void Close()
		{
			Log.Info("Closing window.");
			window.Exit();
		}

		public void Dispose()
		{
			//window.Exit();
			window.Dispose();
		}
		
		private void WindowOnRenderFrame(object sender, FrameEventArgs frameEventArgs)
		{
			if (Draw != null)
			{
				Draw(PreparedContext);
			}
		}
		
		private void WindowOnUpdateFrame(object sender, FrameEventArgs frameEventArgs)
		{
			if (!timeSinceLastFrame.IsRunning)
				timeSinceLastFrame.Start();

			var time = new Time((float)frameEventArgs.Time, timeSinceStart.Elapsed.TotalSeconds);
			timeSinceLastFrame.Restart();

			InputHandler.Update(time);

			if (Update != null)
			{
				Update(time);
			}

			InputHandler.Clear();
		}

		private void WindowOnResize(object sender, EventArgs eventArgs)
		{
			Size s = window.ClientSize;
			
			var v = new Vector2I(s.Width, s.Height);
			Log.Frequent("Resize event. New size: " + v);
			
			if (Resize != null)
			{
				Resize.Invoke(v);
			}

			Draw(PreparedContext); // Not 100% sure this is a good idea, but leave it in for now.
			Update(new Time());
		}

		private void WindowOnStateChanged(object sender, EventArgs eventArgs)
		{
			if (StateChanged != null)
			{
				StateChanged.Invoke((WindowState)window.WindowState);
			}
		}

		private void WindowOnLoad(object sender, EventArgs eventArgs)
		{
			renderContext = new RenderContext();
			if (Load != null)
			{
				Load.Invoke();
			}
		}

		private void WindowOnUnload(object sender, EventArgs eventArgs)
		{
			if (Unload != null)
			{
				Unload.Invoke();
			}
		}

		private void WindowOnClosed(object sender, EventArgs eventArgs)
		{
			if (Closed != null)
			{
				Closed();
			}
		}
	}
}