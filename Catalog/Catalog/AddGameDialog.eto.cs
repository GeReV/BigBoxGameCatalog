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
        private readonly Button okButton = new Button
        {
            Text = "Finish"
        };

        private readonly GameInfoForm gameInfoForm = new GameInfoForm();
        private readonly GameItemsForm gameItemsForm = new GameItemsForm();

        private Button showGameItemsFormButton;
        private Button showGameInfoFormButton;

        void InitializeComponent()
        {
            Title = "Add Game Dialog";
            ClientSize = new Size(800, 700);
            Padding = 10;

            AbortButton = new Button
            {
                Text = "Cancel"
            };

            showGameInfoFormButton = new Button
            {
                Text = "Edit Info",
                Command = new Command((sender, args) =>
                {
                    ShowForm(gameInfoForm);
                })
            };

            showGameItemsFormButton = new Button
            {
                Text = "Edit Items",
                Command = new Command((sender, args) =>
                {
                    BuildItems();

                    ShowForm(gameItemsForm);
                })
            };

            NegativeButtons.Add(AbortButton);
            PositiveButtons.Add(okButton);
            PositiveButtons.Add(showGameInfoFormButton);
            PositiveButtons.Add(showGameItemsFormButton);

            ShowForm(gameInfoForm);
        }

        private void ShowForm(Control form)
        {
            Content = form;

            showGameInfoFormButton.Visible = form != gameInfoForm;
            showGameItemsFormButton.Visible = form != gameItemsForm;

            UpdateBindings(BindingUpdateMode.Destination);
        }
    }
}