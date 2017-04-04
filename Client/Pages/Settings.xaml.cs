using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Classes;
using Object = Client.Classes.Object;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();


            styleBox.ItemsSource = Enum.GetValues(typeof(Classes.Settings.Themes)).Cast<Classes.Settings.Themes>();
            styleBox.SelectedItem = Classes.Settings.Current.Theme;
            styleBox.SelectionChanged += (s, e) => { Classes.Settings.Current.Theme = (Classes.Settings.Themes)styleBox.SelectedItem; };

            langBox.ItemsSource = Enum.GetValues(typeof(Classes.Settings.Languages)).Cast<Classes.Settings.Languages>();
            langBox.SelectedItem = Classes.Settings.Current.Language;
            langBox.SelectionChanged += (s, e) => { Classes.Settings.Current.Language = (Classes.Settings.Languages)langBox.SelectedItem; };

            setuintBox.ItemsSource = new[] { "1", "5", "30", "60", "600", "1800", "3600" };
            setuintBox.SelectedItem = Classes.Settings.Current.AutoUpdateInterval.ToString();
            setuintBox.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, new Binding { Source = Classes.Settings.Current, Path = new PropertyPath("AutoUpdateInterval"), Mode = BindingMode.TwoWay });

        }

        private void Button3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Rectangle) == null) return;
            radial.Center = e.GetPosition(((Rectangle)sender));
            radial.GradientOrigin = e.GetPosition(((Rectangle)sender));
            var pa = new DoubleAnimation
            {
                From = 0,
                To = ((Rectangle)sender).ActualWidth,
                Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                AutoReverse = true
            };
            radial.BeginAnimation(RadialGradientBrush.RadiusXProperty, pa);
            radial.BeginAnimation(RadialGradientBrush.RadiusYProperty, pa);
        }
    }
}
