using System;
using Catalog.Model;
using Eto.Forms;
using Eto.Drawing;

namespace Catalog.Forms
{
	partial class ItemManagementForm : Panel
	{
		private readonly ListBox itemsListBox = new ListBox();
		private readonly DynamicLayout editForm = new DynamicLayout
		{
			DefaultPadding = new Padding(5),
			DefaultSpacing = new Size(5,5),
		};
		private readonly DropDown typeDropDown = new DropDown();
		private readonly CheckBox missingCheckBox = new CheckBox();
		private readonly EnumDropDown<Condition> conditionDropDown = new EnumDropDown<Condition>();
		private readonly TextArea conditionTextBox = new TextArea();
		private readonly TextArea notesTextArea = new TextArea();

		void InitializeComponent()
		{
			Padding = 10;

			var splitter = new Splitter
			{
				Panel1 = itemsListBox,
				Panel1MinimumSize = 200,
				Panel2 = editForm,
			};

			Content = splitter;

			editForm.BeginVertical();

			editForm.AddLabeledRow("Type", typeDropDown);

			editForm.AddLabeledRow("Missing", missingCheckBox);

			editForm.AddLabeledRow("Condition", conditionDropDown);

			editForm.AddLabeledRow("Condition Details", conditionTextBox);

			editForm.AddLabeledRow("Scans", new Panel());

			editForm.AddLabeledRow("Files", new Panel());

			editForm.AddLabeledRow("Notes", notesTextArea);

			editForm.EndVertical();
		}
	}
}
