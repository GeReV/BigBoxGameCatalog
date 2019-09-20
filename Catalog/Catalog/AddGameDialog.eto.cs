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
        protected TextBox TitleTextbox = new TextBox();
        protected Button SearchMobyGamesButton = new Button
        {
            Text = "Search MobyGames",
        };
        protected ComboBox PublisherList = new ComboBox
        {
            AutoComplete = true,
        };
        protected CheckBoxList DeveloperList =new CheckBoxList()
        {
            Orientation = Orientation.Vertical,
        };
        protected CheckBox HasBoxCheckbox = new CheckBox();
        protected EnumCheckBoxList<Platform> PlatformList = new EnumCheckBoxList<Platform>
        {
            Orientation = Orientation.Vertical,
            GetText = platform => platform.GetDescription(),
        };
        protected ThumbnailSelect Screenshots= new ThumbnailSelect();
        protected AddMediaPanel AddMediaPanel = new AddMediaPanel();
        protected Control DefaultControl { get; private set; }

        void InitializeComponent()
        {
            Title = "Add Game Dialog";
            ClientSize = new Size(800, 600);
            Padding = 10;

            DefaultButton = new Button
            {
                Text = "OK",
            };

            AbortButton = new Button
            {
                Text = "Cancel",
            };

            NegativeButtons.Add(AbortButton);
            PositiveButtons.Add(DefaultButton);

            DefaultControl = TitleTextbox;

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(5, 5),
            };

            Content = layout;

            layout.BeginVertical();

            AddRow(layout, "Title", l =>
            {
                l.Add(TitleTextbox, true);
                layout.Add(SearchMobyGamesButton);
            });

            AddRow(layout, "Publisher", PublisherList);

            AddRow(layout, "Developers", new Scrollable
            {
                BackgroundColor = Colors.White,
                ExpandContentHeight = false,
                Height = 120,
                Content = DeveloperList,
            });

            AddRow(layout, "Has Game Box", HasBoxCheckbox);

            AddRow(layout, "Media", AddMediaPanel);

            AddRow(layout, "Platforms", new Panel
            {
                Content = PlatformList,
            });

            AddRow(layout, "Screenshots", Screenshots);

            layout.EndVertical();
        }

        private void AddRow(DynamicLayout layout, string label, Control control)
        {
            AddRow(layout, label, l => l.Add(control, true));
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