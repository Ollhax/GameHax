using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.EditorCommon.Editors;

namespace MG.ParticleEditor
{
	class ParticlePropertyProxy : ICustomTypeDescriptor
	{
		public static int UndoGroup = 965168485;

		class ParticlePropertyChangeset : UndoableAction
		{
			public ParticleDefinition CurrentDefinition;
			public ParticleDefinition ChangedDefinition;
			public ParticleDefinition OriginalDefinition;

			public ParticlePropertyChangeset(ParticleDefinition particleDefinition)
			{
				CurrentDefinition = particleDefinition;
				ChangedDefinition = new ParticleDefinition(particleDefinition);
				OriginalDefinition = new ParticleDefinition(particleDefinition);
			}

			public void CommitCurrentChanges()
			{
				ChangedDefinition.CopyFrom(CurrentDefinition);
			}

			protected override bool CallExecute()
			{
				if (ChangedDefinition.Equals(OriginalDefinition))
				{
					return false;
				}

				CurrentDefinition.CopyFrom(ChangedDefinition);
				return true;
			}

			protected override void CallUndo()
			{
				CurrentDefinition.CopyFrom(OriginalDefinition);
			}
			
			public override int GetUndoGroup()
			{
				return UndoGroup;
			}
		}

		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return changeset.CurrentDefinition.Name; } set { changeset.CurrentDefinition.Name = value; } }
		
		private ParticleDeclaration particleDeclaration;
		private ParticlePropertyChangeset changeset;
		
		public UndoableAction CommitAction()
		{
			var oldChangeset = changeset;
			changeset = new ParticlePropertyChangeset(changeset.CurrentDefinition);

			oldChangeset.CommitCurrentChanges();
			return oldChangeset;
		}
		

		public ParticlePropertyProxy(ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
		{
			this.particleDeclaration = particleDeclaration;
			changeset = new ParticlePropertyChangeset(particleDefinition);
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

			foreach (var param in changeset.CurrentDefinition.Parameters)
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
