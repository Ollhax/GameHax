using System;

using Gtk;

using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Components.PropertyGrid.PropertyEditors;

namespace MG.ParticleEditor
{
	public class FloatEditor : PropertyEditorCell
	{
		bool valueSelector;

		bool HasValue
		{
			get
			{
				int val = (int)Convert.ChangeType(Value, typeof(int));
				return (val != -1);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			if (Property.Converter.GetStandardValuesSupported())
			{
				valueSelector = true;
			}
		}

		public override void GetSize(int availableWidth, out int width, out int height)
		{
			if (HasValue)
				base.GetSize(availableWidth, out width, out height);
			else
				width = height = 0;
		}

		public override void Render(Gdk.Drawable window, Cairo.Context ctx, Gdk.Rectangle bounds, StateType state)
		{
			if (HasValue)
				base.Render(window, ctx, bounds, state);
		}

		protected override IPropertyEditor CreateEditor(Gdk.Rectangle cell_area, Gtk.StateType state)
		{
			if (valueSelector)
				return new TextEditor();
			//if (optInteger)
			//    return new OptIntRange(0, null);
			//else
			return new FloatRangeEditor();
		}
	}
	
	public class FloatRangeEditor : Gtk.SpinButton, IPropertyEditor
	{
		Type propType;

		public FloatRangeEditor()
			: base(0, 0, 0.01)
		{
		}

		public void Initialize(EditSession session)
		{
			propType = session.Property.PropertyType;
			var declarationParameter = ((AnyPropertyDescriptor)session.Property).DeclarationParameter;

			double min, max;

			if (propType == typeof(double))
			{
				min = Double.MinValue;
				max = Double.MaxValue;
			}
			else if (propType == typeof(float))
			{
				min = float.MinValue;
				max = float.MaxValue;
			}
			else
				throw new ApplicationException("FloatRange editor does not support editing values of type " + propType);

			if (declarationParameter.MinValue != null && declarationParameter.MinValue.IsFloat())
			{
				min = (double)Convert.ChangeType(declarationParameter.MinValue.Get<float>(), typeof(double));
			}

			if (declarationParameter.MaxValue != null && declarationParameter.MaxValue.IsFloat())
			{
				max = (double)Convert.ChangeType(declarationParameter.MaxValue.Get<float>(), typeof(double));
			}

			if (declarationParameter.ValueStep != null && declarationParameter.ValueStep.IsFloat())
			{
				float step = declarationParameter.ValueStep.Get<float>();
				SetIncrements(step, step);
			}
			
			SetRange(min, max);
			Digits = declarationParameter.ValueDigits;
		}

		public void AttachObject(object ob)
		{
		}

		object IPropertyEditor.Value
		{
			get { return Convert.ChangeType(base.Value, propType); }
			set { base.Value = (double)Convert.ChangeType(value, typeof(double)); }
		}
	}
}
