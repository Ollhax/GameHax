using System;

using Gtk;

using MG.EditorCommon.HaxWidgets;
using MG.Framework.Numerics;

using MonoDevelop.Components.PropertyGrid;

namespace MG.EditorCommon.Editors
{
	public class GradientEditor : PropertyEditorCell
	{
		private HaxGradient gradient;

		protected override void Initialize()
		{
			base.Initialize();

			gradient = new HaxGradient();
			gradient.Gradient = (Gradient)Property.GetValue(Instance);
		}

		public override void GetSize(int availableWidth, out int width, out int height)
		{
			base.GetSize(availableWidth, out width, out height);
			//height = 40;
		}

		public override void Render(Gdk.Drawable window, Cairo.Context ctx, Gdk.Rectangle bounds, StateType state)
		{
			gradient.Draw(window, ctx, bounds, state);
		}

		protected override IPropertyEditor CreateEditor(Gdk.Rectangle cell_area, Gtk.StateType state)
		{
			return new GradientPropertyEditor();
		}
	}

	public class GradientPropertyEditor : Gtk.Bin, IPropertyEditor
	{
		private HaxGradient gradient;
		private int pad = 0;

		public GradientPropertyEditor()
		{
			gradient = new HaxGradient();
			gradient.Changed += OnChanged;
			
			KeyPressEvent += OnKeyPressEvent;

			Add(gradient);
			ShowAll();
			
			SizeRequested += OnSizeRequested;
			SizeAllocated += OnSizeAllocated;
		}

		private void OnKeyPressEvent(object o, KeyPressEventArgs args)
		{
			gradient.KeyPress(args.Event);
		}

		private void OnChanged()
		{
			ValueChanged.Invoke(this, EventArgs.Empty);
		}

		void OnSizeRequested(object o, SizeRequestedArgs args)
		{
			if (gradient != null)
			{
				int width = args.Requisition.Width;
				int height = args.Requisition.Height;

				gradient.GetSizeRequest(out width, out height);
				if (width == -1 || height == -1)
					width = height = 80;
				SetSizeRequest(width + pad * 2, height + pad * 2);
			}
		}

		void OnSizeAllocated(object o, SizeAllocatedArgs args)
		{
			if (gradient != null)
			{
				Gdk.Rectangle mine = args.Allocation;
				Gdk.Rectangle his = mine;

				his.X += pad;
				his.Y += pad;
				his.Width -= pad * 2;
				his.Height -= pad * 2;
				gradient.SizeAllocate(his);
			}
		}
		
		public void Initialize(EditSession session)
		{
			if (session.Property.PropertyType != typeof(Gradient))
				throw new ApplicationException("Graph editor does not support editing values of type " + session.Property.PropertyType);
		}
		
		object IPropertyEditor.Value
		{
			get { return gradient.Gradient; }
			set { gradient.Gradient = (Gradient)value; }
		}

		public event EventHandler ValueChanged;
	}
}
