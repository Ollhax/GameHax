using System;
using System.ComponentModel;

using MG.EditorCommon;
using MG.EditorCommon.Undo;
using MG.Framework.Particle;
using MG.ParticleEditor.Actions;
using MG.ParticleEditor.Controllers;

namespace MG.ParticleEditor
{
	class ParameterProxy : ICustomTypeDescriptor
	{
		protected class Changeset : UndoableParticleAction
		{
			private MainController controller;
			private Model model;
			private int undoGroup;

			public ParticleDefinition CurrentDefinition;
			public ParticleDefinition ChangedDefinition;
			public ParticleDefinition OriginalDefinition;

			public Changeset(MainController controller, Model model, ParticleDefinition particleDefinition, int undoGroup)
			{
				this.controller = controller;
				this.model = model;
				this.undoGroup = undoGroup;
				CurrentDefinitionId = particleDefinition.Id;
				CurrentDefinition = particleDefinition;
				ChangedDefinition = new ParticleDefinition(particleDefinition);
				OriginalDefinition = new ParticleDefinition(particleDefinition);
			}

			public Changeset(MainController controller, Model model, Changeset changeset)
			{
				this.controller = controller;
				this.model = model;
				undoGroup = changeset.undoGroup;
				CurrentDefinitionId = changeset.CurrentDefinitionId;
				CurrentParameter = changeset.CurrentParameter;
				CurrentDefinition = changeset.CurrentDefinition;
				ChangedDefinition = new ParticleDefinition(changeset.ChangedDefinition);
				OriginalDefinition = new ParticleDefinition(changeset.OriginalDefinition);
			}
			
			protected override bool CallExecute()
			{
				if (ChangedDefinition.Equals(OriginalDefinition))
				{
					return false;
				}

				controller.UpdateParticleSystem = true;
				model.Modified = true;
				CurrentDefinition.CopyFrom(ChangedDefinition);
				return true;
			}

			protected override void CallUndo()
			{
				controller.UpdateParticleSystem = true;
				CurrentDefinition.CopyFrom(OriginalDefinition);
			}

			public override int GetUndoGroup()
			{
				return undoGroup;
			}
		}

		protected MainController controller;
		protected Model model;
		protected ParticleDeclaration particleDeclaration;
		protected ParticleDefinition particleDefinition;
		protected Changeset changeset;

		public readonly int UndoGroup;

		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return changeset.CurrentDefinition.Name; } set { changeset.CurrentDefinition.Name = value; } }
		
		public UndoableAction CommitAction()
		{
			// Move all changes (which are done in the CURRENT definition) to the CHANGED definition.
			changeset.ChangedDefinition.CopyFrom(changeset.CurrentDefinition);

			// Copy the changeset that is about to be committed.
			var copy = new Changeset(controller, model, changeset);

			// Rebase the ORIGINAL definition so that we save changes from now -> next commit.
			changeset.OriginalDefinition.CopyFrom(changeset.CurrentDefinition);

			return copy;
		}

		public ParameterProxy(MainController controller, Model model, ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
		{
			this.controller = controller;
			this.model = model;
			this.particleDeclaration = particleDeclaration;
			this.particleDefinition = particleDefinition;
			UndoGroup = 515135;
			changeset = new Changeset(controller, model, particleDefinition, UndoGroup);
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
				var definitionParam = param.Value;

				ParticleDeclaration.Parameter declarationParameter;
				if (particleDeclaration.Parameters.TryGetValue(definitionParam.Name, out declarationParameter))
				{
					var p = new ParticleParameterDescriptor(declarationParameter, definitionParam);
					p.PropertyChanged += OnPropertyChanged;
					pdc.Add(p);
				}
			}
			
			return pdc;
		}

		private void OnPropertyChanged(string name)
		{
			changeset.CurrentParameter = name;
		}
	}
}
