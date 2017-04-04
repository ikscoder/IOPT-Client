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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Classes;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Monitoring.xaml
    /// </summary>
    public partial class Monitoring : UserControl
    {
        public Monitoring()
        {
            InitializeComponent();
            Lobjects.SelectionChanged += (s, e) =>
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Classes.Object);
            };
            Classes.Platform.Current.Models.CollectionChanged += Notified;
        }

        public void Notified(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var tmp = Lmodels.SelectedIndex;
            Lmodels.ItemsSource = null;
            Lmodels.ItemsSource = Classes.Platform.Current.Models;
            Lmodels.SelectedIndex = tmp;
            if (Lmodels.SelectedItem != null)
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Classes.Object);
            }
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (!(button?.Tag is Object)) return;
            //if (Lobjects.SelectedItem == null) { Message.Show((string)Application.Current.Resources["Dialogid6"], (string)Application.Current.Resources["Dialogid5"]); return; }
            Lobjects.SelectedItem = button.Tag;
            new WDashbordCreate((Object)button.Tag).ShowDialog();
            Lobjects.SelectedIndex = -1;
            Lobjects.SelectedItem = button.Tag;
        }
    }
}
