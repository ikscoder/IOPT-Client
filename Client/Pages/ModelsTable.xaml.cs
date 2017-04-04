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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Classes;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для ModelsTable.xaml
    /// </summary>
    public partial class ModelsTable : UserControl
    {
        public ModelsTable()
        {
            InitializeComponent();
            DGProp.ItemsSource = View.ModelToView();
        }
    }
}
