//using System;
//using System.ComponentModel;

//using MG.Framework.Particle;

//namespace MG.ParticleEditor.Proxies
//{
//    public class ParticleParameterDescriptor : PropertyDescriptor
//    {
//        public ParticleParameterDescriptor(ParticleDefinition.Parameter parameter)
//            : base(parameter.Name, null)
//        {
			
//        }
		
//        public override string DisplayName { get { return DeclarationParameter.PrettyName; } }
//        public override bool CanResetValue(object component) { return false; }
//        public override Type ComponentType { get { return null; } }
//        public override object GetValue(object component) { return any.GetAsObject(); }
//        public override string Description { get { return DeclarationParameter.Description; } }
//        public override string Category { get { return DeclarationParameter.Category; } }
//        public override bool IsReadOnly { get { return false; } }
//        public override void ResetValue(object component) { }
//        public override bool ShouldSerializeValue(object component) { return false; }

//        public override TypeConverter Converter
//        {
//            get
//            {
//                if (DeclarationParameter.ValueList != null)
//                {
//                    return new ValueListConverter(DeclarationParameter.ValueList);
//                }

//                return base.Converter;
//            }
//        }

//        public override void SetValue(object component, object value)
//        {
//            any.Set(value);
//            if (PropertyChanged != null)
//            {
//                PropertyChanged.Invoke();
//            }
//        }

//        public override Type PropertyType
//        {
//            get { return any.GetTypeOfValue(); }
//        }

//        public override object GetEditor(Type editorBaseType)
//        {
//            if (any.IsInt()) return new IntEditor();
//            if (any.IsFloat()) return new FloatEditor();
//            if (any.IsFilePath()) return new FilePathEditor();

//            return base.GetEditor(editorBaseType);
//        }
//    }
//}
