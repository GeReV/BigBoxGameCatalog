using System;
using System.Collections.Generic;
using Catalog.Model;
using Eto.Forms;
using Eto.Drawing;

namespace Catalog.Forms
{
	public partial class GameItemsForm : Panel
	{
		public GameItemsForm()
		{
			InitializeComponent();
		}

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);

			Bindings.Clear();

			ItemsGrid.SelectedItemBinding.Bind(null, item => { EditForm.DataContext = item; });
			ItemsGrid.BindDataContext<GridView, GameCopy, List<Item>>(list => (List<Item>) list.DataStore, gc => gc.Items);

			ItemTypeDropDown.ItemTextBinding = Binding.Property<ItemType, string>(type => type.Description);
			ItemTypeDropDown.ItemKeyBinding = Binding.Property<ItemType, string>(type => type.Type);
			ItemTypeDropDown.DataStore = ItemTypes.All;
			ItemTypeDropDown.SelectedValueBinding.BindDataContext<Item>(item => item.ItemType);

			MissingCheckBox.CheckedBinding.BindDataContext<Item>(item => item.Missing);

			ConditionDropDown.SelectedValueBinding.BindDataContext<Item>(item => item.Condition);

			ConditionTextBox.TextBinding.BindDataContext<Item>(item => item.ConditionDetails);
		}
	}
}
