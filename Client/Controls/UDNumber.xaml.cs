using System.Windows;
using System.Windows.Controls;

namespace Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для Dasboard4Number.xaml
    /// </summary>
    public partial class UDNumber : UserControl
    {
        public UDNumber()
        {
            InitializeComponent();
            BChart.Click += (s,e) => { pop.IsOpen = true; };
        }

        private void BExit_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = false;
        }
    }
}
