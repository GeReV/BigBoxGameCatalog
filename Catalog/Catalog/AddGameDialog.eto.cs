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
            Text = "Finish"
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

        private TextArea notes = new TextArea
        {
            AcceptsTab = false
        };

        void InitializeComponent()
        {
            Title = "Add Game Dialog";
            ClientSize = new Size(800, 700);
            Padding = 10;

            AbortButton = new Button
            {
                Text = "Cancel"
            };

            NegativeButtons.Add(AbortButton);
            PositiveButtons.Add(okButton);
//            PositiveButtons.Add(new Button
//            {
//                Text = "Edit Items",
//                Command = new Command((sender, args) =>
//                {
//                    Content = new ItemManagementForm();
//                })
//            });

            DefaultControl = titleTextbox;

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
            };

            Content = layout;

            layout.BeginVertical();

            layout.AddLabeledRow("Title", l =>
            {
                l.Add(titleTextbox, true);
                layout.Add(searchMobyGamesButton);
            });

            layout.AddLabeledRow("Publisher", publisherList);

            layout.AddLabeledRow("Developers", new Scrollable
            {
                BackgroundColor = Colors.White,
                ExpandContentHeight = false,
                Height = 120,
                Content = developerList,
            });

            layout.AddLabeledRow("Has Game Box", hasBoxCheckbox);

            layout.AddLabeledRow("Media", addMediaPanel);

            layout.AddLabeledRow("Platforms", new Panel
            {
                Content = platformList,
            });

            layout.AddLabeledRow("Screenshots", screenshots, false);

            layout.AddLabeledRow("Notes", notes);

            layout.AddSpace();

            layout.BeginHorizontal();
            layout.Add(statusLabel);
            layout.Add(progressBar, true, false);
            layout.EndHorizontal();

            layout.EndVertical();
        }
    }
}