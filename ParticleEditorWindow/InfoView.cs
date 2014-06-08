using System;

using Gtk;

using MonoDevelop.Components.PropertyGrid;

using WrapMode = Pango.WrapMode;

namespace MG.ParticleEditorWindow
{
	public class InfoView
	{
		internal Frame Widget;

		private VBox container;
		private Label labelName;
		private Label labelDescription;
		private Frame propertyGridFrame;
		
		public ParameterView MetaProperties;
		public string Name { set { labelName.Markup = "<b>" + value + "</b>"; } }
		public string Description { set { labelDescription.Text = value; } }
		public bool Visible { get { return Widget.Visible; } set { Widget.Visible = value; } }
		
		public InfoView()
		{
			container = new VBox(false, 0);

			Widget = new Frame();
			Widget.Add(container);
			Widget.ShadowType = ShadowType.EtchedIn;
			
			labelName = new Label();
			labelName.Justify = Justification.Left;
			labelName.UseMarkup = true;
			var labelNameBox = new HBox();
			labelNameBox.PackStart(labelName, false, false, 3);
			container.PackStart(labelNameBox, false, true, 3);
			
			labelDescription = new Label();
			labelDescription.LineWrap = true;
			labelDescription.LineWrapMode = WrapMode.Word;
			labelDescription.Text = "";
			labelDescription.WidthRequest = 190;
			var labelDescriptionBox = new HBox();
			labelDescriptionBox.PackStart(labelDescription, false, false, 3);
			container.PackStart(labelDescriptionBox, false, false, 3);

			MetaProperties = new ParameterView();
			MetaProperties.Widget.HeightRequest = 40;
			MetaProperties.Widget.PropertySort = PropertySort.NoSort;
			MetaProperties.Widget.CurrentObjectChanged += OnCurrentObjectChanged;
			
			propertyGridFrame = new Frame();
			propertyGridFrame.Add(MetaProperties.Widget);
			propertyGridFrame.BorderWidth = 0;
			propertyGridFrame.ShadowType = ShadowType.In;
			container.PackStart(propertyGridFrame, true, true, 0);
		}

		private void OnCurrentObjectChanged(object sender, EventArgs eventArgs)
		{
			var height = MetaProperties.Widget.Height;
			propertyGridFrame.Visible = height > 0;
			MetaProperties.Widget.SetSizeRequest(-1, height);
		}
	}
}
