using System;

using Gtk;

using MG.EditorCommon.HaxGraph;
using MG.Framework.Numerics;

using Action = System.Action;

namespace MG.ParticleEditorWindow
{
	public class GraphView
	{
		internal HaxGraph Widget;

		public event Action Changed = delegate { };
		
		public GraphView()
		{
			Widget = new HaxGraph();
			Widget.Changed += OnChanged;
		}

		private void OnChanged()
		{
			Changed();
		}

		public ComplexCurve Curve
		{
			get { return Widget.Curve; }
			set { Widget.Curve = value; }
		}
	}
}
