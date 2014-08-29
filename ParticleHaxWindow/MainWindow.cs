using System;
using System.Collections.Generic;

using EditorCommon;

using GLib;

using Gdk;

using Gtk;

using MG.EditorCommon;
using MG.Framework.Utility;

using Log = MG.Framework.Utility.Log;

namespace MG.ParticleEditorWindow
{
	public class MainWindow : IDisposable
	{
		public enum MessageType
		{
			Info = 0,
			Warning = 1,
			Question = 2,
			Error = 3,
			Other = 4,
		}
		
		public enum ResponseType
		{
			// Note: Must match GTK's flags
			//Help = -11,
			//Apply = -10,
			No = -9,
			Yes = -8,
			//Close = -7,
			Cancel = -6,
			Ok = -5,
			//DeleteEvent = -4,
			//Accept = -3,
			//Reject = -2,
			//None = -1,
		}
		
		public struct DialogResult
		{
			public FilePath SelectedPath;
			public int FilterIndex;
			public bool Accepted;
		}

		public class ClosingEventArgs : EventArgs
		{
			public bool Cancel;
		}

		public delegate void ClosingEventHandler(object sender, ClosingEventArgs e);
		
		public event System.Action FileNew = delegate { };
		public event System.Action FileOpen = delegate { };
		public event System.Action FileClose = delegate { };
		public event System.Action FileSave = delegate { };
		public event System.Action FileSaveAs = delegate { };
		public event System.Action EditRestart = delegate { };
		public event System.Action EditUndo = delegate { };
		public event System.Action EditRedo = delegate { };
		public event System.Action EditCut = delegate { };
		public event System.Action EditCopy = delegate { };
		public event System.Action EditPaste = delegate { };
		public event System.Action<ClosingEventArgs> Closing = delegate { };
		public event System.Action Closed = delegate { };
		public event System.Action ToggleShowOrigin = delegate { };
		public event System.Action BackgroundColorChanged = delegate { };
		public event System.Action ViewModeChanged = delegate { };
		
		public readonly RenderView RenderView;
		public readonly TreeView TreeView;
		public readonly ParameterView ParameterView;
		public readonly Clipboard Clipboard = new Clipboard();
		
		private bool sensitive = true;
		
		public string Title { get { return window.Title; } set { window.Title = value; } }
		public bool UndoEnabled { get { return editUndo.Sensitive; } set { editUndo.Sensitive = value; } }
		public bool RedoEnabled { get { return editRedo.Sensitive; } set { editRedo.Sensitive = value; } }
		public bool CutCopyEnabled { get { return editCut.Sensitive; } set { editCut.Sensitive = editCopy.Sensitive = value; } }
		
		public bool ViewShowOrigin { get { return viewShowOrigin.Active; } set { viewShowOrigin.Active = value; } }

		public int ViewMode
		{
			get
			{
				if (viewModeShowSelected.Active) return 0;
				if (viewModeShowSelectedChildren.Active) return 1;
				if (viewModeShowFull.Active) return 2;
				return 0;
			}

			set
			{
				if (value == 0) viewModeShowSelected.Active = true;
				else if (value == 1) viewModeShowSelectedChildren.Active = true;
				else viewModeShowFull.Active = true;
			}
		}

		public int CurrentBackgroundColorIndex
		{
			get
			{
				for (int i = 0; i < viewShowColor.Count; i++)
				{
					if (viewShowColor[i].Active) return i;
				}

				return 0;
			}

			set
			{
				if (value < 0 || value >= viewShowColor.Count) return;
				viewShowColor[value].Active = true;
			}
		}

		public MG.Framework.Numerics.Color GetBackgroundColor(int index)
		{
			return viewShowColor[index].Color;
		}

		public void SetBackgroundColor(int index, MG.Framework.Numerics.Color color)
		{
			viewShowColor[index].Color = color;
		}

		private class GtkWindow : Gtk.Window
		{
			public GtkWindow(string title)
				: base(title)
			{

			}
		}

		private GtkWindow window;
		private AccelGroup accelerators;
		private Statusbar statusbar;

