using System;
using MonoDevelop.Components.PropertyGrid;
using Action = System.Action;

namespace MG.ParticleEditorWindow
{
	public class ParameterView
	{
		internal PropertyGrid Widget;

		public event Action Changed = delegate { };
		public event Action StartedEdit = delegate { };
		public event Action<bool> EndedEdit = delegate { };
		public event Action<string> SelectionChanged = delegate { };
		
		public ParameterView()
		{
			Widget = new PropertyGrid();
			Widget.Changed += OnChanged;
			Widget.StartedEdit += OnStartedEdit;
			Widget.EndedEdit += OnEndedEdit;
			Widget.SelectionChanged += OnSelectionChanged;
		}

		public void CommitChanges()
		{
			Widget.CommitPendingChanges();
		}

		public void CancelChanges()
		{
			Widget.CancelPendingChanges();
		}

		public void Refresh()
		{
			Widget.Refresh();
		}
		
		public void SetCurrentObject(object o)
		{
			Widget.CurrentObject = o;
		}

		public void SelectParameter(string parameter)
		{
			Widget.SelectedProperty = parameter;
		}

		private void OnChanged(object sender, EventArgs eventArgs)
		{
			Changed.Invoke();
		}

		private void OnStartedEdit(object sender, EventArgs eventArgs)
		{
			StartedEdit.Invoke();
		}
		
		private void OnEndedEdit(object sender, PropertyGrid.EndedEditEventArgs endedEditEventArgs)
		{
			EndedEdit.Invoke(endedEditEventArgs.Canceled);
		}
		
		private void OnSelectionChanged(object sender, EventArgs eventArgs)
		{
			SelectionChanged(Widget.SelectedProperty);
		}
	}
}
