using System;
using Eto.Forms;
using Eto.Drawing;

namespace Catalog
{
    partial class MainForm : Form
    {
        protected ListBox items;

        void InitializeComponent()
        {
            Title = "Catalog";
            ClientSize = new Size(1280, 960);
            Padding = 10;

            var dynamicLayout = new DynamicLayout();

            var scrollable = new Scrollable();

            items = new ListBox();

            scrollable.Content = items;

            dynamicLayout.BeginHorizontal(true);
            dynamicLayout.Add(scrollable, true);
            dynamicLayout.Add(new Panel
            {
                Width = 200,
            }, false, true);
            dynamicLayout.EndHorizontal();

            Content = dynamicLayout;

            // create a few commands that can be used for the menu and toolbar
            var addGame = new Command { MenuText = "Add Game...", ToolBarText = "Add Game...", Shortcut = Application.Instance.CommonModifier | Keys.N };
            addGame.Executed += (sender, e) => AddGame(new AddGameDialog().ShowModal());

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

            // create menu
            Menu = new MenuBar
            {
                Items =
                {
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = { addGame } },
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
                ApplicationItems =
                {
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
                },
                QuitItem = quitCommand,
                AboutItem = aboutCommand
            };

            // create toolbar			
            ToolBar = new ToolBar { Items = { addGame } };
        }
    }
}
