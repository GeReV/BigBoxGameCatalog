using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Catalog.Wpf.Forms.Controls
{
    public partial class ColorPicker : UserControl
    {
        public static readonly DependencyProperty ColorsProperty = DependencyProperty.Register(
            nameof(Colors), typeof(IEnumerable), typeof(ColorPicker),
            new PropertyMetadata(default(IEnumerable)));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color), typeof(ColorPicker), new PropertyMetadata(default(Color)));

        public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
            nameof(SelectCommand), typeof(ICommand), typeof(ColorPicker), new PropertyMetadata(default(ICommand)));

        public IEnumerable Colors
        {
            get => (IEnumerable) GetValue(ColorsProperty);
            set => SetValue(ColorsProperty, value);
        }

        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public ICommand SelectCommand
        {
            get => (ICommand) GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        public ColorPicker()
        {
            InitializeComponent();
        }
    }
}
