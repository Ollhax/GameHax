using System;
using System.Collections.Generic;

using Gtk;

namespace MG.ParticleEditorWindow
{
	public class TreeView
	{
		internal Gtk.TreeView Widget;
		
		private Gtk.TreeStore storage;

		public event System.Action<int> ItemSelected = delegate { };

		public TreeView()
		{
			Widget = new Gtk.TreeView();
			Widget.CanFocus = true;
			Widget.Name = "treeview";
			Widget.EnableSearch = true;
			Widget.SearchColumn = 1;
			//Widget.EnableGridLines = TreeViewGridLines.Horizontal;
			//Widget.EnableTreeLines = true;
			
			var effectColumn = new Gtk.TreeViewColumn();
			effectColumn.Title = "Effects";

			Widget.AppendColumn(effectColumn);

			storage = new Gtk.TreeStore(typeof(int), typeof(string));
			//Gtk.TreeIter iter = listStore.AppendValues("Moop");
			//Gtk.TreeIter iter = listStore.AppendValues("Moop");
			//listStore.AppendValues(iter, "Garbage", "Dog New Tricks");
			//listStore.AppendValues(iter, "Google", "Schmoogle");

			//iter = listStore.AppendValues("Doop");
			//listStore.AppendValues(iter, "Wooga", "Googa");

			//foreach (var particleDefinitionPair in particleDefinitionTable.Definitions)
			//{
			//    var def = particleDefinitionPair.Value;
			//    listStore.AppendValues(def.Name);
			//}

			Widget.Model = storage;
			Widget.Selection.Changed += OnSelectionChanged;
			//Widget.CursorChanged += (sender, args) => ItemSelected(Widget.Selection.);
			//Widget.CursorChanged += OnCursorChanged;
			//Widget.RowActivated += WidgetOnRowActivated;

			var artistNameCell = new CellRendererText();
			effectColumn.PackStart(artistNameCell, true);
			effectColumn.AddAttribute(artistNameCell, "text", 1);
		}

		private void OnSelectionChanged(object sender, EventArgs eventArgs)
		{
			TreeModel model;
			TreeIter iter;
			if (Widget.Selection.GetSelected(out model, out iter))
			{
				int id = (int)model.GetValue(iter, 0);
				ItemSelected.Invoke(id);
			}
		}

		//private void OnCursorChanged(object sender, EventArgs eventArgs)
		//{
		//    TreeIter iter;
		//    storage.GetIter(out iter, eventArgs.Path);
		//    var value = model.GetValue(iter, 0);
		//    Console.WriteLine(value);
		//}

		public void SetValues(List<KeyValuePair<int, string>> values)
		{
			storage.Clear();

			foreach (var kvp in values)
			{
				storage.AppendValues(kvp.Key, kvp.Value);
			}

			//storage.AppendValues(id, value);
		}
	}
}
