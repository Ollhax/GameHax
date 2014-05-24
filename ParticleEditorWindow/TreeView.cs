using System;
using System.Collections.Generic;

using Gtk;

using Action = System.Action;

namespace MG.ParticleEditorWindow
{
	public class TreeView
	{
		private ScrolledWindow scrolledWindow;
		private Gtk.TreeStore storage;
		private Gtk.TreeView treeView;
		private const int ColumnId = 0;
		private const int ColumnName = 1;

		internal Widget Widget { get { return scrolledWindow; } }
		
		public class ContextMenu
		{
			public int ItemId;

			public class Entry
			{
				public string Text;
				public Action Action;
				public List<Entry> SubEntries;

				public Entry(string text, Action action)
				{
					Text = text;
					Action = action;
				}
			}

			public List<Entry> Entries;
		}

		public event Action<int> ItemSelected = delegate { };
		public event Func<int, string, bool> ItemRenamed = delegate { return false; };
		public event Action<ContextMenu> CreateContextMenu = delegate { };
		
		public TreeView()
		{
			treeView = new Gtk.TreeView();
			scrolledWindow = new ScrolledWindow();
			
			scrolledWindow.Add(treeView);
			treeView.CanFocus = true;
			treeView.Name = "treeview";
			treeView.EnableSearch = true;
			treeView.SearchColumn = ColumnName;
			//treeView.EnableGridLines = TreeViewGridLines.Horizontal;
			//treeView.EnableTreeLines = true;
			
			var effectColumn = new Gtk.TreeViewColumn();
			effectColumn.Title = "Effects";
			
			treeView.AppendColumn(effectColumn);

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

			treeView.Model = storage;
			treeView.Selection.Changed += OnSelectionChanged;
			//treeView.CursorChanged += (sender, args) => ItemSelected(treeView.Selection.);
			//treeView.CursorChanged += OnCursorChanged;
			//treeView.RowActivated += WidgetOnRowActivated;
			treeView.ButtonPressEvent += OnButtonPress;
			
			var cellRendererText = new CellRendererText();
			cellRendererText.Editable = true;
			cellRendererText.Edited += OnTextEdited;
			effectColumn.PackStart(cellRendererText, true);
			effectColumn.AddAttribute(cellRendererText, "text", ColumnName);
		}
		
		public void SelectItem(int id)
		{
			storage.Foreach(delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					if ((int)model.GetValue(treeIter, ColumnId) == id)
					{
						treeView.Selection.SelectIter(treeIter);
						treeView.ScrollToCell(path, null, false, 0, 0);
						return true;
					}
					return false;
				});
		}
		
		private void OnTextEdited(object o, EditedArgs args)
		{
			TreeIter iter;
			if (storage.GetIterFromString(out iter, args.Path))
			{
				int id = (int)storage.GetValue(iter, ColumnId);
				if (ItemRenamed(id, args.NewText))
				{
					storage.SetValue(iter, ColumnName, args.NewText);
				}
			}
			//args.Path
		}

		private void OnSelectionChanged(object sender, EventArgs eventArgs)
		{
			ItemSelected.Invoke(GetSelectedItemId());
		}

		private int GetSelectedItemId()
		{
			TreeModel model;
			TreeIter iter;
			if (treeView.Selection.GetSelected(out model, out iter))
			{
				int id = (int)model.GetValue(iter, ColumnId);
				return id;
			}

			return 0;
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

		[GLib.ConnectBefore]
		private void OnButtonPress(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) // Right click
			{
				TreePath selectedItemPath;
				treeView.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out selectedItemPath);
				treeView.Selection.SelectPath(selectedItemPath);
				
				var menu = new ContextMenu();
				menu.ItemId = GetSelectedItemId();
				menu.Entries = new List<ContextMenu.Entry>();
				CreateContextMenu(menu);

				if (menu.Entries.Count > 0)
				{
					var m = new Menu();
					foreach (var entry in menu.Entries)
					{
						AddMenuEntry(entry, m);
					}

					m.ShowAll();
					m.Popup();
				}
			}
		}

		private void AddMenuEntry(ContextMenu.Entry entry, Menu parentMenu)
		{
			var item = new MenuItem(entry.Text);
			if (entry.Action != null)
			{
				item.ButtonPressEvent += delegate { entry.Action(); };
			}

			parentMenu.Add(item);

			if (entry.SubEntries != null)
			{
				var itemMenu = new Menu();
				item.Submenu = itemMenu;

				foreach (var subEntry in entry.SubEntries)
				{
					AddMenuEntry(subEntry, itemMenu);
				}
			}
		}
	}
}
