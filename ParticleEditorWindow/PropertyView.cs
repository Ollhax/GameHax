using System;

using MonoDevelop.Components.PropertyGrid;

namespace MG.ParticleEditorWindow
{
	public class PropertyView
	{
		internal PropertyGrid Widget;

		public event Action PropertyChanged = delegate { };

		public PropertyView()
		{
			Widget = new PropertyGrid();
			Widget.Name = "propertyGrid";
			Widget.ShowToolbar = true;
			Widget.ShowHelp = true;
			Widget.Changed += OnChanged;
		}
		
		public void SetCurrentObject(object o)
		{
			Widget.CurrentObject = o;
		}

		private void OnChanged(object sender, EventArgs eventArgs)
		{
			PropertyChanged.Invoke();
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
