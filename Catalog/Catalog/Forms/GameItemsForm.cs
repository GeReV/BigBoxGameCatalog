using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Catalog.Model;
using Eto.Forms;
using Eto.Drawing;
using Image = Eto.Drawing.Image;

namespace Catalog.Forms
{
    public partial class GameItemsForm : Panel
    {
        public GameItemsForm()
        {
            InitializeComponent();
        }

        private GameCopy GameCopy
        {
            get => ((GameCopy) DataContext);
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            Bindings.Clear();

            ItemsGrid.SelectedItemBinding.Bind(
                null,
                item => { EditForm.DataContext = item; },
                handler => EditForm.DataContextChanged += handler,
                handler => EditForm.DataContextChanged -= handler
            );
            ItemsGrid.DataStore = GameCopy.Items;

            ItemTypeDropDown.ItemTextBinding = Binding.Property<ItemType, string>(type => type.Description);
//            ItemTypeDropDown.ItemImageBinding = Binding.Property<ItemType, Image>(type => type.Icon);
            ItemTypeDropDown.DataStore = ItemTypes.All;
            ItemTypeDropDown.SelectedValueBinding.BindDataContext<Item>(item => item.ItemType);
            ItemTypeDropDown.SelectedIndexChanged += (sender, args) => ItemsGrid.ReloadData(ItemsGrid.SelectedRow);

            MissingCheckBox.CheckedBinding.BindDataContext<Item>(item => item.Missing);

            ConditionDropDown.SelectedValueBinding.BindDataContext<Item>(item => item.Condition);

            ConditionTextBox.TextBinding.BindDataContext<Item>(item => item.ConditionDetails);

            ItemsAddRemoveButtons.AddCommand = new Command(AddItem);
            ItemsAddRemoveButtons.RemoveCommand = new Command(RemoveItem);
        }

        private void AddItem(object sender, EventArgs e)
        {
            var item = new Item();

            GameCopy.Items.Add(item);

            ItemsGrid.DataStore = GameCopy.Items;
            ItemsGrid.SelectRow(GameCopy.Items.Count - 1);
        }

        private void RemoveItem(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}