using System;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;

namespace Catalog
{
	partial class AddGameDialog : Dialog<GameCopy>
	{
        private GameCopy game;

        protected Control DefaultControl { get; private set; }

		void InitializeComponent()
		{
			Title = "Add Game Dialog";
			ClientSize = new Size(800, 600);
			Padding = 10;

            PositiveButtons.Add(new Button
            {
                Text = "OK",
                Command = new Command((sender, e) => Close(game))
            });
            NegativeButtons.Add(new Button
            {
                Text = "Cancel",
                Command = new Command((sender, e) => Close())
            });

            game = new GameCopy();

            var titleTextbox = new TextBox();
            titleTextbox.TextBinding.Bind(game, g => g.Title);
            titleTextbox.KeyUp += TitleTextbox_KeyUp;

            DefaultControl = titleTextbox;

            var layout = new DynamicLayout
            {
                Spacing = new Size(5, 5),
            };

            layout.BeginVertical();
            layout.BeginHorizontal();
            layout.Add(new Label { Text = "Title", Width = 200 });
            layout.Add(titleTextbox, true);
            layout.Add(new Button
                {
                    Text = "Search MobyGames",
                    Command = new Command((sender, e) => SearchMobyGames(titleTextbox.Text.Trim()))
                }
            );
            layout.EndHorizontal();
            layout.EndVertical();

            layout.BeginVertical();
            layout.AddSpace();
            layout.AddRow(
                null,
                AbortButton,
                DefaultButton
            );

            layout.EndVertical();

            Content = layout;

        }

        private void TitleTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            var titleTextbox = sender as TextBox;

            if (!string.IsNullOrWhiteSpace(titleTextbox.Text) && e.Key == Keys.Enter)
            {
                SearchMobyGames(titleTextbox.Text.Trim());
            }
        }

        private void SearchMobyGames(string term)
        {
            var entries = new MobyGamesScraper().Search(term);

            var choice = new GameDisambiguationDialog(entries).ShowModal();

            if (choice == null)
            {
                return;
            }

            MessageBox.Show(choice.Name);
        }
    }
}
