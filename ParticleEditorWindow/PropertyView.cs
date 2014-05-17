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
		public event Action Deselected = delegate { };

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
		
		public void SetCurrentObject(object o)
		{
			Widget.CurrentObject = o;
		}

		private void OnChanged(object sender, EventArgs eventArgs)
		{
			PropertyChanged.Invoke();
		}

		private void OnDeselected(object o, EventArgs args)
		{
			Deselected.Invoke();
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
