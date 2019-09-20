using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms
{
	[ContentProperty("Items")]
	public class ThumbnailSelect : Scrollable
	{
		ItemDataStore dataStore;
		readonly List<Thumbnail> thumbnails = new List<Thumbnail>();
		bool settingChecked;

		/// <summary>
		/// Gets or sets the binding to get the text for each check box.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Text" property, or <see cref="IListItem.Text"/> when implemented.
		/// </remarks>
		/// <value>The text binding.</value>
		public IIndirectBinding<Image> ItemImageBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding to get the key for each check box.
		/// </summary>
		/// <remarks>
		/// By default, this will bind to a "Key" property, or <see cref="IListItem.Key"/> when implemented.
		/// </remarks>
		/// <value>The key binding.</value>
		public IIndirectBinding<string> ItemKeyBinding { get; set; }

		private static readonly object SelectedValuesChangedKey = new object();

		/// <summary>
		/// Occurs when <see cref="SelectedValues"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedValuesChanged
		{
			add => Properties.AddEvent(SelectedValuesChangedKey, value);
			remove => Properties.RemoveEvent(SelectedValuesChangedKey, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedValuesChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedValuesChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectedValuesChangedKey, this, e);
		}

		private static readonly object SelectedKeysChangedKey = new object();

		/// <summary>
		/// Occurs when <see cref="SelectedKeys"/> changes.
		/// </summary>
		public event EventHandler<EventArgs> SelectedKeysChanged
		{
			add => Properties.AddEvent(SelectedKeysChangedKey, value);
			remove => Properties.RemoveEvent(SelectedKeysChangedKey, value);
		}

		/// <summary>
		/// Raises the <see cref="SelectedKeysChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectedKeysChanged(EventArgs e) =>
			Properties.TriggerEvent(SelectedKeysChangedKey, this, e);

		/// <summary>
		/// Gets or sets the selected key of the currently selected item using the <see cref="ItemKeyBinding"/>.
		/// </summary>
		/// <value>The selected key.</value>
		public IEnumerable<string> SelectedKeys
		{
			get => SelectedValues?.Select(r => ItemKeyBinding.GetValue(r));
			set
			{
				var keys = value.ToList();

				settingChecked = true;

				var changed = false;

				foreach (var shot in thumbnails)
				{
					var key = ItemKeyBinding.GetValue(shot.Tag);
					var isSelected = keys.Contains(key);
					if (shot.Checked != isSelected)
					{
						changed = true;
						shot.Checked = isSelected;
					}
				}

				settingChecked = false;
				if (changed)
					TriggerSelectionChanged();
			}
		}

		/// <summary>
		/// Gets or sets the selected value, which is the <see cref="ListItem"/> or object in your custom data store.
		/// </summary>
		/// <value>The selected value.</value>
		public IEnumerable<object> SelectedValues
		{
			get => thumbnails.Where(r => r.Checked).Select(r => r.Tag);
			set
			{
				var items = value.ToList();

				settingChecked = true;

				var changed = false;

				foreach (var shot in thumbnails)
				{
					var item = shot.Tag;
					var isChecked = items.Contains(item);

					if (shot.Checked != isChecked)
					{
						changed = true;
						shot.Checked = isChecked;
					}
				}

				settingChecked = false;

				if (changed)
				{
					TriggerSelectionChanged();
				}
			}
		}


		class ItemDataStore : EnumerableChangedHandler<object>
		{
			public ThumbnailSelect Handler { get; set; }

			public override void AddItem(object item)
			{
				var thumbnail = Handler.CreateThumbnail(item);

				Handler.thumbnails.Add(thumbnail);

				Handler.LayoutThumbnails();
			}

			public override void InsertItem(int index, object item)
			{
				var thumbnail = Handler.CreateThumbnail(item);

				Handler.thumbnails.Insert(index, thumbnail);

				Handler.LayoutThumbnails();
			}

			public override void RemoveItem(int index)
			{
				var thumbnail = Handler.thumbnails[index];

				Handler.thumbnails.RemoveAt(index);

				Handler.UnregisterThumbnail(thumbnail);

				Handler.LayoutThumbnails();

				if (thumbnail.Checked)
				{
					Handler.TriggerSelectionChanged();
				}
			}

			public override void RemoveAllItems()
			{
				Handler.Clear();
			}
		}

		/// <summary>
		/// Gets the item collection, when adding items programatically.
		/// </summary>
		/// <remarks>
		/// This is used when you want to add items manually.  Use the <see cref="DataStore"/>
		/// when you have an existing collection you want to bind to directly.
		/// </remarks>
		/// <value>The item collection.</value>
		public ListItemCollection Items
		{
			get
			{
				var items = (ListItemCollection) DataStore;
				if (items == null)
				{
					items = CreateDefaultItems();
					DataStore = items;
				}

				return items;
			}
		}

		/// <summary>
		/// Gets or sets the data store of the items shown in the list.
		/// </summary>
		/// <remarks>
		/// When using a custom object collection, you can use the <see cref="ItemImageBinding"/> and <see cref="ItemKeyBinding"/>
		/// to specify how to get the text/key values for each item.
		/// </remarks>
		/// <value>The data store.</value>
		public IEnumerable<object> DataStore
		{
			get => dataStore?.Collection;
			set
			{
				thumbnails.Clear();
				dataStore = new ItemDataStore {Handler = this};
				dataStore.Register(value);
				LayoutThumbnails();
			}
		}

		public ThumbnailSelect()
		{
			ItemImageBinding = new ListItemImageBinding();
			ItemKeyBinding = new ListItemKeyBinding();

			BackgroundColor = Colors.White;

			Padding = new Padding(5);

			MinimumSize = new Size(0, 120 + 10 + Padding.Bottom + Padding.Top);
		}

		/// <summary>
		/// Raises the load event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (DataStore == null)
				DataStore = CreateDefaultItems();
			else
			{
				LayoutThumbnails();
			}
		}

		void EnsureThumbnails()
		{
			if (DataStore == null)
				DataStore = CreateDefaultItems();
		}

		void LayoutThumbnails()
		{
			if (!Loaded)
				return;

			SuspendLayout();

			var stackLayout = new StackLayout
			{
				Spacing = 0,
				Orientation = Orientation.Horizontal,
			};

			foreach (var thumbnail in thumbnails)
			{
				stackLayout.Items.Add(thumbnail);
			}

			Content = stackLayout;

			ResumeLayout();
		}

		void Clear()
		{
			foreach (var thumbnail in thumbnails)
				UnregisterThumbnail(thumbnail);

			thumbnails.Clear();

			LayoutThumbnails();
		}

		void TriggerSelectionChanged()
		{
			OnSelectedValuesChanged(EventArgs.Empty);
			OnSelectedKeysChanged(EventArgs.Empty);
		}

		Thumbnail CreateThumbnail(object item)
		{
			var thumbnail = new Thumbnail
			{
				Image = ItemImageBinding.GetValue(item),
				Tag = item,
				Enabled = base.Enabled
			};

			thumbnail.CheckedChanged += HandleCheckedChanged;

			return thumbnail;
		}

		void UnregisterThumbnail(Thumbnail thumbnail)
		{
			thumbnail.CheckedChanged -= HandleCheckedChanged;
		}

		void HandleCheckedChanged(object sender, EventArgs e)
		{
			if (!settingChecked)
			{
				TriggerSelectionChanged();
			}
		}

		/// <summary>
		/// Creates the default items.
		/// </summary>
		/// <returns>The default items.</returns>
		protected virtual ListItemCollection CreateDefaultItems() => new ListItemCollection();

		/// <summary>
		/// Gets a binding to the <see cref="SelectedValues"/> property.
		/// </summary>
		/// <value>The selected value binding.</value>
		public BindableBinding<ThumbnailSelect, IEnumerable<object>> SelectedValuesBinding
		{
			get
			{
				return new BindableBinding<ThumbnailSelect, IEnumerable<object>>(
					this,
					c => c.SelectedValues,
					(c, v) => c.SelectedValues = v,
					(c, h) => c.SelectedValuesChanged += h,
					(c, h) => c.SelectedValuesChanged -= h
				);
			}
		}

		/// <summary>
		/// Gets a binding to the <see cref="SelectedKeys"/> property.
		/// </summary>
		/// <value>The selected index binding.</value>
		public BindableBinding<ThumbnailSelect, IEnumerable<string>> SelectedKeysBinding
		{
			get
			{
				return new BindableBinding<ThumbnailSelect, IEnumerable<string>>(
					this,
					c => c.SelectedKeys,
					(c, v) => c.SelectedKeys = v,
					(c, h) => c.SelectedKeysChanged += h,
					(c, h) => c.SelectedKeysChanged -= h
				);
			}
		}

		public void Select(int index)
		{
			thumbnails[index].Checked = true;
		}

        public void Unselect(int index)
        {
	        thumbnails[index].Checked = false;
        }

        public void SelectAll()
        {
	        foreach (var thumbnail in thumbnails)
	        {
		        thumbnail.Checked = true;
	        }
        }

        public void UnselectAll()
        {
	        foreach (var thumbnail in thumbnails)
	        {
		        thumbnail.Checked = false;
	        }
        }
	}
}