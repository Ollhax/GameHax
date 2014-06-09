using System;
using Gtk;
using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Components.PropertyGrid.PropertyEditors;

namespace MG.EditorCommon.Editors
{
	public class IntEditor : PropertyEditorCell
	{
		bool optInteger;
		bool valueSelector;

		bool HasValue
		{
			get
			{
				if (!optInteger) return true;
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

			//if (Property.Minimum != null && Property.PropertyType == typeof(int))
			//{
			//    int min = (int)Convert.ChangeType(Property.Minimum, typeof(int));
			//    optInteger = (min == -1);
			//}
			//else
			//    optInteger = false;
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
				return new IntRangeEditor();
		}
	}
	
	public class IntRangeEditor : Gtk.SpinButton, IPropertyEditor
	{
		private Type propType;

		public IntRangeEditor()
			: base(0, 0, 1.0)
		{
			this.HasFrame = false;
		}

		public void Initialize(EditSession session)
		{
			propType = session.Property.PropertyType;
			var declarationParameter = ((ParticleParameterDescriptor)session.Property).DeclarationParameter;

			double min, max;

			switch (Type.GetTypeCode(propType))
			{
				case TypeCode.Int16:
					min = (double)Int16.MinValue;
					max = (double)Int16.MaxValue;
					break;
				case TypeCode.UInt16:
					min = (double)UInt16.MinValue;
					max = (double)UInt16.MaxValue;
					break;
				case TypeCode.Int32:
					min = (double)Int32.MinValue;
					max = (double)Int32.MaxValue;
					break;
				case TypeCode.UInt32:
					min = (double)UInt32.MinValue;
					max = (double)UInt32.MaxValue;
					break;
				case TypeCode.Int64:
					min = (double)Int64.MinValue;
					max = (double)Int64.MaxValue;
					break;
				case TypeCode.UInt64:
					min = (double)UInt64.MinValue;
					max = (double)UInt64.MaxValue;
					break;
				case TypeCode.Byte:
					min = (double)Byte.MinValue;
					max = (double)Byte.MaxValue;
					break;
				case TypeCode.SByte:
					min = (double)SByte.MinValue;
					max = (double)SByte.MaxValue;
					break;
				default:
					throw new ApplicationException(
						"IntRange editor does not support editing values of type " + session.Property.PropertyType);
			}
			
			if (declarationParameter.MinValue != null && declarationParameter.MinValue.IsInt())
			{
				min = (double)Convert.ChangeType(declarationParameter.MinValue.Get<int>(), typeof(double));
			}

			if (declarationParameter.MaxValue != null && declarationParameter.MaxValue.IsInt())
			{
				max = (double)Convert.ChangeType(declarationParameter.MaxValue.Get<int>(), typeof(double));
			}

			if (declarationParameter.ValueStep != null && declarationParameter.ValueStep.IsInt())
			{
				int step = declarationParameter.ValueStep.Get<int>();
				SetIncrements(step, step);
			}
			
			SetRange(min, max);
		}

		public void AttachObject(object ob)
		{
		}

		object IPropertyEditor.Value { get { return Convert.ChangeType(base.Value, propType); } set { base.Value = (double)Convert.ChangeType(value, typeof(double)); } }
	}
}