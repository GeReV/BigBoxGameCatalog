using System;
using Catalog.Model;
using Eto.Forms;
using Eto.Drawing;

namespace Catalog.Forms
{
	partial class GameItemsForm : Panel
	{
		public readonly GridView<Item> ItemsGrid = new GridView<Item>
		{
			AllowMultipleSelection = false,
			ShowHeader = false,
			Columns =
			{
				new GridColumn
				{
					DataCell = new ImageTextCell
					{
						TextBinding = Binding.Property<Item, string>(item => item.ItemType.Description),
					},
				}
			}
		};

		public readonly Button AddItem = new Button
		{
			Text = "Add"
		};

		public readonly Button RemoveItem = new Button
		{
			Text = "Remove"
		};

		public readonly DynamicLayout EditForm = new DynamicLayout
		{
			DefaultPadding = new Padding(5),
			DefaultSpacing = new Size(5,5),
		};
		public readonly DropDown ItemTypeDropDown = new DropDown();
		public readonly CheckBox MissingCheckBox = new CheckBox();
		public readonly EnumDropDown<Condition?> ConditionDropDown = new EnumDropDown<Condition?>();
		public readonly TextArea ConditionTextBox = new TextArea
		{
			AcceptsTab = false
		};
		public readonly ThumbnailSelect ScansThumbnailSelect = new ThumbnailSelect();

		public readonly TextArea NotesTextArea = new TextArea
		{
			AcceptsTab = false
		};

		void InitializeComponent()
		{
			Padding = 10;

			var itemGridLayout = new StackLayout
			{
				Orientation = Orientation.Vertical,
				Padding = new Padding(5),
				Spacing = 5,
				Items =
				{
					new StackLayoutItem(ItemsGrid, HorizontalAlignment.Stretch, true),
					new StackLayout
					{
						Spacing = 5,
						Orientation = Orientation.Horizontal,
						Items = { AddItem, RemoveItem }
					}
				}
			};

			var splitter = new Splitter
			{
				Panel1 = itemGridLayout,
				Panel1MinimumSize = 200,
				Panel2 = EditForm,
				Panel2MinimumSize = 300
			};

			Content = splitter;

			EditForm.BeginVertical();

			EditForm.AddLabeledRow("Type", ItemTypeDropDown, labelWidth: 120);

			EditForm.AddLabeledRow("Missing", MissingCheckBox, labelWidth: 120);

			EditForm.AddLabeledRow("Condition", ConditionDropDown, labelWidth: 120);

			EditForm.AddLabeledRow("Condition Details", ConditionTextBox, labelWidth: 120);

			EditForm.AddLabeledRow("Scans", l =>
			{
				l.Add(ScansThumbnailSelect);
				l.Add(new StackLayout
				{
					Orientation = Orientation.Vertical,
					Spacing = 5,
					Items =
					{
						new Button
						{
							Text = "Add"
						},
						new Button
						{
							Text = "Remove"
						}
					}
				});
			}, labelWidth: 120);

			EditForm.AddLabeledRow("Files", l =>
			{
				l.Add(new ListBox());
				l.Add(new StackLayout
				{
					Orientation = Orientation.Vertical,
					Spacing = 5,
					Items =
					{
						new Button
						{
							Text = "Add"
						},
						new Button
						{
							Text = "Remove"
						}
					}
				});
			}, labelWidth: 120);

			EditForm.AddLabeledRow("Notes", NotesTextArea, labelWidth: 120);

			EditForm.EndVertical();
		}
	}
}
