using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using Gtk;

using MG.Framework.Assets;
using MG.Framework.Graphics;
using MG.Framework.Numerics;
using MG.Framework.Particle;
using MG.Framework.Utility;
using MG.ParticleEditor;
using MG.ParticleEditor.Property;

using MonoDevelop.Components.PropertyGrid;

public partial class MainWindow: Gtk.Window
{
	const int id = 1;
	private Texture2D texture;

	private AssetHandler assetHandler;
	private ParticleSystem particleSystem;
	private ParticleDefinition particleDefinition;
	private ParticleDefinitionTable particleDefinitionTable;
	private ParticleDeclarationTable particleDeclarationTable;

	ParticleDefinition CreateParticle(string name)
	{
	    ParticleDeclaration declaration;
	    if (!particleDeclarationTable.Declarations.TryGetValue(name, out declaration)) return null;

	    var definition = new ParticleDefinition();
	    definition.Name = declaration.Name;
		definition.Declaration = name;

	    foreach (var declarationParameterPair in declaration.Parameters)
	    {
	    	var declarationParameter = declarationParameterPair.Value;
	    	var definitionParameter = new ParticleDefinition.Parameter();

	    	definitionParameter.Name = declarationParameter.Name;
	    	definitionParameter.Value = declarationParameter.DefaultValue;
			definitionParameter.Random = declarationParameter.DefaultValueRandom;

			definition.Parameters.Add(definitionParameter.Name, definitionParameter);
	    }

		return definition;
	}

	public MainWindow() : base (Gtk.WindowType.Toplevel)
	{
		Build();
		
		particleDeclarationTable = new ParticleDeclarationTable();
		particleDeclarationTable.Load("ParticleDeclarations.xml");

		particleDefinitionTable = new ParticleDefinitionTable();
		//particleDefinitionTable.Load("definitions.xml");

		particleDefinition = CreateParticle("Basic");
		particleDefinitionTable.Definitions.Add(particleDefinition.Name, particleDefinition);

		//MainGL.DoubleBuffered = false;
		MainGL.Draw += MainGlOnDraw;
		MainGL.Load += MainGlOnLoad;

		statusbar5.Push(id, "Meep");
		
		SetupTree();
		SetupPropertyGrid();
	}
	
	//private class Meeper : ICustomTypeDescriptor
	//{
	//    public enum Woopa
	//    {
	//        Goopa,
	//        Doopa
	//    }

	//    [Category("Doop")]
	//    [DisplayName("Name")]
	//    [Description("Name of the file.")]
	//    public int Value1 { get; set; }

	//    public float Value2 { get; set; }

	//    [Description("Value of beeper.")]

	//    public string Beeper { get; set; }

	//    public Woopa Selector { get; set; }

	//    public Rectangle Rectangle { get; set; }

	//    public RectangleF RectangleF { get; set; }

	//    public Vector2 Vector2 { get; set; }

	//    public Vector2I Vector2I { get; set; }

	//    public FilePath File { get; set; }

	//    AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
	//    string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
	//    string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
	//    TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
	//    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
	//    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
	//    object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
	//    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
	//    EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
	//    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }

	//    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
	//    {
	//        return GetProperties(null);
	//    }

	//    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
	//    {
	//        var properties = new List<PropertyDescriptor>();
	//        PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this.GetType(), attributes);

	//        foreach (PropertyDescriptor oPropertyDescriptor in pdc)
	//        {
	//            properties.Add(oPropertyDescriptor);
	//        }

	//        return new PropertyDescriptorCollection(properties.ToArray());
	//    }
	//}

	//Meeper m = new Meeper();

	private void SetupPropertyGrid()
	{
		ParticleDeclaration particleDeclaration;
		if (particleDeclarationTable.Declarations.TryGetValue(particleDefinition.Declaration, out particleDeclaration))
		{
			propertygrid1.CurrentObject = new ParticlePropertyProxy(particleDeclaration, particleDefinition);
			propertygrid1.Changed += Propertygrid1OnChanged;
		}

		//propertygrid1.CurrentObject = m;
	}

	private void Propertygrid1OnChanged(object sender, EventArgs eventArgs)
	{
		particleSystem.Reload();
		//Console.WriteLine(m.Beeper);
		//Console.WriteLine(m.Selector);
		//Console.WriteLine(m.Value1);
	}
	
