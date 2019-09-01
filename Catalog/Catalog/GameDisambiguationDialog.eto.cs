using System;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Forms.Controls;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;

namespace Catalog
{
	partial class GameDisambiguationDialog : Dialog<SearchResult>
	{
        protected RichListBox List { get; private set; }

		void InitializeComponent()
		{
			Title = "Disambiguation";
			ClientSize = new Size(640, 480);
			Padding = 10;

            List = new RichListBox();
            List.MouseDoubleClick += List_MouseDoubleClick;
            List.KeyUp += List_KeyUp;

            PositiveButtons.Add(new Button
            {
                Text = "OK",
                Command = new Command((sender, e) => Close(List.SelectedValue as SearchResult))
            });

            NegativeButtons.Add(new Button
            {
                Text = "Cancel",
                Command = new Command((sender, e) => Close())
            });

            var layout = new DynamicLayout();

            layout.BeginVertical();
            layout.AddRow(string.Format("Found {0} entries:", Entries.Count));
            layout.AddRow(List);
            layout.EndVertical();

            Content = layout;
		}

        private void List_KeyUp(object sender, KeyEventArgs e)
        {
            if (List.SelectedValue != null && e.Key == Keys.Enter)
            {
                Close(List.SelectedValue as SearchResult);
            }
        }

        private void List_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
            if (List.SelectedValue != null)
            {
                Close(List.SelectedValue as SearchResult);
            }
        }
    }
}
