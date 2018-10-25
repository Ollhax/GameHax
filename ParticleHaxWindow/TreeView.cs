using System;
using System.Collections.Generic;

using Gtk;

using Action = System.Action;
using Key = Gdk.Key;

namespace MG.ParticleEditorWindow
{
	public class TreeView
	{
		private Gtk.VBox box;
		private ScrolledWindow scrolledWindow;
		private Gtk.TreeStore storage;
		private Gtk.TreeView treeView;
		private Gtk.TreeViewColumn effectColumn;
		private Gtk.Button sortButton;
		private const int ColumnId = 0;
		private const int ColumnName = 1;
		private const int ColumnShow = 2;
		private const int ColumnTogglable = 3;
		private int disableChangeCallbacks;
		private int lastReorderId;
		private bool disableReorderChanges;
		private HashSet<int> expandedStatus = new HashSet<int>();
		private int currentSelection;

		internal Widget Widget { get { return box; } }
		
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
			public bool IsGroup;
			public List<ItemIndex> Children = new List<ItemIndex>();
		}
		
		public event Action<int> ItemSelected = delegate { };
		public event Action<int> ItemMoved = delegate { };
		public event Action<int> ItemDeleted = delegate { };
		public event Action<int, bool> ItemInvisible = delegate { };
		public event Func<int, string, bool> ItemRenamed = delegate { return false; };
		public event Action<ContextMenu> CreateContextMenu = delegate { };
		public event Action SortEffects = delegate { };
		
		public TreeView()
		{
			box = new VBox();
			treeView = new Gtk.TreeView();
			scrolledWindow = new ScrolledWindow();
			sortButton = new Gtk.Button("Sort by name");

			box.PackStart(sortButton, false, false, 0);
			sortButton.Clicked += OnSortEffects;

			box.PackStart(scrolledWindow, true, true, 0);
			scrolledWindow.Add(treeView);
			treeView.CanFocus = true;
			treeView.Name = "treeview";
			treeView.EnableSearch = true;
			treeView.SearchColumn = ColumnName;
			treeView.Reorderable = true;
			treeView.HeadersVisible = false;
			
			//treeView.EnableGridLines = TreeViewGridLines.Horizontal;
			//treeView.EnableTreeLines = true;

			effectColumn = new Gtk.TreeViewColumn();
			effectColumn.Title = "Effects";
			
			treeView.AppendColumn(effectColumn);
			treeView.AppendColumn(new TreeViewColumn()); // Add a dummy column. This steals the horizontal space right of the label, allowing us to select entries properly.

			storage = new Gtk.TreeStore(typeof(int), typeof(string), typeof(bool), typeof(bool));
			storage.RowChanged += OnRowChanged;
			storage.RowDeleted += OnRowDeleted;


			treeView.Model = storage;
			treeView.Selection.Changed += OnSelectionChanged;
			//treeView.CursorChanged += (sender, args) => ItemSelected(treeView.Selection.);
			//treeView.CursorChanged += OnCursorChanged;
			//treeView.RowActivated += WidgetOnRowActivated;
			treeView.ButtonPressEvent += OnButtonPress;
			treeView.KeyPressEvent += OnKeyPress;

			var tog = new Gtk.CellRendererToggle();
			tog.Toggled += new Gtk.ToggledHandler(OnToggled);
			effectColumn.PackStart(tog, false);
			effectColumn.AddAttribute(tog, "active", ColumnShow);
			effectColumn.AddAttribute(tog, "activatable", ColumnTogglable);

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

		private void OnToggled(object o, ToggledArgs args)
		{
			Gtk.TreeIter iter;
 			if (storage.GetIterFromString (out iter, args.Path)) {
 				bool val = (bool) storage.GetValue (iter, ColumnShow);
 				storage.SetValue (iter, ColumnShow, !val);
				var id = (int)storage.GetValue(iter, ColumnId);
				ItemInvisible.Invoke(id, val);
			}
		}

		public void ShowAll(bool invokeAction)
		{
			storage.Foreach(
				delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					bool val = (bool)storage.GetValue(treeIter, ColumnShow);
					if (!val)
					{
						model.SetValue(treeIter, ColumnShow, true);
						if (invokeAction)
						{
							var id = (int)storage.GetValue(treeIter, ColumnId);
							ItemInvisible.Invoke(id, false);
						}
					}
					return false;
				});
		}

