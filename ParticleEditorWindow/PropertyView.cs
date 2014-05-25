using System;

using Gtk;

using MonoDevelop.Components.PropertyGrid;

using Action = System.Action;

namespace MG.ParticleEditorWindow
{
	public class PropertyView
	{
		internal PropertyGrid Widget;

		public event Action PropertyChanged = delegate { };
		public event Action<bool> Deselected = delegate { };

		public PropertyView()
		{
			Widget = new PropertyGrid();
			Widget.Name = "propertyGrid";
			Widget.ShowToolbar = true;
			Widget.ShowHelp = true;
			Widget.Changed += OnChanged;
			Widget.Deselected += OnDeselected;
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

		private void OnChanged(object sender, EventArgs eventArgs)
		{
			PropertyChanged.Invoke();
		}
		
		private void OnDeselected(object sender, PropertyGrid.DeselectEventArgs deselectEventArgs)
		{
			Deselected.Invoke(deselectEventArgs.Canceled);
		}

		//ParticleDeclaration particleDeclaration;
		//if (particleDeclarationTable.Declarations.TryGetValue(particleDefinition.Declaration, out particleDeclaration))
		//{
		//    //propertygrid1.CurrentObject = new ParticlePropertyProxy(particleDeclaration, particleDefinition);
		//    propertygrid1.Changed += Propertygrid1OnChanged;
		//}

		////propertygrid1.CurrentObject = m;
	}
}
