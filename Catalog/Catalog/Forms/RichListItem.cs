using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Forms
{
    /// <summary>
	/// Interface for an item in a list control.
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and using TextBinding/KeyBinding to specify how to get/set the
	/// Text and Key properties.
	/// 
	/// Otherwise, use <see cref="RichListItem"/> to define items.  This may be depricated in the future.
	/// </remarks>
	public interface IRichListItem : IImageListItem
    {
        /// <summary>
        /// Gets or sets the subtitle of the item.
        /// </summary>
        /// <value>The subtitle.</value>
        string Subtitle { get; set; }
    }

    /// <summary>
	/// List item for list controls that accept an image and subtitle (e.g. <see cref="RichListBox"/>)
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and use <see cref="ListControl.ItemTextBinding"/>, <see cref="RichListBox.ItemSubtitleBinding"/>, <see cref="ListControl.ItemKeyBinding"/>,
	/// and <see cref="ListBox.ItemImageBinding"/>.
	/// </remarks>
	public class RichListItem : ImageListItem, IRichListItem
    {
        /// <summary>
        /// Gets or sets the subtitle for this item.
        /// </summary>
        /// <value>The item's subtitle.</value>
        public string Subtitle { get; set; }
    }
}
