using System;
using EditorCommon;

using Gdk;

using Gtk;

using MG.EditorCommon;
using MG.Framework.Utility;

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
		public event System.Action EditUndo = delegate { };
		public event System.Action EditRedo = delegate { };
		public event System.Action<ClosingEventArgs> Closing = delegate { };
		public event System.Action Closed = delegate { };
		
		public readonly RenderView RenderView;
		public readonly TreeView TreeView;
		public readonly ParameterView PropertyView;
		public readonly InfoView InfoView;
		public readonly GraphView GraphView;

		private bool sensitive = true;
		
		public string Title { get { return window.Title; } set { window.Title = value; } }
		public bool UndoEnabled { get { return editUndo.Sensitive; } set { editUndo.Sensitive = value; } }
		public bool RedoEnabled { get { return editRedo.Sensitive; } set { editRedo.Sensitive = value; } }

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
			
			window.Name = title;
			window.WindowPosition = WindowPosition.CenterOnParent;
			window.DefaultWidth = 1280;
			window.DefaultHeight = 720;

			// Subcomponents
			RenderView = new RenderView();
			TreeView = new TreeView();
			PropertyView = new ParameterView();
			InfoView = new InfoView();
			GraphView = new GraphView();

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
			hpaneMain.Position = 200;
			
			var rightBox = new HBox(false, 1);
			hpaneMain.Pack2(rightBox, true, true);
			
			// Setup gl widget and graph
			var vpaneRight = new VPaned();
			rightBox.PackStart(vpaneRight, true, true, 0);
			vpaneRight.Position = window.DefaultHeight - 220;
			
			var f1 = new Frame();
			f1.Add(RenderView.Widget);
			f1.ShadowType = ShadowType.In;

			var f2 = new Frame();
			f2.Add(GraphView.Widget);
			f2.ShadowType = ShadowType.None;
			
			vpaneRight.Pack1(f1, true, true);
			vpaneRight.Pack2(f2, true, true);

			// Tree view
			TreeView.Widget.SetSizeRequest(200, -1);
			rightBox.PackEnd(TreeView.Widget, false, true, 0);

			// Info view
			var leftBox = new VBox(false, 0);
			hpaneMain.Pack1(leftBox, false, false);
			leftBox.PackEnd(InfoView.Widget, false, true, 0);

			// Property view
			leftBox.PackEnd(PropertyView.Widget, true, true, 0);
			PropertyView.Widget.DividerPosition = 0.4;

			window.ShowAll();

			InfoView.Visible = false;
			InfoView.MetaProperties.Widget.ShowToolbar = false;
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
			dialog.AddButton(Stock.Save, Gtk.ResponseType.Ok);
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
				PropertyView.Widget.Sensitive = value;
				TreeView.Widget.Sensitive = value;
				RenderView.Widget.Sensitive = value;
				InfoView.Widget.Sensitive = value;
				GraphView.Widget.Sensitive = value;
				editMenuItem.Sensitive = value;
				fileSave.Sensitive = value;
				fileSaveAs.Sensitive = value;
				fileClose.Sensitive = value;
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
		private MenuItem editUndo;
		private MenuItem editRedo;

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

			editUndo = new ImageMenuItem(Stock.Undo, accelerators);
			editUndo.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.z, ModifierType.ControlMask, AccelFlags.Visible));
			editUndo.Activated += (sender, args) => EditUndo.Invoke();
			editMenu.Append(editUndo);

			editRedo = new ImageMenuItem(Stock.Redo, accelerators);
			editRedo.AddAccelerator("activate", accelerators, new AccelKey(Gdk.Key.y, ModifierType.ControlMask, AccelFlags.Visible));
			editRedo.Activated += (sender, args) => EditRedo.Invoke();
			editMenu.Append(editRedo);
			
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
