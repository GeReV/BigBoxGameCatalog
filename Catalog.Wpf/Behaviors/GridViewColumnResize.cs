using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Catalog.Wpf.Behaviors
{
    public static class GridViewColumnResize
    {
        #region DependencyProperties

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(GridLength), typeof(GridViewColumnResize),
                new PropertyMetadata(OnSetWidthCallback));

        public static readonly DependencyProperty GridViewColumnResizeBehaviorProperty =
            DependencyProperty.RegisterAttached("GridViewColumnResizeBehavior",
                typeof(GridViewColumnResizeBehavior), typeof(GridViewColumnResize),
                null);

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(GridViewColumnResize),
                new PropertyMetadata(OnSetEnabledCallback));

        public static readonly DependencyProperty ListViewResizeBehaviorProperty =
            DependencyProperty.RegisterAttached("ListViewResizeBehaviorProperty",
                typeof(ListViewResizeBehavior), typeof(GridViewColumnResize), null);

        #endregion

        public static GridLength GetWidth(DependencyObject obj)
        {
            return (GridLength) obj.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(WidthProperty, value);
        }

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool) obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        #region CallBack

        private static void OnSetWidthCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is GridViewColumn element)
            {
                var behavior = GetOrCreateBehavior(element);
                behavior.Width = e.NewValue is GridLength value ? value : default;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type GridViewColumn but found " +
                                        dependencyObject.GetType().Name);
            }
        }

        private static void OnSetEnabledCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as ListView;
            if (element != null)
            {
                var behavior = GetOrCreateBehavior(element);
                behavior.Enabled = (bool) e.NewValue;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type ListView but found " + dependencyObject.GetType().Name);
            }
        }

        private static ListViewResizeBehavior GetOrCreateBehavior(ListView element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as ListViewResizeBehavior;
            if (behavior == null)
            {
                behavior = new ListViewResizeBehavior(element);
                element.SetValue(ListViewResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        private static GridViewColumnResizeBehavior GetOrCreateBehavior(GridViewColumn element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior;
            if (behavior == null)
            {
                behavior = new GridViewColumnResizeBehavior(element);
                element.SetValue(GridViewColumnResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        #endregion

        #region Nested type: GridViewColumnResizeBehavior

        /// <summary>
        /// GridViewColumn class that gets attached to the GridViewColumn control
        /// </summary>
        public class GridViewColumnResizeBehavior
        {
            private readonly GridViewColumn element;

            public GridViewColumnResizeBehavior(GridViewColumn element)
            {
                this.element = element;
            }

            public GridLength Width { get; set; }

            public bool IsStatic => StaticWidth >= 0;

            public double StaticWidth => Width.IsAbsolute ? Width.Value : -1;

            public double Percentage
            {
                get
                {
                    if (!IsStatic)
                    {
                        return Multiplier * 100;
                    }

                    return 0;
                }
            }

            public double Multiplier
            {
                get
                {
                    if (Width.IsStar)
                    {
                        return Width.Value;
                    }

                    return 1;
                }
            }

            public void SetWidth(double allowedSpace, double totalPercentage)
            {
                if (IsStatic)
                {
                    element.Width = StaticWidth;
                }
                else
                {
                    var width = allowedSpace * (Percentage / totalPercentage);
                    element.Width = width;
                }
            }
        }

        #endregion

        #region Nested type: ListViewResizeBehavior

        /// <summary>
        /// ListViewResizeBehavior class that gets attached to the ListView control
        /// </summary>
        public class ListViewResizeBehavior
        {
            private const int MARGIN = 25;
            private const long REFRESH_TIME = Timeout.Infinite;
            private const long DELAY = 500;

            private readonly ListView element;
            private readonly Timer timer;

            public ListViewResizeBehavior(ListView element)
            {
                if (element == null) throw new ArgumentNullException(nameof(element));
                this.element = element;
                element.Loaded += OnLoaded;

                // Action for resizing and re-enable the size lookup
                // This stops the columns from constantly resizing to improve performance
                Action resizeAndEnableSize = () =>
                {
                    Resize();
                    this.element.SizeChanged += OnSizeChanged;
                };
                timer = new Timer(x => Application.Current.Dispatcher?.BeginInvoke(resizeAndEnableSize), null, DELAY,
                    REFRESH_TIME);
            }

            public bool Enabled { get; set; }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                element.SizeChanged += OnSizeChanged;
            }

            private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                if (!e.WidthChanged)
                {
                    return;
                }

                element.SizeChanged -= OnSizeChanged;

                timer.Change(DELAY, REFRESH_TIME);
            }

            private void Resize()
            {
                if (!Enabled)
                {
                    return;
                }

                var totalWidth = element.ActualWidth;

                if (!(element.View is GridView gv))
                {
                    return;
                }

                var allowedSpace = totalWidth - GetAllocatedSpace(gv);

                allowedSpace -= MARGIN;

                var totalPercentage = GridViewColumnResizeBehaviors(gv).Sum(x => x.Percentage);

                foreach (var behavior in GridViewColumnResizeBehaviors(gv))
                {
                    behavior.SetWidth(Math.Max(0, allowedSpace), totalPercentage);
                }
            }

            private static IEnumerable<GridViewColumnResizeBehavior> GridViewColumnResizeBehaviors(GridView gv)
            {
                foreach (GridViewColumn t in gv.Columns)
                {
                    if (t.GetValue(GridViewColumnResizeBehaviorProperty) is GridViewColumnResizeBehavior
                        gridViewColumnResizeBehavior)
                    {
                        yield return gridViewColumnResizeBehavior;
                    }
                }
            }

            private static double GetAllocatedSpace(GridView gv)
            {
                double totalWidth = 0;
                foreach (var t in gv.Columns)
                {
                    if (t.GetValue(GridViewColumnResizeBehaviorProperty) is GridViewColumnResizeBehavior
                        gridViewColumnResizeBehavior)
                    {
                        if (gridViewColumnResizeBehavior.IsStatic)
                        {
                            totalWidth += gridViewColumnResizeBehavior.StaticWidth;
                        }
                    }
                    else
                    {
                        totalWidth += t.ActualWidth;
                    }
                }

                return totalWidth;
            }
        }

        #endregion
    }
}