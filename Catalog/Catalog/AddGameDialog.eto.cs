using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Forms;
using Catalog.Forms.Controls;
using Eto.Forms;
using Eto.Drawing;
using Catalog.Model;
using Catalog.Scrapers.MobyGames;
using Catalog.Scrapers.MobyGames.Model;
using Image = Eto.Drawing.Image;

namespace Catalog
{
    partial class AddGameDialog : Dialog<GameCopy>
    {
        private readonly TextBox titleTextbox = new TextBox();

        private readonly Button searchMobyGamesButton = new Button
        {
            Text = "Search MobyGames",
        };

        private readonly ComboBox publisherList = new ComboBox
        {
            AutoComplete = true,
        };

        private readonly CheckBoxList developerList = new CheckBoxList()
        {
            Orientation = Orientation.Vertical,
        };

        private readonly CheckBox hasBoxCheckbox = new CheckBox();

        private readonly EnumCheckBoxList<Platform> platformList = new EnumCheckBoxList<Platform>
        {
            Orientation = Orientation.Vertical,
            GetText = platform => platform.GetDescription(),
        };

        private readonly ThumbnailSelect screenshots = new ThumbnailSelect();
        private readonly AddMediaPanel addMediaPanel = new AddMediaPanel();

        private readonly Button okButton = new Button
        {
            Text = "OK"
        };

        private Control DefaultControl { get; set; }

        private readonly Label statusLabel = new Label
        {
            Width = 200
        };

        private readonly ProgressBar progressBar = new ProgressBar
        {
            Visible = false
        };

        void InitializeComponent()
        {
            Title = "Add Game Dialog";
            ClientSize = new Size(800, 700);
            Padding = 10;

            NegativeButtons.Add(AbortButton);
            PositiveButtons.Add(okButton);

            DefaultControl = titleTextbox;

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
            };

            Content = layout;

            layout.BeginVertical();

            AddRow(layout, "Title", l =>
            {
                l.Add(titleTextbox, true);
                layout.Add(searchMobyGamesButton);
            });

            AddRow(layout, "Publisher", publisherList);

            AddRow(layout, "Developers", new Scrollable
            {
                BackgroundColor = Colors.White,
                ExpandContentHeight = false,
                Height = 120,
                Content = developerList,
            });

            AddRow(layout, "Has Game Box", hasBoxCheckbox);

            AddRow(layout, "Media", addMediaPanel);

            AddRow(layout, "Platforms", new Panel
            {
                Content = platformList,
            });

            AddRow(layout, "Screenshots", screenshots, false);

            layout.AddSpace();

            layout.BeginHorizontal();
            layout.Add(statusLabel);
            layout.Add(progressBar, true, false);
            layout.EndHorizontal();

            layout.EndVertical();
        }

        private void AddRow(DynamicLayout layout, string label, Control control, bool? yscale = null)
        {
            AddRow(layout, label, l => l.Add(control, true, yscale));
        }

        private void AddRow(DynamicLayout layout, string label, Action<DynamicLayout> func)
        {
            layout.BeginHorizontal();
            layout.Add(new Label {Text = label, Width = 200});
            func(layout);
            layout.EndHorizontal();
        }
    }
}