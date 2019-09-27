using System;
using Catalog.Forms.Controls;
using Catalog.Model;
using Eto.Forms;
using Eto.Drawing;

namespace Catalog.Forms
{
	partial class GameInfoForm : Panel
	{
		public readonly TextBox TitleTextbox = new TextBox();

		public readonly Button SearchMobyGamesButton = new Button
		{
			Text = "Search MobyGames",
		};

		public readonly ComboBox PublisherList = new ComboBox
		{
			AutoComplete = true,
		};

		public readonly CheckBoxList DeveloperList = new CheckBoxList()
		{
			Orientation = Orientation.Vertical,
		};

		public readonly CheckBox HasBoxCheckbox = new CheckBox();

		public readonly EnumCheckBoxList<Platform> PlatformList = new EnumCheckBoxList<Platform>
		{
			Orientation = Orientation.Vertical,
			GetText = platform => platform.GetDescription(),
		};

		public readonly ThumbnailSelect Screenshots = new ThumbnailSelect();
		public readonly AddMediaPanel AddMediaPanel = new AddMediaPanel();

		public Control DefaultControl { get; set; }

		public readonly Label StatusLabel = new Label
		{
			Width = 200
		};

		public readonly ProgressBar ProgressBar = new ProgressBar
		{
			Visible = false
		};

		public TextArea NotesTextArea = new TextArea
		{
			AcceptsTab = false
		};

		void InitializeComponent()
		{
			DefaultControl = TitleTextbox;

			var layout = new DynamicLayout
			{
				DefaultSpacing = new Size(5, 5),
			};

			Content = layout;

			layout.BeginVertical();

			layout.AddLabeledRow("Title", l =>
			{
				l.Add(TitleTextbox, true);
				layout.Add(SearchMobyGamesButton);
			});

			layout.AddLabeledRow("Publisher", PublisherList);

			layout.AddLabeledRow("Developers", new Scrollable
			{
				BackgroundColor = Colors.White,
				ExpandContentHeight = false,
				Height = 120,
				Content = DeveloperList,
			});

			layout.AddLabeledRow("Has Game Box", HasBoxCheckbox);

			layout.AddLabeledRow("Media", AddMediaPanel);

			layout.AddLabeledRow("Platforms", new Panel
			{
				Content = PlatformList,
			});

			layout.AddLabeledRow("Screenshots", Screenshots, false);

			layout.AddLabeledRow("Notes", NotesTextArea);

			layout.AddSpace();

			layout.BeginHorizontal();
			layout.Add(StatusLabel);
			layout.Add(ProgressBar, true, false);
			layout.EndHorizontal();

			layout.EndVertical();
		}
	}
}
