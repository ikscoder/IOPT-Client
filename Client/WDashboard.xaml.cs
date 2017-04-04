using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Client.Classes;
using OxyPlot;
using OxyPlot.Wpf;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;
using System.Windows.Data;
using System.Windows.Shapes;
using Client.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для WDashboard.xaml
    /// </summary>
    public partial class WDashboard : Window
    {
        private Dashboard _dashboard;


        public WDashboard(Dashboard dashboard)
        {
            InitializeComponent();
            _dashboard = dashboard;
            TTitle.Text = (from t in Platform.Current.Models.SelectMany(x => x.Objects) where t.Id == dashboard.ObjectId select t.Name).FirstOrDefault();
            BExit.Click += (s, e) => { Close(); };
            BMinimize.Click += (s, e) => { WindowState = WindowState.Minimized; };
            BMaximize.Click += (s, e) => { WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; };

            foreach (var pm in dashboard.View.OrderBy(x=>x.Property.Type))
            {
                if (pm.Property.Type == 3)
                {
                    if (pm.IsControl)
                    {
                        var numdash = new CDBool();
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldX = "Name";
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldY = "Value";
                        numdash.Plot.Series.FirstOrDefault().ItemsSource = pm.Property.Changes;
                        numdash.Plot.Axes.FirstOrDefault().AbsoluteMinimum =
                        pm.Property.Changes.Min(x => x.Name).ToOADate();
                        numdash.Title.Content = pm.Property.Name;
                        if (pm.Property.PathUnit.Equals("Terminator"))
                            {
                                numdash.ValueBox.Checked += (s, e) =>
                                {
                                    Controller.Terminate();
                                };
                                numdash.ValueBox.Unchecked += (s, e) =>
                                {
                                    Controller.Terminate();
                                };
                            }
                        if (pm.Property.PathUnit.Equals("VentState"))
                        {
                            numdash.ValueBox.Checked += (s, e) =>
                            {
                                numdash.BLoading.Visibility=Visibility.Visible;
                                numdash.ValueBox.Visibility=Visibility.Hidden;
                                Controller.SendVentState();
                                numdash.BLoading.Visibility = Visibility.Hidden;
                                numdash.ValueBox.Visibility = Visibility.Visible;
                            };
                            numdash.ValueBox.Unchecked += (s, e) =>
                            {
                                numdash.BLoading.Visibility = Visibility.Visible;
                                numdash.ValueBox.Visibility = Visibility.Hidden;
                                Controller.SendVentState();
                                numdash.BLoading.Visibility = Visibility.Hidden;
                                numdash.ValueBox.Visibility = Visibility.Visible;
                            };
                        }
                        numdash.ValueBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                        boolwrap.Children.Add(numdash);
                    }
                    else
                    {
                        var numdash = new UDBool();
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldX = "Name";
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldY = "Value";
                        numdash.Plot.Series.FirstOrDefault().ItemsSource = pm.Property.Changes;
                        numdash.Plot.Axes.FirstOrDefault().AbsoluteMinimum =
                            pm.Property.Changes.Min(x => x.Name).ToOADate();
                        numdash.Title.Content = pm.Property.Name;
                        numdash.ValueBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay });
                        pm.Property.PropertyChanged += (s, e) =>
                        {
                            try { numdash.ValueBox.IsChecked = bool.Parse(pm.Property.Value); } catch { }
                        };
                        boolwrap.Children.Add(numdash);
                    }
                }
                if (pm.Property.Type >= 7 && pm.Property.Type <= 15)
                {
                    if (pm.IsControl)
                    {
                        var numdash = new CDNumber();
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldX = "Name";
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldY = "Value";
                        numdash.Plot.Series.FirstOrDefault().ItemsSource = pm.Property.Changes;
                        numdash.Plot.Axes.FirstOrDefault().AbsoluteMinimum =
                            pm.Property.Changes.Min(x => x.Name).ToOADate();
                        numdash.Title.Content = pm.Property.Name;
                        if (pm.Min != null && pm.Max != null)
                        {
                            numdash.SValue.Minimum = (double) pm.Min;
                            numdash.SValue.Maximum = (double) pm.Max;
                            numdash.SValue.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty,
                                new Binding
                                {
                                    Source = pm.Property,
                                    Path = new PropertyPath("Value"),
                                    Mode = BindingMode.TwoWay
                                });
                        }
                        if (pm.Property.PathUnit.Equals("LampBrightness"))
                            numdash.SValue.ValueChanged += async (s, e) =>
                            {
                                if (View.lamp) return;
                                View.lamp = true;
                                await Task.Run(() => { Thread.Sleep(1000); });
                                Controller.SendLampState();
                                View.lamp = false;
                            };
                        pm.Property.Changes.CollectionChanged += (s, e) =>
                        {
                            try
                            {
                                numdash.VMin.Text = pm.Property.Changes.Min(x => x.Value).ToString("F1");
                                numdash.VMax.Text = pm.Property.Changes.Max(x => x.Value).ToString("##.##");

                                numdash.labelMedian.Content =
                                    pm.Property.Changes.Select(x => x.Value).GetMedian().ToString("F1");
                                numdash.VAvg.Text = pm.Property.Changes.GetAvg().ToString("F1");
                            }
                            catch
                            {
                            }
                        };
                        pm.Property.PropertyChanged += (s, e) =>
                        {
                            try
                            {
                                numdash.VValue.Value = double.Parse(pm.Property.Value.Replace('.', ','));
                                //numdash.SValue.Value = double.Parse(pm.Property.Value);
                            }
                            catch
                            {
                            }
                        };
                        Pane.Children.Add(numdash);
                    }
                    else
                    {
                        var numdash = new UDNumber();
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldX = "Name";
                        (numdash.Plot.Series.FirstOrDefault() as LineSeries).DataFieldY = "Value";
                        numdash.Plot.Series.FirstOrDefault().ItemsSource = pm.Property.Changes;
                        numdash.Plot.Axes.FirstOrDefault().AbsoluteMinimum =
                            pm.Property.Changes.Min(x => x.Name).ToOADate();

                        numdash.Title.Content = pm.Property.Name;
                        if (pm.Min != null && pm.Max != null)
                        {
                            numdash.VValue.Min = (double) pm.Min;
                            numdash.VValue.Max = (double) pm.Max;
                            numdash.VValue.SetBinding(RoundProgressBar.ValueProperty,
                                new Binding
                                {
                                    Source = pm.Property,
                                    Path = new PropertyPath("Value"),
                                    Mode = BindingMode.OneWay
                                });
                        }
                        pm.Property.Changes.CollectionChanged += (s, e) =>
                        {
                            try
                            {
                                numdash.VMin.Text = pm.Property.Changes.Min(x => x.Value).ToString("F1");
                                numdash.VMax.Text = pm.Property.Changes.Max(x => x.Value).ToString("##.##");
                                numdash.labelMedian.Content =
                                    pm.Property.Changes.Select(x => x.Value).GetMedian().ToString("F1");
                                numdash.VAvg.Text = pm.Property.Changes.GetAvg().ToString("F1");
                            }
                            catch
                            {
                            }
                        };
                        pm.Property.PropertyChanged += (s, e) =>
                        {
                            try
                            {
                                numdash.VValue.Value = double.Parse(pm.Property.Value.Replace('.', ','));
                            }
                            catch
                            {
                            }
                        };
                        Pane.Children.Add(numdash);
                    }
                    if (pm.Property.Type == 18)
                    {
                        if (pm.IsControl)
                        {
                            var stringDash = new CDString {Title = {Content = pm.Property.Name}};
                            stringDash.TValue.SetBinding(TextBox.TextProperty,
                                new Binding
                                {
                                    Source = pm.Property,
                                    Path = new PropertyPath("Value"),
                                    Mode = BindingMode.TwoWay
                                });
                            boolwrap.Children.Add(stringDash);
                        }
                        else
                        {
                            var stringDash = new UDString {Title = {Content = pm.Property.Name}};
                            stringDash.TValue.SetBinding(ContentProperty,
                                new Binding
                                {
                                    Source = pm.Property,
                                    Path = new PropertyPath("Value"),
                                    Mode = BindingMode.OneWay
                                });
                            boolwrap.Children.Add(stringDash);
                        }
                    }
                    pm.Property.Changes.Clear();
                }
            }
        }

        private static Plot CreatePlot(Dashboard.PropertyMap pm)
        {
            var plot = new Plot
            {
                Margin = new Thickness(0),
                //Title = pm.Property.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                // Width = SystemParameters.PrimaryScreenWidth * 0.4,
                DefaultTrackerTemplate = Application.Current.FindResource("OxyLineTemplate") as ControlTemplate,
            };

            plot.SetResourceReference(BackgroundProperty, "BackgroundColor");
            plot.SetResourceReference(ForegroundProperty, "MainColor");
            var convertFromString = ColorConverter.ConvertFromString("#F62A00");
            if (convertFromString != null)
            {
                var lineserie = new LineSeries
                {
                    ItemsSource = pm.Property.Changes,
                    DataFieldY = "Value",
                    DataFieldX = "Name",
                    StrokeThickness = 1,
                    MarkerSize = 3,
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Cross,
                    Color = (Color)convertFromString,
                };

                var dateAxis = new DateTimeAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None, IntervalLength = 80, AbsoluteMinimum = pm.Property.Changes.Min(x => x.Name).ToOADate() };
                dateAxis.SetResourceReference(Axis.TitleProperty, "Viewid7");
                plot.Axes.Add(dateAxis);
                var valueAxis = new LinearAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None };
                valueAxis.SetResourceReference(Axis.TitleProperty, "Viewid6");
                //lags
                //pm.Property.Changes.CollectionChanged += (s, e) =>
                //{
                //    valueAxis.AbsoluteMaximum = pm.Property.Changes.Max(x => x.Value);
                //    valueAxis.AbsoluteMinimum= pm.Property.Changes.Min(x => x.Value);
                //};
                if (pm.Property.Type == 3)
                {
                    valueAxis.AbsoluteMaximum = 1;
                    valueAxis.AbsoluteMinimum = 0;
                }
                plot.Axes.Add(valueAxis);



                plot.Series.Add(lineserie);
            }
            plot.UpdateLayout();
            return plot;
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Opacity = 0.5;
                DragMove();
                Opacity = 1;
            }
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                WindowState = WindowState != WindowState.Minimized ? WindowState.Minimized : WindowState.Normal;
            }
        }


        #region ResizeWindows

        private bool _resizeInProcess;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect == null) return;
            _resizeInProcess = true;
            senderRect.CaptureMouse();
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect == null) return;
            _resizeInProcess = false; ;
            senderRect.ReleaseMouseCapture();
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (!_resizeInProcess) return;
            var senderRect = sender as Rectangle;
            var mainWindow = senderRect?.Tag as Window;
            if (mainWindow == null) return;
            var width = e.GetPosition(mainWindow).X;
            var height = e.GetPosition(mainWindow).Y;
            senderRect.CaptureMouse();
            if (senderRect.Name.ToLower().Contains("right"))
            {
                width += 1;
                if (width > 0)
                    mainWindow.Width = width;
            }
            if (senderRect.Name.ToLower().Contains("left"))
            {
                width -= 1;
                mainWindow.Left += width;
                width = mainWindow.Width - width;
                if (width > 0)
                {
                    mainWindow.Width = width;
                }
            }
            if (senderRect.Name.ToLower().Contains("bottom"))
            {
                height += 1;
                if (height > 0)
                    mainWindow.Height = height;
            }
            if (senderRect.Name.ToLower().Contains("top"))
            {
                height -= 1;
                mainWindow.Top += height;
                height = mainWindow.Height - height;
                if (height > 0)
                {
                    mainWindow.Height = height;
                }
            }
        }
        #endregion
    }
}
