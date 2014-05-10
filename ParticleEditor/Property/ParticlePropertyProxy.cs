using System;
using System.ComponentModel;

using MG.Framework.Particle;

namespace MG.ParticleEditor.Property
{
	class ParticlePropertyProxy : ICustomTypeDescriptor
	{
		[Category("General")]
		[ReadOnly(true)]
		public string Name { get { return particleDefinition.Name; } set { particleDefinition.Name = value; } }

		[Category("General")]
		public string Emitter { get { return particleDefinition.Emitter; } set { particleDefinition.Emitter = value; } }

		//[Category("General")]
		//public string AssetName { get { return currentLevelEntity.AssetName; } set { currentLevelEntity.AssetName = changedLevelEntity.AssetName = value; } }

		//[EditorAttribute(typeof(Vector2UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		//[Category("General")]
		//public Vector2 Position { get { return currentLevelEntity.Position; } set { currentLevelEntity.Position = changedLevelEntity.Position = value; } }

		//[EditorAttribute(typeof(Vector2UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		//[Category("General")]
		//public Vector2 Scale { get { return currentLevelEntity.Scale; } set { currentLevelEntity.Scale = changedLevelEntity.Scale = value; } }

		//[Category("General")]
		//public float Rotation { get { return MathHelper.ToDegrees(currentLevelEntity.Rotation); } set { currentLevelEntity.Rotation = changedLevelEntity.Rotation = MathHelper.ToRadians(value); } }

		//[EditorAttribute(typeof(Vector2UITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		//[Category("General")]
		//public RelativePosition2 Origin { get { return currentLevelEntity.Origin; } set { currentLevelEntity.Origin = changedLevelEntity.Origin = value; } }

		//private CustomLevelEntity currentLevelEntity;
		//private CustomLevelEntity changedLevelEntity;
		//private CustomLevelEntity originalLevelEntity;
		//private CustomEntityDefinition customEntityDefinition;

		private ParticleDeclaration particleDeclaration;
		private ParticleDefinition particleDefinition;

		public ParticlePropertyProxy(ParticleDeclaration particleDeclaration, ParticleDefinition particleDefinition)
		{
			this.particleDeclaration = particleDeclaration;
			this.particleDefinition = particleDefinition;
			//this.customEntityDefinition = customEntityDefinition;
			//this.currentLevelEntity = levelEntity;
			//this.changedLevelEntity = new CustomLevelEntity(levelEntity);
			//this.originalLevelEntity = new CustomLevelEntity(levelEntity);
		}

		//protected override bool CallExecute()
		//{
		//    currentLevelEntity.CopyFrom(changedLevelEntity);
		//    return true;
		//}

		//protected override void CallUndo()
		//{
		//    currentLevelEntity.CopyFrom(originalLevelEntity);
		//}

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
			//var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			//var typeProperties = TypeDescriptor.GetProperties(this);
			//foreach (PropertyDescriptor pd in typeProperties)
			//{
			//    AnyProperty property;
			//    if (customEntityDefinition.Properties.TryGetValue(pd.Name, out property))
			//    {
			//        pdc.Add(new AnyPropertyDefaultDescriptor(pd, property, pd.Name, attributes));
			//    }
			//}

			//foreach (var changedEntityProperty in changedLevelEntity.Properties)
			//{
			//    // Don't add default properties
			//    if (typeProperties.Find(changedEntityProperty.Key, false) != null)
			//    {
			//        continue;
			//    }

			//    AnyProperty property;
			//    if (customEntityDefinition.Properties.TryGetValue(changedEntityProperty.Key, out property))
			//    {
			//        pdc.Add(new AnyPropertyDescriptor(changedEntityProperty.Value, property, "Special Properties", attributes, null));
			//    }
			//}

			var pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			var typeProperties = TypeDescriptor.GetProperties(this.GetType(), attributes);
			foreach (PropertyDescriptor pd in typeProperties)
			{
				pdc.Add(pd);
			}

			foreach (var param in particleDefinition.Parameters)
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
