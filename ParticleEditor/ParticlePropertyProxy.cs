using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;

namespace MG.ParticleEditor
{
	class ParticlePropertyProxy : UndoableAction, ICustomTypeDescriptor
	{
		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return changedDefinition.Name; } set { changedDefinition.Name = value; } }
		
		private ParticleDeclaration particleDeclaration;
		private ParticleDefinition currentDefinition;
		private ParticleDefinition changedDefinition;
		private ParticleDefinition originalDefinition;

		public ParticlePropertyProxy(ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
		{
			this.particleDeclaration = particleDeclaration;
			currentDefinition = particleDefinition;
			changedDefinition = new ParticleDefinition(particleDefinition);
			originalDefinition = new ParticleDefinition(particleDefinition);
		}

		protected override bool CallExecute()
		{
			if (changedDefinition.Equals(originalDefinition))
			{
				return false;
			}

			currentDefinition.CopyFrom(changedDefinition);
			return true;
		}

		protected override void CallUndo()
		{
			currentDefinition.CopyFrom(originalDefinition);
		}

		public override int GetUndoGroup()
		{
			return 965168485;
		}

		//--------------------------------------
		// ICustomTypeDescriptor
		//--------------------------------------
		AttributeCollection ICustomTypeDescriptor.GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() { return GetProperties(null); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			var typeProperties = TypeDescriptor.GetProperties(this.GetType(), attributes);
			foreach (PropertyDescriptor pd in typeProperties)
			{
				pdc.Add(pd);
			}

			foreach (var param in changedDefinition.Parameters)
			{
				var value = param.Value.Value;

				ParticleDeclaration.Parameter declarationParameter;
				if (particleDeclaration.Parameters.TryGetValue(param.Value.Name, out declarationParameter))
				{
					pdc.Add(new AnyPropertyDescriptor(declarationParameter, value));
				}
			}

			return pdc;
		}
	}
}
