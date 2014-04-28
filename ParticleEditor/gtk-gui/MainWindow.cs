
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.Action FileAction;
	private global::Gtk.Action MenuAction;
	private global::Gtk.Action FileAction1;
	private global::Gtk.Action FileAction2;
	private global::Gtk.VBox vbox3;
	private global::Gtk.MenuBar menubar7;
	private global::Gtk.HPaned hpaned2;
	private global::Gtk.VPaned vpaned5;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.TreeView treeview2;
	private global::Gtk.VPaned vpaned4;
	private global::EditorCommon.HaxGLWidget MainGL;
	private global::Gtk.Notebook notebook1;
	private global::Gtk.Label label1;
	private global::EditorCommon.HaxGLWidget haxglwidget1;
	private global::Gtk.Label label2;
	private global::Gtk.Statusbar statusbar5;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.MenuAction = new global::Gtk.Action ("MenuAction", global::Mono.Unix.Catalog.GetString ("Menu"), null, null);
		this.MenuAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Menu");
		w1.Add (this.MenuAction, null);
		this.FileAction1 = new global::Gtk.Action ("FileAction1", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction1, null);
		this.FileAction2 = new global::Gtk.Action ("FileAction2", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction2.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction2, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='menubar7'><menu name='FileAction2' action='FileAction2'/></menubar></ui>");
		this.menubar7 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar7")));
		this.menubar7.Name = "menubar7";
		this.vbox3.Add (this.menubar7);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.menubar7]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hpaned2 = new global::Gtk.HPaned ();
		this.hpaned2.CanFocus = true;
		this.hpaned2.Name = "hpaned2";
		this.hpaned2.Position = 121;
		// Container child hpaned2.Gtk.Paned+PanedChild
		this.vpaned5 = new global::Gtk.VPaned ();
		this.vpaned5.CanFocus = true;
		this.vpaned5.Name = "vpaned5";
		this.vpaned5.Position = 257;
		// Container child vpaned5.Gtk.Paned+PanedChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.treeview2 = new global::Gtk.TreeView ();
		this.treeview2.CanFocus = true;
		this.treeview2.Name = "treeview2";
		this.GtkScrolledWindow.Add (this.treeview2);
		this.vpaned5.Add (this.GtkScrolledWindow);
		global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.vpaned5 [this.GtkScrolledWindow]));
		w4.Resize = false;
		this.hpaned2.Add (this.vpaned5);
		global::Gtk.Paned.PanedChild w5 = ((global::Gtk.Paned.PanedChild)(this.hpaned2 [this.vpaned5]));
		w5.Resize = false;
		// Container child hpaned2.Gtk.Paned+PanedChild
		this.vpaned4 = new global::Gtk.VPaned ();
		this.vpaned4.CanFocus = true;
		this.vpaned4.Name = "vpaned4";
		this.vpaned4.Position = 273;
		// Container child vpaned4.Gtk.Paned+PanedChild
		this.MainGL = new global::EditorCommon.HaxGLWidget ();
		this.MainGL.Name = "MainGL";
		this.MainGL.SingleBuffer = false;
		this.MainGL.ColorBPP = 0;
		this.MainGL.AccumulatorBPP = 0;
		this.MainGL.DepthBPP = 0;
		this.MainGL.StencilBPP = 0;
		this.MainGL.Samples = 0;
		this.MainGL.Stereo = false;
		this.MainGL.GlVersionMajor = 2;
		this.MainGL.GlVersionMinor = 1;
		this.vpaned4.Add (this.MainGL);
		global::Gtk.Paned.PanedChild w6 = ((global::Gtk.Paned.PanedChild)(this.vpaned4 [this.MainGL]));
		w6.Resize = false;
		// Container child vpaned4.Gtk.Paned+PanedChild
		this.notebook1 = new global::Gtk.Notebook ();
		this.notebook1.CanFocus = true;
		this.notebook1.Name = "notebook1";
		this.notebook1.CurrentPage = 0;
		// Notebook tab
		global::Gtk.Label w7 = new global::Gtk.Label ();
		w7.Visible = true;
		this.notebook1.Add (w7);
		this.label1 = new global::Gtk.Label ();
		this.label1.Name = "label1";
		this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("page1");
		this.notebook1.SetTabLabel (w7, this.label1);
		this.label1.ShowAll ();
		// Container child notebook1.Gtk.Notebook+NotebookChild
		this.haxglwidget1 = new global::EditorCommon.HaxGLWidget ();
		this.haxglwidget1.Name = "haxglwidget1";
		this.haxglwidget1.SingleBuffer = false;
		this.haxglwidget1.ColorBPP = 0;
		this.haxglwidget1.AccumulatorBPP = 0;
		this.haxglwidget1.DepthBPP = 0;
		this.haxglwidget1.StencilBPP = 0;
		this.haxglwidget1.Samples = 0;
		this.haxglwidget1.Stereo = false;
		this.haxglwidget1.GlVersionMajor = 2;
		this.haxglwidget1.GlVersionMinor = 1;
		this.notebook1.Add (this.haxglwidget1);
		global::Gtk.Notebook.NotebookChild w8 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.haxglwidget1]));
		w8.Position = 1;
		// Notebook tab
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("page2");
		this.notebook1.SetTabLabel (this.haxglwidget1, this.label2);
		this.label2.ShowAll ();
		this.vpaned4.Add (this.notebook1);
		this.hpaned2.Add (this.vpaned4);
		this.vbox3.Add (this.hpaned2);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hpaned2]));
		w11.Position = 1;
		// Container child vbox3.Gtk.Box+BoxChild
		this.statusbar5 = new global::Gtk.Statusbar ();
		this.statusbar5.Name = "statusbar5";
		this.statusbar5.Spacing = 6;
		this.vbox3.Add (this.statusbar5);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.statusbar5]));
		w12.Position = 2;
		w12.Expand = false;
		w12.Fill = false;
		this.Add (this.vbox3);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 937;
		this.DefaultHeight = 526;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
