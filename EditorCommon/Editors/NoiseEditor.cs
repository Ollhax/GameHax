using System;

using Gtk;

using MG.EditorCommon.HaxWidgets;
using MG.Framework.Numerics;

using MonoDevelop.Components.PropertyGrid;

namespace MG.EditorCommon.Editors
{
	public class NoiseEditor : PropertyEditorCell
	{
		private HaxNoise noiseGraph;

		protected override void Initialize()
		{
			base.Initialize();

			var declarationParameter = ((ParticleParameterDescriptor)Property).DeclarationParameter;

			noiseGraph = new HaxNoise();
			noiseGraph.Noise = (Noise)Property.GetValue(Instance);
			noiseGraph.MinValueY = declarationParameter.CurveMin;
			noiseGraph.MaxValueY = declarationParameter.CurveMax;
		}

		public override void GetSize(int availableWidth, out int width, out int height)
		{
			base.GetSize(availableWidth, out width, out height);
			height = 80;
		}

		public override void Render(Gdk.Drawable window, Cairo.Context ctx, Gdk.Rectangle bounds, StateType state)
		{
			noiseGraph.Draw(window, ctx, bounds, state);
			//base.Render(window, ctx, bounds, state);
		}

		protected override IPropertyEditor CreateEditor(Gdk.Rectangle cell_area, Gtk.StateType state)
		{
			return new NoisePropertyEditor();
		}
	}

	public class NoisePropertyEditor : Gtk.Bin, IPropertyEditor
	{
		private HaxNoise noiseGraph;
		private int pad = 0;

		public NoisePropertyEditor()
		{
			noiseGraph = new HaxNoise();
			noiseGraph.Changed += GraphOnChanged;

			//KeyPressEvent += OnKeyPressEvent;

			Add(noiseGraph);
			ShowAll();
			
			SizeRequested += OnSizeRequested;
			SizeAllocated += OnSizeAllocated;
		}

		//private void OnKeyPressEvent(object o, KeyPressEventArgs args)
		//{
		//    noiseGraph.KeyPress(args.Event);
		//}

		private void GraphOnChanged()
		{
			ValueChanged.Invoke(this, EventArgs.Empty);
		}

		void OnSizeRequested(object o, SizeRequestedArgs args)
		{
			if (noiseGraph != null)
			{
				int width = args.Requisition.Width;
				int height = args.Requisition.Height;

				noiseGraph.GetSizeRequest(out width, out height);
				if (width == -1 || height == -1)
					width = height = 80;
				SetSizeRequest(width + pad * 2, height + pad * 2);
			}
		}

		void OnSizeAllocated(object o, SizeAllocatedArgs args)
		{
			if (noiseGraph != null)
			{
				Gdk.Rectangle mine = args.Allocation;
				Gdk.Rectangle his = mine;

				his.X += pad;
				his.Y += pad;
				his.Width -= pad * 2;
				his.Height -= pad * 2;
				noiseGraph.SizeAllocate(his);
			}
		}
		
		public void Initialize(EditSession session)
		{
			if (session.Property.PropertyType != typeof(Noise))
				throw new ApplicationException("Noise editor does not support editing values of type " + session.Property.PropertyType);

			var declarationParameter = ((ParticleParameterDescriptor)session.Property).DeclarationParameter;
			noiseGraph.MaxValueY = declarationParameter.CurveMax;
			noiseGraph.MinValueY = declarationParameter.CurveMin;
		}
		
		object IPropertyEditor.Value
		{
			get { return noiseGraph.Noise; }
			set { noiseGraph.Noise = (Noise)value; }
		}

		public event EventHandler ValueChanged;
	}
}