		public MainWindow(string title)
		{
			window = new GtkWindow(title);
			window.DeleteEvent += WindowOnDeleteEvent;
			window.Destroyed += (sender, args) => Closed.Invoke();

			Log.Info("Loading window icon.");
			window.Icon = Pixbuf.LoadFromResource("icon.png");
			window.Name = title;
			window.WindowPosition = WindowPosition.CenterOnParent;
			window.DefaultWidth = 1280;
			window.DefaultHeight = 720;
			
			// Subcomponents
			RenderView = new RenderView();
			TreeView = new TreeView();
			ParameterView = new ParameterView();
			
			accelerators = new AccelGroup();
			window.AddAccelGroup(accelerators);

			// Setup menu
			var menuBar = CreateMenu();
			var menuBox = new VBox(false, 0);
			menuBox.Spacing = 0;
			window.Add(menuBox);

			// Setup status bar
			statusbar = new Statusbar();
			statusbar.Name = "statusbar";
			statusbar.Spacing = 0;
			statusbar.Push(0, "");
			menuBox.PackEnd(statusbar, false, false, 0);
			menuBox.PackStart(menuBar, false, false, 0);

			// Setup left/right pane
			var hpaneMain = new HPaned();
			menuBox.PackStart(hpaneMain, true, true, 0);
			hpaneMain.Position = 300;
			
			var rightBox = new HBox(false, 1);
			hpaneMain.Pack2(rightBox, true, true);
			
			//// Setup gl widget and graph
			//var vpaneRight = new VPaned();
			//rightBox.PackStart(vpaneRight, true, true, 0);
			//vpaneRight.Position = window.DefaultHeight - 220;
			
			//var f1 = new Frame();
			//f1.Add(RenderView.Widget);
			//f1.ShadowType = ShadowType.In;

			//var f2 = new Frame();
			//f2.Add(GraphView.Widget);
			//f2.ShadowType = ShadowType.None;
			
			//vpaneRight.Pack1(f1, true, true);
			//vpaneRight.Pack2(f2, true, true);

			rightBox.PackStart(RenderView.Widget, true, true, 0);

			// Tree view
			TreeView.Widget.SetSizeRequest(200, -1);
			rightBox.PackEnd(TreeView.Widget, false, true, 0);

			// Parameter view
			ParameterView.Widget.SetSizeRequest(150, -1);
			hpaneMain.Pack1(ParameterView.Widget, false, false);
			ParameterView.Widget.DividerPosition = 0.40;

			Log.Info("Showing window.");
			window.ShowAll();
		}

		public string StatusText
		{
			set
			{
				statusbar.Pop(0);
				statusbar.Push(0, value ?? "");
			}
		}
		
		public void ShowMessage(string message, MessageType messageType)
		{
			var md = new MessageDialog(window, DialogFlags.Modal, (Gtk.MessageType)messageType, ButtonsType.Ok, true, message);
			
			md.Run();
			md.Destroy();
		}

		public ResponseType ShowMessageOkCancel(string message, MessageType messageType)
		{
			var md = new MessageDialog(window, DialogFlags.Modal, (Gtk.MessageType)messageType, ButtonsType.OkCancel, true, message);

			var response = md.Run();
			md.Destroy();

			return (response == (int)Gtk.ResponseType.Ok ? ResponseType.Ok : ResponseType.Cancel);
		}

		public ResponseType ShowMessageYesNoCancel(string message, MessageType messageType)
		{
			var md = new MessageDialog(window, DialogFlags.Modal, (Gtk.MessageType)messageType, ButtonsType.None, true, message);
			md.AddButton(Stock.Yes, Gtk.ResponseType.Yes);
			md.AddButton(Stock.No, Gtk.ResponseType.No);
			md.AddButton(Stock.Cancel, Gtk.ResponseType.Cancel);
			
			var response = md.Run();
			md.Destroy();

			return (ResponseType)response;
		}

		public DialogResult ShowSaveDialog(string title, string filter, FilePath startPath)
		{
			return ShowDialog(title, filter, startPath, FileChooserAction.Save);
		}

		public DialogResult ShowOpenDialog(string title, string filter, FilePath startPath)
		{
			return ShowDialog(title, filter, startPath, FileChooserAction.Open);
		}

