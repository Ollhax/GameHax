using System;
using System.Collections.Generic;

using Gdk;

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
		private int disableChangeCallbacks;
		private int lastReorderId;
		private bool disableReorderChanges;
		private HashSet<int> expandedStatus = new HashSet<int>();
		private int currentSelection;

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

		public class ItemIndex
		{
			public int Id;
			public string Name;
			public List<ItemIndex> Children = new List<ItemIndex>();
		}
		
		public event Action<int> ItemSelected = delegate { };
		public event Action<int> ItemMoved = delegate { };
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
			treeView.Reorderable = true;
			//treeView.EnableGridLines = TreeViewGridLines.Horizontal;
			//treeView.EnableTreeLines = true;
			
			var effectColumn = new Gtk.TreeViewColumn();
			effectColumn.Title = "Effects";
			
			treeView.AppendColumn(effectColumn);

			storage = new Gtk.TreeStore(typeof(int), typeof(string));
			storage.RowChanged += OnRowChanged;
			storage.RowDeleted += OnRowDeleted;
			
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
		
		public void SelectItem(int id, bool scrollToSelected)
		{
			storage.Foreach(delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					if ((int)model.GetValue(treeIter, ColumnId) == id)
					{
						if (scrollToSelected)
						{
							treeView.ExpandToPath(path);
							treeView.ExpandRow(path, false);
							treeView.ScrollToCell(path, null, false, 0, 0);
						}

						treeView.Selection.SelectIter(treeIter);
						
						return true;
					}
					return false;
				});
		}
		
		public List<ItemIndex> GetItemIndices()
		{
			var indices = new List<ItemIndex>();
			
			storage.Foreach(
				delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					var id = (int)model.GetValue(treeIter, ColumnId);
					var name = (string)model.GetValue(treeIter, ColumnName);
					int parentId = 0;

					TreeIter parentIter;
					if (storage.IterParent(out parentIter, treeIter))
					{
						parentId = (int)storage.GetValue(parentIter, ColumnId);
					}
					
					var childList = GetParentList(parentId, indices);
					childList.Add(new ItemIndex { Id = id, Name = name });
					return false;
				});

			return indices;
		}

		private List<ItemIndex> GetParentList(int parentId, List<ItemIndex> baseList)
		{
			var list = GetParentListRecusive(parentId, baseList);
			if (list != null) return list;

			return baseList;
		}

		private List<ItemIndex> GetParentListRecusive(int parentId, List<ItemIndex> parentList)
		{
			foreach (var item in parentList)
			{
				if (item.Id == parentId)
				{
					return item.Children;
				}

				var r = GetParentListRecusive(parentId, item.Children);
				if (r != null) return r;
			}

			return null;
		}

		private void SaveStatus()
		{
			expandedStatus.Clear();
			currentSelection = GetSelectedItemId();
			treeView.MapExpandedRows(delegate(Gtk.TreeView view, TreePath path)
				{
					TreeIter iter;
					if (treeView.Model.GetIter(out iter, path))
					{
						expandedStatus.Add((int)storage.GetValue(iter, ColumnId));
					}
				});
		}

		private void RestoreStatus()
		{
			storage.Foreach(delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					var id = (int)model.GetValue(treeIter, ColumnId);
					if (expandedStatus.Contains(id))
					{
						treeView.ExpandToPath(path);
					}
					return false;
				});
			
			expandedStatus.Clear();
			
			if (currentSelection != 0)
			{
				SelectItem(currentSelection, false);
			}
		}

		public void SetValues(List<ItemIndex> values)
		{
			disableChangeCallbacks++;
			SaveStatus();
			storage.Clear();
			
			SetValues(TreeIter.Zero, values);

			RestoreStatus();
			disableChangeCallbacks--;
		}

		private void SetValues(TreeIter parent, List<ItemIndex> values)
		{
			foreach (var v in values)
			{
				TreeIter iter;
				if (parent.Equals(TreeIter.Zero))
				{
					iter = storage.AppendValues(v.Id, v.Name);
				}
				else
				{
					iter = storage.AppendValues(parent, v.Id, v.Name);
				}

				SetValues(iter, v.Children);
			}
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
		
		[GLib.ConnectBefore]
		private void OnButtonPress(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) // Right click
			{
				TreePath selectedItemPath;
				treeView.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out selectedItemPath);

				if (selectedItemPath == null)
				{
					treeView.Selection.UnselectAll();
				}
				else
				{
					treeView.Selection.SelectPath(selectedItemPath);
				}
				
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

		private void OnRowChanged(object o, RowChangedArgs args)
		{
			if (disableChangeCallbacks != 0) return;
			if (disableReorderChanges) return;

			lastReorderId = (int)storage.GetValue(args.Iter, ColumnId);
			disableReorderChanges = true; // If we move several items at once, we only want to know about the first one. (Yes, this is an awful hack.)
		}

		private void OnRowDeleted(object o, RowDeletedArgs args)
		{
			// Listening to RowsDeleted is a hack. We want to listen for reorder, but all other events are either not being called at all
			// or called in the middle of the reordering, not letting us getting the new item indices.

			if (disableChangeCallbacks != 0) return;
			ItemMoved.Invoke(lastReorderId);
			disableReorderChanges = false;
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