		public void SetVisibilityRecursive(int id, bool visible)
		{
			storage.Foreach(
				delegate(TreeModel model, TreePath path, TreeIter treeIter)
				{
					if ((int)storage.GetValue(treeIter, ColumnId) == id)
					{
						if ((bool)storage.GetValue(treeIter, ColumnTogglable) == true)
						{
							model.SetValue(treeIter, ColumnShow, visible);
							ItemInvisible.Invoke(id, !visible);
						}
						if (storage.IterHasChild(treeIter))
						{
							int count = storage.IterNChildren(treeIter);
							for (int i = 0; i < count; ++i)
							{
								TreeIter childIter;
								if (storage.IterNthChild(out childIter, treeIter, i) == true)
								{
									int childId = (int)storage.GetValue(childIter, ColumnId);
									if (storage.IterHasChild(childIter))
									{
										SetVisibilityRecursive(childId, visible);
									}
									else
									{
										if ((bool)storage.GetValue(childIter, ColumnTogglable) == true)
										{
											model.SetValue(childIter, ColumnShow, visible);
											ItemInvisible.Invoke(childId, !visible);
										}
									}
								}
							}
						}
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
					var togglable = (bool)model.GetValue(treeIter, ColumnTogglable);
					int parentId = 0;

					TreeIter parentIter;
					if (storage.IterParent(out parentIter, treeIter))
					{
						parentId = (int)storage.GetValue(parentIter, ColumnId);
					}
					
					var childList = GetParentList(parentId, indices);
					childList.Add(new ItemIndex { Id = id, Name = name, IsGroup = !togglable });
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
					if (storage.GetIter(out iter, path))
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

		public void SetValues(List<ItemIndex> values, HashSet<int> invisibleIds)
		{
			disableChangeCallbacks++;
			SaveStatus();
			storage.Clear();

			SetValues(TreeIter.Zero, values, invisibleIds);

			RestoreStatus();
			disableChangeCallbacks--;
		}

		private void SetValues(TreeIter parent, List<ItemIndex> values, HashSet<int> invisibleIds)
		{
			foreach (var v in values)
			{
				TreeIter iter;
				if (parent.Equals(TreeIter.Zero))
				{
					iter = storage.AppendValues(v.Id, v.Name, !invisibleIds.Contains(v.Id), !v.IsGroup);
				}
				else
				{
					iter = storage.AppendValues(parent, v.Id, v.Name, !invisibleIds.Contains(v.Id), !v.IsGroup);
				}

				SetValues(iter, v.Children, invisibleIds);
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
			if (treeView.Selection != null && treeView.Selection.GetSelected(out model, out iter))
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

		private void OnSortEffects(object sender, EventArgs eventArgs)
		{
			SortEffects.Invoke();
		}
		
		[GLib.ConnectBefore]
		private void OnButtonPress(object o, ButtonPressEventArgs args)
		{
			TreePath selectedItemPath;
			TreeViewColumn column;
			treeView.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out selectedItemPath, out column);

			bool deselect = false;

			if (selectedItemPath == null || column != effectColumn)
			{
				treeView.Selection.UnselectAll();
				deselect = true;
			}
			else
			{
				treeView.Selection.SelectPath(selectedItemPath);
			}

			if (args.Event.Button == 3) // Right click
			{
				if (deselect)
				{
					args.RetVal = true; // Don't select the entry
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

		[GLib.ConnectBefore]
		private void OnKeyPress(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Key.Delete)
			{
				var selectedItem = GetSelectedItemId();
				if (selectedItem > 0)
				{
					ItemDeleted(selectedItem);
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
			var label = item.Child as Label;

			if (label != null)
			{
				label.UseUnderline = false;
				label.UseMarkup = false;
			}
			
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