		private DialogResult ShowDialog(string title, string filter, FilePath startPath, FileChooserAction action)
		{
			var dialog = new FileChooserDialog(title, window, action);
			dialog.AddButton(Stock.Cancel, Gtk.ResponseType.Cancel);

			if (action == FileChooserAction.Save)
			{
				dialog.AddButton(Stock.Save, Gtk.ResponseType.Ok);
			}
			else
			{
				dialog.AddButton(Stock.Open, Gtk.ResponseType.Ok);
			}
			
			dialog.SelectFilename(startPath);

			var filters = GtkTools.ParseFilterString(filter);
			if (filters != null)
			{
				filters.ForEach(dialog.AddFilter);
			}

			var response = dialog.Run();
			var result = new DialogResult
			{
				Accepted = response == (int)Gtk.ResponseType.Ok,
				FilterIndex = GetFilterIndex(dialog),
				SelectedPath = dialog.Filename,
			};

			dialog.Destroy();
			return result;
		}
		
		public bool Sensitive
		{
			get { return sensitive; }
			set
			{
				sensitive = value;
				ParameterView.Widget.Sensitive = value;
				TreeView.Widget.Sensitive = value;
				RenderView.Widget.Sensitive = value;
				editMenuItem.Sensitive = value;
				fileSave.Sensitive = value;
				fileSaveAs.Sensitive = value;
				fileClose.Sensitive = value;
				editPaste.Sensitive = value;
				editRestart.Sensitive = value;

				if (!sensitive)
				{
					editCut.Sensitive = value;
					editCopy.Sensitive = value;
				}
			}
		}
		
		private int GetFilterIndex(FileChooserDialog dialog)
		{
			for (int i = 0; i < dialog.Filters.Length; i++)
			{
				if (dialog.Filters[i] == dialog.Filter) return i;
			}
			return - 1;
		}

		private void WindowOnDeleteEvent(object o, DeleteEventArgs args)
		{
			var eventArgs = new ClosingEventArgs();
			Closing.Invoke(eventArgs);

			args.RetVal = eventArgs.Cancel;			
		}

		private MenuItem fileMenuItem;
		private ImageMenuItem fileNew;
		private ImageMenuItem fileOpen;
		private ImageMenuItem fileClose;
		private ImageMenuItem fileSave;
		private ImageMenuItem fileSaveAs;
		private ImageMenuItem fileExit;

		private MenuItem editMenuItem;
		private ImageMenuItem editRestart;
		private ImageMenuItem editUndo;
		private ImageMenuItem editRedo;
		private ImageMenuItem editCut;
		private ImageMenuItem editCopy;
		private ImageMenuItem editPaste;

		private MenuItem viewMenuItem;
		private CheckMenuItem viewShowOrigin;
		private List<ColorMenuItem> viewShowColor = new List<ColorMenuItem>();

		private RadioMenuItem viewModeShowSelected;
		private RadioMenuItem viewModeShowSelectedChildren;
		private RadioMenuItem viewModeShowFull;
		
		private MenuBar CreateMenu()
		{
			var menuBar = new MenuBar();
			
			// File menu
			fileMenuItem = new MenuItem("_File");
			var fileMenu = new Menu();
			fileMenuItem.Submenu = fileMenu;
			menuBar.Append(fileMenuItem);
			
			fileNew = new ImageMenuItem(Stock.New, accelerators);
			fileNew.Activated += (sender, args) => FileNew.Invoke();
			fileMenu.Append(fileNew);

			fileOpen = new ImageMenuItem(Stock.Open, accelerators);
			fileOpen.Activated += (sender, args) => FileOpen.Invoke();
			fileMenu.Append(fileOpen);

			fileClose = new ImageMenuItem(Stock.Close, accelerators);
			fileClose.Activated += (sender, args) => FileClose.Invoke();
			fileMenu.Append(fileClose);

			fileMenu.Append(new SeparatorMenuItem());

			fileSave = new ImageMenuItem(Stock.Save, accelerators);
			fileSave.Activated += (sender, args) => FileSave.Invoke();
			fileMenu.Append(fileSave);

			fileSaveAs = new ImageMenuItem(Stock.SaveAs, accelerators);
			fileSaveAs.Activated += (sender, args) => FileSaveAs.Invoke();
			fileMenu.Append(fileSaveAs);

			fileMenu.Append(new SeparatorMenuItem());

			fileExit = new ImageMenuItem(Stock.Quit, accelerators);
			fileExit.Activated += FileExitOnActivated;
			fileMenu.Append(fileExit);

			// Edit menu
			editMenuItem = new MenuItem("_Edit");
			var editMenu = new Menu();
			editMenuItem.Submenu = editMenu;
			menuBar.Append(editMenuItem);

			editRestart = new ImageMenuItem("Restart");
			editRestart.Image = new Gtk.Image(Stock.Refresh, IconSize.Button);
			editRestart.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.F5, ModifierType.None, AccelFlags.Visible));
			editRestart.Activated += (sender, args) => EditRestart.Invoke();
			editMenu.Append(editRestart);