	private void SetupTree()
	{
		// Create a column for the artist name
		Gtk.TreeViewColumn artistColumn = new Gtk.TreeViewColumn();
		artistColumn.Title = "Artist";

		// Create a column for the song title
		Gtk.TreeViewColumn songColumn = new Gtk.TreeViewColumn();
		songColumn.Title = "Song Title";

		// Add the columns to the TreeView
		treeview2.AppendColumn(artistColumn);
		treeview2.AppendColumn(songColumn);

		// Create a model that will hold two strings - Artist Name and Song Title
		Gtk.TreeStore musicListStore = new Gtk.TreeStore(typeof(string), typeof(string));
		Gtk.TreeIter iter = musicListStore.AppendValues("Moop");
		musicListStore.AppendValues(iter, "Garbage", "Dog New Tricks");
		musicListStore.AppendValues(iter, "Google", "Schmoogle");

		iter = musicListStore.AppendValues("Doop");
		musicListStore.AppendValues(iter, "Wooga", "Googa");

		// Assign the model to the TreeView
		treeview2.Model = musicListStore;


		// Create the text cell that will display the artist name
		Gtk.CellRendererText artistNameCell = new Gtk.CellRendererText();

		// Add the cell to the column
		artistColumn.PackStart(artistNameCell, true);

		// Do the same for the song title column
		Gtk.CellRendererText songTitleCell = new Gtk.CellRendererText();
		songColumn.PackStart(songTitleCell, true);


		// Tell the Cell Renderers which items in the model to display
		artistColumn.AddAttribute(artistNameCell, "text", 0);
		songColumn.AddAttribute(songTitleCell, "text", 1);
	}

	protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
	{
		//propertygrid1.Refresh(); // Fixes stuff not redrawing properly
		return base.OnConfigureEvent(evnt);
	}

	private void MainGlOnLoad()
	{
		assetHandler = new AssetHandler(".");
		
		//texture = new Texture2D("weapon_laser_red.png");
		particleSystem = new ParticleSystem(assetHandler, particleDefinition);
		
		//GLib.Idle.Add(Refresh);
		GLib.Timeout.Add(33, Refresh);

		//drawThread = new Thread(DrawThreadUpdate);
		//drawThread.Start();
	}


	//private void DrawThreadUpdate()
	//{
	//    while (true)
	//    {
	//        Refresh();
	//    }
	//}

	private void MainGlOnDraw(RenderContext renderContext)
	{
		GraphicsDevice.ClearColor = Color.CornflowerBlue;
		GraphicsDevice.Clear();
		GraphicsDevice.SetViewport((Rectangle)renderContext.ActiveScreen.NormalizedScreenArea, renderContext.ActiveScreen);

		particleSystem.Position = new Vector2(renderContext.ActiveScreen.NormalizedScreenArea.Center);
		particleSystem.Draw(renderContext);
	}

	private Stopwatch stopwatch = new Stopwatch();
	//private Thread drawThread;
	
	private bool Refresh()
	{
		float elapsedTime = 0;
		if (!stopwatch.IsRunning)
		{
			stopwatch.Start();
		}
		else
		{
			elapsedTime = (float)stopwatch.Elapsed.TotalSeconds;
			stopwatch.Restart();
		}

		assetHandler.Update();
		particleSystem.Update(new Time(elapsedTime, 0));

		//Gtk.Application.Invoke(
		//    (sender, args) =>
		//        {
		//            MainGL.QueueDraw();
		//            statusbar5.Pop(0);
		//            statusbar5.Push(0, "Particles: " + particleSystem.ActiveParticles.ToString());
		//        });

		//Thread.Sleep(10);

		statusbar5.Pop(0);
		statusbar5.Push(0, "Particles: " + particleSystem.ActiveParticles.ToString());

		MainGL.QueueDraw();
		// again after the timeout period expires.   Returning false would
		// terminate the timeout.

		return true;
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	//protected void OnGlwidget2RenderFrame(object sender, EventArgs e)
	//{
	//    GraphicsDevice.Clear();
	//}

	//protected void OnGlwidget1RenderFrame(object sender, EventArgs e)
	//{
	//    GraphicsDevice.Clear();
	//}
}
