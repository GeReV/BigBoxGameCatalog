using System;
using System.Collections.Generic;
using System.Text;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Catalog.Forms.Controls
{
    //
    // Summary:
    //     A collection of Eto.Forms.ListItem objects for use with Eto.Forms.ListControl
    //     objects
    //
    // Remarks:
    //     This is used to provide an easy way to add items to a Eto.Forms.ListControl.
    //     It is not mandatory to use this collection, however, since each control can specify
    //     bindings to your own model objects using Eto.Forms.ListControl.ItemKeyBinding,
    //     Eto.Forms.ListControl.ItemTextBinding, or other subclass bindings.
    public class RichListItemCollection : ExtendedObservableCollection<IRichListItem>
    {
        //
        // Summary:
        //     Initializes a new instance of the ListItemCollection class.
        public RichListItemCollection()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the ListItemCollection class with the specified
        //     collection.
        //
        // Parameters:
        //   collection:
        //     Collection of items to populate this collection with
        public RichListItemCollection(IEnumerable<IRichListItem> collection) : base(collection)
        {

        }

        //
        // Summary:
        //     Adds a new item to the list with the specified text
        //
        // Parameters:
        //   text:
        //     Text to display for the item.
        public void Add(string text, string subtitle = null, Image image = null)
        {
            Items.Add(new RichListItem
            {
                Text = text,
                Subtitle = subtitle,
                Image = image,
            });
        }
        //
        // Summary:
        //     Add a new item to the list with the specified text and key
        //
        // Parameters:
        //   text:
        //     Text to display for the item.
        //
        //   key:
        //     Key for the item.
        public void Add(string text, string key, string subtitle = null, Image image = null)
        {
            Items.Add(new RichListItem
            {
                Key = key,
                Text = text,
                Subtitle = subtitle,
                Image = image,
            });
        }
    }

    internal class RichListItemSubtitleBinding : PropertyBinding<string>
    {
        public RichListItemSubtitleBinding()
            : base("Subtitle")
        {
        }

        protected override string InternalGetValue(object dataItem)
        {
            return dataItem is IRichListItem item ? item.Subtitle : base.InternalGetValue(dataItem);
        }
    }

    /// <summary>
	/// Control to show a list of items that the user can select
	/// </summary>
    [Handler(typeof(RichListBox.IHandler))]
    public class RichListBox : ListBox
    {
        private new IHandler Handler { get { return (IHandler)base.Handler; } }

        /// <summary>
        /// Gets or sets the binding for the Image of each item
        /// </summary>
        /// <remarks>
        /// By default will be an public Image property on your object
        /// </remarks>
        /// <value>The image binding.</value>
        public IIndirectBinding<string> ItemSubtitleBinding { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catalog.Controls.RichListBox"/> class.
        /// </summary>
        public RichListBox()
        {
            ItemSubtitleBinding = new RichListItemSubtitleBinding();
        }

        private static readonly object callback = new Callback();
        /// <summary>
        /// Gets an instance of an object used to perform callbacks to the widget from handler implementations
        /// </summary>
        /// <returns>The callback instance to use for this widget</returns>
        protected override object GetCallback() { return callback; }


        /// <summary>
        /// Gets the list of items in the control.
        /// </summary>
        /// <remarks>
        /// This is an alternate to using <see cref="DataStore"/> to easily add items to the list, when you do not
        /// want to use custom objects as the source for the list.
        /// This will set the <see cref="DataStore"/> to a new instance of a <see cref="RichListItemCollection"/>.
        /// </remarks>
        /// <value>The items.</value>
        public new RichListItemCollection Items
        {
            get {
                if (!(DataStore is RichListItemCollection items))
                {
                    items = (RichListItemCollection)CreateDefaultDataStore();
                    DataStore = items;
                }
                return items;
            }
        }

        protected override IEnumerable<object> CreateDefaultDataStore()
        {
            return new RichListItemCollection();
        }

        /// <summary>
        /// Callback interface for the <see cref="RichListBox"/>
        /// </summary>
        public new interface ICallback : ListControl.ICallback
        {
            /// <summary>
            /// Raises the activated event.
            /// </summary>
            void OnActivated(RichListBox widget, EventArgs e);
        }

        /// <summary>
        /// Callback implementation for handlers of <see cref="RichListBox"/>
        /// </summary>
        protected new class Callback : ListControl.Callback, ICallback
        {
            /// <summary>
            /// Raises the activated event.
            /// </summary>
            public void OnActivated(RichListBox widget, EventArgs e)
            {
                using (widget.Platform.Context)
                    widget.OnActivated(e);
            }
        }

        /// <summary>
        /// Handler interface for the <see cref="RichListBox"/>
        /// </summary>
        public new interface IHandler : ListControl.IHandler, IContextMenuHost
        {
        }
    }
}