			editMenu.Append(new SeparatorMenuItem());

			editUndo = new ImageMenuItem(Stock.Undo, accelerators);
			editUndo.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.z, ModifierType.ControlMask, AccelFlags.Visible));
			editUndo.Activated += (sender, args) => EditUndo.Invoke();
			editMenu.Append(editUndo);

			editRedo = new ImageMenuItem(Stock.Redo, accelerators);
			editRedo.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.y, ModifierType.ControlMask, AccelFlags.Visible));
			editRedo.Activated += (sender, args) => EditRedo.Invoke();
			editMenu.Append(editRedo);

			editMenu.Append(new SeparatorMenuItem());
			
			editCut = new ImageMenuItem(Stock.Cut, accelerators);
			editCut.Activated += (sender, args) => EditCut.Invoke();
			editMenu.Append(editCut);

			editCopy = new ImageMenuItem(Stock.Copy, accelerators);
			editCopy.Activated += (sender, args) => EditCopy.Invoke();
			editMenu.Append(editCopy);

			editPaste = new ImageMenuItem(Stock.Paste, accelerators);
			editPaste.Activated += (sender, args) => EditPaste.Invoke();
			editMenu.Append(editPaste);
			
			// View menu
			viewMenuItem = new MenuItem("_View");
			var viewMenu = new Menu();
			viewMenuItem.Submenu = viewMenu;
			menuBar.Append(viewMenuItem);

			viewShowOrigin = new CheckMenuItem("Show Origin");
			viewShowOrigin.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.e, ModifierType.ControlMask, AccelFlags.Visible));
			viewShowOrigin.Activated += (sender, args) => ToggleShowOrigin.Invoke();
			viewMenu.Append(viewShowOrigin);

			// View mode submenu
			var viewModeMenu = new Menu();
			var viewModeMenuItem = new MenuItem("View Mode");
			viewModeMenuItem.Submenu = viewModeMenu;
			viewMenu.Append(viewModeMenuItem);

			viewModeShowSelected = new RadioMenuItem("Selected Effect");
			viewModeShowSelected.AddAccelerator("activate", accelerators, new AccelKey());
			viewModeShowSelected.Activated += (sender, args) => ViewModeChanged.Invoke();
			viewModeMenu.Append(viewModeShowSelected);

			viewModeShowSelectedChildren = new RadioMenuItem(viewModeShowSelected.Group, "Selected Effect + Children");
			viewModeShowSelectedChildren.AddAccelerator("activate", accelerators, new AccelKey());
			viewModeShowSelectedChildren.Activated += (sender, args) => ViewModeChanged.Invoke();
			viewModeMenu.Append(viewModeShowSelectedChildren);

			viewModeShowFull = new RadioMenuItem(viewModeShowSelected.Group, "Full Effect");
			viewModeShowFull.AddAccelerator("activate", accelerators, new AccelKey());
			viewModeShowFull.Activated += (sender, args) => ViewModeChanged.Invoke();
			viewModeMenu.Append(viewModeShowFull);

			// Background color submenu

			var backgroundMenu = new Menu();
			var backgroundMenuItem = new MenuItem("Background Color");
			backgroundMenuItem.Submenu = backgroundMenu;
			viewMenu.Append(backgroundMenuItem);
			
			for (int i = 0; i < 10; i++)
			{
				int index = ((i + 1) % 10);
				var v = new ColorMenuItem("Color");
				
				v.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.Key_0 + index, ModifierType.ControlMask, AccelFlags.Visible));

				if (viewShowColor.Count > 0)
				{
					v.Group = viewShowColor[0].Group;
				}
				
				v.Activated += (sender, args) => BackgroundColorChanged.Invoke();
				viewShowColor.Add(v);
				backgroundMenu.Append(v);
			}

			viewShowColor[0].Active = true;
			
			return menuBar;
		}
		
		private void FileExitOnActivated(object sender, EventArgs eventArgs)
		{
			var e = Gdk.EventHelper.New(Gdk.EventType.Delete);
			if (!window.ProcessEvent(e))
			{
				window.Destroy();
			}
		}

		public void Dispose()
		{
			if (window != null)
			{
				window.Dispose();
				window = null;
			}
		}
	}
}
