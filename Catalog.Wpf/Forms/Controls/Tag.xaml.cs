﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Catalog.Wpf.Extensions;
using Color = System.Drawing.Color;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class Tag : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(Tag),
            new PropertyMetadata(default(string))
        );

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(Tag),
            new PropertyMetadata(
                Color.DarkGray,
                null,
                ColorCoerceValueCallback
            )
        );

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            nameof(TextColor),
            typeof(Color),
            typeof(Tag),
            new PropertyMetadata(default(Color))
        );

        private static object ColorCoerceValueCallback(DependencyObject d, object basevalue)
        {
            var color = (Color)basevalue;

            d.SetValue(TextColorProperty, color.GetLuminance() > 0.5 ? Color.Black : Color.White);

            return color.ToArgb() == 0 ? Color.DarkGray : color;
        }

        public Tag()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
    }
}
