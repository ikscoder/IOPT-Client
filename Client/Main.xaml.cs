using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Client.Classes;
using Object = Client.Classes.Object;
//"$(SolutionDir)\ILMerge\merge_all.bat" "$(SolutionDir)" "$(TargetPath)" $(ConfigurationName)
namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main
    {
        private readonly MediaPlayer _mp;
        private static Main _instance;

        public static Main GetMainWindow()
        {
            return _instance ?? (_instance = new Main());
        }
        private Main()
        {
            InitializeComponent();
            _mp = new MediaPlayer();
            BUpdate_Click(null, null);
            PopSettings.Closed += (s, e) => { Content.Effect = null; };
            PMonitoring.MouseDown += Drag;
            PPlatform.MouseDown += Drag;
            PTable.MouseDown += Drag;
            PMonitoring.stackscroll.MouseDown += Drag;
            LMonitoring.MouseDown += TabSelectionChanged;
            LPlatform.MouseDown += TabSelectionChanged;
            LTable.MouseDown += TabSelectionChanged;
            //Network.AutoUpdate();
            //ELmodels.ItemsSource = Platform.Current.Models;
            //Data.DataContext = Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[1].Changes;
            Controller.TryReadFromPortAsync();
        }


        private async void BUpload_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Loading");
                path.SetResourceReference(Shape.FillProperty, "AlternativeBackgroundColor");
                var but = new Border { Child = path };
                var da = new DoubleAnimation(0, 359, new Duration(TimeSpan.FromMilliseconds(600)));
                var rt = new RotateTransform();
                but.RenderTransform = rt;
                but.RenderTransformOrigin = new Point(0.5, 0.5);
                da.RepeatBehavior = RepeatBehavior.Forever;
                var button = sender as Button;
                if (button != null) button.Content = but;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            await Task.Run(() =>
            {
                try
                {
                    Network.SendDataToServer();
                }
                catch { Message.Show((string)Application.Current.Resources["Dialogid8"], (string)Application.Current.Resources["Dialogid5"]); }
            });
            if (sender != null)
            {
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Upload");
                path.SetResourceReference(Shape.FillProperty, "AlternativeBackgroundColor");
                var but = new Border { Child = path };
                var button = sender as Button;
                if (button != null) button.Content = but;
            }
        }

        public void Drag(object sender, MouseButtonEventArgs e)
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

        private async void BUpdate_Click(object sender, RoutedEventArgs e)
        {
            Controller.GenerateTestData();
            //Snapshot.Current.LastUpdate = DateTimeOffset.Now;
            //Message.Show(Controller.Serialize(), "");
            //Message.Show(JsonConvert.SerializeObject(Platform.Current.Models.First().Objects.First().Properties[3]), "");
            if ((sender as Button) != null)
            {
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Loading");
                path.SetResourceReference(Shape.FillProperty, "AlternativeBackgroundColor");
                var but = new Border { Child = path };
                var da = new DoubleAnimation(0, 359, new Duration(TimeSpan.FromMilliseconds(600)));
                var rt = new RotateTransform();
                but.RenderTransform = rt;
                but.RenderTransformOrigin = new Point(0.5, 0.5);
                da.RepeatBehavior = RepeatBehavior.Forever;
                ((Button)sender).Content = but;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);

            }
            await Task.Run(async () =>
            {
                try
                {
                    //Network.GetDataFromServer();                  
                    await Dispatcher.BeginInvoke(new Action(delegate
                    {
                        PTable.DGProp.ItemsSource = View.ModelToView();
                        PMonitoring.Lmodels.ItemsSource = Platform.Current.Models;
                        if (PMonitoring.Lobjects.SelectedItem != null)
                        {
                            PMonitoring.stackscroll.Content = View.GetDashboard(PMonitoring.Lobjects.SelectedItem as Object);
                        }
                    }));
                }
                catch { Message.Show((string)Application.Current.Resources["Dialogid7"], (string)Application.Current.Resources["Dialogid5"]); }
            });
            if ((sender as Button) != null)
            {
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Update");
                path.SetResourceReference(Shape.FillProperty, "AlternativeBackgroundColor");
                var but = new Border { Child = path };
                ((Button)sender).Content = but;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Controller.Close();
        }

        private void TabSelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if(!(sender is Label))return;
            _mp.Open(new Uri(@"pack://siteoforigin:,,,/Resources/button-29.wav"));
            _mp.Volume = 0.2;
            _mp.Play();

            if (sender == LPlatform)
            {
                LPlatform.SetResourceReference(BackgroundProperty, "OnDarkFontColor");
                LPlatform.SetResourceReference(ForegroundProperty, "MainColor");
                LMonitoring.SetResourceReference(BackgroundProperty, "MainColor");
                LMonitoring.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                LTable.SetResourceReference(BackgroundProperty, "MainColor");
                LTable.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                PMonitoring.Visibility=Visibility.Hidden;
                PPlatform.Visibility=Visibility.Visible;
                PTable.Visibility=Visibility.Hidden;
                return;
            }
            if (sender == LMonitoring)
            {
                LMonitoring.SetResourceReference(BackgroundProperty, "OnDarkFontColor");
                LMonitoring.SetResourceReference(ForegroundProperty, "MainColor");
                LPlatform.SetResourceReference(BackgroundProperty, "MainColor");
                LPlatform.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                LTable.SetResourceReference(BackgroundProperty, "MainColor");
                LTable.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                PMonitoring.Visibility = Visibility.Visible;
                PPlatform.Visibility = Visibility.Hidden;
                PTable.Visibility = Visibility.Hidden;
                return;
            }
            if (sender == LTable)
            {
                LTable.SetResourceReference(BackgroundProperty, "OnDarkFontColor");
                LTable.SetResourceReference(ForegroundProperty, "MainColor");
                LPlatform.SetResourceReference(BackgroundProperty, "MainColor");
                LPlatform.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                LMonitoring.SetResourceReference(BackgroundProperty, "MainColor");
                LMonitoring.SetResourceReference(ForegroundProperty, "OnDarkFontColor");
                PMonitoring.Visibility = Visibility.Hidden;
                PPlatform.Visibility = Visibility.Hidden;
                PTable.Visibility = Visibility.Visible;
                PTable.DGProp.ItemsSource = View.ModelToView();
            }
        }

        private void BReconnectClick(object sender, RoutedEventArgs e)
        {
            _mp.Open(new Uri(@"pack://siteoforigin:,,,/Resources/button-29.wav"));
            _mp.Play();
            System.Media.SystemSounds.Hand.Play();
            e.Handled = true;
            if (!(bool)Message.Show((string)Application.Current.Resources["Dialogid2"], (string)Application.Current.Resources["Dialogid4"], true)) return;
            MainWindow.Instance.Show();
            _instance = null;
            Settings.Current = null;
            Close();
        }

        private void BExitClick(object sender, RoutedEventArgs e)
        {
            _mp.Open(new Uri(@"pack://siteoforigin:,,,/Resources/button-29.wav"));
            _mp.Play();
            System.Media.SystemSounds.Hand.Play();
            e.Handled = true;
            if (!(bool)Message.Show((string)Application.Current.Resources["Dialogid2"], (string)Application.Current.Resources["Dialogid4"], true)) return;
            Controller.Close();
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
            _resizeInProcess = false;
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

        private void BSettingsClick(object sender, RoutedEventArgs e)
        {
            Content.Effect=new BlurEffect();
            PopSettings.IsOpen = true;
        }
    }

}
